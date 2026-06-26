using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Persistence.Models;

namespace UrlShortener.Persistence.Repositories
{
    public class LinkStatsRepository : ILinkStatsRepository
    {
        readonly DatabaseContext _db;

        public LinkStatsRepository(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<LinkStats?> TryGetByIdAsync(long id)
        {
            return await _db.LinksStats.FindAsync(id);
        }

        public async Task AddAsync(LinkStats linkStats)
        {
            await _db.LinksStats.AddAsync(linkStats);
            await _db.SaveChangesAsync(); 
        }

        public async Task UpdateAsync(LinkStats linkStats)
        {
            _db.LinksStats.Update(linkStats);
            await _db.SaveChangesAsync();
        }
    }
}
