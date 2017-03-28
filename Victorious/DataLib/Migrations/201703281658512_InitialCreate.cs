namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Brackets",
                c => new
                    {
                        BracketID = c.Int(nullable: false, identity: true),
                        BracketTitle = c.String(),
                        BracketTypeID = c.Int(nullable: false),
                        Finalized = c.Boolean(nullable: false),
                        Tournament_TournamentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BracketID)
                .ForeignKey("dbo.BracketTypes", t => t.BracketTypeID, cascadeDelete: true)
                .ForeignKey("dbo.Tournaments", t => t.Tournament_TournamentID)
                .Index(t => t.BracketTypeID)
                .Index(t => t.Tournament_TournamentID);
            
            CreateTable(
                "dbo.BracketTypes",
                c => new
                    {
                        BracketTypeID = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BracketTypeID);
            
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
                .ForeignKey("dbo.Brackets", t => t.BracketModel_BracketID)
                .Index(t => t.ChallengerID)
                .Index(t => t.DefenderID)
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
                "dbo.Teams",
                c => new
                    {
                        TeamID = c.Int(nullable: false, identity: true),
                        TeamName = c.String(),
                        CreatedOn = c.DateTime(),
                        UserModel_UserID = c.Int(),
                        TournamentModel_TournamentID = c.Int(),
                    })
                .PrimaryKey(t => t.TeamID)
                .ForeignKey("dbo.Users", t => t.UserModel_UserID)
                .ForeignKey("dbo.Tournaments", t => t.TournamentModel_TournamentID)
                .Index(t => t.UserModel_UserID)
                .Index(t => t.TournamentModel_TournamentID);
            
            CreateTable(
                "dbo.TeamMembers",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        TeamID = c.Int(nullable: false),
                        Permission = c.Int(),
                        DateJoined = c.DateTime(),
                        DateLeft = c.DateTime(),
                        TeamModel_TeamID = c.Int(),
                    })
                .PrimaryKey(t => new { t.UserID, t.TeamID })
                .ForeignKey("dbo.Teams", t => t.TeamID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamModel_TeamID)
                .Index(t => t.UserID)
                .Index(t => t.TeamID)
                .Index(t => t.TeamModel_TeamID);
            
            CreateTable(
                "dbo.Tournaments",
                c => new
                    {
                        TournamentID = c.Int(nullable: false, identity: true),
                        TournamentRulesID = c.Int(),
                        Title = c.String(nullable: false),
                        Description = c.String(unicode: false, storeType: "text"),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByID = c.Int(nullable: false),
                        WinnerID = c.Int(nullable: false),
                        LastEditedOn = c.DateTime(nullable: false),
                        LastEditedByID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TournamentID)
                .ForeignKey("dbo.TournamentRules", t => t.TournamentRulesID)
                .Index(t => t.TournamentRulesID);
            
            CreateTable(
                "dbo.TournamentRules",
                c => new
                    {
                        TournamentRulesID = c.Int(nullable: false, identity: true),
                        TournamentID = c.Int(nullable: false),
                        NumberOfRounds = c.Int(nullable: false),
                        HasEntryFee = c.Boolean(nullable: false),
                        EntryFee = c.Decimal(storeType: "money"),
                        PrizePurse = c.Decimal(nullable: false, storeType: "money"),
                        IsPublic = c.Boolean(nullable: false),
                        RegistrationStartDate = c.DateTime(),
                        RegistrationEndDate = c.DateTime(),
                        TournamentStartDate = c.DateTime(),
                        TournamentEndDate = c.DateTime(),
                        CheckInBegins = c.DateTime(),
                        CheckInEnds = c.DateTime(),
                    })
                .PrimaryKey(t => t.TournamentRulesID);
            
            CreateTable(
                "dbo.UserBracketSeeds",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        TournamentID = c.Int(nullable: false),
                        BracketID = c.Int(nullable: false),
                        Seed = c.Int(nullable: false),
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
                "dbo.UsersInTournaments",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        TournamentID = c.Int(nullable: false),
                        Permission = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserID, t.TournamentID })
                .ForeignKey("dbo.Tournaments", t => t.TournamentID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.TournamentID);
            
            CreateTable(
                "dbo.TournamentModelUserModels",
                c => new
                    {
                        TournamentModel_TournamentID = c.Int(nullable: false),
                        UserModel_UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TournamentModel_TournamentID, t.UserModel_UserID })
                .ForeignKey("dbo.Tournaments", t => t.TournamentModel_TournamentID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserModel_UserID, cascadeDelete: true)
                .Index(t => t.TournamentModel_TournamentID)
                .Index(t => t.UserModel_UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersInTournaments", "UserID", "dbo.Users");
            DropForeignKey("dbo.UsersInTournaments", "TournamentID", "dbo.Tournaments");
            DropForeignKey("dbo.UserBracketSeeds", "BracketModel_BracketID", "dbo.Brackets");
            DropForeignKey("dbo.UserBracketSeeds", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserBracketSeeds", "TournamentID", "dbo.Tournaments");
            DropForeignKey("dbo.UserBracketSeeds", "BracketID", "dbo.Brackets");
            DropForeignKey("dbo.Brackets", "Tournament_TournamentID", "dbo.Tournaments");
            DropForeignKey("dbo.Matches", "BracketModel_BracketID", "dbo.Brackets");
            DropForeignKey("dbo.Matches", "DefenderID", "dbo.Users");
            DropForeignKey("dbo.Matches", "ChallengerID", "dbo.Users");
            DropForeignKey("dbo.TournamentModelUserModels", "UserModel_UserID", "dbo.Users");
            DropForeignKey("dbo.TournamentModelUserModels", "TournamentModel_TournamentID", "dbo.Tournaments");
            DropForeignKey("dbo.Tournaments", "TournamentRulesID", "dbo.TournamentRules");
            DropForeignKey("dbo.Teams", "TournamentModel_TournamentID", "dbo.Tournaments");
            DropForeignKey("dbo.Teams", "UserModel_UserID", "dbo.Users");
            DropForeignKey("dbo.TeamMembers", "TeamModel_TeamID", "dbo.Teams");
            DropForeignKey("dbo.TeamMembers", "UserID", "dbo.Users");
            DropForeignKey("dbo.TeamMembers", "TeamID", "dbo.Teams");
            DropForeignKey("dbo.Brackets", "BracketTypeID", "dbo.BracketTypes");
            DropIndex("dbo.TournamentModelUserModels", new[] { "UserModel_UserID" });
            DropIndex("dbo.TournamentModelUserModels", new[] { "TournamentModel_TournamentID" });
            DropIndex("dbo.UsersInTournaments", new[] { "TournamentID" });
            DropIndex("dbo.UsersInTournaments", new[] { "UserID" });
            DropIndex("dbo.UserBracketSeeds", new[] { "BracketModel_BracketID" });
            DropIndex("dbo.UserBracketSeeds", new[] { "BracketID" });
            DropIndex("dbo.UserBracketSeeds", new[] { "TournamentID" });
            DropIndex("dbo.UserBracketSeeds", new[] { "UserID" });
            DropIndex("dbo.Tournaments", new[] { "TournamentRulesID" });
            DropIndex("dbo.TeamMembers", new[] { "TeamModel_TeamID" });
            DropIndex("dbo.TeamMembers", new[] { "TeamID" });
            DropIndex("dbo.TeamMembers", new[] { "UserID" });
            DropIndex("dbo.Teams", new[] { "TournamentModel_TournamentID" });
            DropIndex("dbo.Teams", new[] { "UserModel_UserID" });
            DropIndex("dbo.Matches", new[] { "BracketModel_BracketID" });
            DropIndex("dbo.Matches", new[] { "DefenderID" });
            DropIndex("dbo.Matches", new[] { "ChallengerID" });
            DropIndex("dbo.Brackets", new[] { "Tournament_TournamentID" });
            DropIndex("dbo.Brackets", new[] { "BracketTypeID" });
            DropTable("dbo.TournamentModelUserModels");
            DropTable("dbo.UsersInTournaments");
            DropTable("dbo.UserBracketSeeds");
            DropTable("dbo.TournamentRules");
            DropTable("dbo.Tournaments");
            DropTable("dbo.TeamMembers");
            DropTable("dbo.Teams");
            DropTable("dbo.Users");
            DropTable("dbo.Matches");
            DropTable("dbo.BracketTypes");
            DropTable("dbo.Brackets");
        }
    }
}
