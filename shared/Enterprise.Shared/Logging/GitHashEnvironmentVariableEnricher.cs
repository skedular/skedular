using Serilog.Core;
using Serilog.Events;

namespace Enterprise.Shared.Logging;

public class GitHashEnvironmentVariableEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var hashEnv = Environment.GetEnvironmentVariable("GIT_COMMIT_HASH");
        var hashShortened = hashEnv is null ? "[---]" : ShortenHash(hashEnv);

        logEvent.AddPropertyIfAbsent(new LogEventProperty("GitHash", new ScalarValue(hashShortened)));
    }

    private static string ShortenHash(string hashEnv) => hashEnv[..Math.Min(hashEnv.Length, 6)];
}
