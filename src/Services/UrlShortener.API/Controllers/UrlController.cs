using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
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

        public UrlController(ILogger<UrlController> logger, IConfiguration configuration, IShortUrlRepository urlRepository, IDistributedCache cache)
        {
            _logger = logger;
            _encoder = new Base62Encoder();
            _configuration = configuration;
            _urlRepository = urlRepository;
            _cache = cache;
        }

        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrlAsync([FromBody] ShortenRequest request)
        {
            string baseUrl = _configuration["ShortenerSettings:BaseUrl"] ?? "https://localhost";
            
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(21) 
            };

            // Проверяем не существует ли уже ссылка в БД
            var existingUrl = await _urlRepository.TryGetByLongUrlAsync(request.LongUrl);
            if (existingUrl != null)
            {
                // Оптимизация: параллельно "прогреваем" кэш в Redis, если ссылки там вдруг не оказалось
                await _cache.SetStringAsync(existingUrl.ShortCode ?? _encoder.Encode(existingUrl.Id), existingUrl.LongUrl, cacheOptions);
            
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

            await _cache.SetStringAsync(code, newUrlModel.LongUrl, cacheOptions);

            // Собираем URL
            var scheme = HttpContext.Request.Scheme;
            string host = HttpContext.Request.Host.ToUriComponent();

            string shortUrl = $"{scheme}://{host}/{code}";

            return Ok(new { ShortUrl = shortUrl });
        }

        [HttpGet("~/{code}")]
        public async Task<IActionResult> RedirectToOriginalAsync(string code)
        {
            var targetUrl = await _cache.GetStringAsync(code);
            if (targetUrl != null)
                return Redirect(targetUrl);

            var existingUrl = await _urlRepository.TryGetByShortCodeAsync(code);
            if (existingUrl == null)
                return NotFound("Ссылка не найдена");

            targetUrl = existingUrl.LongUrl;

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(21)
            };

            await _cache.SetStringAsync(code, targetUrl, cacheOptions);

            return Redirect(targetUrl);
        }
    }
}
