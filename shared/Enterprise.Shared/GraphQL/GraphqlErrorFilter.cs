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
        var className = GetType().ToFullName();
        _graphqlExceptionsCounter.Add(1, GetTags(className));
        logger.LogWarning(error.Exception, "[{Type}] - Error ", className);

        return error.WithMessage(error.Exception?.Message ?? string.Empty);
    }

    public TagList GetTags(string errorTypeName) => new() { { "error-type", errorTypeName } };
}
