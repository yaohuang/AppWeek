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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}