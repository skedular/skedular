using System.Diagnostics;
using Confluent.SchemaRegistry;
using Enterprise.Shared.Azure.Graph;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.HealthCheck;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Logging;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Security.Sso;
using Enterprise.Shared.Security.Token;
using Enterprise.Shared.Telemetry;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
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
    public static WebApplicationBuilder AddServiceDefaults<TProgram>(this WebApplicationBuilder builder) where TProgram : class
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        var appName = typeof(TProgram).Assembly.GetName().Name ?? builder.Environment.ApplicationName;
        AppDomain.CurrentDomain.UnhandledException += RecordExceptionOnActivity;

        configuration.AddEnvironmentVariables("ASPNETCORE");
        configuration.BuildConfig<TProgram>();

        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        ArgumentNullException.ThrowIfNull(applicationConfiguration);
        services.AddSingleton(applicationConfiguration);

        var kafkaConfiguration = configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        if (kafkaConfiguration is not null)
        {
            var bootstrapServers = configuration.GetConnectionString("kafka");
            if (!string.IsNullOrWhiteSpace(bootstrapServers))
            {
                kafkaConfiguration.BootstrapServers = bootstrapServers;
            }

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

        services.AddProblemDetails();

        builder.ConfigureOpenTelemetry(appName);

        if (builder.Environment.IsDevelopment())
        {
            services.AddSwaggerDocument();
        }

        services.AddServiceDiscovery();
        services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        services.AddAuthentication();
        services.AddAuthorization();
        services.AddGrpc();
        services.AddSso();
        services.AddOutboxService();

        services.AddSingleton<IGraphServiceClientFactory, GraphServiceClientFactory>();

        services.AddSingleton<IVersionService, VersionService<TProgram>>();

        services.TryAddSingleton(TimeProvider.System);
        services.AddHttpContextAccessor();

        services
            .AddKafka(configuration)
            .AddRedis(configuration, "RedisConnection")
            .AddMemoryCache()
            .AddSecurity(configuration)
            .AddContext()
            .AddRandomHelper();

        services.AddCors();

        services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), [Constants.LivenessTag]);

        services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(TProgram).Assembly));

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

    public static WebApplication AddWebApplicationDefaults(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseOpenApi();
            app.UseSwaggerUi();

            // redirect root to health
            app.UseRewriter(new RewriteOptions().AddRedirect("^$", Constants.ReadinessPath));
        }

        app.UseRouting();

        // UseAuthentication must appear between UseRouting and UseEndpoints
        app.UseAuthentication();

        // UseAuthorization must appear between UseRouting and UseEndpoints
        app.UseAuthorization();

        // Health checks must go before any middleware
        app.UseHealthChecks(
            Constants.LivenessPath,
            new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains(Constants.LivenessTag) || registration.Name.Contains("self")
            });

        app.UseHealthChecks(
            Constants.ReadinessPath,
            new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains(Constants.ReadinessTag) || registration.Name.Contains("services")
            });

        app
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
