using Api.Shared.Events.Nats;

namespace Api.Shared.UnitTests.Events.NatsSubjectHelperTests;

[NatsSubject(2)]
file sealed class SampleNatsEvent : INatsEvent
{
    public string StreamName => "test.stream";
    public string Subject => "test.event";
    public string RetrySubjectNamePrefix => "test.event.retry";
    public int RetrySubjectCount => 2;
    public string DeadLetterSubject => "test.event.dead";
}

file sealed class EventWithoutNatsSubjectAttribute : INatsEvent
{
    public string StreamName => "test.stream";
    public string Subject => "test.event";
    public string RetrySubjectNamePrefix => "test.event.retry";
    public int RetrySubjectCount => 2;
    public string DeadLetterSubject => "test.event.dead";
}

public class GetNatsSubjectInfoShould
{
    [Fact]
    public void Return_attribute_when_present()
    {
        var info = NatsSubjectAttributeHelper.GetNatsSubjectInfo<SampleNatsEvent>();

        info.RetryCount.ShouldBe(2);
    }

    [Fact]
    public void Throw_when_attribute_missing() =>
        Should.Throw<ArgumentNullException>(NatsSubjectAttributeHelper.GetNatsSubjectInfo<EventWithoutNatsSubjectAttribute>);

    [Fact]
    public void Derive_ordered_retry_subjects()
    {
        var subjects = NatsSubjectAttributeHelper.GetRetrySubjects<SampleNatsEvent>();

        subjects.ShouldBe(["test.event.retry.0", "test.event.retry.1"]);
    }

    [Fact]
    public void Derive_dead_letter_subject() =>
        NatsSubjectAttributeHelper.GetDeadLetterSubject<SampleNatsEvent>().ShouldBe("test.event.dead");
}
