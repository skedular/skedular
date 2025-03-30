using Api.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enterprise.Shared.Outbox.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class Outbox
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Id { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Topic { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public byte[] Key { get; set; } = [];
    public byte[] Payload { get; set; } = [];
    public DateTimeOffset Timestamp { get; set; }
    public DateTimeOffset? LastRetry { get; set; }
    public int RetryCount { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ProcessingErrors { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OutboxConfiguration : IEntityTypeConfiguration<Outbox>
{
    public void Configure(EntityTypeBuilder<Outbox> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id).HasMaxLength(Constants.MaxUniqueIdLength);
        builder.Property(item => item.Topic).HasMaxLength(Constants.MaxKafkaTopicNameLength);
        builder.Property(item => item.ProcessingErrors).HasMaxLength(Constants.MaxOutboxProcessingErrorsLength);

        builder.HasIndex(item => item.LastRetry);
        builder.HasIndex(item => item.RetryCount);
    }
}
