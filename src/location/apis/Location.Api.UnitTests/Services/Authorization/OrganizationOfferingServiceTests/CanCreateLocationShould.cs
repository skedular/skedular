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
        [Frozen]
        ICachedOrganizationService cachedOrganizationService,
        [Frozen]
        IPricingEntitlementEvaluator pricingEntitlementEvaluator,
        [Frozen]
        ISpacesAccessEvaluator spacesAccessEvaluator,
        [Frozen]
        ILogger<OrganizationOfferingService> logger,
        OrganizationOfferingService sut,
        SpacesAccessEvaluator evaluator,
        CancellationToken cancellationToken)
    {
        var organization = new Organization
        {
            Id = "org-1",
            Offering = new Offering
            {
                Code = OfferingCode.PayAsYouGoV1,
            },
            Locations = [],
        };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync("org-1", null, cancellationToken))
            .Returns(organization);
        A.CallTo(() => pricingEntitlementEvaluator.EvaluateLocationCreation(organization.Offering, organization.Locations.Count))
            .Returns(new EntitlementDecision(true, EntitlementReasonCode.Allowed));
        A.CallTo(() => spacesAccessEvaluator.Evaluate(A<DateTimeOffset>._, organization.Offering, SpacesAccessAction.CreateOrModify))
            .Returns(evaluator.Evaluate(TimeProvider.System.GetUtcNow(), organization.Offering, SpacesAccessAction.CreateOrModify));

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
        [Frozen]
        ICachedOrganizationService cachedOrganizationService,
        [Frozen]
        IPricingEntitlementEvaluator pricingEntitlementEvaluator,
        [Frozen]
        ISpacesAccessEvaluator spacesAccessEvaluator,
        OrganizationOfferingService sut,
        SpacesAccessEvaluator evaluator,
        CancellationToken cancellationToken)
    {
        var organization = new Organization
        {
            Id = "org-1",
            Offering = new Offering
            {
                Code = OfferingCode.FreeTierV1,
            },
            Locations = [],
        };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync("org-1", null, cancellationToken))
            .Returns(organization);
        A.CallTo(() => pricingEntitlementEvaluator.EvaluateLocationCreation(organization.Offering, organization.Locations.Count))
            .Returns(new EntitlementDecision(false, EntitlementReasonCode.FreeLocationLimitReached));
        A.CallTo(() => spacesAccessEvaluator.Evaluate(A<DateTimeOffset>._, organization.Offering, SpacesAccessAction.CreateOrModify))
            .Returns(evaluator.Evaluate(TimeProvider.System.GetUtcNow(), organization.Offering, SpacesAccessAction.CreateOrModify));

        var result = await sut.CanCreateLocationAsync("org-1", cancellationToken);

        result.ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_False_At_Exact_Trial_Expiry_Before_Legacy_Entitlement_Check(
        [Frozen]
        ICachedOrganizationService cachedOrganizationService,
        [Frozen]
        IPricingEntitlementEvaluator pricingEntitlementEvaluator,
        [Frozen]
        ISpacesAccessEvaluator spacesAccessEvaluator,
        [Frozen]
        TimeProvider timeProvider,
        OrganizationOfferingService sut,
        SpacesAccessEvaluator evaluator,
        string organizationId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var offering = new Offering
        {
            Code = OfferingCode.SpacesFreeTierV1,
            SpacesProductEnabled = true,
            SpacesTrialStartedAt = now.AddDays(-14),
            SpacesTrialEndsAt = now,
        };
        var organization = new Organization
        {
            Id = organizationId,
            Offering = offering,
            Locations = [],
        };
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken))
            .Returns(organization);
        A.CallTo(() => spacesAccessEvaluator.Evaluate(now, offering, SpacesAccessAction.CreateOrModify))
            .Returns(evaluator.Evaluate(now, offering, SpacesAccessAction.CreateOrModify));

        var result = await sut.CanCreateLocationAsync(organizationId, cancellationToken);

        result.ShouldBeFalse();
        A.CallTo(() => pricingEntitlementEvaluator.EvaluateLocationCreation(A<Offering>._, A<int>._))
            .MustNotHaveHappened();
    }
}
