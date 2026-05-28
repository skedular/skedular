using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Version;

public interface IVersionService
{
    System.Version GetVersion();
}

public class VersionService<TProgram>(ILogger<VersionService<TProgram>> logger) : IVersionService where TProgram : class
{
    public System.Version GetVersion()
    {
        logger.LogDebug("Resolving assembly version for {ProgramType}", typeof(TProgram).FullName);

        var version = typeof(TProgram).Assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        logger.LogInformation("Resolved assembly version {Version} for {ProgramType}", version, typeof(TProgram).FullName);
        return typeof(TProgram).Assembly.GetName().Version!;
    }
}
