using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TermsOfUse : EntityBaseWithDeleted
{
    public bool Active { get; set; }
    public string Terms { get; set; }

    public virtual ICollection<Organization> Organizations { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TermsOfUseConfiguration : IEntityTypeConfiguration<TermsOfUse>
{
    private static TermsOfUse[] SeedData =>
    [
        new()
        {
            Id = "VHzIH3DC09QJrOrCV-PnU",
            Active = true,
            Terms =
                "I verify that I am an authorized representative of this organization and have the right to act on its behalf in the creation and management of this page. The organization and I agree to the additional terms for Pages.",
        },
    ];

    public void Configure(EntityTypeBuilder<TermsOfUse> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Terms).HasMaxLength(Constants.MaxTermsOfUseLength);

        builder.HasData(SeedData.Select(item =>
        {
            item.CreatedAt = new DateTimeOffset(new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new TimeSpan(0, 0, 0, 0, 0));
            return item;
        }));

        builder.HasIndex(item => item.Active);
    }
}
