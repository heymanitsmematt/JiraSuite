namespace JiraSuite.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alterJiraprimarykeyadd : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.JiraIssues", "JiraIssue_IssueKey", "dbo.JiraIssues");
            DropForeignKey("dbo.NetsuiteApiResults", "JiraIssue_IssueKey", "dbo.JiraIssues");
            DropIndex("dbo.JiraIssues", new[] { "JiraIssue_IssueKey" });
            DropIndex("dbo.NetsuiteApiResults", new[] { "JiraIssue_IssueKey" });
            RenameColumn(table: "dbo.JiraIssues", name: "JiraIssue_IssueKey", newName: "JiraIssue_InternalKey");
            RenameColumn(table: "dbo.NetsuiteApiResults", name: "JiraIssue_IssueKey", newName: "JiraIssue_InternalKey");
            DropPrimaryKey("dbo.JiraIssues");
            AddColumn("dbo.JiraIssues", "InternalKey", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.JiraIssues", "IssueKey", c => c.String());
            AlterColumn("dbo.JiraIssues", "JiraIssue_InternalKey", c => c.Int());
            AlterColumn("dbo.NetsuiteApiResults", "JiraIssue_InternalKey", c => c.Int());
            AddPrimaryKey("dbo.JiraIssues", "InternalKey");
            CreateIndex("dbo.JiraIssues", "JiraIssue_InternalKey");
            CreateIndex("dbo.NetsuiteApiResults", "JiraIssue_InternalKey");
            AddForeignKey("dbo.JiraIssues", "JiraIssue_InternalKey", "dbo.JiraIssues", "InternalKey");
            AddForeignKey("dbo.NetsuiteApiResults", "JiraIssue_InternalKey", "dbo.JiraIssues", "InternalKey");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NetsuiteApiResults", "JiraIssue_InternalKey", "dbo.JiraIssues");
            DropForeignKey("dbo.JiraIssues", "JiraIssue_InternalKey", "dbo.JiraIssues");
            DropIndex("dbo.NetsuiteApiResults", new[] { "JiraIssue_InternalKey" });
            DropIndex("dbo.JiraIssues", new[] { "JiraIssue_InternalKey" });
            DropPrimaryKey("dbo.JiraIssues");
            AlterColumn("dbo.NetsuiteApiResults", "JiraIssue_InternalKey", c => c.String(maxLength: 128));
            AlterColumn("dbo.JiraIssues", "JiraIssue_InternalKey", c => c.String(maxLength: 128));
            AlterColumn("dbo.JiraIssues", "IssueKey", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.JiraIssues", "InternalKey");
            AddPrimaryKey("dbo.JiraIssues", "IssueKey");
            RenameColumn(table: "dbo.NetsuiteApiResults", name: "JiraIssue_InternalKey", newName: "JiraIssue_IssueKey");
            RenameColumn(table: "dbo.JiraIssues", name: "JiraIssue_InternalKey", newName: "JiraIssue_IssueKey");
            CreateIndex("dbo.NetsuiteApiResults", "JiraIssue_IssueKey");
            CreateIndex("dbo.JiraIssues", "JiraIssue_IssueKey");
            AddForeignKey("dbo.NetsuiteApiResults", "JiraIssue_IssueKey", "dbo.JiraIssues", "IssueKey");
            AddForeignKey("dbo.JiraIssues", "JiraIssue_IssueKey", "dbo.JiraIssues", "IssueKey");
        }
    }
}
