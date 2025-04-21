using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class BillingAddress : EntityBase
{
    public string Email { get; set; }
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string Suburb { get; set; }
    public string City { get; set; }
    public string? Province { get; set; }
    public string Zipcode { get; set; }
    public string Country { get; set; }

    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class BillingAddressConfiguration : IEntityTypeConfiguration<BillingAddress>
{
    public void Configure(EntityTypeBuilder<BillingAddress> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.Email).HasMaxLength(Constants.MaxEmailLength);
        builder.Property(item => item.AddressLine1).HasMaxLength(Constants.MaxAddressLineLength);
        builder.Property(item => item.AddressLine2).HasMaxLength(Constants.MaxAddressLineLength);
        builder.Property(item => item.Suburb).HasMaxLength(Constants.MaxSuburbLength);
        builder.Property(item => item.City).HasMaxLength(Constants.MaxCityLength);
        builder.Property(item => item.Province).HasMaxLength(Constants.MaxProvinceLength);
        builder.Property(item => item.Zipcode).HasMaxLength(Constants.MaxZipcodeLength);
        builder.Property(item => item.Country).HasMaxLength(Constants.MaxCountryLength);
    }
}
