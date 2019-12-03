namespace HomeOwnerRentEstimates.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<HomeOwnerRentEstimates.Models.HomeOwnerDatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "HomeOwnerRentEstimates.Models.HomeOwnerDatabaseContext";
        }

        protected override void Seed(HomeOwnerRentEstimates.Models.HomeOwnerDatabaseContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
