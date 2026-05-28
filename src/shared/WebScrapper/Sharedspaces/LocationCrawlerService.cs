using Microsoft.Playwright;
using WebScrapper.Services;
using Location = WebScrapper.Models.Location;

namespace WebScrapper.Sharedspaces;

public interface ILocationCrawlerService
{
    Task<Location> CrawlAsync(string type, string url);
}

public class LocationCrawlerServiceService(IPlaywrightProvider playwrightProvider, IContentEnricherService contentEnricherService)
    : ILocationCrawlerService
{
    public async Task<Location> CrawlAsync(string type, string url)
    {
        var browser = await playwrightProvider.GetBrowserAsync();
        var page = await browser.NewPageAsync();

        try
        {
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

            await page.SetViewportSizeAsync(1920, 1080);
            await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.Load, Timeout = 60000 });

            var title = page.Locator("h1.uppercase.text-lg.font-bold.ml-2");
            var titleText = await title.CountAsync() > 0 ? await title.First.InnerTextAsync() : string.Empty;

            var subtitle = page.Locator("h2.font-normal.text-primary.text-lg");
            var subtitleText = await subtitle.CountAsync() > 0 ? await subtitle.First.InnerTextAsync() : string.Empty;

            var description = page.Locator("p.text-sm.text-default");
            var descriptionText = await description.CountAsync() > 0 ? await description.First.InnerTextAsync() : string.Empty;

            var contactLabels = page.Locator("span.text-gray-400.uppercase.text-xxs.tracking-widest");
            var contactPersonText = string.Empty;
            for (var i = 0; i < await contactLabels.CountAsync(); i++)
            {
                var label = contactLabels.Nth(i);
                var labelText = (await label.InnerTextAsync()).Trim();
                if (labelText.Equals("Contact details", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Go up to the parent container
                    var parent = label.Locator("xpath=ancestor::div[contains(@class, 'lg:block') and contains(@class, 'relative')][1]");
                    if (await parent.CountAsync() > 0)
                    {
                        // Find the first .flex.mb-4 > div inside this parent
                        var contactDiv = parent.First.Locator("div.flex.mb-4 > div");
                        if (await contactDiv.CountAsync() > 0)
                        {
                            contactPersonText = (await contactDiv.First.InnerTextAsync()).Trim();
                            break;
                        }
                    }
                }
            }

            var phoneButton = page.Locator("span#phone-number");
            var contactPhoneNumberText = string.Empty;
            if (await phoneButton.CountAsync() > 0)
            {
                await phoneButton.First.ClickAsync();

                // Wait for the phone number link to appear
                var phoneNumberLink = page.Locator("a#phone_number");
                await phoneNumberLink.First.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });
                contactPhoneNumberText = (await phoneNumberLink.First.InnerTextAsync()).Trim();
            }

            var addressRoot = page.Locator("div.address.mb-4.w-full");
            var addressText = await addressRoot.CountAsync() > 0 ? await addressRoot.First.InnerTextAsync() : string.Empty;

            var areaRoot = page.Locator("div.area.mb-4.w-full");
            var areaText = string.Empty;
            if (await areaRoot.CountAsync() > 0)
            {
                var areaValueDiv = areaRoot.First.Locator("div.text-base.text-gray-600");
                if (await areaValueDiv.CountAsync() > 0)
                {
                    areaText = await areaValueDiv.First.InnerTextAsync();
                }
            }

            var peopleRoot = page.Locator("div.people.mb-4.w-full");
            var peopleText = string.Empty;
            if (await peopleRoot.CountAsync() > 0)
            {
                var peopleValueDiv = peopleRoot.First.Locator("div.text-base.text-gray-600");
                if (await peopleValueDiv.CountAsync() > 0)
                {
                    peopleText = await peopleValueDiv.First.InnerTextAsync();
                }
            }

            return new Location(
                type,
                url,
                titleText,
                subtitleText,
                descriptionText,
                contactPersonText,
                contactPhoneNumberText,
                addressText,
                areaText,
                peopleText,
                string.Join(Environment.NewLine, contentEnricherService.ExtractEmails(descriptionText)),
                string.Join(Environment.NewLine, contentEnricherService.ExtractWebsites(descriptionText)));
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}
