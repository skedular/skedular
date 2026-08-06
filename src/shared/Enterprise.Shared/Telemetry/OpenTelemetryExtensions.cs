using Enterprise.Shared.Metrics;
using Enterprise.Shared.Telemetry.Configurations;
using Enterprise.Shared.Telemetry.PropagatorFunctions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Enterprise.Shared.Telemetry;

public static class OpenTelemetryExtensions
{
    private const string LogsOtlpSignalName = "LOGS";
    private const string MetricsOtlpSignalName = "METRICS";
    private const string TracesOtlpSignalName = "TRACES";

    private static bool IsOtlpExporterConfigured(IConfiguration configuration, string signalName) =>
        !string.IsNullOrWhiteSpace(configuration[$"OTEL_EXPORTER_OTLP_{signalName}_ENDPOINT"])
        || !string.IsNullOrWhiteSpace(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

    private static OtlpExporterOptions CreateOtlpExporterOptions(IConfiguration configuration, string signalName)
    {
        var options = new OtlpExporterOptions();
        ConfigureOtlpExporter(configuration, options, signalName);
        return options;
    }

    private static void ConfigureOtlpExporter(IConfiguration configuration, OtlpExporterOptions options, string signalName)
    {
        ConfigureOtlpProtocol(configuration, options, signalName);
        ConfigureOtlpEndpoint(configuration, options, signalName);
        ConfigureOtlpHeaders(configuration, options, signalName);
        ConfigureOtlpTimeout(configuration, options, signalName);
    }

    private static void ConfigureOtlpEndpoint(IConfiguration configuration, OtlpExporterOptions options, string signalName)
    {
        var signalEndpoint = configuration[$"OTEL_EXPORTER_OTLP_{signalName}_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(signalEndpoint))
        {
            options.Endpoint = new Uri(signalEndpoint);
            return;
        }

        var endpoint = configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(endpoint))
        {
            var endpointUri = new Uri(endpoint);
            options.Endpoint = options.Protocol == OtlpExportProtocol.HttpProtobuf
                ? AppendOtlpHttpSignalPath(endpointUri, signalName)
                : endpointUri;
        }
    }

    private static Uri AppendOtlpHttpSignalPath(Uri endpoint, string signalName)
    {
        var signalPath = signalName switch
        {
            LogsOtlpSignalName => "v1/logs",
            MetricsOtlpSignalName => "v1/metrics",
            TracesOtlpSignalName => "v1/traces",
            _ => throw new ArgumentOutOfRangeException(nameof(signalName), signalName,
                $"Unexpected value for {nameof(signalName)}: {signalName}. Update enum mapping or caller input."),
        };

        return new Uri($"{endpoint.ToString().TrimEnd('/')}/{signalPath}");
    }

    private static void ConfigureOtlpProtocol(IConfiguration configuration, OtlpExporterOptions options, string signalName)
    {
        var protocol = GetSignalOrDefaultOtlpConfigurationValue(configuration, signalName, "PROTOCOL");
        if (string.IsNullOrWhiteSpace(protocol))
        {
            return;
        }

        options.Protocol = protocol.Trim().ToLowerInvariant() switch
        {
            "grpc" => OtlpExportProtocol.Grpc,
            "http/protobuf" or "http_protobuf" or "httpprotobuf" => OtlpExportProtocol.HttpProtobuf,
            _ when Enum.TryParse<OtlpExportProtocol>(protocol, true, out var parsedProtocol) => parsedProtocol,
            _ => options.Protocol,
        };
    }

    private static void ConfigureOtlpHeaders(IConfiguration configuration, OtlpExporterOptions options, string signalName)
    {
        var headers = GetSignalOrDefaultOtlpConfigurationValue(configuration, signalName, "HEADERS");
        if (!string.IsNullOrWhiteSpace(headers))
        {
            options.Headers = headers;
        }
    }

    private static void ConfigureOtlpTimeout(IConfiguration configuration, OtlpExporterOptions options, string signalName)
    {
        var timeout = GetSignalOrDefaultOtlpConfigurationValue(configuration, signalName, "TIMEOUT");
        if (int.TryParse(timeout, out var timeoutMilliseconds))
        {
            options.TimeoutMilliseconds = timeoutMilliseconds;
        }
    }

    private static string? GetSignalOrDefaultOtlpConfigurationValue(IConfiguration configuration, string signalName, string optionName) =>
        configuration[$"OTEL_EXPORTER_OTLP_{signalName}_{optionName}"]
        ?? configuration[$"OTEL_EXPORTER_OTLP_{optionName}"];

    /// <param name="services"></param>
    extension(IServiceCollection services)
    {
        public IServiceCollection ConfigureOpenTelemetry(IConfiguration configuration, string appName)
        {
            var openTelemetryConfiguration = configuration.GetSection(OpenTelemetryConfiguration.Key).Get<OpenTelemetryConfiguration>();
            ArgumentNullException.ThrowIfNull(openTelemetryConfiguration);

            services
                .AddSingleton(openTelemetryConfiguration)
                .AddSingleton<IActivityAccessor, ActivityAccessor>()
                .AddSingleton<TextMapPropagator, StandardTextMapPropagator>()
                .AddSingleton<IOpenTelemetryInstrumentation, OpenTelemetryInstrumentation>()
                .AddSingleton<IActivityGetter, ActivityGetter>()
                .AddSingleton<IPropagationContextGetter, PropagationContextGetter>()
                .AddSingleton(typeof(IActivityPropagator<>), typeof(ActivityPropagator<>))
                .AddSingleton<IPropagatorFunctionProvider<IDictionary<string, string>>, StringDictionaryPropagatorFunctions>()
                .AddSingleton<IPropagatorFunctionProvider<IPropagatorEntity>, PropagatorEntityFunctions>();

            services
                .AddOpenTelemetry()
                .WithLogging(logging =>
                {
                    if (IsOtlpExporterConfigured(configuration, LogsOtlpSignalName))
                    {
                        logging.AddOtlpExporter(options => ConfigureOtlpExporter(configuration, options, LogsOtlpSignalName));
                    }
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();

                    if (openTelemetryConfiguration.MetricsIngestEnabled)
                    {
                        metrics.AddMeter(openTelemetryConfiguration.MeterProviderName, MeterProviderNaming.MeterProviderVersion);
                    }

                    if (openTelemetryConfiguration.ConsoleEnabled)
                    {
                        metrics.AddConsoleExporter();
                    }

                    if (IsOtlpExporterConfigured(configuration, MetricsOtlpSignalName))
                    {
                        metrics.AddOtlpExporter(options => ConfigureOtlpExporter(configuration, options, MetricsOtlpSignalName));
                    }
                })
                .WithTracing(tracing =>
                {
                    // Service
                    tracing.SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(appName)
                            .AddTelemetrySdk()
                            .AddEnvironmentVariableDetector());

                    // Instrumentation
                    tracing.AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = context =>
                            !context.Request.Path.StartsWithSegments(HealthCheck.Constants.ReadinessPath)
                            && !context.Request.Path.StartsWithSegments(HealthCheck.Constants.LivenessPath);
                    });

                    tracing
                        .AddHttpClientInstrumentation();

                    if (openTelemetryConfiguration.ConsoleEnabled)
                    {
                        if (openTelemetryConfiguration.ExcludeOutboxTelemetry)
                        {
                            tracing.AddProcessor(
                                new OutboxTraceFilteringProcessor(
                                    new SimpleActivityExportProcessor(
                                        new ConsoleActivityExporter(new ConsoleExporterOptions()))));
                        }
                        else
                        {
                            tracing.AddConsoleExporter();
                        }
                    }

                    if (IsOtlpExporterConfigured(configuration, TracesOtlpSignalName))
                    {
                        if (openTelemetryConfiguration.ExcludeOutboxTelemetry)
                        {
                            tracing.AddProcessor(
                                new OutboxTraceFilteringProcessor(
                                    new BatchActivityExportProcessor(
                                        new OtlpTraceExporter(CreateOtlpExporterOptions(configuration, TracesOtlpSignalName)))));
                        }
                        else
                        {
                            tracing.AddOtlpExporter(options => ConfigureOtlpExporter(configuration, options, TracesOtlpSignalName));
                        }
                    }
                });

            return services;
        }
    }
}
