using Api.Shared.Clients.OpenApi.Skedular.Booking.V1;
using Api.Shared.Clients.OpenApi.Skedular.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.Customer.V1;
using Api.Shared.Clients.OpenApi.Skedular.Location.V1;
using Api.Shared.Clients.OpenApi.Skedular.Marketplace.V1;
using Api.Shared.Clients.OpenApi.Skedular.MsTeams.V1;
using Api.Shared.Clients.OpenApi.Skedular.Organization.V1;
using Api.Shared.Clients.OpenApi.Skedular.Slack.V1;
using Api.Shared.Clients.OpenApi.Skedular.Team.V1;
using Shouldly;
using Testing.Shared;

namespace Skedular.SystemTests.Gateway.Rest.ReverseProxy;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Gateway")]
public class GetVersionShould(
    IBookingClient bookingClient,
    ICoreClient coreClient,
    ICustomerClient customerClient,
    ILocationClient locationClient,
    IMarketplaceClient marketplaceClient,
    IMsTeamsClient msTeamsClient,
    IOrganizationClient organizationClient,
    ISlackClient slackClient,
    ITeamClient teamClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Booking_Version(CancellationToken cancellationToken)
    {
        var result = await bookingClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Core_Version(CancellationToken cancellationToken)
    {
        var result = await coreClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Customer_Version(CancellationToken cancellationToken)
    {
        var result = await customerClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Location_Version(CancellationToken cancellationToken)
    {
        var result = await locationClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Marketplace_Version(CancellationToken cancellationToken)
    {
        var result = await marketplaceClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_MsTeams_Version(CancellationToken cancellationToken)
    {
        var result = await msTeamsClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Organization_Version(CancellationToken cancellationToken)
    {
        var result = await organizationClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Slack_Version(CancellationToken cancellationToken)
    {
        var result = await slackClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Team_Version(CancellationToken cancellationToken)
    {
        var result = await teamClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }
}
