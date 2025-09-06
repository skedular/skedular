using CommandLine;
using Flurl;

namespace Skedularctl.Services.Sharedspaces;

[Verb("crawl-sharedspace")]
public class CrawlOptions;

public interface ICrawlerService
{
    Task HandleAsync(CrawlOptions options, CancellationToken cancellationToken);
}

public class CrawlerService(ILocationsCrawler locationsCrawler) : ICrawlerService
{
    private static readonly string s_baseUrl = "https://www.sharedspace.co.nz";

    private readonly Dictionary<string, string> _urls = new()
    {
        { "office-space", Url.Combine(s_baseUrl, "listings", "office-space") },
        { "meeting-space", Url.Combine(s_baseUrl, "listings", "meeting-space") },
        { "event-space", Url.Combine(s_baseUrl, "listings", "event-space") },
        { "studio-space", Url.Combine(s_baseUrl, "listings", "studio-space") },
        { "carpark-space", Url.Combine(s_baseUrl, "listings", "carpark-space") },
        { "commercial-kitchen", Url.Combine(s_baseUrl, "listings", "commercial-kitchen") },
        { "shoot-location", Url.Combine(s_baseUrl, "listings", "shoot-location") },
        { "storage-space", Url.Combine(s_baseUrl, "listings", "storage-space") },
        { "retail-space", Url.Combine(s_baseUrl, "listings", "retail-space") }
    };

    public async Task HandleAsync(CrawlOptions options, CancellationToken cancellationToken)
    {
        foreach (var url in _urls)
        {
            await locationsCrawler.CrawlAsync(url.Key, url.Value, cancellationToken);
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
