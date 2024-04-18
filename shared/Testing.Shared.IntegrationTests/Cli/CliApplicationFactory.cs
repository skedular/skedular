namespace Testing.Shared.IntegrationTests.Cli;

public class CliApplicationFactory<TProgram> : IDisposable where TProgram : class
{
    private bool _disposed;

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
        }

        _disposed = true;
    }

    ~CliApplicationFactory() => Dispose(false);

    public async Task RunAsync(string[] args)
    {
        var methodInfo = typeof(TProgram).GetMethod("Main");

        ArgumentNullException.ThrowIfNull(methodInfo);

        var task = methodInfo.Invoke(null, [args]) as Task;

        ArgumentNullException.ThrowIfNull(task);

        await task;
    }
}
