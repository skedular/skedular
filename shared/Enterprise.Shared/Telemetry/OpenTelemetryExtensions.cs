using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Confluent.Kafka;
using Enterprise.Shared.Kafka.Telemetry;
using Enterprise.Shared.Metrics;
using Enterprise.Shared.Telemetry.Configurations;
using Enterprise.Shared.Telemetry.PropagatorFunctions;
using Microsoft.AspNetCore.Http;
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
        string appName)
    {
        services.AddSingleton<IActivityAccessor, ActivityAccessor>();
        services.AddSingleton<TextMapPropagator, StandardTextMapPropagator>();

        var openTelemetrySettings = configuration.GetSection(OpenTelemetrySettings.Key).Get<OpenTelemetrySettings>();

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

        services
            .AddOpenTelemetry()
            .WithMetrics(builder =>
                {
                    if (openTelemetrySettings is not null && openTelemetrySettings.MetricsIngestEnabled)
                    {
                        builder.AddMeter(
                            MeterProviderNaming.UnityHubMeterProviderName,
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
                    options.Filter = delegate(HttpContext httpContext)
                    {
                        var pathString = httpContext.Request.Path.ToString();

                        return !pathString.Contains("/health");
                    };
                });

                builder.AddSqlClientInstrumentation();
                builder.AddHttpClientInstrumentation();
                builder.AddHotChocolateInstrumentation();

                if (openTelemetrySettings is not null)
                {
                    if (openTelemetrySettings.EntityFrameworkEnabled)
                    {
                        builder.AddEntityFrameworkCoreInstrumentation(options =>
                        {
                            options.EnrichWithIDbCommand = delegate(Activity activity, IDbCommand command)
                            {
                                activity.DisplayName = $"{command.CommandType} main";
                                activity.SetTag("db.type", command.CommandType);
                                activity.SetTag("db.text", command.CommandText);
                                activity.SetTag("db.parameters",
                                    string.Join(", ",
                                        command.Parameters.OfType<DbParameter>().Select(parameter =>
                                            $"{parameter.ParameterName}={parameter.Value}")));
                            };
                        });
                    }
                }

                services
                    .AddActivitySource(TelemetryKeys.IncomingActivitySourceName)
                    .AddActivitySource(TelemetryKeys.ConsumerActivitySourceName)
                    .AddActivitySource(TelemetryKeys.ProducerActivitySourceName)
                    .AddActivitySource(Outbox.Telemetry.TelemetryKeys.ActivitySourceName);

                if (openTelemetrySettings is not null)
                {
                    if (openTelemetrySettings.ConsoleEnabled)
                    {
                        builder.AddConsoleExporter();
                    }

                    if (openTelemetrySettings.ZipkinEnabled)
                    {
                        builder.AddZipkinExporter(options =>
                        {
                            options.Endpoint = new Uri(openTelemetrySettings.ZipkinEndpoint);
                        });
                    }

                    if (openTelemetrySettings.JaegerEnabled)
                    {
                        builder.AddJaegerExporter(options =>
                        {
                            options.Endpoint = new Uri(openTelemetrySettings.JaegerEndpoint);
                        });
                    }

                    if (openTelemetrySettings.OtlpEnabled)
                    {
                        builder.AddOtlpExporter(options =>
                        {
                            options.Protocol = OtlpExportProtocol.Grpc;
                            options.Endpoint = new Uri(openTelemetrySettings.OtlpEndpoint);
                        });
                    }
                }
            });

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
