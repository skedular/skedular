using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CustomerBillingDetails : EntityBase
{
    public string? CompanyName { get; set; }
    public string Email { get; set; }
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string Suburb { get; set; }
    public string City { get; set; }
    public string? Province { get; set; }
    public string Zipcode { get; set; }
    public string Country { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string CustomerId { get; set; }
    public virtual Customer Customer { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class CustomerBillingDetailsConfiguration : IEntityTypeConfiguration<CustomerBillingDetails>
{
    public void Configure(EntityTypeBuilder<CustomerBillingDetails> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.CompanyName).HasMaxLength(Constants.MaxOrganizationNameLength);
        builder.Property(item => item.Email).HasMaxLength(Constants.MaxEmailLength);
        builder.Property(item => item.AddressLine1).HasMaxLength(Constants.MaxAddressLineLength);
        builder.Property(item => item.AddressLine2).HasMaxLength(Constants.MaxAddressLineLength);
        builder.Property(item => item.Suburb).HasMaxLength(Constants.MaxSuburbLength);
        builder.Property(item => item.City).HasMaxLength(Constants.MaxCityLength);
        builder.Property(item => item.Province).HasMaxLength(Constants.MaxProvinceLength);
        builder.Property(item => item.Zipcode).HasMaxLength(Constants.MaxZipcodeLength);
        builder.Property(item => item.Country).HasMaxLength(Constants.MaxCountryLength);

        builder
            .HasOne(item => item.Customer)
            .WithOne(item => item.BillingDetails)
            .HasForeignKey<CustomerBillingDetails>(item => item.CustomerId);
    }
}
