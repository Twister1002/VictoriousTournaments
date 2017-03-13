namespace DataLib.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DataLib.VictoriousDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;


        }

        protected override void Seed(DataLib.VictoriousDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //context.BracketTypes.AddOrUpdate(
            //    b => b.BracketTypeID,
            //    new BracketTypeModel { BracketTypeID = 1, TypeName = "Single Elimination" },
            //    new BracketTypeModel { BracketTypeID = 2, TypeName = "Double Elimination" },
            //    new BracketTypeModel { BracketTypeID = 3, TypeName = "Round Robin" }
            //    );
        }
    }
}
