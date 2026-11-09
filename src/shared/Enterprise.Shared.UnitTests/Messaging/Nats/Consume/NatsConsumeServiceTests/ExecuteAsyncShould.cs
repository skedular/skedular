using Enterprise.Shared.Hosting;
using Enterprise.Shared.Messaging.Nats.Configuration;
using Enterprise.Shared.Messaging.Nats.Consume;
using Enterprise.Shared.Messaging.Nats.Produce;
using Enterprise.Shared.Messaging.Nats.Provisioning;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;

namespace Enterprise.Shared.UnitTests.Messaging.Nats.Consume.NatsConsumeServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ExecuteAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Include_the_retry_delay_in_the_acknowledgment_lease(
        [Frozen]
        INatsJetStreamProvisioner provisioner,
        [Frozen]
        IHostApplicationLifetimeWrapper lifetime,
        RetryConsumer sut)
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        A.CallTo(() => provisioner.EnsureConsumerAsync(A<NatsConsumerDefinition>._, cancellation.Token))
            .Throws(new OperationCanceledException(cancellation.Token));

        await sut.RunAsync(cancellation.Token);

        A.CallTo(() => provisioner.EnsureConsumerAsync(
                A<NatsConsumerDefinition>.That.Matches(definition =>
                    definition.FilterSubject == "events.main.retry.2" && definition.AckWait == TimeSpan.FromSeconds(70)),
                cancellation.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => lifetime.StopApplication()).MustNotHaveHappened();
    }

    public sealed class RetryConsumer(
        NatsConfiguration configuration,
        INatsConnection connection,
        INatsJetStreamProvisioner provisioner,
        INatsMessageHandler<StringValue, StringValue> handler,
        INatsPublisherForwarder forwarder,
        TimeProvider timeProvider,
        IHostApplicationLifetimeWrapper lifetime,
        ILogger<NatsConsumeService<StringValue, StringValue>> logger)
        : NatsConsumeService<StringValue, StringValue>(
            configuration, connection, provisioner, "events", "consumer", "events.main.retry.2", handler, forwarder,
            NatsRetrySubjectSetting.Create("events.main", 3), "events.main.dead", true, timeProvider, lifetime, logger)
    {
        public Task RunAsync(CancellationToken cancellationToken) => ExecuteAsync(cancellationToken);
    }
}
