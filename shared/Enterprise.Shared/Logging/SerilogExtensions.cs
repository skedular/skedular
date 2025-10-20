using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Enterprise.Shared.Logging;

public static class SerilogExtensions
{
    public static IHostBuilder UseSerilogCustom(this IHostBuilder hostBuilder, string? appName) =>
        hostBuilder.UseSerilog((hostingContext, _, loggerConfiguration) =>
            {
                if (hostingContext.HostingEnvironment.IsDevelopment())
                {
                    loggerConfiguration.WriteTo.Console();
                }

                loggerConfiguration
                    .Enrich.WithProperty("ApplicationContext", appName)
                    .Enrich.WithSpan()
                    .Enrich.FromLogContext()
                    .Enrich.WithSensitiveDataMasking(new SensitiveDataEnricherOptions { Mode = MaskingMode.Globally, MaskValue = "***REDACTED***" })
                    .Filter.ByExcluding(logEvent =>
                    {
                        if (!logEvent.Properties.TryGetValue("SourceContext", out var sourceContextValue) ||
                            sourceContextValue is not ScalarValue { Value: string sourceContext } ||
                            sourceContext != "Microsoft.AspNetCore.Hosting.Diagnostics")
                        {
                            return false;
                        }

                        if (!logEvent.Properties.TryGetValue("RequestPath", out var requestPathValue) ||
                            requestPathValue is not ScalarValue { Value: string requestPath })
                        {
                            return false;
                        }

                        return requestPath.Equals(HealthCheck.Constants.ReadinessPath, StringComparison.InvariantCultureIgnoreCase) ||
                               requestPath.Equals(HealthCheck.Constants.LivenessPath, StringComparison.InvariantCultureIgnoreCase);
                    })
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .WriteTo.Console(new RenderedCompactJsonFormatter());
            },
            false,
            true);
}
