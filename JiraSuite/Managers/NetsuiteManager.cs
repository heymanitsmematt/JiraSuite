using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using JiraSuite.DataAccess.EntityFramework;
using JiraSuite.DataAccess.Models;
using JiraSuite.Netsuite;


namespace JiraSuite.Managers
{
    public class NetsuiteManager
    {
        private NetsuiteConnection _netsuiteConnection = new NetsuiteConnection();
        private JiraSuiteDbContext _dbContext = new JiraSuiteDbContext();

        public List<NetsuiteApiResult> GetAllNetsuiteTickets()
        {
            return _netsuiteConnection.GetAllTickets();
        }

        public void UpdateDb()
        {
            List<NetsuiteApiResult> allTickets = _netsuiteConnection.GetAllTickets();
            foreach (NetsuiteApiResult ticket in allTickets)
            {
                _dbContext.NetsuiteTickets.AddOrUpdate(ticket);
            }
        }
    }
}