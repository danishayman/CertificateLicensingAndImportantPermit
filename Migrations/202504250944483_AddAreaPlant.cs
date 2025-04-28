namespace CLIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAreaPlant : DbMigration
    {
        public override void Up()
        {
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
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AreaPlants", "PlantId", "dbo.Plants");
            DropIndex("dbo.AreaPlants", new[] { "PlantId" });
            DropTable("dbo.AreaPlants");
        }
    }
}
