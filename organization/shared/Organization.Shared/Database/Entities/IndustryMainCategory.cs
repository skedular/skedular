using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class IndustryMainCategory : EntityBaseWithDeleted
{
    public string Name { get; set; }
    public virtual ICollection<IndustrySubCategory> IndustrySubCategories { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class IndustryMainCategoryConfiguration : IEntityTypeConfiguration<IndustryMainCategory>
{
    public static IndustryMainCategory[] SeedData { get; } =
    [
        new IndustryMainCategory { Id = "882bUq1BWqJecAZxMOr51", Name = "Agriculture" },
        new IndustryMainCategory { Id = "0NXImArR8CXFDC9SmmwFn", Name = "Arts" },
        new IndustryMainCategory { Id = "pXKhQk06h0DDf6cYf93C1", Name = "Construction" },
        new IndustryMainCategory { Id = "lg-BOyEpbyAi_AGt3EeNX", Name = "Consumer Goods" },
        new IndustryMainCategory { Id = "xkjX-i2E2Bc6tH2KjCaTu", Name = "Corporate Services" },
        new IndustryMainCategory { Id = "S1mxU6bv5ktRVVIN3AA4K", Name = "Design" },
        new IndustryMainCategory { Id = "wzCmjl5D_n22GAmJquRWB", Name = "Education" },
        new IndustryMainCategory { Id = "kamIaBPmTt1gZCRjqTlvG", Name = "Energy & Mining" },
        new IndustryMainCategory { Id = "-PfBGjlmBqLSUhkj5HGfP", Name = "Entertainment" },
        new IndustryMainCategory { Id = "zwanHBU5wvwbQrGspAXTb", Name = "Finance" },
        new IndustryMainCategory { Id = "s3_JhMKyBezxJzRJq9BO0", Name = "Hardware & Networking" },
        new IndustryMainCategory { Id = "5y-GA2lrc3pk5fHG-3YIy", Name = "Health Care" },
        new IndustryMainCategory { Id = "zLuSwB4G_EuG4YueixLF0", Name = "Legal" },
        new IndustryMainCategory { Id = "LFaZVLT6kUWs-N_tIKdvv", Name = "Manufacturing" },
        new IndustryMainCategory { Id = "gWWnxzMaGrBIp5JsKqTUV", Name = "Media & Communications" },
        new IndustryMainCategory { Id = "620m_qu0dee49rW0104aI", Name = "Nonprofit" },
        new IndustryMainCategory { Id = "08-giYmx7ja5wepmU10j5", Name = "Public Administration" },
        new IndustryMainCategory { Id = "hAxKDrJiJmHK__0M_ewMu", Name = "Public Safety" },
        new IndustryMainCategory { Id = "6pu5HDPw5APjFvcT-eL0Q", Name = "Real Estate" },
        new IndustryMainCategory { Id = "xY4RDCWRG5G2fOEdNXPng", Name = "Recreation & Travel" },
        new IndustryMainCategory { Id = "vS4OynyP3n3kjc3l-bmGS", Name = "Retail" },
        new IndustryMainCategory { Id = "08ILcal4_is07nQlMRtae", Name = "Software & IT Services" },
        new IndustryMainCategory { Id = "9zzrzqbocNXiv9_OLRgtE", Name = "Transportation & Logistics" },
        new IndustryMainCategory { Id = "eO9IbE_ssHvels5sLHtob", Name = "Wellness & Fitness" }
    ];

    public void Configure(EntityTypeBuilder<IndustryMainCategory> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxOrganizationIndustryMainCategoryNameLength);

        builder.HasData(SeedData.Select(item =>
        {
            item.CreatedAt = new DateTimeOffset(new DateTime(2024, 5, 1));
            return item;
        }));
    }
}
