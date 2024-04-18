using System.Reactive.Linq;
using System.Reactive.Subjects;
using Api.Shared.Events;
using Confluent.Kafka;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Kafka.Telemetry;
using Enterprise.Shared.Telemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;

namespace Enterprise.Shared.Kafka.Consume;

public class KafkaConsumeService<TKey, TEvent> : BackgroundService
    where TKey : IEvent, new() where TEvent : IEvent, new()
{
    private readonly IActivityAccessor _activityAccessor;
    private readonly ApplicationConfiguration _applicationConfiguration;
    private readonly IConsumer<byte[], byte[]> _consumer;
    private readonly KafkaConfiguration _consumerKafkaConfiguration;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IAsyncDeserializer<TKey> _keyDeserializer;
    private readonly ILogger _logger;
    private readonly SemaphoreSlim _messageRateLimiter;
    private readonly IProducer<byte[], byte[]> _producer;
    private readonly TimeSpan _releaseTimeout = TimeSpan.FromSeconds(20);
    private readonly double? _retryDelaySeconds;
    private readonly string _retryTopicName;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeProvider _timeProvider;
    private readonly string _topicName;
    private readonly IAsyncDeserializer<TEvent> _valueDeserializer;

    public KafkaConsumeService(
        ApplicationConfiguration applicationConfiguration,
        IServiceProvider serviceProvider,
        IActivityAccessor activityAccessor,
        ILogger<KafkaConsumeService<TKey, TEvent>> logger,
        IAsyncDeserializer<TKey> keyDeserializer,
        IAsyncDeserializer<TEvent> valueDeserializer,
        KafkaConfiguration consumerKafkaConfiguration,
        KafkaConfiguration retryTopicKafkaConfiguration,
        string topicName,
        string retryTopicName,
        double? retryDelaySeconds,
        IHostApplicationLifetime hostApplicationLifetime,
        IConsumerFactory consumerFactory,
        IProducerFactory producerFactory,
        TimeProvider timeProvider)
    {
        _applicationConfiguration = applicationConfiguration;
        _serviceProvider = serviceProvider;
        _activityAccessor = activityAccessor;
        _logger = logger;
        _keyDeserializer = keyDeserializer;
        _valueDeserializer = valueDeserializer;
        _consumerKafkaConfiguration = consumerKafkaConfiguration;
        _topicName = topicName;
        _retryTopicName = retryTopicName;
        _retryDelaySeconds = retryDelaySeconds;
        _hostApplicationLifetime = hostApplicationLifetime;
        _timeProvider = timeProvider;
        _producer = producerFactory.Build<byte[], byte[]>(retryTopicKafkaConfiguration);
        _consumer = consumerFactory.Build<byte[], byte[]>(_consumerKafkaConfiguration);
        _messageRateLimiter = new SemaphoreSlim(consumerKafkaConfiguration.MaxMessageNumberToProcessAtAnyTime);
    }

    /// <summary>
    ///     This method is called when the <see cref="IHostedService" /> starts. The implementation should return a task that
    ///     represents
    ///     the lifetime of the long-running operation(s) being performed.
    /// </summary>
    /// <param name="cancellationToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)" /> is called.</param>
    /// <returns>A <see cref="Task" /> that represents the long running operations.</returns>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{Type}] Subscribing to {TopicName}", GetType().Name, _topicName);

        try
        {
            _consumer.Subscribe(_topicName);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "[{Type}] Failed to subscribe to {TopicName}", GetType().Name, _topicName);

            Environment.ExitCode = KafkaExitCodes.FailedToSubscribe;
            _hostApplicationLifetime.StopApplication();

            return;
        }

        var task = Task.Factory.StartNew(() =>
            {
                // Type subject enables you to use Reactive library and create channel that can
                // be consumed by multiple subscribers. We use the channel here to pump back the
                // paused message back to the main event processing pipeline.
                var pausedConsumeResultsObservable = new Subject<ConsumeResult<byte[], byte[]>>();

                try
                {
                    var realtimeKafkaObservable =
                        WatchTopic(
                                pausedConsumeResultsObservable,
                                cancellationToken)
                            .ToObservable();

                    realtimeKafkaObservable
                        .Where(MatchesConsumerGroup)
                        .Select(consumeResult => PauseIfNeeded(
                            consumeResult,
                            pausedConsumeResultsObservable,
                            cancellationToken))
                        .Where(consumeResult => consumeResult is not null)
                        /* We have two streams generating events for us, Kafka and internal paused channel
                         * We need to apply the same function to both ones, so we use merge function to do that
                         * We only need to merge it here as the message has already travelled back through
                         * the above pipeline earlier and no need to re-do the same work again */
                        .Merge(pausedConsumeResultsObservable)
                        .Select(cr => ProcessEvent(cr!, cancellationToken))
                        // IMPORTANT: This line is to ensure we only process one message at a time from each topic
                        // Our intention is process one message at a time per topic (topic could be the main topic or the retry topic)
                        .Merge(1) // DANGER!!! DON'T CHANGE THIS LINE
                        // We only acknowledge message if the goes through all previous stages of the pipeline
                        // and ends up in the last stage either in error or success state.
                        .Select(AcknowledgeMessage)
                        .Subscribe();
                }
                catch (Exception ex)
                {
                    if (ex is not OperationCanceledException)
                    {
                        _logger.LogCritical(ex, "Exception occurred while running ExecuteAsync method");
                        Environment.ExitCode = KafkaExitCodes.UncaughtException;
                    }

                    _hostApplicationLifetime.StopApplication();
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
                        _hostApplicationLifetime.StopApplication();
                    }
                }
            },
            cancellationToken,
            TaskCreationOptions.None,
            TaskScheduler.Default);

        await task;
    }

    private IEnumerable<ConsumeResult<byte[], byte[]>> WatchTopic(
        IObserver<ConsumeResult<byte[], byte[]>> pausedConsumeResultsObservable,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            ConsumeResult<byte[], byte[]>? consumeResult = null;
            bool isFaulted;

            try
            {
                var successfulRelease =
                    _messageRateLimiter.Wait(_releaseTimeout, cancellationToken);

                if (!successfulRelease)
                {
                    _logger.LogWarning(
                        "Rate limiter timed out on topic {TopicName} after {Timeout}. This may be a sign of incorrect semaphore release",
                        _topicName, _releaseTimeout);
                }

                consumeResult = _consumer.Consume(cancellationToken);
                isFaulted = false;
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Failed to call Consume method - Topic - {Topic}", ex.ConsumerRecord?.Topic);

                if (ex is
                    {
                        Error:
                        {
                            IsBrokerError: true
                        }
                    })
                {
                    // if the error is a broker one, then restart the service
                    // this helps by 
                    // - possibly resetting a connection by restarting
                    // - incrementing the restart counter; letting the observability platform know something is wrong  
                    var error = ex.Error;

                    _logger.LogCritical(ex,
                        "Broker Error! Error: {Code}: {Reason} || Subscription: {Subscription} || Server(s): {Servers} ",
                        error.Code, error.Reason, _consumer.Subscription,
                        _consumerKafkaConfiguration.BootstrapServers);
                    Environment.ExitCode = (int)error.Code;
                    _hostApplicationLifetime.StopApplication();

                    break;
                }

                continue;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Operation cancelled while waiting to receive message on topic {Topic}",
                    _topicName);

                isFaulted = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while waiting to receive message on topic {Topic}",
                    _topicName);

                isFaulted = true;
            }

            if (isFaulted)
            {
                try
                {
                    pausedConsumeResultsObservable.OnCompleted();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to complete pausedConsumeResultsObservable");
                }

                yield break;
            }

            yield return consumeResult!;
        }
    }

    private bool MatchesConsumerGroup(ConsumeResult<byte[], byte[]> consumeResult)
    {
        var consumerGroup = consumeResult.Message.GetConsumerGroup();

        _logger.LogTrace("{Topic} Matching consumer group: {GroupId}",
            consumeResult.Topic,
            _applicationConfiguration.GetSource());

        if (consumerGroup is null)
        {
            _logger.LogTrace("{Topic} No consumer group header set",
                _applicationConfiguration.GetSource());

            return true;
        }

        // if there is no consumer group header, or if there is one it should match the current group
        var match = consumerGroup == _applicationConfiguration.GetSource();

        _logger.LogTrace("{GroupId} [Match: {Match}] {HeaderValue}", _applicationConfiguration.GetSource(), match,
            consumerGroup);

        if (!match)
        {
            AcknowledgeMessage(consumeResult);
        }

        return match;
    }

    private ConsumeResult<byte[], byte[]>? PauseIfNeeded(
        ConsumeResult<byte[], byte[]> consumeResult,
        IObserver<ConsumeResult<byte[], byte[]>> pausedConsumeResultsObservable,
        CancellationToken cancellationToken)
    {
        // This is the main topic, not retry topic, no need to pause. Messages arrived in this
        // topic need to be immediately processed.
        if (_retryDelaySeconds is null)
        {
            return consumeResult;
        }

        var topicPartition = consumeResult.TopicPartition;
        var secondsDifference = GetTimestampDifferenceSecondsFromNow(consumeResult);

        // We only pause if it is needed to be paused, once it is paused, we create a Task to wait
        // until it is time to process the message. We resume processing messages from the
        // Kafka and push the message back from the provided channel back to the main message
        // processing pipeline
        if (secondsDifference < _retryDelaySeconds)
        {
            _consumer.Pause(new[] { consumeResult.TopicPartition });

            var fromSeconds =
                TimeSpan.FromSeconds(_retryDelaySeconds.Value - secondsDifference);

            _ = Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        await Task.Delay(fromSeconds, cancellationToken);

                        _consumer.Resume(new[] { consumeResult.TopicPartition });

                        pausedConsumeResultsObservable.OnNext(consumeResult);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogWarning(
                            "Operation cancelled while waiting to resume message on topic {Topic} and {Partition}",
                            _topicName,
                            consumeResult.Partition.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Exception occurred while waiting to resume message on topic {Topic} and {Partition}",
                            _topicName,
                            consumeResult.Partition.Value);

                        throw;
                    }
                },
                cancellationToken,
                TaskCreationOptions.None,
                TaskScheduler.Default);

            return null;
        }

        _logger.LogTrace("No delay - Processing message from {Topic} - {Partition}", topicPartition.Topic,
            topicPartition.Partition.Value);

        return consumeResult;
    }

    private IObservable<ConsumeResult<byte[], byte[]>> ProcessEvent(
        ConsumeResult<byte[], byte[]> consumeResult,
        CancellationToken cancellationToken) =>
        Observable.FromAsync(async () =>
        {
            try
            {
                await Policy
                    .Handle<DbUpdateConcurrencyException>()
                    .Or<DbUpdateException>(ex =>
                        ex.InnerException is PostgresException &&
                        ex.InnerException.Message.Contains("duplicate key value violates unique constraint"))
                    .Or<InvalidOperationException>(ex =>
                        ex.Message.Contains("cannot be tracked because another instance with the key value") ||
                        (ex.Message.Contains(
                             "An exception has been raised that is likely due to a transient failure") &&
                         ex.InnerException is TimeoutException or NpgsqlException && (
                             ex.InnerException.Message.Contains("The operation has timed out") ||
                             ex.InnerException.Message.Contains("Exception while reading from stream"))))
                    .WaitAndRetryAsync(10, retryAttempt =>
                    {
                        _logger.LogWarning("Failed to call eventSubscriber.HandleAsync - Retry attempt: {retryAttempt}",
                            retryAttempt);

                        return TimeSpan.FromSeconds(1);
                    })
                    .ExecuteAsync(async () =>
                    {
                        await using var scope = _serviceProvider.CreateAsyncScope();

                        var eventSubscriber =
                            scope.ServiceProvider.GetRequiredService<IEventSubscriber<TKey, TEvent>>();
                        var context = scope.ServiceProvider.GetRequiredService<IContext>();
                        var activitySource =
                            _activityAccessor.GetActivitySource(TelemetryKeys.IncomingActivitySourceName);

                        // A consumer activity is created when the message is Consumed.
                        // The following activity uses that as a parent context 
                        // Refer to ConsumerTelemetryDecorator
                        using (activitySource.StartActivity($"handler {eventSubscriber.GetType().Name}"))
                        {
                            var key = await _keyDeserializer.DeserializeAsync(
                                consumeResult.Message.Key,
                                false,
                                SerializationContext.Empty);

                            var @event = await _valueDeserializer.DeserializeAsync(
                                consumeResult.Message.Value,
                                false,
                                SerializationContext.Empty);

                            context.PropertyBag = new PropertyBag();
                            var correlationId = @event.GetCorrelationId();

                            if (!string.IsNullOrWhiteSpace(correlationId))
                            {
                                context.PropertyBag.AddCorrelationId(correlationId);
                            }

                            ConsumeResultContext.Current.Value = consumeResult;

                            try
                            {
                                // Call the subscriber and process the message
                                await eventSubscriber.HandleAsync(consumeResult.Message.Headers, key, @event,
                                    cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _activityAccessor.RecordException(ex);

                                throw;
                            }
                        }
                    });
            }
            catch (Exception ex)
            {
                var topicPartition = consumeResult.TopicPartition;

                _logger.LogError(ex,
                    "Failed to process the message from {Topic} - {Partition}. Moving the message to retry topic: {RetryTopicName}",
                    topicPartition.Topic,
                    topicPartition.Partition.Value,
                    _retryTopicName);

                consumeResult.Message.SetTimestamp();
                consumeResult.Message.SetConsumerGroup(_applicationConfiguration.GetSource());
                consumeResult.Message.SetLastException(ex);

                // This would either move the message into retry topic or dead letter queue.
                await _producer.ProduceAsync(
                    _retryTopicName,
                    consumeResult.Message,
                    cancellationToken);
            }

            return consumeResult;
        });

    private ConsumeResult<byte[], byte[]> AcknowledgeMessage(
        ConsumeResult<byte[], byte[]> consumeResult)
    {
        try
        {
            _consumer.StoreOffset(consumeResult);
        }
        catch (KafkaException ex)
        {
            _logger.LogWarning(ex, "Exception during storing OFFSET {Result}",
                consumeResult.TopicPartitionOffset);

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
        finally
        {
            _messageRateLimiter.Release();
        }

        return consumeResult;
    }

    /// <summary>
    ///     Get the difference in seconds between the current time and the message timestamp
    /// </summary>
    /// <param name="consumeResult"></param>
    /// <returns></returns>
    private double GetTimestampDifferenceSecondsFromNow(ConsumeResult<byte[], byte[]> consumeResult) =>
        (_timeProvider.GetUtcNow() - consumeResult.Message.GetTimestamp()).TotalSeconds;
}
