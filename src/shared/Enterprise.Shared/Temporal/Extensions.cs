using System.Reflection;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Outbox.Temporal;
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
    extension(TemporalClientConnectOptions? temporalClientConnectOptions)
    {
        private TemporalClientConnectOptions ConfigureClient(TemporalConfiguration temporalConfiguration)
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
    }

    extension(TemporalWorkerServiceOptions options)
    {
        private void ConfigureService(TemporalConfiguration temporalConfiguration)
        {
            options.ClientOptions = options.ClientOptions.ConfigureClient(temporalConfiguration);
            options.ConfigureWorker(temporalConfiguration);
        }
    }

    extension(TemporalWorkerOptions options)
    {
        private void ConfigureWorker(TemporalConfiguration temporalConfiguration)
        {
            // rate limits
            options.MaxTaskQueueActivitiesPerSecond = temporalConfiguration.Worker.RateLimits.MaxTaskQueueActivitiesPerSecond;
            options.MaxActivitiesPerSecond = temporalConfiguration.Worker.RateLimits.MaxWorkerActivitiesPerSecond;

            // executors
            options.MaxConcurrentActivities = temporalConfiguration.Worker.Capacity.MaxConcurrentActivityExecutors;
            options.MaxConcurrentLocalActivities = temporalConfiguration.Worker.Capacity.MaxConcurrentLocalActivityExecutors;
            options.MaxConcurrentWorkflowTasks = temporalConfiguration.Worker.Capacity.MaxConcurrentWorkflowTaskExecutors;

            // pollers
            options.MaxConcurrentWorkflowTaskPolls = temporalConfiguration.Worker.Capacity.MaxConcurrentWorkflowTaskPollers;
            options.MaxConcurrentActivityTaskPolls = temporalConfiguration.Worker.Capacity.MaxConcurrentActivityTaskPollers;

            options.MaxCachedWorkflows = temporalConfiguration.Worker.Cache.MaxInstances;
        }
    }

    extension(IServiceCollection services)
    {
        public ITemporalWorkerServiceOptionsBuilder AddTemporalWorker(
            IConfiguration configuration,
            string deploymentName,
            string buildId,
            string connectionName)
        {
            var temporalConfiguration = configuration.GetSection(TemporalConfiguration.Key).Get<TemporalConfiguration>();
            ArgumentNullException.ThrowIfNull(temporalConfiguration);

            var target = configuration.GetConnectionString(connectionName);
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

        public IServiceCollection AddTemporalClient(IConfiguration configuration, string connectionName)
        {
            var temporalConfiguration = configuration.GetSection(TemporalConfiguration.Key).Get<TemporalConfiguration>();
            ArgumentNullException.ThrowIfNull(temporalConfiguration);

            var target = configuration.GetConnectionString(connectionName);
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
    }

    extension(Type type)
    {
        /// <summary>Returns the fully qualified CLR type name used as the Temporal workflow type identifier.</summary>
        public string ToWorkflowType() => type.FullName!;
    }

    extension(MethodInfo methodInfo)
    {
        /// <summary>Returns the Temporal signal type string in the form "DeclaringType.MethodName".</summary>
        public string ToWorkflowSignalType() =>
            $"{methodInfo.DeclaringType?.FullName ?? throw new InvalidOperationException("Workflow signal methods must have a declaring type.")}.{methodInfo.Name}";
    }
}
