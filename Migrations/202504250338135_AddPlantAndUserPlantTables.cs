namespace CLIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlantAndUserPlantTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserPlants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        PlantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plants", t => t.PlantId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.Plants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlantName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPlants", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPlants", "PlantId", "dbo.Plants");
            DropIndex("dbo.UserPlants", new[] { "PlantId" });
            DropIndex("dbo.UserPlants", new[] { "UserId" });
            DropTable("dbo.Plants");
            DropTable("dbo.UserPlants");
        }
    }
}
