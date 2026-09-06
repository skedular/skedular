using Api.Shared.Events.Nats;
using Enterprise.Shared.Messaging.Nats.Configuration;
using Enterprise.Shared.Messaging.Nats.Consume;
using Enterprise.Shared.Messaging.Nats.Provisioning;

namespace Enterprise.Shared.Messaging.Nats;

public interface INatsHelper
{
    ValueTask CreateResourcesForEventAsync<TEvent>(CancellationToken cancellationToken) where TEvent : INatsEvent, new();
}

public sealed class NatsHelper(INatsJetStreamProvisioner provisioner) : INatsHelper
{
    public async ValueTask CreateResourcesForEventAsync<TEvent>(CancellationToken cancellationToken) where TEvent : INatsEvent, new()
    {
        var metadata = NatsSubjectAttributeHelper.GetNatsSubjectInfo<TEvent>();
        var retrySettings = NatsRetrySubjectSetting.Create(NatsSubjectAttributeHelper.GetSubject<TEvent>(), metadata.RetryCount);
        var deadLetterSubject = NatsSubjectAttributeHelper.GetDeadLetterSubject<TEvent>();
        var subjects = new List<string>
        {
            NatsSubjectAttributeHelper.GetSubject<TEvent>(),
        };

        subjects.AddRange(retrySettings.Select(item => item.Subject));
        subjects.Add(deadLetterSubject);

        await provisioner.EnsureStreamAsync(
            new NatsStreamDefinition(NatsSubjectAttributeHelper.GetStreamName<TEvent>(), subjects),
            cancellationToken);
    }
}
