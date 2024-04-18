using Microsoft.AspNetCore.Mvc.Testing;

namespace Testing.Shared.IntegrationTests.Jobs;

public class JobTestFixture<TStartup> : IDisposable where TStartup : class
{
    private readonly WebApplicationFactory<TStartup> _webApplicationFactory;
    private bool _disposed;

    public JobTestFixture(WebApplicationFactory<TStartup> webApplicationFactory)
    {
        ArgumentNullException.ThrowIfNull(_webApplicationFactory);

        _webApplicationFactory = webApplicationFactory;
        _webApplicationFactory.CreateDefaultClient();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _webApplicationFactory.Dispose();
        }

        _disposed = true;
    }

    ~JobTestFixture() => Dispose(false);
}
