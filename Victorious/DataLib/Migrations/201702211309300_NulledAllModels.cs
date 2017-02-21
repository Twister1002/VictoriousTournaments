namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NulledAllModels : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TournamentRules", "BracketID", "dbo.Brackets");
            DropIndex("dbo.TournamentRules", new[] { "BracketID" });
            AlterColumn("dbo.Brackets", "BracketType", c => c.String(maxLength: 50));
            AlterColumn("dbo.Matches", "RoundNumber", c => c.Int());
            AlterColumn("dbo.Matches", "IsBye", c => c.Boolean());
            AlterColumn("dbo.Matches", "StartDateTime", c => c.DateTime());
            AlterColumn("dbo.Matches", "EndDateTime", c => c.DateTime());
            AlterColumn("dbo.Users", "FirstName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "LastName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "Email", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "UserName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "Password", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "CreatedOn", c => c.DateTime());
            AlterColumn("dbo.Users", "LastLogin", c => c.DateTime());
            AlterColumn("dbo.TournamentRules", "TournamentID", c => c.Int());
            AlterColumn("dbo.TournamentRules", "NumberOfRounds", c => c.Int());
            AlterColumn("dbo.TournamentRules", "HasEntryFee", c => c.Boolean());
            AlterColumn("dbo.TournamentRules", "IsPublic", c => c.Boolean());
            AlterColumn("dbo.TournamentRules", "BracketID", c => c.Int());
            AlterColumn("dbo.TournamentRules", "CutoffDate", c => c.DateTime());
            AlterColumn("dbo.TournamentRules", "StartDate", c => c.DateTime());
            AlterColumn("dbo.TournamentRules", "EndDate", c => c.DateTime());
            CreateIndex("dbo.TournamentRules", "BracketID");
            AddForeignKey("dbo.TournamentRules", "BracketID", "dbo.Brackets", "BracketID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TournamentRules", "BracketID", "dbo.Brackets");
            DropIndex("dbo.TournamentRules", new[] { "BracketID" });
            AlterColumn("dbo.TournamentRules", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TournamentRules", "StartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TournamentRules", "CutoffDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TournamentRules", "BracketID", c => c.Int(nullable: false));
            AlterColumn("dbo.TournamentRules", "IsPublic", c => c.Boolean(nullable: false));
            AlterColumn("dbo.TournamentRules", "HasEntryFee", c => c.Boolean(nullable: false));
            AlterColumn("dbo.TournamentRules", "NumberOfRounds", c => c.Int(nullable: false));
            AlterColumn("dbo.TournamentRules", "TournamentID", c => c.Int(nullable: false));
            AlterColumn("dbo.Users", "LastLogin", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "CreatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Users", "UserName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Users", "LastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Users", "FirstName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Matches", "EndDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Matches", "StartDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Matches", "IsBye", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Matches", "RoundNumber", c => c.Int(nullable: false));
            AlterColumn("dbo.Brackets", "BracketType", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.TournamentRules", "BracketID");
            AddForeignKey("dbo.TournamentRules", "BracketID", "dbo.Brackets", "BracketID", cascadeDelete: true);
        }
    }
}
