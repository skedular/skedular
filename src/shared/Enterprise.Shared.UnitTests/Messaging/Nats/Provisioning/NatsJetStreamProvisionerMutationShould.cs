using Enterprise.Shared.Messaging.Nats.Configuration;
using Enterprise.Shared.Messaging.Nats.Provisioning;

namespace Enterprise.Shared.UnitTests.Messaging.Nats.Provisioning;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class NatsJetStreamProvisionerMutationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_stream_without_subjects_before_broker_call(NatsJetStreamProvisioner sut, CancellationToken cancellationToken) =>
        await Should.ThrowAsync<ArgumentException>(() => sut.EnsureStreamAsync(new NatsStreamDefinition("events", []), cancellationToken).AsTask());

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_consumer_without_durable_name_before_broker_call(NatsJetStreamProvisioner sut, CancellationToken cancellationToken) =>
        await Should.ThrowAsync<ArgumentException>(() =>
            sut.EnsureConsumerAsync(new NatsConsumerDefinition(
                    "events",
                    string.Empty,
                    "events.main",
                    TimeSpan.FromSeconds(30), -1),
                cancellationToken).AsTask());
}
