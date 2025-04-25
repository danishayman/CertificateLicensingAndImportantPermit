namespace CLIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompetencyTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompetencyModules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModuleName = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        ValidityMonths = c.Int(nullable: false),
                        IsMandatory = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserCompetencies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        CompetencyModuleId = c.Int(nullable: false),
                        Status = c.String(nullable: false, maxLength: 50),
                        CompletionDate = c.DateTime(storeType: "date"),
                        ExpiryDate = c.DateTime(storeType: "date"),
                        Remarks = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CompetencyModules", t => t.CompetencyModuleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CompetencyModuleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserCompetencies", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserCompetencies", "CompetencyModuleId", "dbo.CompetencyModules");
            DropIndex("dbo.UserCompetencies", new[] { "CompetencyModuleId" });
            DropIndex("dbo.UserCompetencies", new[] { "UserId" });
            DropTable("dbo.UserCompetencies");
            DropTable("dbo.CompetencyModules");
        }
    }
}
