using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationArrearsInvoiceLine : EntityBase
{
    public string SegmentKey { get; set; }
    public DateTimeOffset ServicePeriodStartInclusive { get; set; }
    public DateTimeOffset ServicePeriodEndExclusive { get; set; }
    public DateTimeOffset EarnedAt { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }

    public virtual Booking Booking { get; set; }
    public virtual OrganizationArrearsInvoice OrganizationArrearsInvoice { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationArrearsInvoiceLineConfiguration : IEntityTypeConfiguration<OrganizationArrearsInvoiceLine>
{
    public void Configure(EntityTypeBuilder<OrganizationArrearsInvoiceLine> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.SegmentKey).HasMaxLength(1024);
        builder.Property(item => item.Amount).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.Description).HasMaxLength(Constants.MaxDescriptionLength);

        builder.HasOne(item => item.Booking).WithMany(item => item.OrganizationArrearsInvoiceLines);
        builder.HasOne(item => item.OrganizationArrearsInvoice).WithMany(item => item.Lines);

        builder.HasIndex(item => item.SegmentKey).IsUnique();
        builder.HasIndex(item => item.EarnedAt);
        builder.HasIndex(item => item.Amount);
    }
}
