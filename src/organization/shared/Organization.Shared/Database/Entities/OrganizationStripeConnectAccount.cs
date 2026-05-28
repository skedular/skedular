using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationStripeConnectAccount : EntityBaseWithDeleted
{
    public bool IsDefault { get; set; }
    public string StripeAccountId { get; set; }
    public string Name { get; set; }
    public bool ChargesEnabled { get; set; }
    public bool PayoutsEnabled { get; set; }
    public string Type { get; set; }
    public string? Country { get; set; }
    public string? DefaultCurrency { get; set; }
    public string? BusinessType { get; set; }
    public string? CompanyName { get; set; }
    public string? Url { get; set; }
    public string? SupportUrl { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool DetailsSubmitted { get; set; }
    public string CapabilitiesCardPayments { get; set; }
    public string CapabilitiesTransfers { get; set; }
    public string OnboardingUrl { get; set; }

    public virtual Organization Organization { get; set; }
    public virtual ICollection<OrganizationStripeConnectAccountRefreshCode> OrganizationStripeConnectAccountRefreshCodes { get; set; } = [];
    public virtual OrganizationStripeConnectAccountAuthorization? OrganizationStripeConnectAccountAuthorization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripeConnectAccountConfiguration : IEntityTypeConfiguration<OrganizationStripeConnectAccount>
{
    public void Configure(EntityTypeBuilder<OrganizationStripeConnectAccount> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.StripeAccountId).HasMaxLength(Constants.MaxStripeConnectAccountIdLength);
        builder.Property(item => item.Name).HasMaxLength(Constants.MaxStripeConnectAccountNameLength);
        builder.Property(item => item.ChargesEnabled).HasDefaultValue(false);
        builder.Property(item => item.PayoutsEnabled).HasDefaultValue(false);
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxStripeConnectAccountTypeLength);
        builder.Property(item => item.Country).HasMaxLength(Constants.MaxCountryLength);
        builder.Property(item => item.DefaultCurrency).HasMaxLength(Constants.MaxStripeCurrencyLength);
        builder.Property(item => item.BusinessType).HasMaxLength(Constants.MaxStripeBusinessTypeLength);
        builder.Property(item => item.CompanyName).HasMaxLength(Constants.MaxStripeConnectAccountCompanyNameLength);
        builder.Property(item => item.Url).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.SupportUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.ContactEmail).HasMaxLength(Constants.MaxEmailLength);
        builder.Property(item => item.ContactPhone).HasMaxLength(Constants.MaxPhoneNumberLength);
        builder.Property(item => item.DetailsSubmitted).HasDefaultValue(false);
        builder.Property(item => item.CapabilitiesCardPayments).HasMaxLength(Constants.MaxStripeCapabilitiesStatusLength);
        builder.Property(item => item.CapabilitiesTransfers).HasMaxLength(Constants.MaxStripeCapabilitiesStatusLength);
        builder.Property(item => item.OnboardingUrl).HasMaxLength(Constants.MaxUrlLength);

        builder.HasOne(item => item.Organization).WithMany(item => item.OrganizationStripeConnectAccounts);

        builder.HasIndex(item => item.IsDefault);
        builder.HasIndex(item => item.StripeAccountId);
        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.ChargesEnabled);
        builder.HasIndex(item => item.PayoutsEnabled);
        builder.HasIndex(item => item.Type);
        builder.HasIndex(item => item.Country);
        builder.HasIndex(item => item.DefaultCurrency);
        builder.HasIndex(item => item.DetailsSubmitted);
        builder.HasIndex(item => item.CapabilitiesTransfers);
        builder.HasIndex(item => item.CapabilitiesCardPayments);
    }
}
