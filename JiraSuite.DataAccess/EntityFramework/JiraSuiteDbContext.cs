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
        public DbSet<FixVersion> FixVersions { get; set; }
        public DbSet<NetsuiteApiResult> NetsuiteTickets { get; set; }
        public DbSet<Columns> NetsuiteColumns { get; set; }
        public DbSet<Stage> NetsuiteStages { get; set; }
        public DbSet<Status> NetsuiteStatuses { get; set; }
        public DbSet<Profile> NetsuiteProfiles { get; set; }
        public DbSet<Category> NetsuiteCategories { get; set; }
        public DbSet<Assigned> NetsuiteAssigndes { get; set; }
        public DbSet<Priority> NetsuitePriorities { get; set; }
        public DbSet<Contact> NetuiteContacts { get; set; }
        public DbSet<Company> NetsuiteCompanies { get; set; }
        
        public JiraSuiteDbContext() : base("name=JiraSuiteDbContext") {}
    }


    public class DBContextManager
    {
        private static DBContextManager _instance;
        private static JiraSuiteDbContext _dbContext;
        private DBContextManager() { }

        public static DBContextManager Instance
        {
            get { return _instance = new DBContextManager(); }   
        }

        public JiraSuiteDbContext DbContext
        {
            get { return _dbContext = new JiraSuiteDbContext(); }
        }
    }
}
