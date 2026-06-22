using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public UrlController(ILogger<UrlController> logger, IConfiguration configuration, IShortUrlRepository urlRepository)
        {
            _logger = logger;
            _encoder = new Base62Encoder();
            _configuration = configuration;
            _urlRepository = urlRepository;
        }

        [HttpPost("shorten")]
        public async Task<IResult> ShortenUrlAsync([FromBody] ShortenRequest request)
        {
            string baseUrl = _configuration["ShortenerSettings:BaseUrl"] ?? "https://localhost";

            // Проверяем не существует ли уже ссылка в БД
            var existingUrl = await _urlRepository.TryGetByLongUrlAsync(request.LongUrl);
            if (existingUrl != null)
                return Results.Ok(new { ShortUrl = $"{baseUrl}/{existingUrl.ShortCode}" });

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

            // Собираем URL
            var scheme = HttpContext.Request.Scheme;
            string host = HttpContext.Request.Host.ToUriComponent();

            string shortUrl = $"{scheme}://{host}/{code}";

            return Results.Ok(new { ShortUrl = shortUrl });
        }

        [HttpGet("~/{code}")]
        public async Task<IActionResult> RedirectToOriginalAsync(string code)
        {
            var existingUrl = await _urlRepository.TryGetByShortCodeAsync(code);
            if (existingUrl == null) 
                return NotFound("Ссылка не найдена");

            // Имитируем, что нашли (для теста)
            string targetUrl = existingUrl.LongUrl;

            return Redirect(targetUrl);
        }
    }
}
