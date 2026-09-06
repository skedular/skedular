using Enterprise.Shared.Messaging.Nats.Configuration;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using NATS.Client.JetStream;
using NatsLogEvents = Enterprise.Shared.Messaging.Nats.Logging.NatsLogEvents;

namespace Enterprise.Shared.Messaging.Nats.Provisioning;

public sealed class NatsJetStreamProvisioner(INatsConnection connection, ILogger<NatsJetStreamProvisioner>? logger = null) : INatsJetStreamProvisioner
{
    private readonly NatsJSContext _jetStream = new(connection);

    public async ValueTask EnsureStreamAsync(NatsStreamDefinition definition, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(definition);

        ValidateName(definition.Name, nameof(definition.Name));

        if (definition.Subjects.Count == 0)
        {
            throw new ArgumentException("At least one subject is required.", nameof(definition.Subjects));
        }

        await _jetStream.CreateOrUpdateStreamAsync(definition.ToClientConfig(), cancellationToken);
        foreach (var subject in definition.Subjects)
        {
            if (logger is not null)
            {
                NatsLogEvents.ResourceProvisioned(logger, definition.Name, subject);
            }
        }
    }

    public async ValueTask EnsureConsumerAsync(NatsConsumerDefinition definition, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(definition);

        ValidateName(definition.StreamName, nameof(definition.StreamName));
        ValidateName(definition.DurableName, nameof(definition.DurableName));

        ArgumentException.ThrowIfNullOrWhiteSpace(definition.FilterSubject);

        await _jetStream.CreateOrUpdateConsumerAsync(definition.StreamName, definition.ToClientConfig(), cancellationToken);

        if (logger is not null)
        {
            NatsLogEvents.ResourceProvisioned(logger, definition.StreamName, definition.FilterSubject);
        }
    }

    private static void ValidateName(string value, string parameterName) => ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
}
