using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Persistence.Models
{
    public class ShortUrl
    {
        public long Id { get; set; }
        public string LongUrl { get; set; } = string.Empty;
        public string? ShortCode { get; set; } 
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
