using Flurl;
using Microsoft.Playwright;
using Location = Skedularctl.Services.Models.Location;

namespace Skedularctl.Services.Sharedspaces;

public interface ILocationsCrawler
{
    Task CrawlAsync(string type, string url, Func<Location, Task> onLocationFound, CancellationToken cancellationToken);
}

public class LocationsCrawler(
    SharedSpacesConfiguration sharedSpacesConfiguration,
    IPlaywrightProvider playwrightProvider,
    ILocationCrawler locationCrawler) : ILocationsCrawler
{
    public async Task CrawlAsync(string type, string url, Func<Location, Task> onLocationFound, CancellationToken cancellationToken)
    {
        var browser = await playwrightProvider.GetBrowserAsync();
        var page = await browser.NewPageAsync();

        try
        {
            await page.SetViewportSizeAsync(1920, 1080);

            await page.RouteAsync("**/*", async route =>
            {
                var requestUrl = route.Request.Url;

                if (requestUrl.Contains("doubleclick.net") ||
                    requestUrl.Contains("googlesyndication.com") ||
                    requestUrl.Contains("linkedin.com") ||
                    requestUrl.Contains("mailchimp.com") ||
                    requestUrl.Contains("adservice") ||
                    requestUrl.Contains("banner") ||
                    requestUrl.EndsWith(".gif"))
                {
                    await route.AbortAsync();
                }
                else
                {
                    await route.ContinueAsync();
                }
            });

#pragma warning disable VSTHRD101
            page.Popup += async (_, popupPage) => await popupPage.CloseAsync();
#pragma warning restore VSTHRD101
#pragma warning disable VSTHRD101
            page.Dialog += async (_, dialog) => await dialog.DismissAsync();
#pragma warning restore VSTHRD101

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
                        var location = await locationCrawler.CrawlAsync(
                            type,
                            Url.Combine(sharedSpacesConfiguration.BaseUrl, href),
                            cancellationToken);
                        await onLocationFound(location);
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
        finally
        {
            await page.CloseAsync();
        }
    }
}
