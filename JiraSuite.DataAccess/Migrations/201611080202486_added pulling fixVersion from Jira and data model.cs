namespace JiraSuite.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedpullingfixVersionfromJiraanddatamodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JiraIssues", "FixVersion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.JiraIssues", "FixVersion");
        }
    }
}
