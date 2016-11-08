using System;
using System.Linq;
using System.Xml;
using JiraSuite.DataAccess.EntityFramework;
using JiraSuite.Netsuite;

namespace JiraSuite.Managers
{
    public  class UpdateManager
    {
        private JiraSuiteDbContext _dbContext = DBContextManager.Instance.DbContext;
        private NetsuiteConnection _netsuiteConnection = new NetsuiteConnection();
        public void PostNetsuiteUpdates()
        {
            var i = 0;
            foreach (var ticket in _dbContext.NetsuiteTickets.Where(x => x.columns.JiraIssues.Count > 0))
                foreach (var issue in ticket.columns.JiraIssues.Where(x => x.Status != null))
                    //&& x.IssueKey.Contains("MCLUB"))) //FOR TESTING! REMOVE WHEN PACKAGING TO INCLUDE MOSO.
                {
                    i++;
                    _netsuiteConnection.UpdateExistingTicketWithJiraStatus(ticket, issue);
                }

            foreach (var issue in _dbContext.JiraIssues.Where(x => x.NetsuiteTicketNumber != null && x.Status != null))
                foreach (var ticket in issue.NetsuiteTicketNumber.Split(','))
                    _netsuiteConnection.UpdateTicketWithoutLocalNsTicket(ticket, issue);
        } 
        
    }
}