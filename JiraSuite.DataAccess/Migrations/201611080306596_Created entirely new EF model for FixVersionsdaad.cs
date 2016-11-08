namespace JiraSuite.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedentirelynewEFmodelforFixVersionsdaad : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.JiraIssues", "FixVersion_id", "dbo.FixVersions");
            DropIndex("dbo.JiraIssues", new[] { "FixVersion_id" });
            DropColumn("dbo.JiraIssues", "FixVersion_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JiraIssues", "FixVersion_id", c => c.String(maxLength: 128));
            CreateIndex("dbo.JiraIssues", "FixVersion_id");
            AddForeignKey("dbo.JiraIssues", "FixVersion_id", "dbo.FixVersions", "id");
        }
    }
}
