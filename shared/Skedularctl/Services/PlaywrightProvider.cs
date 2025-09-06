using Microsoft.Playwright;

namespace Skedularctl.Services;

public interface IPlaywrightProvider
{
    ValueTask<IPlaywright> GetPlaywrightInstanceAsync();
    ValueTask<IBrowser> GetBrowserAsync();
}

public class PlaywrightProvider : IPlaywrightProvider, IAsyncDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1);
    private IBrowser? _browser;
    private IPlaywright? _playwright;

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    public async ValueTask<IPlaywright> GetPlaywrightInstanceAsync()
    {
        if (_playwright is not null)
        {
            return _playwright;
        }

        await _semaphore.WaitAsync();

        try
        {
            _playwright = await Playwright.CreateAsync();
            return _playwright;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask<IBrowser> GetBrowserAsync()
    {
        if (_browser is not null)
        {
            return _browser;
        }

        var playwright = await GetPlaywrightInstanceAsync();

        await _semaphore.WaitAsync();

        try
        {
            _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false, SlowMo = 1000, Args = ["--start-maximized"]
            });
            return _browser;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_browser is not null)
        {
            await _browser.DisposeAsync();
            _playwright = null;
        }

        if (_playwright is not null)
        {
            _playwright.Dispose();
            _playwright = null;
        }

        _browser = null;
        _semaphore.Dispose();
    }
}
