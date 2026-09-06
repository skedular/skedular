using System.Globalization;
using Enterprise.Shared.Hosting;
using Enterprise.Shared.Messaging.Nats.Configuration;
using Enterprise.Shared.Messaging.Nats.Produce;
using Enterprise.Shared.Messaging.Nats.Provisioning;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using NATS.Client.JetStream;
using NatsLogEvents = Enterprise.Shared.Messaging.Nats.Logging.NatsLogEvents;

namespace Enterprise.Shared.Messaging.Nats.Consume;

public class NatsConsumeService<TKey, TEvent>(
    INatsConnection connection,
    INatsJetStreamProvisioner provisioner,
    string streamName,
    string durableName,
    string subject,
    INatsMessageHandler<TKey, TEvent> handler,
    INatsPublisherForwarder? forwarder,
    IReadOnlyList<NatsRetrySubjectSetting> retrySubjects,
    string? deadLetterSubject,
    bool reliable,
    TimeProvider timeProvider,
    IHostApplicationLifetimeWrapper hostApplicationLifetime,
    ILogger<NatsConsumeService<TKey, TEvent>> logger) : BackgroundService
    where TKey : new() where TEvent : new()
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        INatsJSConsumer? consumer = null;

        try
        {
            await provisioner.EnsureConsumerAsync(
                new NatsConsumerDefinition(streamName, durableName, subject, TimeSpan.FromSeconds(30), -1),
                cancellationToken);

            consumer = await new NatsJSContext(connection).GetConsumerAsync(streamName, durableName, cancellationToken);

            logger.LogInformation("NATS consumer started. Consumer={Consumer}", consumer.Info.Name);

            await foreach (var message in consumer.ConsumeAsync<byte[]>(
                               opts: new NatsJSConsumeOpts
                               {
                                   MaxMsgs = 1,
                               },
                               cancellationToken: cancellationToken))
            {
                try
                {
                    message.EnsureSuccess();
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "NATS message delivery failed; message remains unacknowledged. Subject={Subject}", message.Subject);
                    continue;
                }

                NatsLogEvents.MessageDelivered(logger, message.Subject);

                var headers = ToHeaders(message.Headers);
                var natsMessage = new NatsMessage
                {
                    Subject = message.Subject,
                    Payload = message.Data ?? [],
                    Headers = headers,
                    AcknowledgeAsync = token => message.AckAsync(cancellationToken: token),
                };

                try
                {
                    await WaitForRetryDelayAsync(natsMessage, cancellationToken);
                    await handler.HandleMessageAsync(natsMessage, cancellationToken);
                    await message.AckAsync(cancellationToken: cancellationToken);

                    NatsLogEvents.MessageAcknowledged(logger, message.Subject);
                }
                catch (Exception exception) when (reliable)
                {
                    var nextSubject = GetNextSubject(message.Subject);
                    if (forwarder is null || nextSubject is null)
                    {
                        throw;
                    }

                    var forwardingHeaders = new Dictionary<string, string>(headers, StringComparer.OrdinalIgnoreCase)
                    {
                        [NatsMessage.RetryTimestampHeader] = timeProvider.GetUtcNow().ToString("O", CultureInfo.InvariantCulture),
                    };

                    try
                    {
                        await forwarder.PublishForwardedAsync(nextSubject, natsMessage.Payload, forwardingHeaders, cancellationToken);
                        await message.AckAsync(cancellationToken: cancellationToken);

                        logger.LogWarning(
                            exception, "NATS message forwarded. Subject={Subject} NextSubject={NextSubject}", message.Subject,
                            nextSubject);
                    }
                    catch (Exception forwardingException)
                    {
                        logger.LogError(
                            forwardingException,
                            "NATS message forwarding failed; message remains unacknowledged. Subject={Subject} NextSubject={NextSubject}",
                            message.Subject,
                            nextSubject);
                    }
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "NATS message processing failed; message remains unacknowledged. Subject={Subject}", message.Subject);
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("NATS consumer stopped due to cancellation. Stream={Stream} Durable={Durable}", streamName, durableName);
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "NATS consumer failed. Stream={Stream} Durable={Durable}", streamName, durableName);
            Environment.ExitCode = MessagingExitCodes.UncaughtException;
            hostApplicationLifetime.StopApplication();
            throw;
        }
        finally
        {
            if (consumer is not null)
            {
                NatsLogEvents.ConsumerShutdown(logger, consumer.Info.Name);
            }
        }
    }

    private async ValueTask WaitForRetryDelayAsync(NatsMessage message, CancellationToken cancellationToken)
    {
        var setting = retrySubjects.FirstOrDefault(item => item.Subject == message.Subject);
        var retryTimestamp = message.GetRetryTimestamp();
        if (setting is null || retryTimestamp is null)
        {
            return;
        }

        var remaining = setting.Delay - (timeProvider.GetUtcNow() - retryTimestamp.Value);
        if (remaining > TimeSpan.Zero)
        {
            NatsLogEvents.RetryDelayPending(logger, message.Subject, remaining);
            await Task.Delay(remaining, cancellationToken);
        }
    }

    private string? GetNextSubject(string subjectName)
    {
        if (subjectName == subject)
        {
            return retrySubjects.FirstOrDefault()?.Subject;
        }

        var index = -1;
        for (var candidate = 0; candidate < retrySubjects.Count; candidate++)
        {
            if (retrySubjects[candidate].Subject == subjectName)
            {
                index = candidate;
                break;
            }
        }

        return index >= 0 && index + 1 < retrySubjects.Count
            ? retrySubjects[index + 1].Subject
            : index >= 0
                ? deadLetterSubject
                : null;
    }

    private static Dictionary<string, string> ToHeaders(NatsHeaders? headers) =>
        headers is null
            ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            : headers.ToDictionary(item => item.Key, item => item.Value.ToString(), StringComparer.OrdinalIgnoreCase);
}
