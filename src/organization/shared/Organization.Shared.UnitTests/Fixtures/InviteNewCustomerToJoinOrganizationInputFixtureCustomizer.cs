using AutoFixture;
using Organization.Shared.Workflows;

namespace Organization.Shared.UnitTests.Fixtures;

public class InviteNewCustomerToJoinOrganizationInputFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture) =>
        fixture.Customize<InviteToJoinOrganizationInput>(composer => composer.With(item => item.IsNewCustomer, true));
}
