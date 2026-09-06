using Api.Shared.Events.Nats;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Hosting;
using Enterprise.Shared.Messaging.Nats.Configuration;
using Enterprise.Shared.Messaging.Nats.Consume;
using Enterprise.Shared.Messaging.Nats.Produce;
using Enterprise.Shared.Messaging.Nats.Provisioning;
using Enterprise.Shared.Messaging.Nats.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using NATS.Client.Hosting;

namespace Enterprise.Shared.Messaging.Nats;

public static class Extensions
{
    private const int DelayBaseSeconds = 10;

    extension(IServiceCollection services)
    {
        public NatsConfiguration AddNats(IConfiguration configuration, string? connectionName = null) =>
            services.AddNatsWithConnectionString(configuration, connectionName is null ? null : configuration.GetConnectionString(connectionName));

        public NatsConfiguration AddNatsWithConnectionString(IConfiguration configuration, string? connectionString)
        {
            var options = configuration.GetSection(NatsConfiguration.Key).Get<NatsConfiguration>()
                          ?? throw new InvalidOperationException($"Missing configuration section '{NatsConfiguration.Key}'.");

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.Urls = [connectionString];
            }

            options.Validate();

            services.AddNats(configureOpts: natsOpts => natsOpts with
            {
                Url = string.Join(',', options.Urls),
                Name = options.Name ?? natsOpts.Name,
            });

            services.AddSingleton(options);
            services.TryAddSingleton<IHostApplicationLifetimeWrapper, HostApplicationLifetimeWrapper>();
            services.TryAddSingleton<INatsPublisher, NatsPublisher>();
            services.TryAddSingleton<INatsPublisherForwarder>(sp => (INatsPublisherForwarder)sp.GetRequiredService<INatsPublisher>());
            services.TryAddSingleton(typeof(INatsPublisher<>), typeof(SerializedNatsPublisher<>));
            services.TryAddSingleton<INatsJetStreamProvisioner, NatsJetStreamProvisioner>();
            services.TryAddSingleton<INatsHelper, NatsHelper>();
            services.TryAddSingleton(typeof(Serialization.INatsSerializer<>), typeof(ProtobufNatsSerializer<>));

            return options;
        }

        public IServiceCollection AddNatsReliableEventConsumer<THandler, TKey, TEvent>()
            where THandler : class, INatsMessageHandler<TKey, TEvent>
            where TKey : new()
            where TEvent : INatsEvent, new() =>
            services.AddNatsEventConsumer<THandler, TKey, TEvent>(true);

        public IServiceCollection AddNatsNonReliableEventConsumer<THandler, TKey, TEvent>()
            where THandler : class, INatsMessageHandler<TKey, TEvent>
            where TKey : new()
            where TEvent : INatsEvent, new() =>
            services.AddNatsEventConsumer<THandler, TKey, TEvent>(false);

        private IServiceCollection AddNatsEventConsumer<THandler, TKey, TEvent>(bool reliable)
            where THandler : class, INatsMessageHandler<TKey, TEvent>
            where TKey : new()
            where TEvent : INatsEvent, new()
        {
            var metadata = NatsSubjectAttributeHelper.GetNatsSubjectInfo<TEvent>();
            var retrySubjects = NatsRetrySubjectSetting.Create(NatsSubjectAttributeHelper.GetSubject<TEvent>(), metadata.RetryCount);
            var deadLetterSubject = NatsSubjectAttributeHelper.GetDeadLetterSubject<TEvent>();
            var subjects = new List<string>
            {
                NatsSubjectAttributeHelper.GetSubject<TEvent>(),
            };

            if (reliable)
            {
                subjects.AddRange(retrySubjects.Select(item => item.Subject));
                subjects.Add(deadLetterSubject);
            }

            services.TryAddSingleton<THandler>();
            foreach (var (subject, index) in subjects.Select((subject, index) => (subject, index)))
            {
                services.AddSingleton<IHostedService>(provider =>
                {
                    var durableConsumerName = provider.GetRequiredService<ApplicationConfiguration>().GetSource();
                    ArgumentException.ThrowIfNullOrWhiteSpace(durableConsumerName);
                    var durableName = index == 0 ? durableConsumerName : $"{durableConsumerName}-{subject.Replace('.', '-')}";

                    return new NatsConsumeService<TKey, TEvent>(
                        provider.GetRequiredService<INatsConnection>(),
                        provider.GetRequiredService<INatsJetStreamProvisioner>(),
                        NatsSubjectAttributeHelper.GetStreamName<TEvent>(),
                        durableName,
                        subject,
                        provider.GetRequiredService<THandler>(),
                        reliable ? provider.GetRequiredService<INatsPublisherForwarder>() : null,
                        retrySubjects,
                        deadLetterSubject,
                        reliable,
                        provider.GetRequiredService<TimeProvider>(),
                        provider.GetRequiredService<IHostApplicationLifetimeWrapper>(),
                        provider.GetRequiredService<ILogger<NatsConsumeService<TKey, TEvent>>>());
                });
            }

            return services;
        }
    }
}
