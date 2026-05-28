using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Telemetry.PropagatorFunctions;

public class
    StringDictionaryPropagatorFunctions(ILogger<StringDictionaryPropagatorFunctions> logger)
    : IPropagatorFunctionProvider<IDictionary<string, string>>
{
    public void Inject(
        IDictionary<string, string> destination,
        string contextFieldName,
        string contextFieldValue)
    {
        logger.LogDebug("Injecting propagation field into string dictionary. FieldName={FieldName}", contextFieldName);
        destination[contextFieldName] = contextFieldValue;
    }

    public IEnumerable<string> Extract(
        IDictionary<string, string> location,
        string contextFieldName)
    {
        logger.LogDebug("Extracting propagation field from string dictionary. FieldName={FieldName}", contextFieldName);
        return location.TryGetValue(contextFieldName, out var value)
            ? new[] { value }
            : [];
    }
}
