namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeInverses : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Matches", "ChallengerID", "dbo.Users");
            DropForeignKey("dbo.Matches", "DefenderID", "dbo.Users");
            DropIndex("dbo.Matches", new[] { "ChallengerID" });
            DropIndex("dbo.Matches", new[] { "DefenderID" });
            AddColumn("dbo.Matches", "Challenger_UserID", c => c.Int());
            AddColumn("dbo.Matches", "Defender_UserID", c => c.Int());
            CreateIndex("dbo.Matches", "Challenger_UserID");
            CreateIndex("dbo.Matches", "Defender_UserID");
            AddForeignKey("dbo.Matches", "Challenger_UserID", "dbo.Users", "UserID");
            AddForeignKey("dbo.Matches", "Defender_UserID", "dbo.Users", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Matches", "Defender_UserID", "dbo.Users");
            DropForeignKey("dbo.Matches", "Challenger_UserID", "dbo.Users");
            DropIndex("dbo.Matches", new[] { "Defender_UserID" });
            DropIndex("dbo.Matches", new[] { "Challenger_UserID" });
            DropColumn("dbo.Matches", "Defender_UserID");
            DropColumn("dbo.Matches", "Challenger_UserID");
            CreateIndex("dbo.Matches", "DefenderID");
            CreateIndex("dbo.Matches", "ChallengerID");
            AddForeignKey("dbo.Matches", "DefenderID", "dbo.Users", "UserID", cascadeDelete: true);
            AddForeignKey("dbo.Matches", "ChallengerID", "dbo.Users", "UserID", cascadeDelete: true);
        }
    }
}
