using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Enterprise.Shared.Telemetry.PropagatorFunctions;

/// <summary>
///     Supporting functions for propagating context to <see cref="IPropagatorEntity" />
/// </summary>
public class PropagatorEntityFunctions(ILogger<PropagatorEntityFunctions> logger)
    : IPropagatorFunctionProvider<IPropagatorEntity>
{
    /// <summary>
    ///     Inject context fields into a <see cref="IPropagatorEntity" />
    /// </summary>
    /// <param name="destination">Entity</param>
    /// <param name="contextFieldName">Field name</param>
    /// <param name="contextFieldValue">Field value</param>
    public void Inject(
        IPropagatorEntity destination,
        string contextFieldName,
        string contextFieldValue)
    {
        JObject context;

        try
        {
            context = string.IsNullOrEmpty(destination.TraceContext)
                ? new JObject()
                : JObject.Parse(destination.TraceContext);
        }
        catch (JsonReaderException readerException)
        {
            logger.LogWarning(readerException, "Failed to parse trace context on Inject `{Context}`",
                destination.TraceContext);

            // reset context
            context = new JObject();
        }

        context[contextFieldName] = contextFieldValue;
        destination.TraceContext = context.ToString(Formatting.None);
    }

    public IEnumerable<string> Extract(IPropagatorEntity location, string contextFieldName)
    {
        if (string.IsNullOrEmpty(location.TraceContext))
        {
            return Enumerable.Empty<string>();
        }

        try
        {
            var jObject = JObject.Parse(location.TraceContext);
            var token = jObject[contextFieldName];

            return token is null ? Enumerable.Empty<string>() : new[] { token.ToString() };
        }
        catch (JsonReaderException readerException)
        {
            logger.LogWarning(readerException, "Failed to parse trace context on Extract `{Context}`",
                location.TraceContext);

            return Enumerable.Empty<string>();
        }
    }
}
