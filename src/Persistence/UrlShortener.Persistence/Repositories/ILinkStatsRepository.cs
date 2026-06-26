using UrlShortener.Persistence.Models;

namespace UrlShortener.Persistence.Repositories
{
    public interface ILinkStatsRepository
    {
        Task<LinkStats?> TryGetByIdAsync(long id);
        Task AddAsync(LinkStats linkStats);
        Task UpdateAsync(LinkStats linkStats);
    }
}