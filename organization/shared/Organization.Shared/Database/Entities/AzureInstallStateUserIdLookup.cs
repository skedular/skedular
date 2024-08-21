using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class AzureInstallStateUserIdLookup : EntityBase
{
    public string InstalledByUserId { get; set; } = string.Empty;
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class InstallStateUserIdLookupConfiguration : IEntityTypeConfiguration<AzureInstallStateUserIdLookup>
{
    public void Configure(EntityTypeBuilder<AzureInstallStateUserIdLookup> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.InstalledByUserId).HasMaxLength(Constants.MaxVerifiableTokenLength);
    }
}
