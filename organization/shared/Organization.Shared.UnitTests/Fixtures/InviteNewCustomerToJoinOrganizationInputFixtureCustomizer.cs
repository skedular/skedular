using AutoFixture;
using Organization.Shared.Workflows.InviteToJoinOrganization;
using Testing.Shared;

namespace Organization.Shared.UnitTests.Fixtures;

public class InviteNewCustomerToJoinOrganizationInputFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture) =>
        fixture.Customize<InviteToJoinOrganizationInput>(composer => composer.With(item => item.IsNewCustomer, true));
}
