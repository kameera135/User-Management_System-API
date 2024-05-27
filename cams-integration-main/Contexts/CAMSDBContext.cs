using SITCAMSClientIntegration.Models;
using Microsoft.EntityFrameworkCore;

namespace SITCAMSClientIntegration.Contexts
{
    public class CAMSDBContext : DbContext
    {
        public CAMSDBContext(DbContextOptions<CAMSDBContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<EndPointRank> EndPointRanks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");
            base.OnModelCreating(modelBuilder);

        }

    }
}
