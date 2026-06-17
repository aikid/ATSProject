using Microsoft.EntityFrameworkCore;
using Domain.Model;

namespace Api.Data
{
    public class AtsDbContext : DbContext
    {
        public AtsDbContext(DbContextOptions<AtsDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<CandidateProfile> CandidateProfiles { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Application> Applications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Application>()
                .HasIndex(a => new { a.JobId, a.CandidateUserId })
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.CandidateProfile)
                .WithOne(p => p.User)
                .HasForeignKey<CandidateProfile>(p => p.UserId);
        }
    }
}
