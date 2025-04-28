namespace CLIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeCompetencyNameUnique : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CompetencyModules", "ModuleName", c => c.String(nullable: false, maxLength: 256));
            CreateIndex("dbo.CompetencyModules", "ModuleName", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.CompetencyModules", new[] { "ModuleName" });
            AlterColumn("dbo.CompetencyModules", "ModuleName", c => c.String(nullable: false));
        }
    }
}
