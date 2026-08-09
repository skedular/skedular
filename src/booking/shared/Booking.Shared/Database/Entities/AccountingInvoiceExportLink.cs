using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class AccountingInvoiceExportLink : EntityBase
{
    public string Provider { get; set; }
    public string LocalEntityType { get; set; }
    public string LocalEntityId { get; set; }
    public string? ExternalInvoiceId { get; set; }
    public string? ExternalInvoiceNumber { get; set; }
    public string? ExternalInvoiceUrl { get; set; }
    public string? ExternalInvoiceMode { get; set; }
    public string ExternalStatus { get; set; }
    public string? ExportConfigurationState { get; set; }
    public string? ExportConfigurationMessage { get; set; }
    public string? RepeatingScheduleSource { get; set; }
    public string? RepeatingScheduleUnit { get; set; }
    public int? RepeatingSchedulePeriod { get; set; }
    public DateTimeOffset? SentAt { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    public DateTimeOffset? LastSyncedAt { get; set; }
    public string? LastError { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class AccountingInvoiceExportLinkConfiguration : IEntityTypeConfiguration<AccountingInvoiceExportLink>
{
    public void Configure(EntityTypeBuilder<AccountingInvoiceExportLink> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.Provider).HasMaxLength(Constants.MaxAccountingProviderLength);
        builder.Property(item => item.LocalEntityType).HasMaxLength(Constants.MaxAccountingEntityTypeLength);
        builder.Property(item => item.LocalEntityId).HasMaxLength(Constants.MaxLocalEntityLength);
        builder.Property(item => item.ExternalInvoiceId).HasMaxLength(Constants.MaxExternalInvoiceIdLength);
        builder.Property(item => item.ExternalInvoiceNumber).HasMaxLength(Constants.MaxInvoiceNumberLength);
        builder.Property(item => item.ExternalInvoiceUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.ExternalInvoiceMode).HasMaxLength(Constants.MaxAccountingEntityTypeLength);
        builder.Property(item => item.ExternalStatus).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.ExportConfigurationState).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.ExportConfigurationMessage).HasMaxLength(Constants.MaxAccountingErrorLength);
        builder.Property(item => item.RepeatingScheduleSource).HasMaxLength(Constants.MaxAccountingEntityTypeLength);
        builder.Property(item => item.RepeatingScheduleUnit).HasMaxLength(Constants.MaxAccountingEntityTypeLength);
        builder.Property(item => item.LastError).HasMaxLength(Constants.MaxAccountingErrorLength);

        builder.HasOne(item => item.Organization).WithMany().HasForeignKey(item => item.OrganizationId);

        builder.HasIndex(item => item.OrganizationId);
        builder.HasIndex(item => new
        {
            item.Provider,
            item.LocalEntityType,
            item.LocalEntityId,
        }).IsUnique();
        builder.HasIndex(item => new
        {
            item.Provider,
            item.ExternalInvoiceId,
        }).IsUnique();
        builder.HasIndex(item => item.ExternalStatus);
    }
}
