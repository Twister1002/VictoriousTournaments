namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGames : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tournaments", "GameID", "dbo.Games");
            DropIndex("dbo.Tournaments", new[] { "GameID" });
            CreateTable(
                "dbo.GameTypes",
                c => new
                    {
                        GameTypeID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.GameTypeID);
            
            AddColumn("dbo.Tournaments", "GameTypeID", c => c.Int());
            AddColumn("dbo.Games", "ChallengerID", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "DefenderID", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "WinnerID", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "MatchID", c => c.Int());
            AddColumn("dbo.Games", "GameNumber", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "ChallengerScore", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "DefenderScore", c => c.Int(nullable: false));
            CreateIndex("dbo.Tournaments", "GameTypeID");
            CreateIndex("dbo.Games", "MatchID");
            AddForeignKey("dbo.Tournaments", "GameTypeID", "dbo.GameTypes", "GameTypeID");
            AddForeignKey("dbo.Games", "MatchID", "dbo.Matches", "MatchID");
            DropColumn("dbo.Tournaments", "GameID");
            DropColumn("dbo.Games", "Title");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "Title", c => c.String());
            AddColumn("dbo.Tournaments", "GameID", c => c.Int());
            DropForeignKey("dbo.Games", "MatchID", "dbo.Matches");
            DropForeignKey("dbo.Tournaments", "GameTypeID", "dbo.GameTypes");
            DropIndex("dbo.Games", new[] { "MatchID" });
            DropIndex("dbo.Tournaments", new[] { "GameTypeID" });
            DropColumn("dbo.Games", "DefenderScore");
            DropColumn("dbo.Games", "ChallengerScore");
            DropColumn("dbo.Games", "GameNumber");
            DropColumn("dbo.Games", "MatchID");
            DropColumn("dbo.Games", "WinnerID");
            DropColumn("dbo.Games", "DefenderID");
            DropColumn("dbo.Games", "ChallengerID");
            DropColumn("dbo.Tournaments", "GameTypeID");
            DropTable("dbo.GameTypes");
            CreateIndex("dbo.Tournaments", "GameID");
            AddForeignKey("dbo.Tournaments", "GameID", "dbo.Games", "GameID");
        }
    }
}
