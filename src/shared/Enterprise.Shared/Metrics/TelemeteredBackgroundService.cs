using Microsoft.Extensions.Hosting;

namespace Enterprise.Shared.Metrics;

public abstract class TelemeteredBackgroundService(IOperationTelemetry operationTelemetry) : BackgroundService
{
    protected async Task RunOnceAsync(
        string operationName,
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken) =>
        await operationTelemetry.ExecuteAsync(
            operationName,
            "hosted.job",
            operation,
            cancellationToken);

    protected async Task<T> RunOnceAsync<T>(
        string operationName,
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken) =>
        await operationTelemetry.ExecuteAsync(
            operationName,
            "hosted.job",
            operation,
            cancellationToken);
}
