using System.Text;
using Confluent.Kafka;

namespace Enterprise.Shared.Telemetry.PropagatorFunctions;

public class
    HeaderPropagatorFunctions : IPropagatorFunctionProvider<Headers>
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
        destination.Remove(contextFieldName);
        destination.Add(contextFieldName, Encoding.UTF8.GetBytes(contextFieldValue));
    }

    public IEnumerable<string> Extract(Headers location, string contextFieldName) =>
        location
            .Where(header => header.Key == contextFieldName)
            .Select(header => Encoding.UTF8.GetString(header.GetValueBytes()))
            .ToArray();
}
