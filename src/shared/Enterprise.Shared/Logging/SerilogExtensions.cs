using Enterprise.Shared.Telemetry.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.OpenTelemetry;

namespace Enterprise.Shared.Logging;

public static class SerilogExtensions
{
    extension(IHostBuilder hostBuilder)
    {
        public IHostBuilder UseSerilogCustom(string? appName) =>
            hostBuilder.UseSerilog((hostingContext, serviceProvider, loggerConfiguration) =>
                {
                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        loggerConfiguration.WriteTo.Console();
                    }

                    loggerConfiguration
                        .Enrich.WithProperty("ApplicationContext", appName)
                        .Enrich.WithSpan()
                        .Enrich.FromLogContext()
                        .Enrich.WithSensitiveDataMasking(new SensitiveDataEnricherOptions
                        {
                            Mode = MaskingMode.Globally,
                            MaskValue = "***REDACTED***",
                        })
                        .Filter.ByExcluding(logEvent =>
                        {
                            if (logEvent.Level is LogEventLevel.Fatal or LogEventLevel.Error or LogEventLevel.Warning)
                            {
                                return false;
                            }

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

                    var configuration = hostingContext.Configuration;
                    var openTelemetryConfiguration = configuration
                        .GetSection(OpenTelemetryConfiguration.Key)
                        .Get<OpenTelemetryConfiguration>();
                    var logsEndpoint = configuration["OTEL_EXPORTER_OTLP_LOGS_ENDPOINT"];
                    var protocolName = configuration["OTEL_EXPORTER_OTLP_LOGS_PROTOCOL"]
                                       ?? configuration["OTEL_EXPORTER_OTLP_PROTOCOL"];
                    var protocol = ParseOtlpProtocol(protocolName);

                    if (string.IsNullOrWhiteSpace(logsEndpoint))
                    {
                        var endpoint = configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
                        if (string.IsNullOrWhiteSpace(endpoint))
                        {
                            endpoint = openTelemetryConfiguration?.LocalOtlpEndpoint;
                        }

                        if (!string.IsNullOrWhiteSpace(endpoint))
                        {
                            logsEndpoint = protocol == OtlpProtocol.HttpProtobuf ? AppendLogsSignalPath(endpoint) : endpoint;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(logsEndpoint))
                    {
                        loggerConfiguration.WriteTo.OpenTelemetry(logsEndpoint, protocol);
                    }
                },
                false,
                true);
    }

    private static OtlpProtocol ParseOtlpProtocol(string? protocol) => protocol?.Trim().ToLowerInvariant() switch
    {
        "http/protobuf" or "http_protobuf" or "httpprotobuf" => OtlpProtocol.HttpProtobuf,
        "grpc" => OtlpProtocol.Grpc,
        _ => OtlpProtocol.Grpc,
    };

    private static string AppendLogsSignalPath(string endpoint) => endpoint.TrimEnd('/') + "/v1/logs";
}
