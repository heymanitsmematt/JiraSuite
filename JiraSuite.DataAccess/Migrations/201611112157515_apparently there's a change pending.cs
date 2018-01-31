namespace JiraSuite.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apparentlytheresachangepending : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FixVersions", "JiraIssue_IssueKey", c => c.String(maxLength: 128));
            CreateIndex("dbo.FixVersions", "JiraIssue_IssueKey");
            AddForeignKey("dbo.FixVersions", "JiraIssue_IssueKey", "dbo.JiraIssues", "IssueKey");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FixVersions", "JiraIssue_IssueKey", "dbo.JiraIssues");
            DropIndex("dbo.FixVersions", new[] { "JiraIssue_IssueKey" });
            DropColumn("dbo.FixVersions", "JiraIssue_IssueKey");
        }
    }
}
