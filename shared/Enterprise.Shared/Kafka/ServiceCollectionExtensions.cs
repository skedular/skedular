using Api.Shared.Events;
using Confluent.Kafka;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Hosting;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Kafka.Logger;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Kafka.Serialization;
using Enterprise.Shared.Kafka.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Kafka;

public static class ServiceCollectionExtensions
{
    private const int DelayBaseSeconds = 10;

    public static IServiceCollection AddKafka(
        this IServiceCollection services,
        bool useTelemetry = true)
    {
        services
            .AddSingleton(typeof(IKafkaPublisher<,>), typeof(KafkaPublisher<,>))
            .AddSingleton(typeof(IAsyncSerializer<>), typeof(CustomProtobufSerializer<>))
            .AddSingleton(typeof(IDeserializer<>), typeof(CustomProtobufDeserializer<>))
            .AddSingleton<IConsumerFactory, ConsumerFactory>()
            .AddTransient<IProducerFactory, ProducerFactory>()
            .AddTransient(typeof(IProducer<,>), typeof(ProducerInstanceFromFactoryAdapter<,>))
            .AddSingleton<IKafkaClientNaming, KafkaClientNaming>()
            .AddSingleton(typeof(IPushToTopic<>), typeof(PushToTopic<>))
            .AddSingleton<IKafkaActivityTracer, KafkaActivityTracer>()
            .AddSingleton(new KafkaTelemetryConfiguration { Enabled = useTelemetry });

        services.TryAddSingleton<IKafkaLogger, KafkaLogger>();
        services.TryAddSingleton<IHostApplicationLifetimeWrapper, HostApplicationLifetimeWrapper>();

        if (useTelemetry)
        {
            services.Decorate<IProducerFactory, ProducerFactoryTelemetryDecorator>();
        }

        services.Decorate<IProducerFactory, ProducerFactoryClientIdDecorator>();

        return services;
    }

    public static IServiceCollection AddKafkaEventConsumers<TSubscriber, TKey, TEvent>(
        this IServiceCollection services,
        KafkaConfiguration kafkaConfiguration)
        where TSubscriber : class, IEventSubscriber<TKey, TEvent>
        where TKey : IEvent, new()
        where TEvent : IEvent, new()
    {
        var topicSetting = new TopicSetting<TEvent>(
            kafkaConfiguration.RetryTopicCount,
            DelayBaseSeconds,
            kafkaConfiguration.IncomingTopicPrefix);

        return services.AddKafkaEventConsumers<TSubscriber, TKey, TEvent>(
            kafkaConfiguration,
            [topicSetting.Topic]);
    }

    public static IServiceCollection AddKafkaEventConsumers<TSubscriber, TKey, TEvent>(
        this IServiceCollection services,
        KafkaConfiguration kafkaConfiguration,
        IReadOnlyCollection<string> topicNames)
        where TSubscriber : class, IEventSubscriber<TKey, TEvent>
        where TKey : IEvent, new()
        where TEvent : IEvent, new()
    {
        services.AddScoped<IEventSubscriber<TKey, TEvent>, TSubscriber>();
        services.TryAddSingleton<IKafkaMessageHandler<TKey, TEvent>, KafkaMessageHandler<TKey, TEvent>>();

        services.AddSingleton<IHostedService, KafkaConsumeService<TKey, TEvent>>(
            sp => new KafkaConsumeService<TKey, TEvent>(
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

        return services;
    }

    public static IServiceCollection AddKafkaReliableEventConsumers<TSubscriber, TKey, TEvent>(
        this IServiceCollection services,
        KafkaConfiguration kafkaConfiguration)
        where TSubscriber : class, IEventSubscriber<TKey, TEvent>
        where TKey : IEvent, new()
        where TEvent : IEvent, new() =>
        services.AddKafkaReliableEventConsumers<TSubscriber, TKey, TEvent>(
            kafkaConfiguration,
            kafkaConfiguration);

    public static IServiceCollection AddKafkaReliableEventConsumers<TSubscriber, TKey, TEvent>(
        this IServiceCollection services,
        KafkaConfiguration consumerKafkaConfiguration,
        KafkaConfiguration retryTopicKafkaConfiguration)
        where TSubscriber : class, IEventSubscriber<TKey, TEvent>
        where TKey : IEvent, new()
        where TEvent : IEvent, new()
    {
        services.AddScoped<IEventSubscriber<TKey, TEvent>, TSubscriber>();
        services.TryAddSingleton<IKafkaMessageHandler<TKey, TEvent>, KafkaMessageHandler<TKey, TEvent>>();

        var kafkaTopicInfo = KafkaTopicHelper.GetKafkaTopicInfo<TEvent>();
        var topicSetting = new TopicSetting<TEvent>(
            kafkaTopicInfo.RetryTopicCount,
            DelayBaseSeconds,
            consumerKafkaConfiguration.IncomingTopicPrefix);

        var retryTopicSetting = new TopicSetting<TEvent>(
            kafkaTopicInfo.RetryTopicCount,
            DelayBaseSeconds,
            retryTopicKafkaConfiguration.OutgoingTopicPrefix);

        services.AddSingleton<IHostedService, KafkaConsumeService<TKey, TEvent>>(
            sp => new KafkaConsumeService<TKey, TEvent>(
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
                .AddSingleton<IHostedService, KafkaConsumeService<TKey, TEvent>>(
                    sp =>
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
}
