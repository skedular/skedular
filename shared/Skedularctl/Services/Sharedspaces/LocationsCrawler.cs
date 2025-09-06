using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Flurl;
using Microsoft.Playwright;

namespace Skedularctl.Services.Sharedspaces;

public interface ILocationsCrawler
{
    Task CrawlAsync(string type, string url, CancellationToken cancellationToken);
}

public record Location(string Url);

public class LocationMap : ClassMap<Location>
{
    public LocationMap() => Map(m => m.Url).Index(0).Name("url");
}

public class LocationsCrawler(IPlaywrightProvider playwrightProvider) : ILocationsCrawler
{
    private static readonly string s_baseUrl = "https://www.sharedspace.co.nz";

    public async Task CrawlAsync(string type, string url, CancellationToken cancellationToken)
    {
        var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "locations");
        Directory.CreateDirectory(directoryPath);

        await using var writer = new StreamWriter(Path.Combine(directoryPath, $"{type}.csv"));
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<LocationMap>();

        csv.WriteHeader<Location>();
        await csv.NextRecordAsync();

        var browser = await playwrightProvider.GetBrowserAsync();

        var page = await browser.NewPageAsync();
        await page.SetViewportSizeAsync(1920, 1080);

        await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.Load, Timeout = 60000 });

        do
        {
            var hitsDiv = page.Locator("#hits");
            var items = hitsDiv.Locator("ul li");
            var count = await items.CountAsync();

            for (var i = 0; i < count; i++)
            {
                var item = items.Nth(i);
                var link = item.Locator("a[class^=\"listing-click-\"]");
                var allATags = await link.AllAsync();

                if (allATags.Count <= 0)
                {
                    continue;
                }

                var href = await allATags[0].GetAttributeAsync("href");
                if (!string.IsNullOrWhiteSpace(href))
                {
                    csv.WriteRecord(new Location(Url.Combine(s_baseUrl, href)));
                    await csv.NextRecordAsync();
                }
            }

            var paginationDiv = page.Locator("#pagination");
            if (await paginationDiv.CountAsync() > 0)
            {
                var nextLink = paginationDiv.Locator("a:text('Next')");
                if (await nextLink.CountAsync() > 0)
                {
                    await nextLink.First.ClickAsync();
                    await page.WaitForLoadStateAsync(LoadState.Load, new PageWaitForLoadStateOptions { Timeout = 60000 });
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        } while (true);
    }
}
