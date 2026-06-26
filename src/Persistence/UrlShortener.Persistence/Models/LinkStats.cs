using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Persistence.Models
{
    public class LinkStats
    {
        public long ShortUrlId { get; set; }
        public string ShortCode { get; set; } = string.Empty;
        public int ClickCount { get; set; }
        public DateTimeOffset LastClickedAt { get; set; }

        public ShortUrl ShortUrl { get; set; } = null!;
    }
}
