using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Persistence;
using UrlShortener.Persistence.Models;
using UrlShortener.Persistence.Repositories;
using UrlShortener.Shared;

namespace UrlShortener.Analytics
{
    public class UrlClickedConsumer : IConsumer<UrlClickedEvent>
    {
        readonly ILogger<UrlClickedConsumer> _logger;
        readonly ILinkStatsRepository _linkStatsRepository;

        public UrlClickedConsumer(ILogger<UrlClickedConsumer> logger, ILinkStatsRepository linkStatsRepository)
        {
            _logger = logger;
            _linkStatsRepository = linkStatsRepository;
        }

        public async Task Consume(ConsumeContext<UrlClickedEvent> context)
        {
            var message = context.Message;

            var stats = await _linkStatsRepository.TryGetByIdAsync(message.UrlId);

            if (stats == null)
            {
                var newStats = new LinkStats
                {
                    ShortUrlId = message.UrlId,
                    ShortCode = message.Code,
                    ClickCount = 1,
                    LastClickedAt = message.ClickedAt
                };

                await _linkStatsRepository.AddAsync(newStats);
            }
            else
            {
                stats.ClickCount++;
                stats.LastClickedAt = message.ClickedAt;

                await _linkStatsRepository.UpdateAsync(stats);
            }
        }
    }
}
