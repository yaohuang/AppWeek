using System;
using System.Data.Entity;
using System.Data.Entity.Config;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace LunchBuddies.Models
{
    [DbConfigurationType(typeof(ModelsDbConfiguration))]
    public class ModelsDbContext : DbContext
    {
        public DbSet<Interest> Interests { get; set; }
        
        public DbSet<LunchRequest> LunchRequests { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLunchTimes> UserLunchTimes { get; set; }
        public DbSet<UserPictures> UserPictures { get; set; }
        public DbSet<UserLunchRequest> UserLunchRequests { get; set; }

        public DbSet<PendingRegistration> PendingRegistrations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // set the default storage type of all DateTime columns to datetime2
            modelBuilder.Properties<DateTime>().Configure(p => p
                .HasColumnType("datetime2"));

            Database.SetInitializer(new System.Data.Entity.MigrateDatabaseToLatestVersion<ModelsDbContext, Migrations.Configuration>());
        }
    }
}