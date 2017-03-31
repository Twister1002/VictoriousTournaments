namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveNumberOfGroups : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Brackets", "NumberOfGroups", c => c.Int(nullable: false));
            DropColumn("dbo.BracketTypes", "NumberOfGroups");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BracketTypes", "NumberOfGroups", c => c.Int(nullable: false));
            DropColumn("dbo.Brackets", "NumberOfGroups");
        }
    }
}
