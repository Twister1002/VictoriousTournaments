namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBracketTypeEnum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BracketTypes", "Type", c => c.Int(nullable: false));
            CreateIndex("dbo.Brackets", "BracketTypeID");
            AddForeignKey("dbo.Brackets", "BracketTypeID", "dbo.BracketTypes", "BracketTypeID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Brackets", "BracketTypeID", "dbo.BracketTypes");
            DropIndex("dbo.Brackets", new[] { "BracketTypeID" });
            DropColumn("dbo.BracketTypes", "Type");
        }
    }
}
