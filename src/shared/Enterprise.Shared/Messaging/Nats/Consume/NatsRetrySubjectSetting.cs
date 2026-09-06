namespace Enterprise.Shared.Messaging.Nats.Consume;

public sealed record NatsRetrySubjectSetting(string Subject, int RetryIndex, TimeSpan Delay)
{
    public static IReadOnlyList<NatsRetrySubjectSetting> Create(
        string subject,
        int retryCount,
        int delayBaseSeconds = 10)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentOutOfRangeException.ThrowIfNegative(retryCount);
        ArgumentOutOfRangeException.ThrowIfNegative(delayBaseSeconds);

        return Enumerable.Range(0, retryCount)
            .Select(index => new NatsRetrySubjectSetting(
                $"{subject}.retry.{index}",
                index,
                TimeSpan.FromSeconds(delayBaseSeconds * Math.Pow(2, index))))
            .ToArray();
    }
}
