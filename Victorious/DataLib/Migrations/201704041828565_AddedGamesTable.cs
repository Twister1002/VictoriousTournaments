namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGamesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameModels",
                c => new
                    {
                        GameID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.GameID);
            
            AddColumn("dbo.Users", "SitePermission_Permission", c => c.Int(nullable: false));
            AddColumn("dbo.Tournaments", "GameID", c => c.Int());
            CreateIndex("dbo.Tournaments", "GameID");
            AddForeignKey("dbo.Tournaments", "GameID", "dbo.GameModels", "GameID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tournaments", "GameID", "dbo.GameModels");
            DropIndex("dbo.Tournaments", new[] { "GameID" });
            DropColumn("dbo.Tournaments", "GameID");
            DropColumn("dbo.Users", "SitePermission_Permission");
            DropTable("dbo.GameModels");
        }
    }
}
