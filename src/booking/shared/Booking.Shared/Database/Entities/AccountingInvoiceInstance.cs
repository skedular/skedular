using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class AccountingInvoiceInstance : EntityBase
{
    public string Provider { get; set; }
    public string ExternalInvoiceId { get; set; }
    public string? ExternalInvoiceNumber { get; set; }
    public string? ExternalInvoiceUrl { get; set; }
    public string ExternalStatus { get; set; }
    public DateTimeOffset? SentAt { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    public DateTimeOffset? LastSyncedAt { get; set; }
    public string? LastError { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string AccountingInvoiceExportLinkId { get; set; }
    public virtual AccountingInvoiceExportLink AccountingInvoiceExportLink { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class AccountingInvoiceInstanceConfiguration : IEntityTypeConfiguration<AccountingInvoiceInstance>
{
    public void Configure(EntityTypeBuilder<AccountingInvoiceInstance> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.Provider).HasMaxLength(Constants.MaxAccountingProviderLength);
        builder.Property(item => item.ExternalInvoiceId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.ExternalInvoiceNumber).HasMaxLength(Constants.MaxInvoiceNumberLength);
        builder.Property(item => item.ExternalInvoiceUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.ExternalStatus).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.LastError).HasMaxLength(Constants.MaxAccountingErrorLength);

        builder.HasOne(item => item.AccountingInvoiceExportLink).WithMany().HasForeignKey(item => item.AccountingInvoiceExportLinkId);
        builder.HasOne(item => item.Organization).WithMany().HasForeignKey(item => item.OrganizationId);

        builder.HasIndex(item => item.OrganizationId);
        builder.HasIndex(item => item.AccountingInvoiceExportLinkId);
        builder.HasIndex(item => new
        {
            item.Provider,
            item.ExternalInvoiceId,
        }).IsUnique();
        builder.HasIndex(item => item.ExternalStatus);
    }
}
