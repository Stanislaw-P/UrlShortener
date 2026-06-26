using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using UrlShortener.API.Models;
using UrlShortener.Persistence.Models;
using UrlShortener.Persistence.Repositories;
using UrlShortener.Shared;

namespace UrlShortener.API.Controllers
{
    [ApiController]
    [Route("api/urls")]
    public class UrlController : ControllerBase
    {
        readonly ILogger<UrlController> _logger;
        readonly Base62Encoder _encoder;
        readonly IConfiguration _configuration;
        readonly IShortUrlRepository _urlRepository;
        readonly IDistributedCache _cache;
        readonly IPublishEndpoint _publishEndpoint;

        public UrlController(ILogger<UrlController> logger, IConfiguration configuration, IShortUrlRepository urlRepository, IDistributedCache cache, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _encoder = new Base62Encoder();
            _configuration = configuration;
            _urlRepository = urlRepository;
            _cache = cache;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrlAsync([FromBody] ShortenRequest request)
        {
            string baseUrl = _configuration["ShortenerSettings:BaseUrl"] ?? "https://localhost";
            
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) 
            };

            // Проверяем не существует ли уже ссылка в БД
            var existingUrl = await _urlRepository.TryGetByLongUrlAsync(request.LongUrl);
            if (existingUrl != null)
            {
                // Оптимизация: параллельно "прогреваем" кэш в Redis, если ссылки там вдруг не оказалось
                var entry = JsonSerializer.Serialize(new CachedUrlEntry { UrlId = existingUrl.Id, LongUrl = existingUrl.LongUrl });
                await _cache.SetStringAsync(existingUrl.ShortCode ?? _encoder.Encode(existingUrl.Id), entry, cacheOptions);
            
                return Ok(new { ShortUrl = $"{baseUrl}/{existingUrl.ShortCode}" });
            }

            // Сохраняем в БД
            var newUrlModel = new ShortUrl
            {
                LongUrl = request.LongUrl,
            };
            await _urlRepository.AddAsync(newUrlModel);

            // Генерируем код на основе Id
            string code = _encoder.Encode(newUrlModel.Id);
            newUrlModel.ShortCode = code;

            // Обновляем запись в БД
            await _urlRepository.UpdateAsync(newUrlModel);

            var newEntry = JsonSerializer.Serialize(new CachedUrlEntry { UrlId = newUrlModel.Id, LongUrl = newUrlModel.LongUrl });
            await _cache.SetStringAsync(code, newEntry, cacheOptions);

            // Собираем URL
            var scheme = HttpContext.Request.Scheme;
            string host = HttpContext.Request.Host.ToUriComponent();

            string shortUrl = $"{scheme}://{host}/{code}";

            return Ok(new { ShortUrl = shortUrl });
        }

        [HttpGet("~/{code}")]
        public async Task<IActionResult> RedirectToOriginalAsync(string code)
        {
            long urlId;
            string targetUrl;

            var cached = await _cache.GetStringAsync(code);
            if (cached != null)
            {
                // Cache hit — десериализуем объект, у нас есть и Id, и LongUrl
                var entry = JsonSerializer.Deserialize<CachedUrlEntry>(cached);
                if (entry == null)
                    return NotFound("Ссылка не найдена");

                urlId = entry.UrlId;
                targetUrl = entry.LongUrl;
            }
            else
            {
                // Cache miss — идём в БД
                var existingUrl = await _urlRepository.TryGetByShortCodeAsync(code);
                if (existingUrl == null)
                    return NotFound("Ссылка не найдена");

                urlId = existingUrl.Id;
                targetUrl = existingUrl.LongUrl;

                // Прогреваем кэш с полным объектом
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                };
                var entry = JsonSerializer.Serialize(new CachedUrlEntry { UrlId = urlId, LongUrl = targetUrl });
                await _cache.SetStringAsync(code, entry, cacheOptions);
            }

            // Публикуем событие
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
            await _publishEndpoint.Publish(new UrlClickedEvent(urlId, code, DateTime.UtcNow, ipAddress));

            return Redirect(targetUrl);
        }

        ////[HttpGet("~/{code}")]
        //public async Task<IActionResult> RedirectTtoOriginalAsync(string code)
        //{
        //    var targetUrl = await _cache.GetStringAsync(code);
        //    if (targetUrl != null)
        //        return Redirect(targetUrl);

        //    var existingUrl = await _urlRepository.TryGetByShortCodeAsync(code);
        //    if (existingUrl == null)
        //        return NotFound("Ссылка не найдена");

        //    targetUrl = existingUrl.LongUrl;

        //    var cacheOptions = new DistributedCacheEntryOptions
        //    {
        //        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
        //    };

        //    // Вместо: await _cache.SetStringAsync(code, longUrl, cacheOptions);
        //    var entry = JsonSerializer.Serialize(new CachedUrlEntry { UrlId = newUrlModel.Id, LongUrl = newUrlModel.LongUrl });
        //    await _cache.SetStringAsync(code, entry, cacheOptions);

        //    await _cache.SetStringAsync(code, targetUrl, cacheOptions);

        //    // Публикуем сообщение в RabbitMQ
        //    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
        //    await _publishEndpoint.Publish(new UrlClickedEvent(existingUrl.Id, code, DateTime.UtcNow, ipAddress));

        //    return Redirect(targetUrl);
        //}
    }
}
