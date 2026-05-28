using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class AccountingPaymentEvent : EntityBase
{
    public string Provider { get; set; }
    public string ExternalInvoiceId { get; set; }
    public string ExternalPaymentId { get; set; }
    public string ExternalStatus { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public string? PayloadJson { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class AccountingPaymentEventConfiguration : IEntityTypeConfiguration<AccountingPaymentEvent>
{
    public void Configure(EntityTypeBuilder<AccountingPaymentEvent> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.Provider).HasMaxLength(Constants.MaxAccountingProviderLength);
        builder.Property(item => item.ExternalInvoiceId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.ExternalPaymentId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.ExternalStatus).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.PayloadJson).HasMaxLength(Constants.MaxDescriptionLength);

        builder.HasOne(item => item.Organization).WithMany().HasForeignKey(item => item.OrganizationId);

        builder.HasIndex(item => item.OrganizationId);
        builder.HasIndex(item => new { item.OrganizationId, item.Provider, item.ExternalPaymentId }).IsUnique();
        builder.HasIndex(item => item.ExternalInvoiceId);
        builder.HasIndex(item => item.ProcessedAt);
    }
}
