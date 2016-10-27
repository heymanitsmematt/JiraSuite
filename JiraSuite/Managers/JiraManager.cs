using System;
using System.Collections.Generic;
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

        public void UpdateDb()
        {
            List<Issue> allIssues = GetAllMosoIssues();
            allIssues.AddRange( GetAllMosoClubIssues());
            using (_dbContext = new JiraSuiteDbContext())
            {
                foreach (var issue in allIssues)
                {
                    _dbContext.JiraIssues.Add(new JiraIssue(issue));
                }
            _dbContext.SaveChangesAsync();
            }
        }

        public List<Issue> GetAllMosoIssues()
        {
            List<Issue> mosoIssues = new List<Issue>();
            foreach (string type in Enum.GetNames(typeof(JiraIssueType)))
            {
                string jql = "WRITE SOME MREANINGFUL JQL TO LIMIT SEARCH?";
                mosoIssues.AddRange(_jiraConnection.Client.GetIssuesByQuery(Enum.GetName(typeof(SoftwareType), SoftwareType.MOSO), type, "").ToList());
            }
            return mosoIssues;
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