namespace CLIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CombinedChanges : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.CompetencyModules", new[] { "ModuleName" });
            CreateTable(
                "dbo.AreaPlants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AreaName = c.String(nullable: false, maxLength: 100),
                        PlantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plants", t => t.PlantId, cascadeDelete: true)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.MonitoringModules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false, maxLength: 100),
                        Type = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            AlterColumn("dbo.CompetencyModules", "ModuleName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.CompetencyModules", "ValidityMonths", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AreaPlants", "PlantId", "dbo.Plants");
            DropIndex("dbo.AreaPlants", new[] { "PlantId" });
            AlterColumn("dbo.CompetencyModules", "ValidityMonths", c => c.Int());
            AlterColumn("dbo.CompetencyModules", "ModuleName", c => c.String(nullable: false, maxLength: 256));
            DropTable("dbo.MonitoringModules");
            DropTable("dbo.AreaPlants");
            CreateIndex("dbo.CompetencyModules", "ModuleName", unique: true);
        }
    }
}
