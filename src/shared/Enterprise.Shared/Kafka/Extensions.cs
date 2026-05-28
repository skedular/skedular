using Api.Shared.Events;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Hosting;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Kafka.Logger;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Kafka.Serialization;
using Enterprise.Shared.Kafka.Telemetry;
using Enterprise.Shared.Outbox.Kafka;
using Enterprise.Shared.Telemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TelemetryKeys = Enterprise.Shared.Kafka.Telemetry.TelemetryKeys;

namespace Enterprise.Shared.Kafka;

public static class Extensions
{
    private const int DelayBaseSeconds = 10;

    extension(IServiceCollection services)
    {
        public KafkaConfiguration AddKafka(IConfiguration configuration, string connectionName, bool useTelemetry = true) =>
            services.AddKafkaWithConnectionString(configuration, configuration.GetConnectionString(connectionName), useTelemetry);

        public KafkaConfiguration AddKafkaWithConnectionString(IConfiguration configuration, string? connectionString, bool useTelemetry = true)
        {
            var kafkaConfiguration = configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
            ArgumentNullException.ThrowIfNull(kafkaConfiguration);

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                kafkaConfiguration.BootstrapServers = connectionString;
            }

            services.AddSingleton(kafkaConfiguration);
            if (kafkaConfiguration.SchemaRegistry is not null)
            {
                var schemaRegistryConfig = new SchemaRegistryConfig { Url = kafkaConfiguration.SchemaRegistry.Url };

                if (!string.IsNullOrWhiteSpace(kafkaConfiguration.SchemaRegistry.ApiKey) &&
                    !string.IsNullOrWhiteSpace(kafkaConfiguration.SchemaRegistry.SecretKey))
                {
                    schemaRegistryConfig.BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo;
                    schemaRegistryConfig.BasicAuthUserInfo =
                        $"{kafkaConfiguration.SchemaRegistry.ApiKey}:{kafkaConfiguration.SchemaRegistry.SecretKey}";
                }

                services.AddSingleton(schemaRegistryConfig);
                services.AddSingleton<ISchemaRegistryClient>(sp =>
                    new CachedSchemaRegistryClient(sp.GetRequiredService<SchemaRegistryConfig>()));
            }

            services
                .AddKafkaOutboxService()
                .AddSingleton<IKafkaHelper, KafkaHelper>()
                .AddSingleton(typeof(IKafkaPublisher<,>), typeof(KafkaPublisher<,>))
                .AddSingleton(typeof(ISerializer<>), typeof(CustomProtobufSerializer<>))
                .AddSingleton(typeof(IDeserializer<>), typeof(CustomProtobufDeserializer<>))
                .AddSingleton<IConsumerFactory, ConsumerFactory>()
                .AddTransient<IProducerFactory, ProducerFactory>()
                .AddTransient(typeof(IProducer<,>), typeof(ProducerInstanceFromFactoryAdapter<,>))
                .AddSingleton<IKafkaClientNaming, KafkaClientNaming>()
                .AddSingleton(typeof(IPushToTopic<>), typeof(PushToTopic<>))
                .AddSingleton<IKafkaActivityTracer, KafkaActivityTracer>()
                .AddSingleton<IKafkaActivityStarter, KafkaActivityStarter>()
                .AddSingleton<IPropagatorFunctionProvider<Headers>, HeaderPropagatorFunctions>()
                .AddSingleton(new KafkaTelemetryConfiguration { Enabled = useTelemetry });

            services.TryAddSingleton<IKafkaLogger, KafkaLogger>();
            services.TryAddSingleton<IHostApplicationLifetimeWrapper, HostApplicationLifetimeWrapper>();

            services
                .AddKafkaActivitySource(TelemetryKeys.IncomingActivitySourceName)
                .AddKafkaActivitySource(TelemetryKeys.ConsumerActivitySourceName)
                .AddKafkaActivitySource(TelemetryKeys.ProducerActivitySourceName);

            if (useTelemetry)
            {
                services.AddKafkaTelemetry();
            }

            return kafkaConfiguration;
        }

        public IServiceCollection AddKafkaEventConsumers<TSubscriber, TKey, TEvent>(KafkaConfiguration kafkaConfiguration)
            where TSubscriber : class, IEventSubscriber<TKey, TEvent>
            where TKey : IEvent, new()
            where TEvent : IEvent, new()
        {
            var topicSetting = new TopicSetting<TEvent>(kafkaConfiguration.RetryTopicCount, DelayBaseSeconds, kafkaConfiguration.IncomingTopicPrefix);

            return services.AddKafkaEventConsumers<TSubscriber, TKey, TEvent>(kafkaConfiguration, [topicSetting.Topic]);
        }

        public IServiceCollection AddKafkaEventConsumers<TSubscriber, TKey, TEvent>(KafkaConfiguration kafkaConfiguration,
            IReadOnlyCollection<string> topicNames)
            where TSubscriber : class, IEventSubscriber<TKey, TEvent>
            where TKey : IEvent, new()
            where TEvent : IEvent, new()
        {
            services.AddScoped<IEventSubscriber<TKey, TEvent>, TSubscriber>();
            services.TryAddSingleton<IKafkaMessageHandler<TKey, TEvent>, KafkaMessageHandler<TKey, TEvent>>();

            return services
                .AddSingleton<IHostedService, KafkaConsumeService<TKey, TEvent>>(sp => new KafkaConsumeService<TKey, TEvent>(
                    sp.GetRequiredService<ILogger<KafkaConsumeService<TKey, TEvent>>>(),
                    sp.GetRequiredService<ApplicationConfiguration>(),
                    sp.GetRequiredService<KafkaTelemetryConfiguration>(),
                    topicNames,
                    kafkaConfiguration,
                    sp.GetRequiredService<IConsumerFactory>(),
                    null,
                    null,
                    null,
                    null,
                    sp.GetRequiredService<IHostApplicationLifetimeWrapper>(),
                    sp.GetRequiredService<IKafkaMessageHandler<TKey, TEvent>>(),
                    sp.GetRequiredService<IKafkaActivityTracer>(),
                    sp.GetRequiredService<TimeProvider>()));
        }

        public IServiceCollection AddKafkaReliableEventConsumers<TSubscriber, TKey, TEvent>(KafkaConfiguration kafkaConfiguration)
            where TSubscriber : class, IEventSubscriber<TKey, TEvent>
            where TKey : IEvent, new()
            where TEvent : IEvent, new() =>
            services.AddKafkaReliableEventConsumers<TSubscriber, TKey, TEvent>(kafkaConfiguration, kafkaConfiguration);

        public IServiceCollection AddKafkaReliableEventConsumers<TSubscriber, TKey, TEvent>(KafkaConfiguration consumerKafkaConfiguration,
            KafkaConfiguration retryTopicKafkaConfiguration)
            where TSubscriber : class, IEventSubscriber<TKey, TEvent>
            where TKey : IEvent, new()
            where TEvent : IEvent, new()
        {
            services.AddScoped<IEventSubscriber<TKey, TEvent>, TSubscriber>();
            services.TryAddSingleton<IKafkaMessageHandler<TKey, TEvent>, KafkaMessageHandler<TKey, TEvent>>();

            var kafkaTopicInfo = KafkaTopicHelper.GetKafkaTopicInfo<TEvent>();
            var topicSetting =
                new TopicSetting<TEvent>(kafkaTopicInfo.RetryTopicCount, DelayBaseSeconds, consumerKafkaConfiguration.IncomingTopicPrefix);
            var retryTopicSetting = new TopicSetting<TEvent>(
                kafkaTopicInfo.RetryTopicCount,
                DelayBaseSeconds,
                retryTopicKafkaConfiguration.OutgoingTopicPrefix);

            services.AddSingleton<IHostedService, KafkaConsumeService<TKey, TEvent>>(sp => new KafkaConsumeService<TKey, TEvent>(
                sp.GetRequiredService<ILogger<KafkaConsumeService<TKey, TEvent>>>(),
                sp.GetRequiredService<ApplicationConfiguration>(),
                sp.GetRequiredService<KafkaTelemetryConfiguration>(),
                [topicSetting.Topic],
                consumerKafkaConfiguration,
                sp.GetRequiredService<IConsumerFactory>(),
                retryTopicSetting.RetryTopics.Any()
                    ? retryTopicSetting.RetryTopics[0].Topic
                    : retryTopicSetting.DeadLetterTopic,
                null,
                retryTopicKafkaConfiguration,
                sp.GetRequiredService<IProducerFactory>(),
                sp.GetRequiredService<IHostApplicationLifetimeWrapper>(),
                sp.GetRequiredService<IKafkaMessageHandler<TKey, TEvent>>(),
                sp.GetRequiredService<IKafkaActivityTracer>(),
                sp.GetRequiredService<TimeProvider>()));

            foreach (var retryTopic in retryTopicSetting.RetryTopics)
            {
                services
                    .AddSingleton<IHostedService, KafkaConsumeService<TKey, TEvent>>(sp =>
                    {
                        var indexOf = retryTopicSetting.RetryTopics.IndexOf(retryTopic);

                        return new KafkaConsumeService<TKey, TEvent>(
                            sp.GetRequiredService<ILogger<KafkaConsumeService<TKey, TEvent>>>(),
                            sp.GetRequiredService<ApplicationConfiguration>(),
                            sp.GetRequiredService<KafkaTelemetryConfiguration>(),
                            [retryTopic.Topic],
                            retryTopicKafkaConfiguration,
                            sp.GetRequiredService<IConsumerFactory>(),
                            indexOf == retryTopicSetting.RetryTopics.Count - 1
                                ? retryTopicSetting.DeadLetterTopic
                                : retryTopicSetting.RetryTopics[indexOf + 1].Topic,
                            retryTopic.RetryDelaySeconds,
                            retryTopicKafkaConfiguration,
                            sp.GetRequiredService<IProducerFactory>(),
                            sp.GetRequiredService<IHostApplicationLifetimeWrapper>(),
                            sp.GetRequiredService<IKafkaMessageHandler<TKey, TEvent>>(),
                            sp.GetRequiredService<IKafkaActivityTracer>(),
                            sp.GetRequiredService<TimeProvider>());
                    });
            }

            return services;
        }

        public IServiceCollection AddKafkaTelemetry()
        {
            services.AddOpenTelemetry().WithTracing(builder => builder
                .AddSource(TelemetryKeys.IncomingActivitySourceName)
                .AddSource(TelemetryKeys.ConsumerActivitySourceName)
                .AddSource(TelemetryKeys.ProducerActivitySourceName));
            return services;
        }

        internal IServiceCollection AddKafkaActivitySource(string activitySourceName)
        {
            if (services.Any(item =>
                    item.ServiceType == typeof(KafkaActivitySourceRegistration) &&
                    item.ImplementationInstance is KafkaActivitySourceRegistration registration &&
                    string.Equals(registration.Name, activitySourceName, StringComparison.Ordinal)))
            {
                return services;
            }

            services.AddSingleton<IActivitySource>(_ => new ActivitySourceFacade(activitySourceName));
            return services.AddSingleton(new KafkaActivitySourceRegistration(activitySourceName));
        }
    }

    private sealed class KafkaActivitySourceRegistration(string name)
    {
        public string Name { get; } = name;
    }
}
