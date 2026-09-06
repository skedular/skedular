namespace Api.Shared.Events.Nats;

public interface INatsEvent
{
    string StreamName { get; }
    string Subject { get; }
    string RetrySubjectNamePrefix { get; }
    int RetrySubjectCount { get; }
    string DeadLetterSubject { get; }
}
