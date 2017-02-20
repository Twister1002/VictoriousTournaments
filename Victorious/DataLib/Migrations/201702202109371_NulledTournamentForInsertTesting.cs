namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NulledTournamentForInsertTesting : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tournaments", "CreatedOn", c => c.DateTime());
            AlterColumn("dbo.Tournaments", "CreatedByID", c => c.Int());
            AlterColumn("dbo.Tournaments", "LastEditedOn", c => c.DateTime());
            AlterColumn("dbo.Tournaments", "LastEditedByID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tournaments", "LastEditedByID", c => c.Int(nullable: false));
            AlterColumn("dbo.Tournaments", "LastEditedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Tournaments", "CreatedByID", c => c.Int(nullable: false));
            AlterColumn("dbo.Tournaments", "CreatedOn", c => c.DateTime(nullable: false));
        }
    }
}
