using Api.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temporalio.Client;

namespace Enterprise.Shared.Outbox.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class TemporalOutbox
{
    public string Id { get; set; }
    public string WorkflowType { get; set; }
    public string? ExecutionArgs { get; set; }
    public WorkflowOptions WorkflowOptions { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public DateTimeOffset? LastRetry { get; set; }
    public int RetryCount { get; set; }
    public string? ProcessingErrors { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TemporalOutboxConfiguration : IEntityTypeConfiguration<TemporalOutbox>
{
    public void Configure(EntityTypeBuilder<TemporalOutbox> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id).HasMaxLength(Constants.MaxUniqueIdLength);
        builder.Property(item => item.WorkflowType).HasMaxLength(Constants.MaxWorkflowTypeLength);
        builder.Property(item => item.ExecutionArgs).HasMaxLength(Constants.MaxWorkflowExecutionArgsLength);
        builder.Property(item => item.WorkflowOptions).HasColumnType("jsonb");
        builder.Property(item => item.ProcessingErrors).HasMaxLength(Constants.MaxOutboxProcessingErrorsLength);

        builder.HasIndex(item => item.LastRetry);
        builder.HasIndex(item => item.RetryCount);
    }
}
