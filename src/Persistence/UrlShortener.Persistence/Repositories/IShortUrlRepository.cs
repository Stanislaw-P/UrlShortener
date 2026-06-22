using UrlShortener.Persistence.Models;

namespace UrlShortener.Persistence.Repositories
{
    public interface IShortUrlRepository
    {
        Task AddAsync(ShortUrl shortUrlModel);
        Task<ShortUrl?> TryGetByLongUrlAsync(string longUrl);
        Task<ShortUrl?> TryGetByShortCodeAsync(string shortCode);
        Task UpdateAsync(ShortUrl shortUrlModel);
    }
}