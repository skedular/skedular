using System.Diagnostics;
using System.Diagnostics.Metrics;
using Enterprise.Shared.Metrics;
using HotChocolate.Execution;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.GraphQL;

public class GraphqlErrorFilter(IOpenTelemetryInstrumentation meters, ILogger<GraphqlErrorFilter> logger) : IErrorFilter
{
    private readonly Counter<long> _graphqlExceptionsCounter = meters.GetCounterByName<long>(MetricNames.GraphqlExceptionsCounter);

    public IError OnError(IError error)
    {
        var className = GetType().ToFullName();
        _graphqlExceptionsCounter.Add(1, GetTags(className));
        logger.LogWarning(error.Exception, "[{Type}] - Error ", className);

        error = error.WithMessage(error.Message);
        if (error.Exception is not null)
        {
            error = error.WithMessage(error.Exception.Message);
        }

        return error;
    }

    private static TagList GetTags(string errorTypeName) => new()
    {
        { "error-type", errorTypeName },
    };
}
