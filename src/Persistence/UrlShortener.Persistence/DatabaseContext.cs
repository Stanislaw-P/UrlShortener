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
    }
}
