using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
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

        public void UpdateDb(List<DbEntityValidationException> saveErrors)
        {
            List<Issue> allIssues = GetAllIssues();
            using (_dbContext)
            {
                foreach (var issue in allIssues.Where(x => !string.IsNullOrWhiteSpace(x.key) && ! string.IsNullOrWhiteSpace(x.fields.customfield_10080)))
                {
                    try
                    {
                        bool isNew = false;
                        var thisIssue = GetCreateJiraIssue(issue, out isNew);

                        for (var i = 0; i < thisIssue.NetsuiteTicketNumber?.Split(',').Length; i++)
                        {
                            var thisNumber = thisIssue.NetsuiteTicketNumber.Split(',')[i];
                            if (_dbContext.NetsuiteTickets.Find(thisNumber) != null)
                                thisIssue.NetsuiteTickets.Add(_dbContext.NetsuiteTickets.Find(thisNumber));
                        }


                        if (isNew)
                            _dbContext.Entry(new JiraIssue(issue)).State = EntityState.Added;
                        else
                        {
                            _dbContext.JiraIssues.AddOrUpdate(thisIssue);
                            //_dbContext.Entry(thisIssue).State = EntityState.Modified;
                        }
                        _dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        DbEntityValidationException item = ex as DbEntityValidationException;
                        if (item != null)
                            saveErrors.Add(item);
                    }
                }
            }
        }

        public void GetJiraTicketsWithMissingInfoFromNetsuite()
        {
            using (_dbContext = DBContextManager.Instance.DbContext)
            {
                var jiraIssues =_dbContext.JiraIssues.Where(x => x.IssueId == null).ToList();
                foreach (var issue in jiraIssues)
                {
                    Issue newJiraIssue = _jiraConnection.Client.LoadIssue(issue.IssueKey);
                        // .GetIssuesByQuery(issue.IssueKey.Substring(0, issue.IssueKey.IndexOf('-')), $"key = {issue.IssueKey}").FirstOrDefault();
                    issue.UpdateFromExisting(newJiraIssue);
                    _dbContext.Entry(issue).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                }

            }
        }

        public List<Issue> GetAllIssues()
        {
            List<Issue> mosoIssues = new List<Issue>();
            foreach (string software in Enum.GetNames(typeof(SoftwareType)))
            {
                foreach (string type in Enum.GetNames(typeof(JiraIssueType)))
                {
                    string jql = "cf[10080] is not empty";


                    mosoIssues.AddRange(_jiraConnection.Client.GetIssuesByQuery(software, type, jql, new[] {"customfield_10080", "fixVersions"}).ToList()); //"MCLUB", type, jql, new[] { "customfield_10080", "fixVersions" }).ToList());
                }
            }
            return mosoIssues;
        }

        public JiraIssue GetCreateJiraIssue(Issue issue, out bool isNew)
        {
            _dbContext = DBContextManager.Instance.DbContext;
            JiraIssue thisIssue = new JiraIssue();
            if (_dbContext.JiraIssues.Any())
            {
                thisIssue = _dbContext.JiraIssues.FirstOrDefault(x => (x.IssueKey != null) && (issue.key != null) && (x.IssueKey == issue.key));
                thisIssue.UpdateFromExisting(issue);
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