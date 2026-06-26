namespace UrlShortener.API.Models
{
    public class CachedUrlEntry
    {
        public long UrlId { get; set; }
        public string LongUrl { get; set; } = string.Empty;
    }
}
