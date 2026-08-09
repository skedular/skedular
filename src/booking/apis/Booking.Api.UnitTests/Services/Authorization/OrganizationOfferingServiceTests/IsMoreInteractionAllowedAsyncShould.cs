using Api.Shared.Services.Offering;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Services.Cache;
using Offering = Api.Shared.Services.Models.Offering;

namespace Booking.Api.UnitTests.Services.Authorization.OrganizationOfferingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class IsMoreInteractionAllowedAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_True_When_Organization_Offering_Is_Unlimited(
        [Frozen]
        ICachedOrganizationService cachedOrganizationService,
        [Frozen]
        IPricingEntitlementEvaluator pricingEntitlementEvaluator,
        OrganizationOfferingService sut,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organization = new Organization
        {
            Id = organizationId,
            Offering = new Offering
            {
                Code = OfferingCode.PayAsYouGoV1,
                ActiveCustomerIds = [],
            },
        };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken)).Returns(organization);
        A.CallTo(() => pricingEntitlementEvaluator.EvaluateActiveUser(organization.Offering, customerId))
            .Returns(new EntitlementDecision(true, EntitlementReasonCode.Allowed));

        var result = await sut.IsMoreInteractionAllowedAsync(organizationId, customerId, cancellationToken);

        result.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_True_When_Customer_Is_Already_Active_In_A_Full_Offering(
        [Frozen]
        ICachedOrganizationService cachedOrganizationService,
        [Frozen]
        IPricingEntitlementEvaluator pricingEntitlementEvaluator,
        OrganizationOfferingService sut,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organization = new Organization
        {
            Id = organizationId,
            Offering = new Offering
            {
                Code = OfferingCode.FreeTierV1,
                ActiveCustomerIds = [customerId],
            },
        };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken)).Returns(organization);
        A.CallTo(() => pricingEntitlementEvaluator.EvaluateActiveUser(organization.Offering, customerId))
            .Returns(new EntitlementDecision(true, EntitlementReasonCode.Allowed));

        var result = await sut.IsMoreInteractionAllowedAsync(organizationId, customerId, cancellationToken);

        result.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_False_When_Max_User_Count_Is_Reached_For_Another_Customer(
        [Frozen]
        ICachedOrganizationService cachedOrganizationService,
        [Frozen]
        IPricingEntitlementEvaluator pricingEntitlementEvaluator,
        OrganizationOfferingService sut,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organization = new Organization
        {
            Id = organizationId,
            Offering = new Offering
            {
                Code = OfferingCode.FreeTierV1,
                ActiveCustomerIds = [.. Enumerable.Range(0, 11).Select(index => $"customer-{index}")],
            },
        };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken)).Returns(organization);
        A.CallTo(() => pricingEntitlementEvaluator.EvaluateActiveUser(organization.Offering, customerId))
            .Returns(new EntitlementDecision(false, EntitlementReasonCode.FreeActiveUserLimitReached));

        var result = await sut.IsMoreInteractionAllowedAsync(organizationId, customerId, cancellationToken);

        result.ShouldBeFalse();
    }
}
