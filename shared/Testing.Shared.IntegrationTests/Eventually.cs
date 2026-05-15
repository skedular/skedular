namespace Testing.Shared.IntegrationTests;

public interface IEventually
{
    Task ConsistentlyAsync(
        Func<CancellationToken, Task> assertion,
        CancellationToken cancellationToken,
        TimeSpan? duration = null,
        TimeSpan? pollInterval = null);

    Task<T> UntilAsync<T>(
        Func<CancellationToken, Task<T>> probe,
        Func<T, bool> predicate,
        CancellationToken cancellationToken,
        TimeSpan? timeout = null,
        TimeSpan? pollInterval = null);
}

public class Eventually : IEventually
{
    public async Task ConsistentlyAsync(
        Func<CancellationToken, Task> assertion,
        CancellationToken cancellationToken,
        TimeSpan? duration = null,
        TimeSpan? pollInterval = null)
    {
        duration ??= TimeSpan.FromSeconds(3);
        pollInterval ??= TimeSpan.FromMilliseconds(250);

        var deadline = TimeProvider.System.GetUtcNow().Add(duration.Value);
        Exception? lastException = null;

        while (TimeProvider.System.GetUtcNow() < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await assertion(cancellationToken);
                lastException = null;
            }
            catch (Exception ex)
            {
                lastException = ex;
                break;
            }

            await Task.Delay(pollInterval.Value, cancellationToken);
        }

        if (lastException is not null)
        {
            throw lastException;
        }
    }

    public async Task<T> UntilAsync<T>(
        Func<CancellationToken, Task<T>> probe,
        Func<T, bool> predicate,
        CancellationToken cancellationToken,
        TimeSpan? timeout = null,
        TimeSpan? pollInterval = null)
    {
        timeout ??= TimeSpan.FromSeconds(30);
        pollInterval ??= TimeSpan.FromMilliseconds(250);

        var deadline = TimeProvider.System.GetUtcNow().Add(timeout.Value);

        while (TimeProvider.System.GetUtcNow() < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lastValue = await probe(cancellationToken);
            if (predicate(lastValue))
            {
                return lastValue;
            }

            await Task.Delay(pollInterval.Value, cancellationToken);
        }

        throw new TimeoutException($"Condition was not satisfied within {timeout.Value}.");
    }
}
