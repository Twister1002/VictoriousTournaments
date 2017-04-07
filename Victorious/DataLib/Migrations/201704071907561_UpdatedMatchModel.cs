namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedMatchModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "MaxGames", c => c.Int());
            DropColumn("dbo.Matches", "WinsNeeded");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Matches", "WinsNeeded", c => c.Int());
            DropColumn("dbo.Matches", "MaxGames");
        }
    }
}
