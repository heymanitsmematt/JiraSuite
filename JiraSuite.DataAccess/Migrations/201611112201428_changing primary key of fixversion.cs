namespace JiraSuite.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changingprimarykeyoffixversion : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.FixVersions");
            AddColumn("dbo.FixVersions", "InternalId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.FixVersions", "id", c => c.String());
            AddPrimaryKey("dbo.FixVersions", "InternalId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.FixVersions");
            AlterColumn("dbo.FixVersions", "id", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.FixVersions", "InternalId");
            AddPrimaryKey("dbo.FixVersions", "id");
        }
    }
}
