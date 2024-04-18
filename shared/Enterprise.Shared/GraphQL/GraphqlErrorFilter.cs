using System.Diagnostics;
using System.Diagnostics.Metrics;
using Enterprise.Shared.Metrics;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.GraphQL;

public class GraphqlErrorFilter(IOpenTelemetryInstrumentation meters, ILogger<GraphqlErrorFilter> logger)
    : IErrorFilter, ITaggable<string>
{
    private readonly Counter<long> _graphqlExceptionsCounter =
        meters.GetCounterByName<long>(MetricNames.GraphqlExceptionsCounter);

    public IError OnError(IError error)
    {
        _graphqlExceptionsCounter.Add(1, GetTags(GetType().Name));
        logger.LogWarning(error.Exception, "[{Type}] - Error ", GetType().Name);

        return error.WithMessage(error.Exception?.Message ?? string.Empty);
    }

    public TagList GetTags(string errorTypeName) => new() { { "error-type", errorTypeName } };
}
