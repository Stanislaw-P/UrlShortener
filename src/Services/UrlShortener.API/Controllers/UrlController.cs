using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using UrlShortener.API.Models;
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

        public UrlController(ILogger<UrlController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _encoder = new Base62Encoder();
            _configuration = configuration;
        }

        [HttpPost("shorten")]
        public async Task<IResult> ShortenUrl([FromBody] ShortenRequest request)
        {
            // ... логика сохранения в БД и получения ID ...
            string code = _encoder.Encode(123);

            // Динамически собираем: "https://" + "localhost:7049" + "/" + "6ax7"
            string baseUrl = _configuration["ShortenerSettings:BaseUrl"] ?? "https://localhost/";

            //string shortUrl = $"{scheme}://{host}/{code}";
            string shortUrl = $"google";

            return Results.Ok(new { ShortUrl = shortUrl });
        }

        [HttpGet("~/{code}")]
        public async Task<IActionResult> RedirectToOriginal(string code)
        {
            // Пока это заглушка для 2 этапа:
            // Ищем в БД длинный URL по нашему короткому коду
            // var urlEntry = await _dbContext.ShortUrls.FirstOrDefaultAsync(x => x.ShortCode == code);

            // if (urlEntry == null)
            //     return NotFound("Ссылка не найдена");

            // Имитируем, что нашли (для теста)
            string targetUrl = "https://google.com";

            // Возвращаем HTTP 302 Redirect
            return Ok(targetUrl);
        }
    }
}
