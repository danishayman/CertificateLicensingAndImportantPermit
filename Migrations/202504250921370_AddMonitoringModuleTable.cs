namespace CLIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMonitoringModuleTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MonitoringModules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false, maxLength: 100),
                        Type = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
           

            DropTable("dbo.MonitoringModules");
           
        }
    }
}
