using System.Diagnostics;
using System.Diagnostics.Metrics;
using Enterprise.Shared.Telemetry;

namespace Enterprise.Shared.Metrics;

public interface IOperationTelemetry
{
    void Execute(
        string operationName,
        string operationType,
        Action operation);

    Task ExecuteAsync(
        string operationName,
        string operationType,
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken);

    Task<T> ExecuteAsync<T>(
        string operationName,
        string operationType,
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken);
}

public sealed class OperationTelemetry(
    IOpenTelemetryInstrumentation instrumentation,
    IActivityAccessor activityAccessor)
    : IOperationTelemetry
{
    private readonly Histogram<double>? _duration = instrumentation.GetHistogramByName<double>(MetricNames.OperationDuration);
    private readonly Counter<long>? _failureCounter = instrumentation.GetCounterByName<long>(MetricNames.OperationFailureCounter);
    private readonly Counter<long>? _successCounter = instrumentation.GetCounterByName<long>(MetricNames.OperationSuccessCounter);

    public void Execute(
        string operationName,
        string operationType,
        Action operation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
        ArgumentException.ThrowIfNullOrWhiteSpace(operationType);

        var tagList = new TagList
        {
            { "operation.name", operationName },
            { "operation.type", operationType },
        };
        var startedAt = Stopwatch.GetTimestamp();
        var activitySource = activityAccessor.GetActivitySource(MetricNames.OperationActivitySource);
        using var activity = activitySource.StartActivity(operationName, tags:
        [
            new KeyValuePair<string, object?>("operation.name", operationName),
            new KeyValuePair<string, object?>("operation.type", operationType),
        ]);

        try
        {
            operation();
            activity?.SetStatus(ActivityStatusCode.Ok);
            _successCounter?.Add(1, tagList);
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity?.AddException(exception);
            activityAccessor.AddException(exception);
            _failureCounter?.Add(1, tagList);
            throw;
        }
        finally
        {
            _duration?.Record(Stopwatch.GetElapsedTime(startedAt).TotalSeconds, tagList);
        }
    }

    public async Task ExecuteAsync(
        string operationName,
        string operationType,
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken) =>
        await ExecuteAsync(
            operationName,
            operationType,
            async ct =>
            {
                await operation(ct);
                return true;
            },
            cancellationToken);

    public async Task<T> ExecuteAsync<T>(
        string operationName,
        string operationType,
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
        ArgumentException.ThrowIfNullOrWhiteSpace(operationType);

        var tagList = new TagList
        {
            { "operation.name", operationName },
            { "operation.type", operationType },
        };
        var startedAt = Stopwatch.GetTimestamp();
        var activitySource = activityAccessor.GetActivitySource(MetricNames.OperationActivitySource);
        using var activity = activitySource.StartActivity(operationName, tags:
        [
            new KeyValuePair<string, object?>("operation.name", operationName),
            new KeyValuePair<string, object?>("operation.type", operationType),
        ]);

        try
        {
            var result = await operation(cancellationToken);
            activity?.SetStatus(ActivityStatusCode.Ok);
            _successCounter?.Add(1, tagList);
            return result;
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity?.AddException(exception);
            activityAccessor.AddException(exception);
            _failureCounter?.Add(1, tagList);
            throw;
        }
        finally
        {
            _duration?.Record(Stopwatch.GetElapsedTime(startedAt).TotalSeconds, tagList);
        }
    }
}
