using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Organization.Shared.Mappers;
using Organization.Shared.Models;

namespace Organization.Shared.UnitTests.Mappers.EventMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapOrganizationOfferingShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void IncludeHostCommissionFromActiveOffering(
        string organizationId,
        string offeringId,
        decimal commissionPercentage,
        DateTimeOffset now,
        EventMapper sut)
    {
        var organization = new Models.Organization
        {
            Id = organizationId,
            Name = "Host organization",
            Type = OrganizationType.Host,
            BillingCycle = OrganizationBillingCycle.Monthly,
            OrganizationOfferings =
            [
                new OrganizationOffering
                {
                    Id = offeringId,
                    Code = OfferingCode.HostStandardV1,
                    Start = now,
                    End = now.AddMonths(1),
                    Currency = Currency.Usd,
                    HostCommissionPercentage = commissionPercentage
                }
            ]
        };

        var result = sut.MapTo(organization);

        result.Offering.HostCommissionPercentage.ShouldBe(decimal.ToDouble(commissionPercentage));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Include_Spaces_Trial_And_Billing_Fields(
        string organizationId,
        string offeringId,
        EventMapper sut)
    {
        var trialStartedAt = new DateTimeOffset(2026, 7, 1, 10, 0, 0, TimeSpan.Zero);
        var billingStartsAt = new DateTimeOffset(2026, 8, 1, 0, 0, 0, TimeSpan.Zero);
        var organization = new Models.Organization
        {
            Id = organizationId,
            Name = "Spaces organization",
            Type = OrganizationType.Marketplace,
            BillingCycle = OrganizationBillingCycle.Monthly,
            SpacesTrialStartedAt = trialStartedAt,
            OrganizationOfferings =
            [
                new OrganizationOffering
                {
                    Id = offeringId,
                    Code = OfferingCode.SpacesGrowthV1,
                    Start = trialStartedAt,
                    End = billingStartsAt,
                    Currency = Currency.Usd,
                    SpacesBillingStartsAt = billingStartsAt
                }
            ]
        };

        var result = sut.MapTo(organization).Offering;

        result.SpacesProductEnabled.ShouldBeTrue();
        result.SpacesTrialStartedAt.ToDateTimeOffset().ShouldBe(trialStartedAt);
        result.SpacesTrialEndsAt.ToDateTimeOffset().ShouldBe(trialStartedAt.AddDays(14));
        result.SpacesNextBillingAt.ToDateTimeOffset().ShouldBe(billingStartsAt);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Omit_Trial_Fields_For_Teams_Organization(
        string organizationId,
        string offeringId,
        EventMapper sut)
    {
        var now = new DateTimeOffset(2026, 7, 1, 10, 0, 0, TimeSpan.Zero);
        var organization = new Models.Organization
        {
            Id = organizationId,
            Name = "Teams organization",
            Type = OrganizationType.Private,
            BillingCycle = OrganizationBillingCycle.Monthly,
            OrganizationOfferings =
            [
                new OrganizationOffering
                {
                    Id = offeringId,
                    Code = OfferingCode.PayAsYouGoV1,
                    Start = now,
                    End = now.AddMonths(1),
                    Currency = Currency.Usd
                }
            ]
        };

        var result = sut.MapTo(organization).Offering;

        result.SpacesProductEnabled.ShouldBeFalse();
        result.SpacesTrialStartedAt.ShouldBeNull();
        result.SpacesTrialEndsAt.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_Organization_Creation_As_Existing_V1_Free_Trial_Fallback(
        string organizationId,
        string offeringId,
        DateTimeOffset organizationCreatedAt,
        EventMapper sut)
    {
        var organization = new Models.Organization
        {
            Id = organizationId,
            CreatedAt = organizationCreatedAt,
            Name = "Existing Spaces organization",
            Type = OrganizationType.Marketplace,
            BillingCycle = OrganizationBillingCycle.Monthly,
            SpacesTrialStartedAt = null,
            OrganizationOfferings =
            [
                new OrganizationOffering
                {
                    Id = offeringId,
                    Code = OfferingCode.SpacesFreeTierV1,
                    Start = organizationCreatedAt,
                    End = organizationCreatedAt.AddMonths(1),
                    Currency = Currency.Usd
                }
            ]
        };

        var result = sut.MapTo(organization).Offering;

        result.Code.ShouldBe(OfferingCode.SpacesFreeTierV1.ToOfferingCode());
        result.SpacesTrialStartedAt.ToDateTimeOffset().ShouldBe(organizationCreatedAt);
        result.SpacesTrialEndsAt.ToDateTimeOffset().ShouldBe(organizationCreatedAt.AddDays(14));
    }
}
