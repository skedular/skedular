using AutoFixture;
using Organization.Shared.Workflows;
using Testing.Shared;

namespace Organization.Shared.UnitTests.Fixtures;

public class InviteExistingCustomerToJoinOrganizationInputFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture) =>
        fixture.Customize<InviteToJoinOrganizationInput>(composer => composer.With(item => item.IsNewCustomer, false));
}
