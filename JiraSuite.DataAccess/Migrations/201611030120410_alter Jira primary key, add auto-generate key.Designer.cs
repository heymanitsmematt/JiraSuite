// <auto-generated />
namespace JiraSuite.DataAccess.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.1.3-40302")]
    public sealed partial class alterJiraprimarykeyaddautogeneratekey : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(alterJiraprimarykeyaddautogeneratekey));
        
        string IMigrationMetadata.Id
        {
            get { return "201611030120410_alter Jira primary key, add auto-generate key"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}