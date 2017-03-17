namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DisabledLazyLoading : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TournamentRules", "CheckInBegins", c => c.DateTime());
            AddColumn("dbo.TournamentRules", "CheckInEnds", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TournamentRules", "CheckInEnds");
            DropColumn("dbo.TournamentRules", "CheckInBegins");
        }
    }
}
