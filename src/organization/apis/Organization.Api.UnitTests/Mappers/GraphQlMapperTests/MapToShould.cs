using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Organization.Api.GraphQL.Organization;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Shared.Models;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.UnitTests.Mappers.GraphQlMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapToShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Preserve_Selected_Fields_And_Explicit_Clear_Values(
        GraphQlMapper sut,
        string id,
        string customDomain,
        string ignoredName)
    {
        var input = new UpdateOrganizationInput
        {
            Id = id,
            CustomDomain = customDomain,
            FieldsToUpdate =
            [
                OrganizationPatchField.Website,
                OrganizationPatchField.ContactPhone,
                OrganizationPatchField.BillingCycle,
            ],
            Name = ignoredName,
            Website = string.Empty,
            ContactPhone = null,
            BillingCycle = OrganizationBillingCycle.Monthly,
        };

        var result = sut.MapTo(input);

        result.Id.ShouldBe(input.Id);
        result.CustomDomain.ShouldBe(input.CustomDomain);
        result.FieldsToUpdate.ShouldBe(input.FieldsToUpdate.ToHashSet());
        result.Name.ShouldBe(input.Name);
        result.Website.ShouldBe(string.Empty);
        result.ContactPhone.ShouldBeNull();
        result.BillingCycle.ShouldBe(input.BillingCycle);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Only_Teams_Available_Offerings_For_Private_Organization(GraphQlMapper sut)
    {
        var organization = new Shared.Models.Organization
        {
            Id = "org-1",
            Name = "Private organization",
            Type = OrganizationType.Private,
            OrganizationOfferings =
            [
                new OrganizationOffering
                {
                    Id = "offering-1",
                    Code = OfferingCode.FreeTierV1,
                    Currency = Currency.Usd,
                },
            ],
        };

        var result = sut.MapTo(organization);

        result.ShouldNotBeNull();
        result.AvailableOfferings.Select(item => item.Code).ShouldBe([
            OfferingCode.PayAsYouGoV1.ToOfferingCode(),
            OfferingCode.EnterpriseCustomV1.ToOfferingCode(),
        ]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Only_Spaces_Available_Offerings_For_Marketplace_Organization(GraphQlMapper sut)
    {
        var organization = new Shared.Models.Organization
        {
            Id = "org-1",
            Name = "Marketplace organization",
            Type = OrganizationType.Marketplace,
            OrganizationOfferings =
            [
                new OrganizationOffering
                {
                    Id = "offering-1",
                    Code = OfferingCode.SpacesFreeTierV1,
                    Currency = Currency.Usd,
                },
            ],
        };

        var result = sut.MapTo(organization);

        result.ShouldNotBeNull();
        result.AvailableOfferings.Select(item => item.Code).ShouldBe([
            OfferingCode.SpacesGrowthV1.ToOfferingCode(),
            OfferingCode.SpacesBusinessV1.ToOfferingCode(),
            OfferingCode.SpacesContactUsV1.ToOfferingCode(),
        ]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Ignore_Deleted_Spaces_Offering_When_Mapping_Current_Spaces_Subscription(GraphQlMapper sut)
    {
        var deletedFreeOffering = new Shared.Database.Entities.OrganizationOffering
        {
            Id = "offering-free",
            Code = OfferingCode.SpacesFreeTierV1,
            Currency = Currency.Usd.ToCurrency(),
            DeletedAt = new DateTimeOffset(2026, 6, 22, 20, 55, 0, TimeSpan.Zero),
        };
        var activeGrowthOffering = new Shared.Database.Entities.OrganizationOffering
        {
            Id = "offering-growth",
            Code = OfferingCode.SpacesGrowthV1,
            Currency = Currency.Usd.ToCurrency(),
            PurchasedTeamCapacity = 500,
        };
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            Name = "Marketplace organization",
            Type = OrganizationTypeConstants.Marketplace,
            BillingCycle = OrganizationBillingCycleConstants.Monthly,
            OrganizationOfferings = [deletedFreeOffering, activeGrowthOffering],
        };
        deletedFreeOffering.Organization = organization;
        activeGrowthOffering.Organization = organization;

        var result = sut.MapTo(organization, new Uri("https://example.test/connect"));

        result.OrganizationSpacesSubscription.ShouldNotBeNull();
        result.OrganizationSpacesSubscription.Id.ShouldBe(activeGrowthOffering.Id);
        result.OrganizationSpacesSubscription.PlanCode.ShouldBe(PricingCatalogSubscriptionPlanCode.Growth);
    }
}
