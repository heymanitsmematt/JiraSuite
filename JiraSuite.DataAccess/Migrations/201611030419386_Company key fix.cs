namespace JiraSuite.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Companykeyfix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Columns", "company_id", "dbo.Companies");
            DropIndex("dbo.Columns", new[] { "company_id" });
            RenameColumn(table: "dbo.Columns", name: "company_id", newName: "company_name");
            DropPrimaryKey("dbo.Companies");
            AlterColumn("dbo.Columns", "company_name", c => c.String(maxLength: 128));
            AlterColumn("dbo.Companies", "name", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Companies", "name");
            CreateIndex("dbo.Columns", "company_name");
            AddForeignKey("dbo.Columns", "company_name", "dbo.Companies", "name");
            DropColumn("dbo.Companies", "id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Companies", "id", c => c.Int(nullable: false));
            DropForeignKey("dbo.Columns", "company_name", "dbo.Companies");
            DropIndex("dbo.Columns", new[] { "company_name" });
            DropPrimaryKey("dbo.Companies");
            AlterColumn("dbo.Companies", "name", c => c.String());
            AlterColumn("dbo.Columns", "company_name", c => c.Int());
            AddPrimaryKey("dbo.Companies", "id");
            RenameColumn(table: "dbo.Columns", name: "company_name", newName: "company_id");
            CreateIndex("dbo.Columns", "company_id");
            AddForeignKey("dbo.Columns", "company_id", "dbo.Companies", "id");
        }
    }
}
