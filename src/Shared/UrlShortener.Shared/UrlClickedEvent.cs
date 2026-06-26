namespace UrlShortener.Shared
{
    public record UrlClickedEvent(long UrlId, string Code, DateTime ClickedAt, string IpAddress);
}
