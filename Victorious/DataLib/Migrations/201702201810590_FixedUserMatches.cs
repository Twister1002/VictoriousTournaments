namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedUserMatches : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TournamentRules", "BracketID", "dbo.Brackets");
            DropIndex("dbo.Matches", new[] { "ChallengerID" });
            DropIndex("dbo.Matches", new[] { "DefenderID" });
            AddColumn("dbo.Matches", "IsBye", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Matches", "ChallengerID", c => c.Int());
            AlterColumn("dbo.Matches", "DefenderID", c => c.Int());
            CreateIndex("dbo.Matches", "ChallengerID");
            CreateIndex("dbo.Matches", "DefenderID");
            AddForeignKey("dbo.TournamentRules", "BracketID", "dbo.Brackets", "BracketID", cascadeDelete: true);
            DropColumn("dbo.TournamentRules", "HasByes");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TournamentRules", "HasByes", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.TournamentRules", "BracketID", "dbo.Brackets");
            DropIndex("dbo.Matches", new[] { "DefenderID" });
            DropIndex("dbo.Matches", new[] { "ChallengerID" });
            AlterColumn("dbo.Matches", "DefenderID", c => c.Int(nullable: false));
            AlterColumn("dbo.Matches", "ChallengerID", c => c.Int(nullable: false));
            DropColumn("dbo.Matches", "IsBye");
            CreateIndex("dbo.Matches", "DefenderID");
            CreateIndex("dbo.Matches", "ChallengerID");
            AddForeignKey("dbo.TournamentRules", "BracketID", "dbo.Brackets", "BracketID");
        }
    }
}
