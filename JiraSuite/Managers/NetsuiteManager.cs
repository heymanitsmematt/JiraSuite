using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using JiraSuite.DataAccess.EntityFramework;
using JiraSuite.DataAccess.Models;
using JiraSuite.Netsuite;


namespace JiraSuite.Managers
{
    public class NetsuiteManager
    {
        private NetsuiteConnection _netsuiteConnection = new NetsuiteConnection();
        private JiraSuiteDbContext _dbContext = DBContextManager.Instance.DbContext;

        public List<NetsuiteApiResult> GetAllNetsuiteTickets()
        {
            return _netsuiteConnection.GetAllTickets(_dbContext);
        }

        public void UpdateDb(List<DbEntityValidationException> saveErrors)
        {
            List<NetsuiteApiResult> allTickets = GetAllResults(_dbContext);
            foreach (NetsuiteApiResult ticket in allTickets.Where(x => !string.IsNullOrWhiteSpace(x.id) && !string.IsNullOrEmpty(x.columns.casenumber)))
            {
                try
                {
                    for (var i = 0; i < ticket.columns.JiraIssues.Count; i++)
                    {
                        GetExistingForeignKeyReferences(ticket);

                        if (_dbContext.JiraIssues.Find(ticket.columns.JiraIssues[i].IssueKey) != null)
                            ticket.columns.JiraIssues[i] =
                                _dbContext.JiraIssues.Find(ticket.columns.JiraIssues[i].IssueKey);
                        if (!_dbContext.NetsuiteTickets.Any() && !_dbContext.NetsuiteTickets.Any(x => x.id == ticket.id))
                        {
                            _dbContext.NetsuiteTickets.Add(ticket);

                            _dbContext.Entry(ticket).State = EntityState.Added;
                        }
                        else
                            _dbContext.NetsuiteTickets.AddOrUpdate(ticket);
                        _dbContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    DbEntityValidationException item = ex as DbEntityValidationException;
                    if (item != null)
                        saveErrors.Add(item);
                }
            }
        }

        private void GetExistingForeignKeyReferences(NetsuiteApiResult ticket)
        {

            if (_dbContext.NetsuiteAssigndes.Any(x => x.name == ticket.columns.assigned.name))
                ticket.columns.assigned = _dbContext.NetsuiteAssigndes.Find(ticket.columns.assigned.name);
            if (_dbContext.NetsuiteCompanies.Any(x => x.name == ticket.columns.company.name))
                ticket.columns.company = _dbContext.NetsuiteCompanies.Find(ticket.columns.company.name);
            if (_dbContext.NetsuitePriorities.Any(x => x.name == ticket.columns.priority.name))
                ticket.columns.priority = _dbContext.NetsuitePriorities.Find(ticket.columns.priority.name);
            if (_dbContext.NetsuiteStages.Any(x => x.name == ticket.columns.stage.name))
                ticket.columns.stage = _dbContext.NetsuiteStages.Find(ticket.columns.stage.name);
            if (_dbContext.NetsuiteStatuses.Any(x => x.name == ticket.columns.status.name))
                ticket.columns.status = _dbContext.NetsuiteStatuses.Find(ticket.columns.status.name);
            if (_dbContext.NetuiteContacts.Any(x => x.name == ticket.columns.contact.name))
                ticket.columns.contact = _dbContext.NetuiteContacts.Find(ticket.columns.contact.name);
            if (_dbContext.NetsuiteCategories.Any(x => x.name == ticket.columns.category.name))
                ticket.columns.category = _dbContext.NetsuiteCategories.Find(ticket.columns.category.name);
            if (_dbContext.NetsuiteProfiles.Any(x => x.name == ticket.columns.profile.name))
                ticket.columns.profile = _dbContext.NetsuiteProfiles.Find(ticket.columns.profile.name);
        }

        public NetsuiteApiResult GetOrCreateResult(NetsuiteApiResult ticket)
        {
            NetsuiteApiResult newTicket = ticket;
            if (_dbContext.NetsuiteTickets.Any() && _dbContext.NetsuiteTickets.Any(x => x.id == ticket.id))
            {
                newTicket = _dbContext.NetsuiteTickets.Find(ticket.id);
            }
            return newTicket;
        }

        public List<NetsuiteApiResult> GetAllResults(JiraSuiteDbContext dbContext)
        {
            return _netsuiteConnection.GetAllTickets(dbContext);
        }
    }
}