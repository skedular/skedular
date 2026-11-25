using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace Enterprise.Shared.Telemetry;

public static class ProcessTelemetryExportRegistry
{
    private static readonly HashSet<string> s_claims = [];
    private static readonly Lock s_lock = new();

    public static IDisposable? TryClaim(string signalName, string destination)
    {
        var key = $"{signalName}:{destination}";
        lock (s_lock)
        {
            return s_claims.Add(key) ? new Claim(key) : null;
        }
    }

    internal static BaseProcessor<T>? TryClaimProcessor<T>(string signalName, string destination)
    {
        var claim = TryClaim(signalName, destination);
        return claim is null ? null : new ClaimProcessor<T>(claim);
    }

    internal static MetricReader? TryClaimReader(string signalName, string destination)
    {
        var claim = TryClaim(signalName, destination);
        return claim is null ? null : new ClaimMetricReader(claim);
    }

    private sealed class ClaimProcessor<T>(IDisposable claim) : BaseProcessor<T>
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                claim.Dispose();
            }

            base.Dispose(disposing);
        }
    }

    private sealed class ClaimMetricReader(IDisposable claim) : MetricReader
    {
        protected override bool OnCollect(int timeoutMilliseconds) => true;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                claim.Dispose();
            }

            base.Dispose(disposing);
        }
    }

    private sealed class Claim(string key) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            lock (s_lock)
            {
                if (!_disposed)
                {
                    s_claims.Remove(key);
                    _disposed = true;
                }
            }
        }
    }
}
