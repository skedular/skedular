using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Hosting;

public interface IHostApplicationLifetimeWrapper
{
    void StopApplication();
}

public class HostApplicationLifetimeWrapper(
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<HostApplicationLifetimeWrapper> logger) : IHostApplicationLifetimeWrapper
{
    public void StopApplication()
    {
        logger.LogInformation("Stopping host application");
        hostApplicationLifetime.StopApplication();
    }
}
