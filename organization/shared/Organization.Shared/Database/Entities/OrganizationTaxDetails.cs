using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationTaxDetails : EntityBase
{
    public string GstNumber { get; set; }
    public decimal GstPercentage { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationTaxDetailsConfiguration : IEntityTypeConfiguration<OrganizationTaxDetails>
{
    public void Configure(EntityTypeBuilder<OrganizationTaxDetails> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.GstNumber).HasMaxLength(Constants.MaxTaxDetailsGstNumberLength);
        builder.Property(item => item.GstPercentage).HasColumnType("DECIMAL(5,2)");

        builder
            .HasOne(item => item.Organization)
            .WithOne(item => item.OrganizationTaxDetails)
            .HasForeignKey<OrganizationTaxDetails>(item => item.OrganizationId);
    }
}
