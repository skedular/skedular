using Polly;
using Polly.Retry;
using Serilog;

namespace Enterprise.Shared.Outbox;

internal static class OutboxParameters
{
    public const int CriticalRetryThreshold = 5;
    public static readonly TimeSpan RetryTime = TimeSpan.FromSeconds(2);

    public static readonly AsyncRetryPolicy DatabasePolicy = Policy.Handle<Exception>()
        .WaitAndRetryForeverAsync(
            _ => TimeSpan.FromSeconds(5),
            (exception, retry, retryTime) =>
                Log.Fatal(
                    exception,
                    "Database issue occured! Retry {RetryCount} will start in {Time}", retry,
                    retryTime));
}
