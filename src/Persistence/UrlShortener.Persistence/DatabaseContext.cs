using Microsoft.EntityFrameworkCore;
using UrlShortener.Persistence.Models;

namespace UrlShortener.Persistence
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ShortUrl> ShortUrls { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShortUrl>(entity =>
            {
                entity.HasIndex(e => e.ShortCode)
                      .IsUnique();

                entity.HasIndex(e => e.LongUrl)
                      .IsUnique();
            });
        }
    }
}
