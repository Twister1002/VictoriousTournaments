namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecondMatchModelUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "CreatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "LastLogin", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "LastLogin", c => c.DateTime());
            AlterColumn("dbo.Users", "CreatedOn", c => c.DateTime());
        }
    }
}
