using AutoFixture;
using Organization.Shared.Activities;

namespace Organization.Shared.UnitTests.Fixtures;

public class ActivitiesFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture)
    {
        fixture.Register(A.Fake<EmailIntegrations>);
        fixture.Register(A.Fake<InvitationIntegrations>);
    }
}
