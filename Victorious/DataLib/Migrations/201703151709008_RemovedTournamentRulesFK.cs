namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedTournamentRulesFK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tournaments", "TournamentRulesID", "dbo.TournamentRules");
            DropIndex("dbo.Tournaments", new[] { "TournamentRulesID" });
            DropColumn("dbo.Tournaments", "TournamentRulesID");
            DropColumn("dbo.TournamentRules", "TournamentID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TournamentRules", "TournamentID", c => c.Int());
            AddColumn("dbo.Tournaments", "TournamentRulesID", c => c.Int());
            CreateIndex("dbo.Tournaments", "TournamentRulesID");
            AddForeignKey("dbo.Tournaments", "TournamentRulesID", "dbo.TournamentRules", "TournamentRulesID");
        }
    }
}
