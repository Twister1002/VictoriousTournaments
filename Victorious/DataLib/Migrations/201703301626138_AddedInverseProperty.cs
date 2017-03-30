namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedInverseProperty : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Matches", "ChallengerID", "dbo.Users");
            DropForeignKey("dbo.Matches", "DefenderID", "dbo.Users");
            AddForeignKey("dbo.Matches", "ChallengerID", "dbo.Users", "UserID", cascadeDelete: false);
            AddForeignKey("dbo.Matches", "DefenderID", "dbo.Users", "UserID", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Matches", "DefenderID", "dbo.Users");
            DropForeignKey("dbo.Matches", "ChallengerID", "dbo.Users");
            AddForeignKey("dbo.Matches", "DefenderID", "dbo.Users", "UserID");
            AddForeignKey("dbo.Matches", "ChallengerID", "dbo.Users", "UserID");
        }
    }
}
