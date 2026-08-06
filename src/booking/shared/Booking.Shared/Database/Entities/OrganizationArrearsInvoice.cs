using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationArrearsInvoice : EntityBase
{
    public string InvoiceNumber { get; set; }
    public string InvoiceUrl { get; set; }
    public DateTimeOffset BillingPeriodStartInclusive { get; set; }
    public DateTimeOffset BillingPeriodEndExclusive { get; set; }
    public string Currency { get; set; }
    public decimal TotalAmount { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string CustomerId { get; set; }
    public virtual Customer Customer { get; set; }
    public virtual ICollection<OrganizationArrearsInvoiceLine> Lines { get; set; } = [];
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationArrearsInvoiceConfiguration : IEntityTypeConfiguration<OrganizationArrearsInvoice>
{
    public void Configure(EntityTypeBuilder<OrganizationArrearsInvoice> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.InvoiceNumber).HasMaxLength(Constants.MaxInvoiceNumberLength);
        builder.Property(item => item.InvoiceUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(item => item.TotalAmount).HasColumnType("DECIMAL(18,4)");

        builder.HasOne(item => item.Organization).WithMany(item => item.OrganizationArrearsInvoices).HasForeignKey(item => item.OrganizationId);
        builder.HasOne(item => item.Customer).WithMany(item => item.OrganizationArrearsInvoices).HasForeignKey(item => item.CustomerId);

        builder.HasIndex(item => item.OrganizationId);
        builder.HasIndex(item => item.CustomerId);
        builder.HasIndex(item => new
        {
            item.OrganizationId,
            item.InvoiceNumber,
        }).IsUnique();
        builder.HasIndex(item => item.BillingPeriodStartInclusive);
        builder.HasIndex(item => item.BillingPeriodEndExclusive);
        builder.HasIndex(item => item.Currency);
    }
}
