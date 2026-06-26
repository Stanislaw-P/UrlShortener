using Microsoft.EntityFrameworkCore;
using UrlShortener.Persistence.Models;

namespace UrlShortener.Persistence
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ShortUrl> ShortUrls { get; set; }
        public DbSet<LinkStats> LinksStats { get; set; }

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

            modelBuilder.Entity<LinkStats>(entity =>
            {
                entity.HasKey(e => e.ShortUrlId);

                entity.Property(e => e.ShortCode)
                      .HasMaxLength(10)
                      .IsRequired();

                entity.HasOne(analytics => analytics.ShortUrl)
                      .WithOne()
                      .HasForeignKey<LinkStats>(analytics => analytics.ShortUrlId) 
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
