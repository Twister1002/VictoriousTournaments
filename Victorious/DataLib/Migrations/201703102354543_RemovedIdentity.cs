namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedIdentity : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TournamentRules", new[] { "TournamentRulesID" });
            AlterColumn("dbo.Tournaments", "TournamentRulesID", c => c.Int());
            CreateIndex("dbo.Tournaments", "TournamentRulesID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tournaments", new[] { "TournamentRulesID" });
            AlterColumn("dbo.Tournaments", "TournamentRulesID", c => c.Int(nullable: false));
            CreateIndex("dbo.TournamentRules", "TournamentRulesID");
        }
    }
}
