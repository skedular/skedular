using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationOffering : EntityBaseWithDeleted
{
    public OfferingCode Code { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool AutoRenew { get; set; }
    public int UnitPrice { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? StripePaymentIntentId { get; set; }
    public virtual StripePaymentIntent? StripePaymentIntent { get; set; }

    public virtual Organization Organization { get; set; }
    public virtual ICollection<OrganizationOfferingActiveMember> OrganizationOfferingActiveMembers { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationOfferingConfiguration : IEntityTypeConfiguration<OrganizationOffering>
{
    public void Configure(EntityTypeBuilder<OrganizationOffering> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.HasOne(item => item.Organization).WithMany(item => item.OrganizationOfferings);
        builder
            .HasOne(item => item.StripePaymentIntent)
            .WithOne(item => item.OrganizationOffering)
            .HasForeignKey<OrganizationOffering>(item => item.StripePaymentIntentId);

        builder.HasIndex(item => item.Code);
        builder.HasIndex(item => item.Start);
        builder.HasIndex(item => item.End);
        builder.HasIndex(item => new { item.Start, item.End });
        builder.HasIndex(item => item.AutoRenew);
        builder.HasIndex(item => item.UnitPrice);
    }
}
