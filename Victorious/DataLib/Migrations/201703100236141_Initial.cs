namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Brackets",
                c => new
                    {
                        BracketID = c.Int(nullable: false, identity: true),
                        BracketTitle = c.String(),
                        BracketType = c.String(maxLength: 50),
                        Tournament_TournamentID = c.Int(),
                    })
                .PrimaryKey(t => t.BracketID)
                .ForeignKey("dbo.Tournaments", t => t.Tournament_TournamentID)
                .Index(t => t.Tournament_TournamentID);
            
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        MatchID = c.Int(nullable: false, identity: true),
                        ChallengerID = c.Int(),
                        DefenderID = c.Int(),
                        WinnerID = c.Int(),
                        ChallengerScore = c.Int(),
                        DefenderScore = c.Int(),
                        RoundIndex = c.Int(),
                        MatchNumber = c.Int(nullable: false),
                        IsBye = c.Boolean(),
                        StartDateTime = c.DateTime(),
                        EndDateTime = c.DateTime(),
                        MatchDuration = c.Time(precision: 7),
                        WinsNeeded = c.Int(),
                        MatchIndex = c.Int(),
                        NextMatchNumber = c.Int(),
                        PrevMatchIndex = c.Int(),
                        NextLoserMatchNumber = c.Int(),
                        PrevDefenderMatchNumber = c.Int(),
                        PrevChallengerMatchNumber = c.Int(),
                        BracketModel_BracketID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MatchID)
                .ForeignKey("dbo.Users", t => t.ChallengerID)
                .ForeignKey("dbo.Users", t => t.DefenderID)
                .ForeignKey("dbo.Users", t => t.WinnerID)
                .ForeignKey("dbo.Brackets", t => t.BracketModel_BracketID)
                .Index(t => t.ChallengerID)
                .Index(t => t.DefenderID)
                .Index(t => t.WinnerID)
                .Index(t => t.BracketModel_BracketID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        Email = c.String(maxLength: 50),
                        Username = c.String(maxLength: 50),
                        Password = c.String(maxLength: 50),
                        PhoneNumber = c.String(maxLength: 15, fixedLength: true),
                        CreatedOn = c.DateTime(),
                        LastLogin = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.Tournaments",
                c => new
                    {
                        TournamentID = c.Int(nullable: false, identity: true),
                        TournamentRulesID = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        Description = c.String(unicode: false, storeType: "text"),
                        CreatedOn = c.DateTime(),
                        CreatedByID = c.Int(),
                        WinnerID = c.Int(),
                        LastEditedOn = c.DateTime(),
                        LastEditedByID = c.Int(),
                    })
                .PrimaryKey(t => t.TournamentID);
            
            CreateTable(
                "dbo.TournamentRules",
                c => new
                    {
                        TournamentRulesID = c.Int(nullable: false, identity: true),
                        TournamentID = c.Int(),
                        NumberOfRounds = c.Int(),
                        HasEntryFee = c.Boolean(),
                        EntryFee = c.Decimal(storeType: "money"),
                        PrizePurse = c.Decimal(storeType: "money"),
                        NumberOfPlayers = c.Int(),
                        IsPublic = c.Boolean(),
                        CutoffDate = c.DateTime(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.TournamentRulesID)
                .ForeignKey("dbo.Tournaments", t => t.TournamentRulesID)
                .Index(t => t.TournamentRulesID);
            
            CreateTable(
                "dbo.UserBracketSeeds",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        TournamentID = c.Int(nullable: false),
                        BracketID = c.Int(nullable: false),
                        Seed = c.Int(),
                        BracketModel_BracketID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserID, t.TournamentID, t.BracketID })
                .ForeignKey("dbo.Brackets", t => t.BracketID, cascadeDelete: true)
                .ForeignKey("dbo.Tournaments", t => t.TournamentID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.Brackets", t => t.BracketModel_BracketID)
                .Index(t => t.UserID)
                .Index(t => t.TournamentID)
                .Index(t => t.BracketID)
                .Index(t => t.BracketModel_BracketID);
            
            CreateTable(
                "dbo.UsersTournaments",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        TournamentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserID, t.TournamentID })
                .ForeignKey("dbo.Tournaments", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.TournamentID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.TournamentID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserBracketSeeds", "BracketModel_BracketID", "dbo.Brackets");
            DropForeignKey("dbo.UserBracketSeeds", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserBracketSeeds", "TournamentID", "dbo.Tournaments");
            DropForeignKey("dbo.UserBracketSeeds", "BracketID", "dbo.Brackets");
            DropForeignKey("dbo.Matches", "BracketModel_BracketID", "dbo.Brackets");
            DropForeignKey("dbo.Matches", "WinnerID", "dbo.Users");
            DropForeignKey("dbo.Matches", "DefenderID", "dbo.Users");
            DropForeignKey("dbo.Matches", "ChallengerID", "dbo.Users");
            DropForeignKey("dbo.UsersTournaments", "TournamentID", "dbo.Users");
            DropForeignKey("dbo.UsersTournaments", "UserID", "dbo.Tournaments");
            DropForeignKey("dbo.TournamentRules", "TournamentRulesID", "dbo.Tournaments");
            DropForeignKey("dbo.Brackets", "Tournament_TournamentID", "dbo.Tournaments");
            DropIndex("dbo.UsersTournaments", new[] { "TournamentID" });
            DropIndex("dbo.UsersTournaments", new[] { "UserID" });
            DropIndex("dbo.UserBracketSeeds", new[] { "BracketModel_BracketID" });
            DropIndex("dbo.UserBracketSeeds", new[] { "BracketID" });
            DropIndex("dbo.UserBracketSeeds", new[] { "TournamentID" });
            DropIndex("dbo.UserBracketSeeds", new[] { "UserID" });
            DropIndex("dbo.TournamentRules", new[] { "TournamentRulesID" });
            DropIndex("dbo.Matches", new[] { "BracketModel_BracketID" });
            DropIndex("dbo.Matches", new[] { "WinnerID" });
            DropIndex("dbo.Matches", new[] { "DefenderID" });
            DropIndex("dbo.Matches", new[] { "ChallengerID" });
            DropIndex("dbo.Brackets", new[] { "Tournament_TournamentID" });
            DropTable("dbo.UsersTournaments");
            DropTable("dbo.UserBracketSeeds");
            DropTable("dbo.TournamentRules");
            DropTable("dbo.Tournaments");
            DropTable("dbo.Users");
            DropTable("dbo.Matches");
            DropTable("dbo.Brackets");
        }
    }
}
