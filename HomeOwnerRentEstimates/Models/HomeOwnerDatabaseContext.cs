using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HomeOwnerRentEstimates.Models
{
    public class HomeOwnerDatabaseContext :DbContext
    {
        public HomeOwnerDatabaseContext():base("Home Owners Rent Estimate DB Connection")
        {

        }
        public DbSet<HomeOwner> Owners { get; set; }
    }
}