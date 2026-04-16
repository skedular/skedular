using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enterprise.Shared.Outbox.Kafka;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class KafkaOutbox
{
    public string Id { get; set; }
    public string Topic { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public byte[] Key { get; set; } = [];
    public byte[] Payload { get; set; } = [];
    public DateTimeOffset Timestamp { get; set; }
    public DateTimeOffset? LastRetry { get; set; }
    public int RetryCount { get; set; }
    public string? ProcessingErrors { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class KafkaOutboxConfiguration : IEntityTypeConfiguration<KafkaOutbox>
{
    public void Configure(EntityTypeBuilder<KafkaOutbox> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id).HasMaxLength(Constants.MaxUniqueIdLength);
        builder.Property(item => item.Topic).HasMaxLength(Constants.MaxKafkaTopicNameLength);
        builder.Property(item => item.ProcessingErrors).HasMaxLength(Constants.MaxOutboxProcessingErrorsLength);
        builder
            .Property(item => item.Headers)
            .HasConversion(
                OutboxJsonValueConverter.CreateConverter<Dictionary<string, string>>(),
                OutboxJsonValueConverter.CreateComparer<Dictionary<string, string>>());

        builder.HasIndex(item => item.LastRetry);
        builder.HasIndex(item => item.RetryCount);
    }
}
