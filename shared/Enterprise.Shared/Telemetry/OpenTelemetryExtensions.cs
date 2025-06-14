using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Confluent.Kafka;
using Enterprise.Shared.Kafka.Telemetry;
using Enterprise.Shared.Metrics;
using Enterprise.Shared.Telemetry.Configurations;
using Enterprise.Shared.Telemetry.PropagatorFunctions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Enterprise.Shared.Telemetry;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection ConfigureOpenTelemetry(this IServiceCollection services, IConfiguration configuration, string appName)
    {
        var openTelemetrySettings = configuration.GetSection(OpenTelemetrySettings.Key).Get<OpenTelemetrySettings>();

        services
            .AddSingleton<IActivityAccessor, ActivityAccessor>()
            .AddSingleton<TextMapPropagator, StandardTextMapPropagator>()
            .AddSingleton<IOpenTelemetryInstrumentation, OpenTelemetryInstrumentation>()
            .AddSingleton<IActivityGetter, ActivityGetter>()
            .AddSingleton<IPropagationContextGetter, PropagationContextGetter>()
            .AddSingleton<IKafkaActivityStarter, KafkaActivityStarter>()
            .AddSingleton(typeof(IActivityPropagator<>), typeof(ActivityPropagator<>))
            .AddSingleton<IPropagatorFunctionProvider<Headers>, HeaderPropagatorFunctions>()
            .AddSingleton<IPropagatorFunctionProvider<IDictionary<string, string>>, StringDictionaryPropagatorFunctions>()
            .AddSingleton<IPropagatorFunctionProvider<IPropagatorEntity>, PropagatorEntityFunctions>();

        services
            .AddOpenTelemetry()
            .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddNpgsqlInstrumentation();

                    if (openTelemetrySettings is null)
                    {
                        return;
                    }

                    if (openTelemetrySettings.MetricsIngestEnabled)
                    {
                        metrics.AddMeter(MeterProviderNaming.MeterProviderName, MeterProviderNaming.MeterProviderVersion);
                    }

                    if (openTelemetrySettings.ConsoleEnabled)
                    {
                        metrics.AddConsoleExporter();
                    }
                }
            )
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
                    .AddSqlClientInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsql()
                    .AddHotChocolateInstrumentation();

                if (openTelemetrySettings is not null && openTelemetrySettings.EntityFrameworkEnabled)
                {
                    tracing.AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.EnrichWithIDbCommand = delegate(Activity activity, IDbCommand command)
                        {
                            activity.DisplayName = $"{command.CommandType} main";
                            activity.SetTag("db.type", command.CommandType);
                            activity.SetTag("db.text", command.CommandText);
                            activity.SetTag(
                                "db.parameters",
                                string.Join(",",
                                    command.Parameters.OfType<DbParameter>()
                                        .Select(parameter => $"{parameter.ParameterName}={parameter.Value}")));
                        };
                    });
                }


                services
                    .AddActivitySource(TelemetryKeys.IncomingActivitySourceName)
                    .AddActivitySource(TelemetryKeys.ConsumerActivitySourceName)
                    .AddActivitySource(TelemetryKeys.ProducerActivitySourceName)
                    .AddActivitySource(Outbox.Telemetry.TelemetryKeys.KafkaActivitySourceName);

                if (openTelemetrySettings is null)
                {
                    return;
                }

                if (openTelemetrySettings.ConsoleEnabled)
                {
                    tracing.AddConsoleExporter();
                }

                if (!string.IsNullOrWhiteSpace(openTelemetrySettings.ZipkinEndpoint))
                {
                    tracing.AddZipkinExporter(options => options.Endpoint = new Uri(openTelemetrySettings.ZipkinEndpoint));
                }

                if (!string.IsNullOrWhiteSpace(openTelemetrySettings.JaegerEndpoint))
                {
                    tracing.AddJaegerExporter(options => options.Endpoint = new Uri(openTelemetrySettings.JaegerEndpoint));
                }
            });

        return services.AddOtherOpenTelemetryExporters(configuration);
    }

    private static IServiceCollection AddOtherOpenTelemetryExporters(this IServiceCollection services, IConfiguration configuration)
    {
        if (!string.IsNullOrWhiteSpace(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
        {
            services.AddOpenTelemetry().UseOtlpExporter();
        }

        return services;
    }

    /// <summary>
    ///     Adds an activity source using the provided name. The activity source is added to the telemetry source list
    ///     automatically
    /// </summary>
    /// <param name="services"></param>
    /// <param name="activitySourceName"></param>
    /// <returns></returns>
    public static IServiceCollection AddActivitySource(this IServiceCollection services, string activitySourceName) =>
        services
            .AddSingleton<IActivitySource>(_ => new ActivitySourceFacade(activitySourceName))
            .ConfigureOpenTelemetryTracerProvider(builder => builder.AddSource(activitySourceName));
}
