using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationBankAccount : EntityBaseWithDeleted
{
    public bool IsDefault { get; set; }
    public string Name { get; set; }
    public string BankName { get; set; }
    public string AccountHolderName { get; set; }
    public string AccountNumber { get; set; }
    public string Country { get; set; }

    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationBankAccountConfiguration : IEntityTypeConfiguration<OrganizationBankAccount>
{
    public void Configure(EntityTypeBuilder<OrganizationBankAccount> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxOrganizationBankAccountNameLength);
        builder.Property(item => item.BankName).HasMaxLength(Constants.MaxOrganizationBankNameLength);
        builder.Property(item => item.AccountHolderName).HasMaxLength(Constants.MaxOrganizationAccountHolderNameLength);
        builder.Property(item => item.AccountNumber).HasMaxLength(Constants.MaxOrganizationAccountNumberLength);
        builder.Property(item => item.Country).HasMaxLength(Constants.MaxCountryLength);

        builder.HasOne(item => item.Organization).WithMany(item => item.OrganizationBankAccounts);

        builder.HasIndex(item => item.IsDefault);
        builder.HasIndex(item => item.Name);
    }
}
