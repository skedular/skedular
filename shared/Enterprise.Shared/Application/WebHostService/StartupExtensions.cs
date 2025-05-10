using System.Diagnostics;
using Confluent.SchemaRegistry;
using Enterprise.Shared.Azure.Graph;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.HealthCheck;
using Enterprise.Shared.Infrastructure.Filters;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Logging;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Security.Sso;
using Enterprise.Shared.Security.Token;
using Enterprise.Shared.Telemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Application.WebHostService;

public static class StartupExtensions
{
    private const string ReadinessPath = "/health/readiness";
    private const string LivenessPath = "/health/liveness";

    public static WebApplicationBuilder AddDefaultServices<TProgram>(this WebApplicationBuilder builder) where TProgram : class
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        var appName = typeof(TProgram).Assembly.GetName().Name;
        AppDomain.CurrentDomain.UnhandledException += RecordExceptionOnActivity;

        configuration.AddEnvironmentVariables("ASPNETCORE");
        configuration.BuildConfig<TProgram>();

        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        ArgumentNullException.ThrowIfNull(applicationConfiguration);
        services.AddSingleton(applicationConfiguration);

        var kafkaConfiguration = configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        if (kafkaConfiguration is not null)
        {
            services.AddSingleton(kafkaConfiguration);
            if (kafkaConfiguration.SchemaRegistry is not null)
            {
                services.AddSingleton(new SchemaRegistryConfig { Url = kafkaConfiguration.SchemaRegistry.Url });
            }
        }

        var azureEntraConfiguration = configuration.GetSection(AzureEntraConfiguration.Key).Get<AzureEntraConfiguration>();
        if (azureEntraConfiguration is null)
        {
            services.AddSingleton(new AzureEntraConfiguration());
        }
        else
        {
            if (string.IsNullOrWhiteSpace(azureEntraConfiguration.ClientId))
            {
                Console.Error.WriteLine("azureEntraConfiguration.ClientId is null");
            }

            if (string.IsNullOrWhiteSpace(azureEntraConfiguration.ClientSecret))
            {
                Console.Error.WriteLine("azureEntraConfiguration.ClientSecret is null");
            }

            services.AddSingleton(azureEntraConfiguration);
        }

        services.WithOpenTelemetryCustom(configuration, typeof(TProgram).Assembly.GetName().Name!);

        if (builder.Environment.IsDevelopment())
        {
            services.AddSwaggerDocument();
        }

        services.AddAuthentication();
        services.AddAuthorization();
        services.AddGrpc();
        services.AddSso();
        services.AddOutboxService();

        services.AddSingleton<IGraphServiceClientFactory, GraphServiceClientFactory>();

        services.TryAddSingleton(TimeProvider.System);
        services.AddHttpContextAccessor();

        services
            .AddKafka()
            .AddRedis(configuration, "RedisConnection")
            .AddMemoryCache()
            .AddSecurity(configuration)
            .AddContext()
            .AddRandomHelper();

        services.AddCors();

        services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), [HealthCheckTags.Liveness]);

        services.AddScoped<IGlobalHttpExceptionHandler, GlobalHttpExceptionHandler>();

        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(HttpGlobalExceptionFilter));

            if (services.Any(descriptor => descriptor.ServiceType == typeof(TraceSettings)))
            {
                options.Filters.Add<TraceIdAsyncActionFilter>();
            }
        });

        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        services
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddEventSourceLogger();
            });
        builder.Host.UseSerilogCustom(appName);

        Console.WriteLine($"EnvironmentName = {builder.Environment.EnvironmentName}");
        Console.WriteLine($"AppName = {appName}");

        return builder;
    }

    public static WebApplication UseApplicationBuilderDefaults(this WebApplication app, Action? middleAction = null)
    {
        app.UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseOpenApi();
            app.UseSwaggerUi();

            // redirect root to health
            app.UseRewriter(new RewriteOptions().AddRedirect("^$", ReadinessPath));
        }

        app.UseRouting();

        // UseAuthentication must appear between UseRouting and UseEndpoints
        app.UseAuthentication();

        // UseAuthorization must appear between UseRouting and UseEndpoints
        app.UseAuthorization();

        // Health checks must go before any middleware
        app.UseHealthChecks(
            LivenessPath,
            new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains(HealthCheckTags.Liveness) || registration.Name.Contains("self")
            });

        app.UseHealthChecks(
            ReadinessPath,
            new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains(HealthCheckTags.Readiness) || registration.Name.Contains("services")
            });

        app
            .UseMiddleware<TelemetryMiddleware>()
            .UseMiddleware<SecurityContextEnricherMiddleware>()
            .UseMiddleware<SsoContextEnricherMiddleware>()
            .UseMiddleware<ContextEnricherMiddleware>();

        app.MapGraphqlEndpoints(app.Configuration);
        app.MapControllers();

        return app;
    }

    private static void RecordExceptionOnActivity(object sender, UnhandledExceptionEventArgs e)
    {
        if (Activity.Current is { } activity && e.ExceptionObject is Exception ex)
        {
            activity.AddException(ex);
        }
    }
}
