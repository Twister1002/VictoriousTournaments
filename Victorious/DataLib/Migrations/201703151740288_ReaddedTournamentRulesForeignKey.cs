namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReaddedTournamentRulesForeignKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "TournamentRulesID", c => c.Int());
            AddColumn("dbo.TournamentRules", "TournamentID", c => c.Int());
            CreateIndex("dbo.Tournaments", "TournamentRulesID");
            AddForeignKey("dbo.Tournaments", "TournamentRulesID", "dbo.TournamentRules", "TournamentRulesID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tournaments", "TournamentRulesID", "dbo.TournamentRules");
            DropIndex("dbo.Tournaments", new[] { "TournamentRulesID" });
            DropColumn("dbo.TournamentRules", "TournamentID");
            DropColumn("dbo.Tournaments", "TournamentRulesID");
        }
    }
}
