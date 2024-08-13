using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MsTeams.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class InstallStateUserIdLookup : EntityBase
{
    public string InstalledByUserId { get; set; } = string.Empty;
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class InstallStateUserIdLookupConfiguration : IEntityTypeConfiguration<InstallStateUserIdLookup>
{
    public void Configure(EntityTypeBuilder<InstallStateUserIdLookup> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.InstalledByUserId).HasMaxLength(Constants.MaxVerifiableTokenLength);
    }
}
