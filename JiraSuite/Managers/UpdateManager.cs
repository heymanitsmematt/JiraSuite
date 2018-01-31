using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            List<Task> netsuiTasks = new List<Task>();
            List<Task> jiraTasks = new List<Task>();
            foreach (var ticket in _dbContext.NetsuiteTickets.Where(x => x.columns.JiraIssues.Count > 0))
            {
                netsuiTasks.AddRange(ticket.columns.JiraIssues.Where(x => x?.Status != null).Select(issue => new Task(
                    () =>
                    {
                        _netsuiteConnection.UpdateExistingTicketWithJiraStatus(ticket, issue);
                    })));
            }
            Task.Factory.StartNew(() => { netsuiTasks.ForEach(t => t.Start()); });

            foreach (var issue in _dbContext.JiraIssues.Where(x => x.NetsuiteTicketNumber != null && x.Status != null && !x.NetsuiteApiResults.Any()))
            {
                jiraTasks.AddRange(issue.NetsuiteTicketNumber.Split(',').Select(ticket => new Task(
                    () =>
                    {
                        _netsuiteConnection.UpdateTicketWithoutLocalNsTicket(ticket, issue);
                    })));
            }

            Task.Factory.StartNew(() => { jiraTasks.ForEach(t => t.Start()); });
        }
        
    }
}