using Booking.Shared.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.CancellationDecisionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ResolveCustomerDecisionShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void RejectOverrideReasonWithoutManagementPermission(CancellationDecisionService sut) =>
        Should.Throw<UnauthorizedAccessException>(() => sut.ResolveCustomerDecision("customer-1", "organization-1", false, "forged reason"));

    [Theory]
    [AutoFakeItEasyData]
    public void ClassifyAdministratorWithManagementPermission(CancellationDecisionService sut)
    {
        var result = sut.ResolveCustomerDecision("operator-1", "organization-1", true, "Operator approved it.");

        result.Actor.Category.ShouldBe(CancellationActorCategory.Administrator);
        result.Actor.OrganizationId.ShouldBe("organization-1");
        result.CanOverridePolicy.ShouldBeTrue();
        result.HasOverrideReason.ShouldBeTrue();
    }
}
