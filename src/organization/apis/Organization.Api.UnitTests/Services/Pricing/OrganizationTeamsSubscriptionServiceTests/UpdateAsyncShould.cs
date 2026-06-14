using Organization.Api.Services.Pricing;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.UnitTests.Services.Pricing.OrganizationTeamsSubscriptionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Legacy_Early_Bird_For_Customer_Offering_Update(
        OrganizationTeamsSubscriptionService sut,
        CancellationToken cancellationToken) =>
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () =>
            await sut.UpdateAsync("organization-1", PricingCatalogSubscriptionPlanCode.LegacyEarlyBird, null, null, null, cancellationToken));

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Purchased_Capacity_For_Pay_As_You_Go(
        OrganizationTeamsSubscriptionService sut,
        CancellationToken cancellationToken) =>
        await Should.ThrowAsync<ArgumentException>(async () =>
            await sut.UpdateAsync("organization-1", PricingCatalogSubscriptionPlanCode.PayAsYouGo, 100, null, null, cancellationToken));

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Enterprise_Capacity_For_Customer_Offering_Update(
        OrganizationTeamsSubscriptionService sut,
        CancellationToken cancellationToken) =>
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () =>
            await sut.UpdateAsync("organization-1", PricingCatalogSubscriptionPlanCode.EnterpriseCapacity, 42, null, null, cancellationToken));
}
