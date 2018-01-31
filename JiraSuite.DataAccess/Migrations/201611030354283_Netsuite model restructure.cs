namespace JiraSuite.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Netsuitemodelrestructure : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Columns", "company_id", "dbo.Companies");
            DropPrimaryKey("dbo.NetsuiteApiResults");
            DropPrimaryKey("dbo.Companies");
            AlterColumn("dbo.NetsuiteApiResults", "id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Companies", "id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.NetsuiteApiResults", "id");
            AddPrimaryKey("dbo.Companies", "id");
            AddForeignKey("dbo.Columns", "company_id", "dbo.Companies", "id");
            DropColumn("dbo.NetsuiteApiResults", "InternalID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NetsuiteApiResults", "InternalID", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Columns", "company_id", "dbo.Companies");
            DropPrimaryKey("dbo.Companies");
            DropPrimaryKey("dbo.NetsuiteApiResults");
            AlterColumn("dbo.Companies", "id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.NetsuiteApiResults", "id", c => c.String());
            AddPrimaryKey("dbo.Companies", "id");
            AddPrimaryKey("dbo.NetsuiteApiResults", "InternalID");
            AddForeignKey("dbo.Columns", "company_id", "dbo.Companies", "id");
        }
    }
}
