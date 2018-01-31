namespace JiraSuite.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedentirelynewEFmodelforFixVersion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FixVersions",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        self = c.String(),
                        description = c.String(),
                        name = c.String(),
                        archived = c.Boolean(nullable: false),
                        released = c.Boolean(nullable: false),
                        releaseDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            AddColumn("dbo.JiraIssues", "FixVersion_id", c => c.String(maxLength: 128));
            CreateIndex("dbo.JiraIssues", "FixVersion_id");
            AddForeignKey("dbo.JiraIssues", "FixVersion_id", "dbo.FixVersions", "id");
            DropColumn("dbo.JiraIssues", "FixVersion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JiraIssues", "FixVersion", c => c.String());
            DropForeignKey("dbo.JiraIssues", "FixVersion_id", "dbo.FixVersions");
            DropIndex("dbo.JiraIssues", new[] { "FixVersion_id" });
            DropColumn("dbo.JiraIssues", "FixVersion_id");
            DropTable("dbo.FixVersions");
        }
    }
}
