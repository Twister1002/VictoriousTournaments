namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPlatformEnum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TournamentRules", "Platform", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TournamentRules", "Platform");
        }
    }
}
