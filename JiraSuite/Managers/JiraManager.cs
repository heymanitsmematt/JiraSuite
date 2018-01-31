using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using JiraSuite.DataAccess.EntityFramework;
using JiraSuite.DataAccess.Models;
using JiraSuite.Jira;
using TechTalk.JiraRestClient;
using IssueType = TechTalk.JiraRestClient.IssueType;
using static JiraSuite.DataAccess.Models.JiraTypeHelper;

namespace JiraSuite.Managers
{
    public class JiraManager
    {
        private JiraConnection _jiraConnection = new JiraConnection();
        private JiraSuiteDbContext _dbContext = DBContextManager.Instance.DbContext;

        public async void UpdateDb(List<DbEntityValidationException> saveErrors)
        {
            List<Issue> allIssues = GetAllIssues();
            List<Task> threadList = new List<Task>();
            using (_dbContext)
            {

                foreach(var issue in allIssues.Where(
                        x => !string.IsNullOrWhiteSpace(x.key) && !string.IsNullOrWhiteSpace(x.fields.customfield_10080)))
                    {
                        try
                        {
                            threadList.Add(new Task(() =>
                            {
                                bool isNew = false;
                                var thisIssue = GetCreateJiraIssue(issue, out isNew);

                                if (isNew)
                                    _dbContext.Entry(new JiraIssue(issue)).State = EntityState.Added;
                                else
                                    _dbContext.JiraIssues.AddOrUpdate(thisIssue);
                                _dbContext.SaveChanges();
                            }));
                        }
                        catch (Exception ex)
                        {
                            DbEntityValidationException item = ex as DbEntityValidationException;
                            if (item != null)
                                saveErrors.Add(item);
                        }
                    }
                await Task.Factory.StartNew(() => threadList.ForEach(t => t.Start()));
            }
        }

        public void GetJiraTicketsWithMissingInfoFromNetsuite()
        {
            using (_dbContext = DBContextManager.Instance.DbContext)
            {
                var jiraIssues =_dbContext.JiraIssues.Where(x => x.IssueId == null).ToList();
                Parallel.ForEach(jiraIssues, issue =>
                {
                    try
                    {
                        Issue newJiraIssue = _jiraConnection.Client.LoadIssue(issue.IssueKey);
                        // .GetIssuesByQuery(issue.IssueKey.Substring(0, issue.IssueKey.IndexOf('-')), $"key = {issue.IssueKey}").FirstOrDefault();
                        issue.UpdateFromExisting(newJiraIssue);
                        _dbContext.Entry(issue).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.Write($"Error Caught: {e.Message}.");
                    }
                });

            }
        }

        public List<Issue> GetAllIssues()
        {
            List<Issue> mosoIssues = new List<Issue>();
            Parallel.ForEach(Enum.GetNames(typeof (SoftwareType)), software =>
            {
                Parallel.ForEach(Enum.GetNames(typeof (JiraIssueType)), type =>
                {
                    string jql = "cf[10080] is not empty";
                    mosoIssues.AddRange(
                        _jiraConnection.Client.GetIssuesByQuery(software, type, jql,
                            new[] {"customfield_10080", "fixVersions"}).ToList());
                           //"MCLUB", type, jql, new[] { "customfield_10080", "fixVersions" }).ToList());
                });
            });
            return mosoIssues;
        }

        public JiraIssue GetCreateJiraIssue(Issue issue, out bool isNew)
        {
            _dbContext = DBContextManager.Instance.DbContext;
            JiraIssue thisIssue = new JiraIssue();
            if (_dbContext.JiraIssues.Any())
            {
                thisIssue = _dbContext.JiraIssues.FirstOrDefault(x => (x.IssueKey != null) && (issue.key != null) && (x.IssueKey == issue.key));
                if (thisIssue != null)
                {
                    thisIssue.UpdateFromExisting(issue);
                    _dbContext.SaveChanges();
                }
            }

            bool state = isNew = string.IsNullOrEmpty(thisIssue?.IssueKey) ? true : false;
            return thisIssue;
        }
        private List<Issue> GetAllMosoClubIssues()
        {
            return _jiraConnection.Client.GetIssues("MCLUB").ToList();
        }

        #region Constructors

        public JiraManager()
        {
        }

        #endregion
    }
}