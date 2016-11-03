namespace JiraSuite.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alterjiraprimarykeyagain : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.JiraIssues", "JiraIssue_InternalKey", "dbo.JiraIssues");
            DropForeignKey("dbo.NetsuiteApiResults", "JiraIssue_InternalKey", "dbo.JiraIssues");
            DropIndex("dbo.JiraIssues", new[] { "JiraIssue_InternalKey" });
            DropIndex("dbo.NetsuiteApiResults", new[] { "JiraIssue_InternalKey" });
            RenameColumn(table: "dbo.JiraIssues", name: "JiraIssue_InternalKey", newName: "JiraIssue_IssueKey");
            RenameColumn(table: "dbo.NetsuiteApiResults", name: "JiraIssue_InternalKey", newName: "JiraIssue_IssueKey");
            DropPrimaryKey("dbo.JiraIssues");
            AlterColumn("dbo.JiraIssues", "IssueKey", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.JiraIssues", "JiraIssue_IssueKey", c => c.String(maxLength: 128));
            AlterColumn("dbo.NetsuiteApiResults", "JiraIssue_IssueKey", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.JiraIssues", "IssueKey");
            CreateIndex("dbo.JiraIssues", "JiraIssue_IssueKey");
            CreateIndex("dbo.NetsuiteApiResults", "JiraIssue_IssueKey");
            AddForeignKey("dbo.JiraIssues", "JiraIssue_IssueKey", "dbo.JiraIssues", "IssueKey");
            AddForeignKey("dbo.NetsuiteApiResults", "JiraIssue_IssueKey", "dbo.JiraIssues", "IssueKey");
            DropColumn("dbo.JiraIssues", "InternalKey");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JiraIssues", "InternalKey", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.NetsuiteApiResults", "JiraIssue_IssueKey", "dbo.JiraIssues");
            DropForeignKey("dbo.JiraIssues", "JiraIssue_IssueKey", "dbo.JiraIssues");
            DropIndex("dbo.NetsuiteApiResults", new[] { "JiraIssue_IssueKey" });
            DropIndex("dbo.JiraIssues", new[] { "JiraIssue_IssueKey" });
            DropPrimaryKey("dbo.JiraIssues");
            AlterColumn("dbo.NetsuiteApiResults", "JiraIssue_IssueKey", c => c.Int());
            AlterColumn("dbo.JiraIssues", "JiraIssue_IssueKey", c => c.Int());
            AlterColumn("dbo.JiraIssues", "IssueKey", c => c.String());
            AddPrimaryKey("dbo.JiraIssues", "InternalKey");
            RenameColumn(table: "dbo.NetsuiteApiResults", name: "JiraIssue_IssueKey", newName: "JiraIssue_InternalKey");
            RenameColumn(table: "dbo.JiraIssues", name: "JiraIssue_IssueKey", newName: "JiraIssue_InternalKey");
            CreateIndex("dbo.NetsuiteApiResults", "JiraIssue_InternalKey");
            CreateIndex("dbo.JiraIssues", "JiraIssue_InternalKey");
            AddForeignKey("dbo.NetsuiteApiResults", "JiraIssue_InternalKey", "dbo.JiraIssues", "InternalKey");
            AddForeignKey("dbo.JiraIssues", "JiraIssue_InternalKey", "dbo.JiraIssues", "InternalKey");
        }
    }
}
