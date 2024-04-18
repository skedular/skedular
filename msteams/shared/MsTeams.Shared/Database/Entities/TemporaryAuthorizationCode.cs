using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MsTeams.Shared.Database.Entities;

public class TemporaryAuthorizationCode : EntityBase;

public class TemporaryAuthorizationCodeConfiguration : IEntityTypeConfiguration<TemporaryAuthorizationCode>
{
    public void Configure(EntityTypeBuilder<TemporaryAuthorizationCode> builder) => builder.ConfigureEntityBase();
}
