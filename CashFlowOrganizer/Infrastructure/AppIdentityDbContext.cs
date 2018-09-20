using CashFlowOrganizer.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CashFlowOrganizer.Infrastructure
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext() : base("CashFlowDB")
        {
            Database.SetInitializer<AppIdentityDbContext>(new TestAuthDBInit());
        }

        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }

    public class TestAuthDBInit : NullDatabaseInitializer<AppIdentityDbContext>
    {
        /* 
            ONLY US THE BELOW METHODS IN ORDER TO DROP THE CURRENT DATABASE (AND ALL
            OF ITS DATA) AND CREATE A NEW DATABASE WITH AN UPDATED TABLE STRUCTURE.
            ALSO THIS CLASS WILL NEED TO INHEIRIT FROM:  DropCreateDatabaseIfModelChanges<AppIdentityDbContext>

            INHEIRIT FROM NullDatabaseInitializer<AppIdentityDbContext> WHEN YOU DO NOT WANT 
            THE DATABASE TO BE CHANGED
        */

        //protected override void Seed(AppIdentityDbContext context)
        //{
        //    PerformInitialSetup(context);
        //    base.Seed(context);
        //}

        //public void PerformInitialSetup(AppIdentityDbContext context)
        //{

        //}
    }
}