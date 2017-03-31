namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedUsersInTournaments : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UsersInTournaments", "TournamentID", "dbo.Tournaments");
            DropForeignKey("dbo.UsersInTournaments", "UserID", "dbo.Users");
            
            AddColumn("dbo.BracketTypes", "NumberOfGroups", c => c.Int(nullable: false));
           
        }
        
        public override void Down()
        {
           
            
            DropForeignKey("dbo.UsersInTournaments", "TournamentID", "dbo.Tournaments");
            DropForeignKey("dbo.UsersInTournaments", "UserID", "dbo.Users");
            DropColumn("dbo.BracketTypes", "NumberOfGroups");
        
            AddForeignKey("dbo.UsersInTournaments", "UserID", "dbo.Users", "UserID", cascadeDelete: true);
            AddForeignKey("dbo.UsersInTournaments", "TournamentID", "dbo.Tournaments", "TournamentID", cascadeDelete: true);
        }
    }
}
