using HotChocolate.Diagnostics;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.GraphQL;

public static class GraphqlInstrumentationExtension
{
    extension(IRequestExecutorBuilder builder)
    {
        /// <summary>
        ///     Used to display only the necessary spans in OpenTelemetry tracing
        /// </summary>
        public IRequestExecutorBuilder AddCustomGraphqlInstrumentation() =>
            builder.AddInstrumentation(options => options.Scopes = ActivityScopes.All);
    }
}
