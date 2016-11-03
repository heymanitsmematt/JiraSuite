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
        private JiraSuiteDbContext _dbContext;

        public void UpdateDb(List<DbEntityValidationException> saveErrors)
        {
            List<Issue> allIssues = GetAllIssues();
            //allIssues.AddRange(GetAllMosoClubIssues());
            //FOR FASTER DEBUG ONLY GET MCLUB ISSUES!!!
            //List<Issue> allIssues = GetAllMosoClubIssues();
            using (_dbContext = new JiraSuiteDbContext())
            {
                foreach (var issue in allIssues.Where(x => !string.IsNullOrWhiteSpace(x.key) && ! string.IsNullOrWhiteSpace(x.fields.customfield_10080)))
                {
                    try
                    {
                        bool isNew = false;
                        var thisIssue = GetCreateJiraIssue(issue, out isNew);
                        if (isNew)
                            _dbContext.Entry(new JiraIssue(issue)).State = EntityState.Added;
                        else
                        {
                            _dbContext.JiraIssues.AddOrUpdate(thisIssue);
                            //_dbContext.Entry(thisIssue).State = EntityState.Modified;
                        }
                        _dbContext.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        saveErrors.Add(ex);
                    }
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

                    mosoIssues.AddRange(_jiraConnection.Client.GetIssuesByQuery(software, type, jql, new[] {"customfield_10080"}).ToList());
                }
            }
            return mosoIssues;
        }

        public JiraIssue GetCreateJiraIssue(Issue issue, out bool isNew)
        {
            _dbContext = new JiraSuiteDbContext();
            JiraIssue thisIssue = new JiraIssue();
            if (_dbContext.JiraIssues.Any())
                thisIssue = _dbContext.JiraIssues.FirstOrDefault(x => x.IssueKey == issue.key);
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