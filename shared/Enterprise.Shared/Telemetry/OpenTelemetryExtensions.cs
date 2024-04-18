using Confluent.Kafka;
using Enterprise.Shared.Kafka.Telemetry;
using Enterprise.Shared.Metrics;
using Enterprise.Shared.Telemetry.Configurations;
using Enterprise.Shared.Telemetry.PropagatorFunctions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Enterprise.Shared.Telemetry;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection WithOpenTelemetryCustom(
        this IServiceCollection services,
        IConfiguration configuration,
        string appName,
        string environ = "")
    {
        services.AddSingleton<IActivityAccessor, ActivityAccessor>();
        services.AddSingleton<TextMapPropagator, StandardTextMapPropagator>();

        var openTelemetrySettings = configuration
            .GetSection("OpenTelemetry")
            .Get<OpenTelemetrySettings>();

        services.AddSingleton<IOpenTelemetryInstrumentation, OpenTelemetryInstrumentation>();
        services.AddSingleton<IActivityGetter, ActivityGetter>();
        services.AddSingleton<IPropagationContextGetter, PropagationContextGetter>();

        services.AddSingleton<IKafkaActivityStarter, KafkaActivityStarter>();
        services.AddSingleton(typeof(IActivityPropagator<>), typeof(ActivityPropagator<>));

        services
            .AddSingleton<IPropagatorFunctionProvider<Headers>, HeaderPropagatorFunctions>()
            .AddSingleton<IPropagatorFunctionProvider<IDictionary<string, string>>,
                StringDictionaryPropagatorFunctions>()
            .AddSingleton<IPropagatorFunctionProvider<IPropagatorEntity>,
                PropagatorEntityFunctions>();


        services.AddOpenTelemetry()
            .WithMetrics(builder =>
                {
                    if (openTelemetrySettings is
                        {
                            MetricsIngestEnabled: true
                        })
                    {
                        builder.AddMeter(MeterProviderNaming.UnityHubMeterProviderName,
                            MeterProviderNaming.UnityHubMeterProviderVersion);
                    }
                }
            )
            .WithTracing(builder =>
            {
                // Service
                builder.SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(appName)
                        .AddTelemetrySdk()
                        .AddEnvironmentVariableDetector());

                // Instrumentation
                builder.AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = httpContext =>
                    {
                        var pathString = httpContext.Request.Path.ToString();

                        return !pathString.Contains("/health");
                    };
                });

                builder.AddSqlClientInstrumentation();
                builder.AddHttpClientInstrumentation();
                builder.AddHotChocolateInstrumentation();

                services
                    .AddActivitySource(TelemetryKeys.IncomingActivitySourceName)
                    .AddActivitySource(TelemetryKeys.ConsumerActivitySourceName)
                    .AddActivitySource(TelemetryKeys.ProducerActivitySourceName)
                    .AddActivitySource(Outbox.Telemetry.TelemetryKeys.ActivitySourceName);

                var telemetrySettings = configuration
                    .GetSection("OpenTelemetry")
                    .Get<OpenTelemetrySettings>();

                // Exporters
                if (telemetrySettings is
                    {
                        ConsoleEnabled: true
                    })
                {
                    builder.AddConsoleExporter();
                }

                if (telemetrySettings is
                    {
                        ZipkinEnabled: true
                    })
                    // Zipkin is the lighter / less featured predecessor of Jaegar
                {
                    builder.AddZipkinExporter(options =>
                    {
                        options.Endpoint =
                            new Uri(telemetrySettings.ZipkinEndpoint);
                    });
                }

                if (telemetrySettings is
                    {
                        OtlpEnabled: true
                    })
                {
                    builder.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                        otlpOptions.Endpoint = new Uri(telemetrySettings.OtlpEndpoint);
                    });
                }
            });

        return services;
    }

    /// <summary>
    ///     Adds an activity source using the provided name. The activity source is added to the telemetry source list
    ///     automatically
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="activitySourceName"></param>
    /// <returns></returns>
    public static IServiceCollection AddActivitySource(
        this IServiceCollection serviceCollection,
        string activitySourceName)
    {
        serviceCollection.AddSingleton<IActivitySource>(_ =>
            new ActivitySourceFacade(activitySourceName));

        serviceCollection.ConfigureOpenTelemetryTracerProvider(builder =>
            builder.AddSource(activitySourceName));

        return serviceCollection;
    }
}
