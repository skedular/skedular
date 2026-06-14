using Api.Shared.Services.Offering;
using Location.Api.Services.Authorization;
using Location.Shared.Services.Cache;
using Microsoft.Extensions.Logging;
using Offering = Api.Shared.Services.Models.Offering;
using Organization = Location.Shared.Database.Entities.Organization;

namespace Location.Api.UnitTests.Services.Authorization.OrganizationOfferingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CanCreateLocationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_True_When_Entitlement_Allows_Location_Creation(
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IPricingEntitlementEvaluator pricingEntitlementEvaluator,
        [Frozen] ILogger<OrganizationOfferingService> logger,
        OrganizationOfferingService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Organization { Id = "org-1", Offering = new Offering { Code = OfferingCode.PayAsYouGoV1 }, Locations = [] };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync("org-1", null, cancellationToken))
            .Returns(organization);
        A.CallTo(() => pricingEntitlementEvaluator.EvaluateLocationCreation(organization.Offering, organization.Locations.Count))
            .Returns(new EntitlementDecision(true, EntitlementReasonCode.Allowed));

        var result = await sut.CanCreateLocationAsync("org-1", cancellationToken);

        result.ShouldBeTrue();
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_False_When_Entitlement_Denies_Location_Creation(
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IPricingEntitlementEvaluator pricingEntitlementEvaluator,
        OrganizationOfferingService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Organization { Id = "org-1", Offering = new Offering { Code = OfferingCode.FreeTierV1 }, Locations = [] };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync("org-1", null, cancellationToken))
            .Returns(organization);
        A.CallTo(() => pricingEntitlementEvaluator.EvaluateLocationCreation(organization.Offering, organization.Locations.Count))
            .Returns(new EntitlementDecision(false, EntitlementReasonCode.FreeLocationLimitReached));

        var result = await sut.CanCreateLocationAsync("org-1", cancellationToken);

        result.ShouldBeFalse();
    }
}
