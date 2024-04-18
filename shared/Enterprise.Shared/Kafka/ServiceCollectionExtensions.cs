using Api.Shared.Events;
using Confluent.Kafka;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Kafka.Logger;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Kafka.Serialization;
using Enterprise.Shared.Kafka.Telemetry;
using Enterprise.Shared.Telemetry;
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
        services.TryAddSingleton<IKafkaLogger, KafkaLogger>();

        services
            .AddSingleton(typeof(IKafkaPublisher<,>), typeof(KafkaPublisher<,>));

        services
            .AddSingleton(typeof(IAsyncSerializer<>), typeof(CustomProtobufSerializer<>))
            .AddSingleton(typeof(IAsyncDeserializer<>), typeof(CustomProtobufDeserializer<>))
            .AddSingleton<IConsumerFactory, ConsumerFactory>()
            .AddTransient<IProducerFactory, ProducerFactory>()
            .AddTransient(typeof(IProducer<,>), typeof(ProducerInstanceFromFactoryAdapter<,>))
            .AddSingleton<IKafkaClientNaming, KafkaClientNaming>()
            .AddSingleton(typeof(IPushToTopic<>), typeof(PushToTopic<>))
            .AddSingleton<IKafkaActivityTracer, KafkaActivityTracer>();

        if (useTelemetry)
        {
            services
                .Decorate<IConsumerFactory, ConsumerFactoryTelemetryDecorator>()
                .Decorate<IProducerFactory, ProducerFactoryTelemetryDecorator>();
        }

        services.Decorate<IProducerFactory, ProducerFactoryClientIdDecorator>();

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
                sp.GetRequiredService<ApplicationConfiguration>(),
                sp.GetRequiredService<IServiceProvider>(),
                sp.GetRequiredService<IActivityAccessor>(),
                sp.GetRequiredService<
                    ILogger<KafkaConsumeService<TKey, TEvent>>>(),
                sp.GetRequiredService<IAsyncDeserializer<TKey>>(),
                sp.GetRequiredService<IAsyncDeserializer<TEvent>>(),
                consumerKafkaConfiguration,
                retryTopicKafkaConfiguration,
                topicSetting.Topic,
                retryTopicSetting.RetryTopics.Any()
                    ? retryTopicSetting.RetryTopics[0].Topic
                    : retryTopicSetting.DeadLetterTopic,
                null,
                sp.GetRequiredService<IHostApplicationLifetime>(),
                sp.GetRequiredService<IConsumerFactory>(),
                sp.GetRequiredService<IProducerFactory>(),
                sp.GetRequiredService<TimeProvider>()));

        foreach (var retryTopic in retryTopicSetting.RetryTopics)
        {
            services
                .AddSingleton<IHostedService, KafkaConsumeService<TKey, TEvent>>(
                    sp =>
                    {
                        var indexOf = retryTopicSetting.RetryTopics.IndexOf(retryTopic);

                        return new KafkaConsumeService<TKey, TEvent>(
                            sp.GetRequiredService<ApplicationConfiguration>(),
                            sp.GetRequiredService<IServiceProvider>(),
                            sp.GetRequiredService<IActivityAccessor>(),
                            sp.GetRequiredService<
                                ILogger<KafkaConsumeService<TKey, TEvent>>>(),
                            sp.GetRequiredService<IAsyncDeserializer<TKey>>(),
                            sp.GetRequiredService<IAsyncDeserializer<TEvent>>(),
                            retryTopicKafkaConfiguration,
                            retryTopicKafkaConfiguration,
                            retryTopic.Topic,
                            indexOf == retryTopicSetting.RetryTopics.Count - 1
                                ? retryTopicSetting.DeadLetterTopic
                                : retryTopicSetting.RetryTopics[indexOf + 1].Topic,
                            retryTopic.RetryDelaySeconds,
                            sp.GetRequiredService<IHostApplicationLifetime>(),
                            sp.GetRequiredService<IConsumerFactory>(),
                            sp.GetRequiredService<IProducerFactory>(),
                            sp.GetRequiredService<TimeProvider>());
                    });
        }

        return services;
    }
}
