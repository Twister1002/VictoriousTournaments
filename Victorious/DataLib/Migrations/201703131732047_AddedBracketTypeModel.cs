namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBracketTypeModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BracketTypes",
                c => new
                    {
                        BracketTypeID = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                    })
                .PrimaryKey(t => t.BracketTypeID);
            
            AddColumn("dbo.Brackets", "BracketTypeID", c => c.Int(nullable: false));
            DropColumn("dbo.Brackets", "BracketType");
            DropColumn("dbo.TournamentRules", "MatchesBeginAutomatically");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TournamentRules", "MatchesBeginAutomatically", c => c.Boolean());
            AddColumn("dbo.Brackets", "BracketType", c => c.String(maxLength: 50));
            DropColumn("dbo.Brackets", "BracketTypeID");
            DropTable("dbo.BracketTypes");
        }
    }
}
