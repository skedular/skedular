using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temporalio.Client;

namespace Enterprise.Shared.Outbox.Temporal;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class TemporalSignalOutbox
{
    public string Id { get; set; }
    public string WorkflowId { get; set; }
    public string SignalType { get; set; }
    public string? ExecutionArgs { get; set; }
    public WorkflowSignalOptions WorkflowSignalOptions { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public DateTimeOffset? LastRetry { get; set; }
    public int RetryCount { get; set; }
    public string? ProcessingErrors { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TemporalSignalOutboxConfiguration : IEntityTypeConfiguration<TemporalSignalOutbox>
{
    public void Configure(EntityTypeBuilder<TemporalSignalOutbox> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id).HasMaxLength(Constants.MaxUniqueIdLength);
        builder.Property(item => item.WorkflowId).HasMaxLength(Constants.MaxWorkflowUniqueIdLength);
        builder.Property(item => item.SignalType).HasMaxLength(Constants.MaxWorkflowSignalLength);
        builder.Property(item => item.ExecutionArgs).HasMaxLength(Constants.MaxWorkflowExecutionArgsLength);
        builder
            .Property(item => item.WorkflowSignalOptions)
            .HasConversion(
                OutboxJsonValueConverter.CreateConverter<WorkflowSignalOptions>(),
                OutboxJsonValueConverter.CreateComparer<WorkflowSignalOptions>());
        builder.Property(item => item.ProcessingErrors).HasMaxLength(Constants.MaxOutboxProcessingErrorsLength);

        builder.HasIndex(item => item.LastRetry);
        builder.HasIndex(item => item.RetryCount);
    }
}
