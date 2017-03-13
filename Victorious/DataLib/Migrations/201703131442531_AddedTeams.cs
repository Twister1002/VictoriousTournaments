namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTeams : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        TeamID = c.Int(nullable: false, identity: true),
                        TeamName = c.String(),
                        CreatedOn = c.DateTime(),
                        TournamentModel_TournamentID = c.Int(),
                    })
                .PrimaryKey(t => t.TeamID)
                .ForeignKey("dbo.Tournaments", t => t.TournamentModel_TournamentID)
                .Index(t => t.TournamentModel_TournamentID);
            
            CreateTable(
                "dbo.TeamMembers",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        TeamID = c.Int(nullable: false),
                        Role = c.Int(),
                        DateJoined = c.DateTime(),
                        DateLeft = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.UserID, t.TeamID })
                .ForeignKey("dbo.Teams", t => t.TeamID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.TeamID);
            
            AddColumn("dbo.TournamentRules", "MatchesBeginAutomatically", c => c.Boolean());
            AddColumn("dbo.TournamentRules", "RegistrationStartDate", c => c.DateTime());
            AddColumn("dbo.TournamentRules", "RegistrationEndDate", c => c.DateTime());
            AddColumn("dbo.TournamentRules", "TournamentStartDate", c => c.DateTime());
            AddColumn("dbo.TournamentRules", "TournamentEndDate", c => c.DateTime());
            DropColumn("dbo.TournamentRules", "NumberOfPlayers");
            DropColumn("dbo.TournamentRules", "CutoffDate");
            DropColumn("dbo.TournamentRules", "StartDate");
            DropColumn("dbo.TournamentRules", "EndDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TournamentRules", "EndDate", c => c.DateTime());
            AddColumn("dbo.TournamentRules", "StartDate", c => c.DateTime());
            AddColumn("dbo.TournamentRules", "CutoffDate", c => c.DateTime());
            AddColumn("dbo.TournamentRules", "NumberOfPlayers", c => c.Int());
            DropForeignKey("dbo.Teams", "TournamentModel_TournamentID", "dbo.Tournaments");
            DropForeignKey("dbo.TeamMembers", "UserID", "dbo.Users");
            DropForeignKey("dbo.TeamMembers", "TeamID", "dbo.Teams");
            DropIndex("dbo.TeamMembers", new[] { "TeamID" });
            DropIndex("dbo.TeamMembers", new[] { "UserID" });
            DropIndex("dbo.Teams", new[] { "TournamentModel_TournamentID" });
            DropColumn("dbo.TournamentRules", "TournamentEndDate");
            DropColumn("dbo.TournamentRules", "TournamentStartDate");
            DropColumn("dbo.TournamentRules", "RegistrationEndDate");
            DropColumn("dbo.TournamentRules", "RegistrationStartDate");
            DropColumn("dbo.TournamentRules", "MatchesBeginAutomatically");
            DropTable("dbo.TeamMembers");
            DropTable("dbo.Teams");
        }
    }
}
