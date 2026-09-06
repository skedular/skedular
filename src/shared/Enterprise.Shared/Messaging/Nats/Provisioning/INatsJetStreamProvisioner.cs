using Enterprise.Shared.Messaging.Nats.Configuration;

namespace Enterprise.Shared.Messaging.Nats.Provisioning;

public interface INatsJetStreamProvisioner
{
    ValueTask EnsureStreamAsync(NatsStreamDefinition definition, CancellationToken cancellationToken);
    ValueTask EnsureConsumerAsync(NatsConsumerDefinition definition, CancellationToken cancellationToken);
}
