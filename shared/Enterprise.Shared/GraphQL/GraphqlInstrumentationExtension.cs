using HotChocolate.Diagnostics;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.GraphQL;

public static class GraphqlInstrumentationExtension
{
    /// <summary>
    ///     Used to display only the necessary spans in OpenTelemetry tracing
    /// </summary>
    public static IRequestExecutorBuilder AddCustomGraphqlInstrumentation(this IRequestExecutorBuilder builder) =>
        builder.AddInstrumentation(options =>
        {
            options.Scopes = ActivityScopes.ExecuteHttpRequest |
                             ActivityScopes.FormatHttpResponse |
                             ActivityScopes.ExecuteRequest |
                             ActivityScopes.ParseDocument |
                             ActivityScopes.AnalyzeComplexity |
                             ActivityScopes.CoerceVariables |
                             ActivityScopes.ResolveFieldValue;
        });
}
