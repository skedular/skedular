using System.Reflection;

namespace Api.Shared.Events.Nats;

public static class NatsSubjectAttributeHelper
{
    public static NatsSubjectAttribute GetNatsSubjectInfo<TEvent>() where TEvent : INatsEvent
    {
        var eventType = typeof(TEvent);
        var attribute = eventType.GetCustomAttribute<NatsSubjectAttribute>();

        return attribute ?? throw new ArgumentNullException(nameof(attribute), $"{eventType.FullName} does not have NatsTopicAttribute implemented");
    }

    public static string GetStreamName<TEvent>() where TEvent : INatsEvent, new() => new TEvent().StreamName;

    public static string GetSubject<TEvent>() where TEvent : INatsEvent, new() => new TEvent().Subject;

    public static IReadOnlyList<string> GetRetrySubjects<TEvent>() where TEvent : INatsEvent, new()
    {
        var metadata = GetNatsSubjectInfo<TEvent>();
        var @event = new TEvent();

        return
        [
            .. Enumerable.Range(0, metadata.RetryCount).Select(index => $"{@event.RetrySubjectNamePrefix}.{index}"),
        ];
    }

    public static string GetDeadLetterSubject<TEvent>() where TEvent : INatsEvent, new() => new TEvent().DeadLetterSubject;
}
