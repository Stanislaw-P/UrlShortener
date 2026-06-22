using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Persistence.Models;

namespace UrlShortener.Persistence.Repositories
{
    public class ShortUrlRepository : IShortUrlRepository
    {
        readonly DatabaseContext _db;
        readonly ILogger<ShortUrlRepository> _logger;

        public ShortUrlRepository(DatabaseContext db, ILogger<ShortUrlRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task AddAsync(ShortUrl shortUrlModel)
        {
            try
            {
                await _db.AddAsync(shortUrlModel);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка сохранения записи");
            }
        }

        public async Task<ShortUrl?> TryGetByLongUrlAsync(string longUrl)
        {
            return await _db.ShortUrls.FirstOrDefaultAsync(sh => sh.LongUrl == longUrl);
        }

        public async Task<ShortUrl?> TryGetByShortCodeAsync(string shortCode)
        {
            return await _db.ShortUrls.FirstOrDefaultAsync(sh => sh.ShortCode == shortCode);
        }

        public async Task UpdateAsync(ShortUrl shortUrlModel)
        {
            _db.ShortUrls.Update(shortUrlModel);
            await _db.SaveChangesAsync();
        }
    }
}
