namespace Enterprise.Shared.Telemetry.PropagatorFunctions;

public class
    StringDictionaryPropagatorFunctions : IPropagatorFunctionProvider<IDictionary<string, string>>
{
    public void Inject(
        IDictionary<string, string> destination,
        string contextFieldName,
        string contextFieldValue) =>
        destination[contextFieldName] = contextFieldValue;

    public IEnumerable<string> Extract(
        IDictionary<string, string> location,
        string contextFieldName) =>
        location.TryGetValue(contextFieldName, out var value)
            ? new[] { value }
            : [];
}
