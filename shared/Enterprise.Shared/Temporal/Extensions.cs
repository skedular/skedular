using Enterprise.Shared.Configurations;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Temporal.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Temporalio.Client;
using Temporalio.Common;
using Temporalio.Extensions.Hosting;
using Temporalio.Worker;

namespace Enterprise.Shared.Temporal;

public static class Extensions
{
    public static ITemporalWorkerServiceOptionsBuilder AddTemporalWorker(
        this IServiceCollection services,
        IConfiguration configuration,
        string deploymentName,
        string buildId)
    {
        var temporalConfiguration = configuration.GetSection(TemporalConfiguration.Key).Get<TemporalConfiguration>();
        ArgumentNullException.ThrowIfNull(temporalConfiguration);

        var target = configuration.GetConnectionString("temporal");
        if (!string.IsNullOrWhiteSpace(target))
        {
            temporalConfiguration.Connection.Target = target;
        }

        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        ArgumentNullException.ThrowIfNull(applicationConfiguration);
        if (!string.IsNullOrWhiteSpace(applicationConfiguration.Environment))
        {
            temporalConfiguration.Worker.TaskQueue = $"{applicationConfiguration.Environment}.{temporalConfiguration.Worker.TaskQueue}";
            deploymentName = $"{applicationConfiguration.Environment}.{deploymentName}";
        }

        var workerDeploymentOptions =
            new WorkerDeploymentOptions(new WorkerDeploymentVersion(deploymentName.Replace(".", "-").Replace(":", "-"), buildId), false)
            {
                DefaultVersioningBehavior = VersioningBehavior.Unspecified
            };

        return services
            .AddTemporalOutboxService()
            .AddSingleton(temporalConfiguration)
            .AddSingleton<ITemporalHelperService, TemporalHelperService>()
            .AddTemporalClient(temporalClientConnectOptions => temporalClientConnectOptions.ConfigureClient(temporalConfiguration))
            .Configure<ITemporalClient>(_ => { })
            .AddHostedTemporalWorker(temporalConfiguration.Worker.TaskQueue, workerDeploymentOptions)
            .ConfigureOptions(temporalWorkerServiceOptions => temporalWorkerServiceOptions.ConfigureService(temporalConfiguration));
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

        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        ArgumentNullException.ThrowIfNull(applicationConfiguration);
        if (!string.IsNullOrWhiteSpace(applicationConfiguration.Environment))
        {
            temporalConfiguration.Worker.TaskQueue = $"{applicationConfiguration.Environment}.{temporalConfiguration.Worker.TaskQueue}";
        }

        return services
            .AddTemporalOutboxService()
            .AddSingleton(temporalConfiguration)
            .AddSingleton<ITemporalHelperService, TemporalHelperService>()
            .AddTemporalClient(temporalClientConnectOptions => temporalClientConnectOptions.ConfigureClient(temporalConfiguration))
            .Configure<ITemporalClient>(_ => { });
    }

    private static TemporalClientConnectOptions ConfigureClient(
        this TemporalClientConnectOptions? temporalClientConnectOptions,
        TemporalConfiguration temporalConfiguration)
    {
        ArgumentNullException.ThrowIfNull(temporalConfiguration.Connection);

        temporalClientConnectOptions ??= new TemporalClientConnectOptions();
        temporalClientConnectOptions.Namespace = temporalConfiguration.Connection.Namespace;
        temporalClientConnectOptions.TargetHost = temporalConfiguration.Connection.Target;

        if (temporalConfiguration.Connection.Mtls is not null)
        {
            temporalClientConnectOptions.Tls = new TlsOptions
            {
                ClientCert = File.ReadAllBytes(temporalConfiguration.Connection.Mtls.CertChainFile),
                ClientPrivateKey = File.ReadAllBytes(temporalConfiguration.Connection.Mtls.KeyFile)
            };
        }

        return temporalClientConnectOptions;
    }

    private static void ConfigureService(this TemporalWorkerServiceOptions opts, TemporalConfiguration temporalConfiguration)
    {
        opts.ClientOptions = opts.ClientOptions.ConfigureClient(temporalConfiguration);
        opts.ConfigureWorker(temporalConfiguration);
    }

    private static void ConfigureWorker(this TemporalWorkerOptions temporalWorkerOptions, TemporalConfiguration temporalConfiguration)
    {
        // rate limits
        temporalWorkerOptions.MaxTaskQueueActivitiesPerSecond = temporalConfiguration.Worker.RateLimits.MaxTaskQueueActivitiesPerSecond;
        temporalWorkerOptions.MaxActivitiesPerSecond = temporalConfiguration.Worker.RateLimits.MaxWorkerActivitiesPerSecond;

        // executors
        temporalWorkerOptions.MaxConcurrentActivities = temporalConfiguration.Worker.Capacity.MaxConcurrentActivityExecutors;
        temporalWorkerOptions.MaxConcurrentLocalActivities = temporalConfiguration.Worker.Capacity.MaxConcurrentLocalActivityExecutors;
        temporalWorkerOptions.MaxConcurrentWorkflowTasks = temporalConfiguration.Worker.Capacity.MaxConcurrentWorkflowTaskExecutors;

        // pollers
        temporalWorkerOptions.MaxConcurrentWorkflowTaskPolls = temporalConfiguration.Worker.Capacity.MaxConcurrentWorkflowTaskPollers;
        temporalWorkerOptions.MaxConcurrentActivityTaskPolls = temporalConfiguration.Worker.Capacity.MaxConcurrentActivityTaskPollers;

        temporalWorkerOptions.MaxCachedWorkflows = temporalConfiguration.Worker.Cache.MaxInstances;
    }
}
