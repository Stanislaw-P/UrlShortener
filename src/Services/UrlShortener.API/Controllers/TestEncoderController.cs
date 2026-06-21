using Microsoft.AspNetCore.Mvc;
using UrlShortener.Shared;

namespace UrlShortener.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestEncoderController : ControllerBase
    {
        readonly ILogger<TestEncoderController> _logger;
        readonly Base62Encoder _base62Encoder;

        public TestEncoderController(ILogger<TestEncoderController> logger)
        {
            _logger = logger;
            _base62Encoder = new Base62Encoder();
        }


        [HttpGet("encode/{num}")]
        public IActionResult EncodeTest(ulong num)
        {
            var encoded = _base62Encoder.Encode(num);
            return Ok(encoded);
        }

        [HttpGet("decode/{str}")]
        public IActionResult DecodeTest(string str)
        {
            var decoded = _base62Encoder.Decode(str);
            return Ok(decoded);
        }
    }
}
