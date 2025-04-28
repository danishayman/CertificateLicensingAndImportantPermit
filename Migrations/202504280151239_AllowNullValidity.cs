namespace CLIP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllowNullValidity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CompetencyModules", "ValidityMonths", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CompetencyModules", "ValidityMonths", c => c.Int(nullable: false));
        }
    }
}
