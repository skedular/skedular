using Microsoft.Extensions.Hosting;

namespace Enterprise.Shared.Hosting;

public interface IHostApplicationLifetimeWrapper
{
    void StopApplication();
}

public class HostApplicationLifetimeWrapper(IHostApplicationLifetime hostApplicationLifetime)
    : IHostApplicationLifetimeWrapper
{
    public void StopApplication() => hostApplicationLifetime.StopApplication();
}
