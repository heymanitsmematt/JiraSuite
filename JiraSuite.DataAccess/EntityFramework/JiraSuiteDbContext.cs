using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraSuite.DataAccess.Models;

namespace JiraSuite.DataAccess.EntityFramework
{
    public class JiraSuiteDbContext : DbContext
    {
        public DbSet<JiraIssue> JiraIssues { get; set; }
        public DbSet<NetsuiteTicket> NetsuiteTickets { get; set; }
    }
}
