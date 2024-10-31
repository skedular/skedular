using Confluent.SchemaRegistry;
using Enterprise.Shared.Azure.Graph;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Security.Jobs;
using Enterprise.Shared.Telemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Enterprise.Shared.Application.WebHostService;

public abstract class StartupCustom(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
{
    protected IConfiguration Configuration { get; } = configuration;
    protected IWebHostEnvironment Environment { get; } = webHostEnvironment;

    public virtual void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(Environment, Configuration);

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .WithOpenTelemetryCustom(Configuration, GetType().Assembly.GetName().Name!, Environment.EnvironmentName);

        var applicationConfiguration =
            Configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        ArgumentNullException.ThrowIfNull(applicationConfiguration);
        services.AddSingleton(applicationConfiguration);

        var kafkaConfiguration = Configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        if (kafkaConfiguration is not null)
        {
            services.AddSingleton(kafkaConfiguration);
            if (kafkaConfiguration.SchemaRegistry is not null)
            {
                services.AddSingleton(new SchemaRegistryConfig { Url = kafkaConfiguration.SchemaRegistry.Url });
            }
        }

        var azureEntraConfiguration =
            Configuration.GetSection(AzureEntraConfiguration.Key).Get<AzureEntraConfiguration>();
        if (azureEntraConfiguration is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(azureEntraConfiguration.ClientId);
            ArgumentException.ThrowIfNullOrWhiteSpace(azureEntraConfiguration.ClientSecret);
            services.AddSingleton(azureEntraConfiguration);
        }

        if (Environment.IsDevelopment())
        {
            services.AddSwaggerDocument();
        }

        services.AddAuthentication();
        services.AddAuthorization();

        services.AddGrpc();

        ConfigureCustomServices(services);

        services.AddSingleton<IGraphServiceClientFactory, GraphServiceClientFactory>();

        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services
            .AddMemoryCache()
            .AddSecurity()
            .AddContext()
            .AddRandomHelper()
            .AddTimeHelper();
    }

    protected abstract void ConfigureCustomServices(IServiceCollection services);
}
