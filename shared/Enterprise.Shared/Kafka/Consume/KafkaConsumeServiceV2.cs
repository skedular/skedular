using Api.Shared.Events;
using Confluent.Kafka;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Hosting;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Kafka.Telemetry;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Kafka.Consume;

public class KafkaConsumeServiceV2<TKey, TEvent> : BackgroundService
    where TKey : IEvent, new() where TEvent : IEvent, new()
{
    private readonly IConsumer<byte[], byte[]> _consumer;
    private readonly string _formattedTopicNames;
    private readonly string _groupId;
    private readonly IHostApplicationLifetimeWrapper _hostApplicationLifetimeWrapper;
    private readonly IKafkaActivityTracer _kafkaActivityTracer;
    private readonly KafkaConfiguration _kafkaConfiguration;
    private readonly IKafkaMessageHandler<TKey, TEvent> _kafkaMessageHandler;
    private readonly KafkaTelemetryConfiguration _kafkaTelemetryConfiguration;
    private readonly ILogger _logger;
    private readonly IProducer<byte[], byte[]> _producer;
    private readonly List<string> _topicNames;
    private readonly IReadOnlyCollection<string> _topicsToSubscribe;

    public KafkaConsumeServiceV2(
        ILogger<KafkaConsumeServiceV2<TKey, TEvent>> logger,
        ApplicationConfiguration applicationConfiguration,
        KafkaTelemetryConfiguration kafkaTelemetryConfiguration,
        IReadOnlyCollection<string> topicNames,
        KafkaConfiguration kafkaConfiguration,
        IConsumerFactory consumerFactory,
        IProducerFactory producerFactory,
        IHostApplicationLifetimeWrapper hostHostApplicationLifetimeWrapper,
        IKafkaMessageHandler<TKey, TEvent> kafkaMessageHandler,
        IKafkaActivityTracer kafkaActivityTracer)
    {
        _groupId = applicationConfiguration.GetSource();
        ArgumentException.ThrowIfNullOrWhiteSpace(_groupId);

        if (topicNames.Count <= 1)
        {
            throw new ArgumentException("At least main topic and dead letter topic are required.", nameof(topicNames));
        }

        _hostApplicationLifetimeWrapper = hostHostApplicationLifetimeWrapper;
        _kafkaMessageHandler = kafkaMessageHandler;
        _kafkaActivityTracer = kafkaActivityTracer;
        _topicNames = topicNames.ToList();
        _topicsToSubscribe = _topicNames.Take(topicNames.Count - 1).ToList();
        _formattedTopicNames = _topicsToSubscribe.Count == 1 ? _topicsToSubscribe.First() : $"\"{string.Join(",", _topicsToSubscribe)}\"";
        _logger = logger;
        _kafkaTelemetryConfiguration = kafkaTelemetryConfiguration;
        _kafkaConfiguration = kafkaConfiguration;

        _producer = producerFactory.Build<byte[], byte[]>(_kafkaConfiguration);
        _consumer = consumerFactory.Build<byte[], byte[]>(kafkaConfiguration, builder =>
        {
            builder.SetPartitionsRevokedHandler(PartitionRevokedHandler);
            builder.SetPartitionsAssignedHandler(PartitionAssignedHandler);
            builder.SetPartitionsLostHandler(PartitionsLostHandler);
        });
    }

    private void PartitionsLostHandler(IConsumer<byte[], byte[]> consumer, List<TopicPartitionOffset> partitionsLost) =>
        _logger.LogInformation("[LOST] Partitions lost on {topic}: {partitions}", _formattedTopicNames, partitionsLost);

    private void PartitionAssignedHandler(IConsumer<byte[], byte[]> consumer, List<TopicPartition> topicPartitions) =>
        _logger.LogInformation("[ASSIGNED] Partitions assigned on {topic}: {partitions}", _formattedTopicNames, topicPartitions);

    private void PartitionRevokedHandler(IConsumer<byte[], byte[]> consumer, List<TopicPartitionOffset> topicPartitionOffsets) =>
        _logger.LogInformation("[REVOKED] Partitions revoked on {topic}: {partitions}", _formattedTopicNames, topicPartitionOffsets);

    /// <summary>
    ///     This method is called when the <see cref="IHostedService" /> starts. The implementation should return a task that
    ///     represents
    ///     the lifetime of the long-running operation(s) being performed.
    /// </summary>
    /// <param name="cancellationToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)" /> is called.</param>
    /// <returns>A <see cref="Task" /> that represents the long-running operations.</returns>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var typeName = GetType().ToFullName();

        try
        {
            await Task.Run(async () =>
            {
                _logger.LogInformation("[{Type}] Subscribing to {TopicName}", typeName, _formattedTopicNames);

                try
                {
                    _consumer.Subscribe(_topicsToSubscribe);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "[{Type}] Failed to subscribe to {TopicName}", typeName, _formattedTopicNames);

                    Environment.ExitCode = KafkaExitCodes.FailedToSubscribe;
                    _hostApplicationLifetimeWrapper.StopApplication();

                    return;
                }

                try
                {
                    await WatchTopicAndProcessEventsAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    switch (ex)
                    {
                        case OperationCanceledException:
                            _logger.LogInformation("{KafkaService}: Stopped due to cancellation", typeName);

                            break;

                        default:
                            _logger.LogCritical(ex, "{KafkaService}: Exception occurred while running ExecuteAsync method", typeName);

                            Environment.ExitCode = KafkaExitCodes.UncaughtException;
                            _hostApplicationLifetimeWrapper.StopApplication();

                            break;
                    }
                }
                finally
                {
                    try
                    {
                        _consumer.Close();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, "Failed to close consumer");

                        Environment.ExitCode = KafkaExitCodes.FailedToCloseConsumer;
                        _hostApplicationLifetimeWrapper.StopApplication();
                    }
                }
            }, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation(
                "{KafkaService}: Stopped due to cancellation",
                typeName);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "{KafkaService}: Exception occurred while running ExecuteAsync method", typeName);
            _hostApplicationLifetimeWrapper.StopApplication();

            throw;
        }
    }

    private async Task WatchTopicAndProcessEventsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                var activity = _kafkaTelemetryConfiguration.Enabled ? _kafkaActivityTracer.CreateConsumeActivity(consumeResult) : null;

                try
                {
                    await ProcessEventAsync(consumeResult, cancellationToken);
                }
                catch (Exception ex)
                {
                    activity?.AddException(ex);
                    throw;
                }
                finally
                {
                    activity?.Dispose();
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Failed to call Consume method - Topic - {Topic}", ex.ConsumerRecord?.Topic);

                if (ex.Error?.IsBrokerError == true)
                {
                    // if the error is a broker one, then restart the service
                    // this helps by 
                    // - possibly resetting a connection by restarting
                    // - incrementing the restart counter; letting the observability platform know something is wrong  
                    var error = ex.Error;

                    _logger.LogCritical(
                        ex,
                        "Broker Error! Error: {Code}: {Reason} || Subscription: {Subscription} || Server(s): {Servers} ",
                        error.Code, error.Reason, _consumer.Subscription,
                        _kafkaConfiguration.BootstrapServers);
                    Environment.ExitCode = (int)error.Code;
                    _hostApplicationLifetimeWrapper.StopApplication();

                    break;
                }
            }
        }
    }

    private bool MatchesConsumerGroup(ConsumeResult<byte[], byte[]> consumeResult)
    {
        // This is the main topic
        if (consumeResult.Topic == _topicNames.First())
        {
            _logger.LogTrace("{Topic} is the main topic", consumeResult.Topic);

            return true;
        }

        var consumerGroup = consumeResult.Message.GetConsumerGroup();

        _logger.LogTrace("{Topic} Matching consumer group: {GroupId}", consumeResult.Topic, _groupId);

        if (consumerGroup is null)
        {
            _logger.LogTrace("{Topic} No consumer group header set", consumeResult.Topic);

            return true;
        }

        // if there is no consumer group header, or if there is one it should match the current group
        var match = consumerGroup == _groupId;

        _logger.LogTrace("{GroupId} [Match: {Match}] {HeaderValue}", _groupId, match, consumerGroup);

        if (!match)
        {
            // Immediately commit offset for the messages from retry topics where the consumer group does not match
            // the consumer group of the current running consumer. This way we tell kafka that we are not interested
            // in this message, and we can move on.
            AcknowledgeMessage(consumeResult);
        }

        return match;
    }

    private async Task ProcessEventAsync(ConsumeResult<byte[], byte[]> consumeResult, CancellationToken cancellationToken)
    {
        try
        {
            if (MatchesConsumerGroup(consumeResult))
            {
                await _kafkaMessageHandler.HandleMessageAsync(consumeResult, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            await PushExceptionToRetryAsync(consumeResult, ex, cancellationToken);
        }

        AcknowledgeMessage(consumeResult);
    }

    private void AcknowledgeMessage(ConsumeResult<byte[], byte[]> consumeResult)
    {
        try
        {
            _consumer.StoreOffset(consumeResult);
        }
        catch (KafkaException ex)
        {
            _logger.LogWarning(ex, "Exception during storing OFFSET {Result}", consumeResult.TopicPartitionOffset);

            switch (ex.Error)
            {
                case
                {
                    Code: ErrorCode.Local_State
                }:
                    // Likely an issue with trying to store an offset to a partition that this consumer isn't managing anymore
                    // Log & Continue.
                    break;
                default:
                    throw;
            }
        }
    }

    private async Task PushExceptionToRetryAsync(ConsumeResult<byte[], byte[]> consumeResult, Exception ex, CancellationToken cancellationToken)
    {
        var retryTopicName = _topicNames[_topicNames.IndexOf(consumeResult.Topic) + 1];
        var topicPartition = consumeResult.TopicPartition;

        _logger.LogError(
            ex,
            "Failed to process the message from {Topic} - {Partition}. Moving the message to retry topic: {RetryTopicName}",
            topicPartition.Topic,
            topicPartition.Partition.Value,
            retryTopicName);

        consumeResult.Message.SetTimestamp();
        consumeResult.Message.SetConsumerGroup(_groupId);
        consumeResult.Message.SetLastException(ex);

        // This would either move the message into retry topic or dead letter queue.
        await _producer.ProduceAsync(retryTopicName, consumeResult.Message, cancellationToken);
    }
}
