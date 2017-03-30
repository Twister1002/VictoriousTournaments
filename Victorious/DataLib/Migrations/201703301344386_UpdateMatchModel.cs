namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMatchModel : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Matches", new[] { "ChallengerID" });
            DropIndex("dbo.Matches", new[] { "DefenderID" });
            AlterColumn("dbo.Matches", "ChallengerID", c => c.Int());
            AlterColumn("dbo.Matches", "DefenderID", c => c.Int());
            CreateIndex("dbo.Matches", "ChallengerID");
            CreateIndex("dbo.Matches", "DefenderID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Matches", new[] { "DefenderID" });
            DropIndex("dbo.Matches", new[] { "ChallengerID" });
            AlterColumn("dbo.Matches", "DefenderID", c => c.Int(nullable: false));
            AlterColumn("dbo.Matches", "ChallengerID", c => c.Int(nullable: false));
            CreateIndex("dbo.Matches", "DefenderID");
            CreateIndex("dbo.Matches", "ChallengerID");
        }
    }
}
