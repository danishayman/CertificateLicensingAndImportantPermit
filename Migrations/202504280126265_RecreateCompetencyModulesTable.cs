namespace CLIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecreateCompetencyModulesTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CompetencyModules", "ModuleName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CompetencyModules", "ModuleName", c => c.String(nullable: false, maxLength: 100));
        }
    }
}
