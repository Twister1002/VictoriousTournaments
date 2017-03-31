namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReAddTournamentToUserInTournament : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UsersInTournaments", "UserID", "dbo.Users");
            DropPrimaryKey("dbo.UsersInTournaments");
            AddPrimaryKey("dbo.UsersInTournaments", new[] { "UserID", "TournamentID" });
            AddForeignKey("dbo.UsersInTournaments", "UserID", "dbo.Users", "UserID", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersInTournaments", "UserID", "dbo.Users");
            DropPrimaryKey("dbo.UsersInTournaments");
            AddPrimaryKey("dbo.UsersInTournaments", "UserID");
            AddForeignKey("dbo.UsersInTournaments", "UserID", "dbo.Users", "UserID");
        }
    }
}
