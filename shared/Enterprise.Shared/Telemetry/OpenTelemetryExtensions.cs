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
    /// <param name="services"></param>
    extension(IServiceCollection services)
    {
        public IServiceCollection ConfigureOpenTelemetry(IConfiguration configuration, string appName)
        {
            var openTelemetryConfiguration = configuration.GetSection(OpenTelemetryConfiguration.Key).Get<OpenTelemetryConfiguration>();
            ArgumentNullException.ThrowIfNull(openTelemetryConfiguration);
            var otlpEndpoint = configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

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
                    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    {
                        logging.AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(otlpEndpoint);
                            options.Protocol = OtlpExportProtocol.Grpc;
                        });
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

                    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    {
                        metrics.AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(otlpEndpoint);
                            options.Protocol = OtlpExportProtocol.Grpc;
                        });
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
                        tracing.AddConsoleExporter();
                    }

                    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    {
                        if (openTelemetryConfiguration.ExcludeOutboxTelemetry)
                        {
                            tracing.AddProcessor(
                                new OutboxTraceFilteringProcessor(
                                    new BatchActivityExportProcessor(
                                        new OtlpTraceExporter(new OtlpExporterOptions
                                        {
                                            Endpoint = new Uri(otlpEndpoint), Protocol = OtlpExportProtocol.Grpc
                                        }))));
                        }
                        else
                        {
                            tracing.AddOtlpExporter(options =>
                            {
                                options.Endpoint = new Uri(otlpEndpoint);
                                options.Protocol = OtlpExportProtocol.Grpc;
                            });
                        }
                    }
                });

            return services;
        }
    }
}
