namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedGamesTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.GameModels", newName: "Games");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Games", newName: "GameModels");
        }
    }
}
