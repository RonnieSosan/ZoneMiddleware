using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Persistence
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext()
         : base("BlendUpgrade")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserOTP> UserOTP { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
