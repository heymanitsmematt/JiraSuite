namespace JiraSuite.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Importofcustomnetsuitefield : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JiraIssues", "NetsuiteTicketNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.JiraIssues", "NetsuiteTicketNumber");
        }
    }
}
