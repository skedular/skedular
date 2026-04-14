using System.Text;
using Confluent.Kafka;
using Enterprise.Shared.Telemetry;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Kafka.Telemetry;

public class
    HeaderPropagatorFunctions(ILogger<HeaderPropagatorFunctions> logger) : IPropagatorFunctionProvider<Headers>
{
    /// <summary>
    ///     Inject adds or overwrites existing keys
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="contextFieldName"></param>
    /// <param name="contextFieldValue"></param>
    public void Inject(
        Headers destination,
        string contextFieldName,
        string contextFieldValue)
    {
        logger.LogDebug("Injecting propagation header. HeaderName={HeaderName}", contextFieldName);
        destination.Remove(contextFieldName);
        destination.Add(contextFieldName, Encoding.UTF8.GetBytes(contextFieldValue));
    }

    public IEnumerable<string> Extract(Headers location, string contextFieldName)
    {
        logger.LogDebug("Extracting propagation header. HeaderName={HeaderName}", contextFieldName);
        return location
            .Where(header => header.Key == contextFieldName)
            .Select(header => Encoding.UTF8.GetString(header.GetValueBytes()))
            .ToArray();
    }
}
