namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TournamentRulesKeyUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TournamentRules", "TournamentID", "dbo.Tournaments");
            DropIndex("dbo.TournamentRules", new[] { "TournamentID" });
            AddColumn("dbo.Tournaments", "TournamentRules_TournamnetRulesID", c => c.Int());
            CreateIndex("dbo.Tournaments", "TournamentRules_TournamnetRulesID");
            AddForeignKey("dbo.Tournaments", "TournamentRules_TournamnetRulesID", "dbo.TournamentRules", "TournamnetRulesID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tournaments", "TournamentRules_TournamnetRulesID", "dbo.TournamentRules");
            DropIndex("dbo.Tournaments", new[] { "TournamentRules_TournamnetRulesID" });
            DropColumn("dbo.Tournaments", "TournamentRules_TournamnetRulesID");
            CreateIndex("dbo.TournamentRules", "TournamentID");
            AddForeignKey("dbo.TournamentRules", "TournamentID", "dbo.Tournaments", "TournamentID");
        }
    }
}
