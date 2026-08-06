using System.Globalization;
using CommandLine;
using CsvHelper;
using CsvHelper.Configuration;
using Flurl;
using WebScrapper.Models;
using WebScrapper.Sharedspaces;

namespace WebScrapper.Services;

[Verb("crawl-sharedspaces")]
public class CrawlSharedspacesOptions;

public interface ICrawlerService
{
    Task HandleAsync(CrawlSharedspacesOptions options, CancellationToken cancellationToken);
}

public class CrawlerService(SharedSpacesConfiguration sharedSpacesConfiguration, ILocationsCrawlerService locationsCrawlerService) : ICrawlerService
{
    private readonly Dictionary<string, string> _urls = new()
    {
        { "office-space", Url.Combine(sharedSpacesConfiguration.BaseUrl, "listings", "office-space") },
        { "meeting-space", Url.Combine(sharedSpacesConfiguration.BaseUrl, "listings", "meeting-space") },
        { "event-space", Url.Combine(sharedSpacesConfiguration.BaseUrl, "listings", "event-space") },
        { "studio-space", Url.Combine(sharedSpacesConfiguration.BaseUrl, "listings", "studio-space") },
        { "carpark-space", Url.Combine(sharedSpacesConfiguration.BaseUrl, "listings", "carpark-space") },
        { "commercial-kitchen", Url.Combine(sharedSpacesConfiguration.BaseUrl, "listings", "commercial-kitchen") },
        { "shoot-location", Url.Combine(sharedSpacesConfiguration.BaseUrl, "listings", "shoot-location") },
        { "storage-space", Url.Combine(sharedSpacesConfiguration.BaseUrl, "listings", "storage-space") },
        { "retail-space", Url.Combine(sharedSpacesConfiguration.BaseUrl, "listings", "retail-space") },
    };

    public async Task HandleAsync(CrawlSharedspacesOptions options, CancellationToken cancellationToken)
    {
        await using var writer =
            new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "locations-output.csv"));
        await using var csv = new CsvWriter(
            writer,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "|",
                ShouldQuote = _ => true,
                NewLine = Environment.NewLine,
            });
        csv.Context.RegisterClassMap<LocationMap>();

        csv.WriteHeader<Location>();
        await csv.NextRecordAsync();
        await writer.FlushAsync(cancellationToken);

        foreach (var url in _urls)
        {
            await locationsCrawlerService.CrawlAsync(
                url.Key,
                url.Value,
                async location =>
                {
                    csv.WriteRecord(location);
                    await csv.NextRecordAsync();
                    await writer.FlushAsync(cancellationToken);
                }, cancellationToken);
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
