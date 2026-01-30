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
    }
}
