using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.DefaultImplementation.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
         : base("BlendUpgrade")
        {
            //Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }

        public DbSet<ApiSecuritySpec> APISecuritySpecs { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserOTP> UserOTP { get; set; }
        public DbSet<UserBeneficiaries> UserBeneficiaries { get; set; }

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
