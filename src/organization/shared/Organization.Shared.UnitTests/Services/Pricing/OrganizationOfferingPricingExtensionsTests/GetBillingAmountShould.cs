using Api.Shared.Services.Offering;
using Organization.Shared.Database.Entities;
using Organization.Shared.Models.PricingCatalog;
using Organization.Shared.Services.Pricing;

namespace Organization.Shared.UnitTests.Services.Pricing.OrganizationOfferingPricingExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetBillingAmountShould
{
    [Theory]
    [InlineData(4900, 0, 4900)]
    [InlineData(4900, 50, 2450)]
    [InlineData(4900, 100, 0)]
    public void Apply_Discount_To_Fixed_Price(int fixedPrice, int discountPercentage, int expectedAmount)
    {
        var organizationOffering = new OrganizationOffering
        {
            Code = OfferingCode.SpacesGrowthV1, FixedPrice = fixedPrice, UnitPrice = null, DiscountPercentage = discountPercentage
        };

        organizationOffering.GetBillingAmount().ShouldBe(expectedAmount);
    }

    [Fact]
    public void Apply_Discount_To_Unit_Price_Total()
    {
        var organizationOffering = new OrganizationOffering
        {
            Code = OfferingCode.PayAsYouGoV1,
            FixedPrice = null,
            UnitPrice = 300,
            DiscountPercentage = 25,
            OrganizationOfferingActiveMembers =
            [
                new OrganizationOfferingActiveMember(),
                new OrganizationOfferingActiveMember()
            ]
        };

        organizationOffering.GetBillingAmount().ShouldBe(450);
    }

    [Fact]
    public void Apply_Spaces_Catalog_Version_For_Spaces_Offering()
    {
        var organizationOffering = new OrganizationOffering();

        organizationOffering.ApplyOfferingTemplate(OfferingCode.SpacesGrowthV1);

        organizationOffering.CatalogVersion.ShouldBe(PricingCatalogConstants.CurrentSpacesCatalogVersion);
    }

    [Fact]
    public void Apply_Teams_Catalog_Version_For_Teams_Offering()
    {
        var organizationOffering = new OrganizationOffering();

        organizationOffering.ApplyOfferingTemplate(OfferingCode.PayAsYouGoV1);

        organizationOffering.CatalogVersion.ShouldBe(PricingCatalogConstants.CurrentTeamsCatalogVersion);
    }
}
