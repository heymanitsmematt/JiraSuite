namespace JiraSuite.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alterNetsuitemodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JiraIssues",
                c => new
                    {
                        IssueKey = c.String(nullable: false, maxLength: 128),
                        IssueId = c.String(),
                        Description = c.String(),
                        Summary = c.String(),
                        Reporter = c.String(),
                        Assignee = c.String(),
                        Status = c.String(),
                        SoftwareType = c.Int(nullable: false),
                        IssueType = c.Int(nullable: false),
                        LastRefreshTime = c.DateTime(),
                        JiraIssue_IssueKey = c.String(maxLength: 128),
                        Columns_casenumber = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.IssueKey)
                .ForeignKey("dbo.JiraIssues", t => t.JiraIssue_IssueKey)
                .ForeignKey("dbo.Columns", t => t.Columns_casenumber)
                .Index(t => t.JiraIssue_IssueKey)
                .Index(t => t.Columns_casenumber);
            
            CreateTable(
                "dbo.NetsuiteApiResults",
                c => new
                    {
                        InternalID = c.Int(nullable: false, identity: true),
                        id = c.String(),
                        recordtype = c.String(),
                        columns_casenumber = c.String(maxLength: 128),
                        JiraIssue_IssueKey = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.InternalID)
                .ForeignKey("dbo.Columns", t => t.columns_casenumber)
                .ForeignKey("dbo.JiraIssues", t => t.JiraIssue_IssueKey)
                .Index(t => t.columns_casenumber)
                .Index(t => t.JiraIssue_IssueKey);
            
            CreateTable(
                "dbo.Columns",
                c => new
                    {
                        casenumber = c.String(nullable: false, maxLength: 128),
                        title = c.String(),
                        startdate = c.String(),
                        createddate = c.String(),
                        helpdesk = c.Boolean(nullable: false),
                        custevent10 = c.Boolean(nullable: false),
                        custevent27 = c.Boolean(nullable: false),
                        escalatedto = c.String(),
                        assigned_name = c.String(maxLength: 128),
                        category_name = c.String(maxLength: 128),
                        company_id = c.Int(),
                        contact_name = c.String(maxLength: 128),
                        priority_name = c.String(maxLength: 128),
                        profile_name = c.String(maxLength: 128),
                        stage_name = c.String(maxLength: 128),
                        status_name = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.casenumber)
                .ForeignKey("dbo.Assigneds", t => t.assigned_name)
                .ForeignKey("dbo.Categories", t => t.category_name)
                .ForeignKey("dbo.Companies", t => t.company_id)
                .ForeignKey("dbo.Contacts", t => t.contact_name)
                .ForeignKey("dbo.Priorities", t => t.priority_name)
                .ForeignKey("dbo.Profiles", t => t.profile_name)
                .ForeignKey("dbo.Stages", t => t.stage_name)
                .ForeignKey("dbo.Status", t => t.status_name)
                .Index(t => t.assigned_name)
                .Index(t => t.category_name)
                .Index(t => t.company_id)
                .Index(t => t.contact_name)
                .Index(t => t.priority_name)
                .Index(t => t.profile_name)
                .Index(t => t.stage_name)
                .Index(t => t.status_name);
            
            CreateTable(
                "dbo.Assigneds",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 128),
                        internalid = c.String(),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 128),
                        internalid = c.String(),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        internalid = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 128),
                        internalid = c.String(),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.Priorities",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 128),
                        internalid = c.String(),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 128),
                        internalid = c.String(),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.Stages",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 128),
                        internalid = c.String(),
                    })
                .PrimaryKey(t => t.name);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 128),
                        internalid = c.String(),
                    })
                .PrimaryKey(t => t.name);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NetsuiteApiResults", "JiraIssue_IssueKey", "dbo.JiraIssues");
            DropForeignKey("dbo.NetsuiteApiResults", "columns_casenumber", "dbo.Columns");
            DropForeignKey("dbo.Columns", "status_name", "dbo.Status");
            DropForeignKey("dbo.Columns", "stage_name", "dbo.Stages");
            DropForeignKey("dbo.Columns", "profile_name", "dbo.Profiles");
            DropForeignKey("dbo.Columns", "priority_name", "dbo.Priorities");
            DropForeignKey("dbo.JiraIssues", "Columns_casenumber", "dbo.Columns");
            DropForeignKey("dbo.Columns", "contact_name", "dbo.Contacts");
            DropForeignKey("dbo.Columns", "company_id", "dbo.Companies");
            DropForeignKey("dbo.Columns", "category_name", "dbo.Categories");
            DropForeignKey("dbo.Columns", "assigned_name", "dbo.Assigneds");
            DropForeignKey("dbo.JiraIssues", "JiraIssue_IssueKey", "dbo.JiraIssues");
            DropIndex("dbo.Columns", new[] { "status_name" });
            DropIndex("dbo.Columns", new[] { "stage_name" });
            DropIndex("dbo.Columns", new[] { "profile_name" });
            DropIndex("dbo.Columns", new[] { "priority_name" });
            DropIndex("dbo.Columns", new[] { "contact_name" });
            DropIndex("dbo.Columns", new[] { "company_id" });
            DropIndex("dbo.Columns", new[] { "category_name" });
            DropIndex("dbo.Columns", new[] { "assigned_name" });
            DropIndex("dbo.NetsuiteApiResults", new[] { "JiraIssue_IssueKey" });
            DropIndex("dbo.NetsuiteApiResults", new[] { "columns_casenumber" });
            DropIndex("dbo.JiraIssues", new[] { "Columns_casenumber" });
            DropIndex("dbo.JiraIssues", new[] { "JiraIssue_IssueKey" });
            DropTable("dbo.Status");
            DropTable("dbo.Stages");
            DropTable("dbo.Profiles");
            DropTable("dbo.Priorities");
            DropTable("dbo.Contacts");
            DropTable("dbo.Companies");
            DropTable("dbo.Categories");
            DropTable("dbo.Assigneds");
            DropTable("dbo.Columns");
            DropTable("dbo.NetsuiteApiResults");
            DropTable("dbo.JiraIssues");
        }
    }
}
