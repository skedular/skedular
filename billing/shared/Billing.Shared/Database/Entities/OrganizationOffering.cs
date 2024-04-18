using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationOffering : ReplicatedEntityBaseWithDeleted
{
    public OfferingCode Code { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public int UnitPrice { get; set; }
    public int TotalNumberOfActiveCustomers { get; set; }
    public long TotalCost { get; set; }
    public DateTimeOffset? InvoiceDate { get; set; }

    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationOfferingConfiguration : IEntityTypeConfiguration<OrganizationOffering>
{
    public void Configure(EntityTypeBuilder<OrganizationOffering> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder
            .HasOne(item => item.Organization)
            .WithMany(item => item.OrganizationOfferings);

        builder.HasIndex(item => item.Code);
        builder.HasIndex(item => item.Start);
        builder.HasIndex(item => item.End);
        builder.HasIndex(item => item.UnitPrice);
        builder.HasIndex(item => item.TotalCost);
        builder.HasIndex(item => item.InvoiceDate);
    }
}
