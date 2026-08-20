using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

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
        JsonObject context;

        try
        {
            context = string.IsNullOrEmpty(destination.TraceContext)
                ? new JsonObject()
                : JsonNode.Parse(destination.TraceContext)?.AsObject() ?? new JsonObject();
        }
        catch (JsonException readerException)
        {
            logger.LogWarning(readerException, "Failed to parse trace context on Inject `{Context}`",
                destination.TraceContext);

            // reset context
            context = new JsonObject();
        }

        context[contextFieldName] = contextFieldValue;
        destination.TraceContext = context.ToJsonString();
    }

    public IEnumerable<string> Extract(IPropagatorEntity location, string contextFieldName)
    {
        if (string.IsNullOrEmpty(location.TraceContext))
        {
            return [];
        }

        try
        {
            var jsonObject = JsonNode.Parse(location.TraceContext)?.AsObject();
            var token = jsonObject?[contextFieldName];

            return token is null ? [] : [token.ToString()];
        }
        catch (JsonException readerException)
        {
            logger.LogWarning(readerException, "Failed to parse trace context on Extract `{Context}`",
                location.TraceContext);

            return [];
        }
    }
}
