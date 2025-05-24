using Enterprise.Shared.Temporal.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Temporalio.Client;
using Temporalio.Extensions.Hosting;
using Temporalio.Worker;

namespace Enterprise.Shared.Temporal;

public static class Extensions
{
    public static ITemporalWorkerServiceOptionsBuilder AddTemporalWorker(this IServiceCollection services, IConfiguration configuration)
    {
        var temporalConfiguration = configuration.GetSection(TemporalConfiguration.Key).Get<TemporalConfiguration>();
        ArgumentNullException.ThrowIfNull(temporalConfiguration);

        var target = configuration.GetConnectionString("temporal");
        if (!string.IsNullOrWhiteSpace(target))
        {
            temporalConfiguration.Connection.Target = target;
        }

        return services
            .AddHostedTemporalWorker(temporalConfiguration.Worker.TaskQueue)
            .ConfigureOptions(o => { o.ConfigureService(temporalConfiguration); });
    }

    public static IServiceCollection AddTemporalClient(this IServiceCollection services, IConfiguration configuration)
    {
        var temporalConfiguration = configuration.GetSection(TemporalConfiguration.Key).Get<TemporalConfiguration>();
        ArgumentNullException.ThrowIfNull(temporalConfiguration);

        var target = configuration.GetConnectionString("temporal");
        if (!string.IsNullOrWhiteSpace(target))
        {
            temporalConfiguration.Connection.Target = target;
        }

        return services.AddTemporalClient(o => { o.ConfigureClient(temporalConfiguration); }).Configure<ITemporalClient>(c =>
        {
            // connect when the container is built
            c.Connection.ConnectAsync(); // TODO: 20250524 - Morteza: Check this line later
        });
    }

    private static TemporalClientConnectOptions ConfigureClient(
        this TemporalClientConnectOptions? temporalClientConnectOptions,
        TemporalConfiguration temporalConfiguration)
    {
        ArgumentNullException.ThrowIfNull(temporalConfiguration.Connection);

        temporalClientConnectOptions ??= new TemporalClientConnectOptions();
        temporalClientConnectOptions.Namespace = temporalConfiguration.Connection.Namespace;
        temporalClientConnectOptions.TargetHost = temporalConfiguration.Connection.Target;

        if (temporalConfiguration.Connection.Mtls != null)
        {
            temporalClientConnectOptions.Tls = new TlsOptions
            {
                ClientCert = File.ReadAllBytes(temporalConfiguration.Connection.Mtls.CertChainFile),
                ClientPrivateKey = File.ReadAllBytes(temporalConfiguration.Connection.Mtls.KeyFile)
            };
        }

        return temporalClientConnectOptions;
    }

    private static void ConfigureWorker(this TemporalWorkerOptions temporalWorkerOptions, TemporalConfiguration temporalConfiguration)
    {
        temporalWorkerOptions.UseWorkerVersioning = false;

        // rate limits
        temporalWorkerOptions.MaxTaskQueueActivitiesPerSecond = temporalConfiguration.Worker.RateLimits.MaxTaskQueueActivitiesPerSecond;
        temporalWorkerOptions.MaxActivitiesPerSecond = temporalConfiguration.Worker.RateLimits.MaxWorkerActivitiesPerSecond;

        // executors
        temporalWorkerOptions.MaxConcurrentActivities = temporalConfiguration.Worker.Capacity.MaxConcurrentActivityExecutors;
        temporalWorkerOptions.MaxConcurrentLocalActivities = temporalConfiguration.Worker.Capacity.MaxConcurrentLocalActivityExecutors;
        temporalWorkerOptions.MaxConcurrentWorkflowTasks = temporalConfiguration.Worker.Capacity.MaxConcurrentWorkflowTaskExecutors;

        // pollers
        temporalWorkerOptions.MaxConcurrentActivityTaskPolls = temporalConfiguration.Worker.Capacity.MaxConcurrentWorkflowTaskPollers;
        temporalWorkerOptions.MaxConcurrentActivityTaskPolls = temporalConfiguration.Worker.Capacity.MaxConcurrentActivityTaskPollers;

        temporalWorkerOptions.MaxCachedWorkflows = temporalConfiguration.Worker.Cache.MaxInstances;
    }

    private static void ConfigureService(this TemporalWorkerServiceOptions opts, TemporalConfiguration temporalConfiguration)
    {
        opts.ClientOptions = opts.ClientOptions.ConfigureClient(temporalConfiguration);
        opts.ConfigureWorker(temporalConfiguration);
    }
}
