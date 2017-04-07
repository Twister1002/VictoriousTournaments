namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTournamentRowVersion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tournaments", "RowVersion");
        }
    }
}
