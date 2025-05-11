namespace Enterprise.Shared.Version;

public interface IVersionService
{
    System.Version GetVersion();
}

public class VersionService<TProgram> : IVersionService where TProgram : class
{
    public System.Version GetVersion()
    {
        var version = typeof(TProgram).Assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return typeof(TProgram).Assembly.GetName().Version!;
    }
}
