namespace JiraSuite.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addmanytomanyrelationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NetsuiteApiResults", "JiraIssue_IssueKey", "dbo.JiraIssues");
            AddColumn("dbo.NetsuiteApiResults", "JiraIssue_IssueKey1", c => c.String(maxLength: 128));
            CreateIndex("dbo.NetsuiteApiResults", "JiraIssue_IssueKey1");
            AddForeignKey("dbo.NetsuiteApiResults", "JiraIssue_IssueKey", "dbo.JiraIssues", "IssueKey");
            AddForeignKey("dbo.NetsuiteApiResults", "JiraIssue_IssueKey1", "dbo.JiraIssues", "IssueKey");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NetsuiteApiResults", "JiraIssue_IssueKey1", "dbo.JiraIssues");
            DropForeignKey("dbo.NetsuiteApiResults", "JiraIssue_IssueKey", "dbo.JiraIssues");
            DropIndex("dbo.NetsuiteApiResults", new[] { "JiraIssue_IssueKey1" });
            DropColumn("dbo.NetsuiteApiResults", "JiraIssue_IssueKey1");
            AddForeignKey("dbo.NetsuiteApiResults", "JiraIssue_IssueKey", "dbo.JiraIssues", "IssueKey");
        }
    }
}
