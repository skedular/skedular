using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationInvoiceCounter : EntityBase
{
    public int InvoiceNumber { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationInvoiceCounterConfiguration : IEntityTypeConfiguration<OrganizationInvoiceCounter>
{
    public void Configure(EntityTypeBuilder<OrganizationInvoiceCounter> builder)
    {
        builder.ConfigureEntityBase();

        builder.HasOne(item => item.Organization).WithMany(item => item.OrganizationInvoiceCounters).HasForeignKey(item => item.OrganizationId);

        builder.HasIndex(item => item.OrganizationId).IsUnique();
    }
}
