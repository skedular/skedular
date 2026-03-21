using Skedular.SystemTests.Skedular.GraphQL.V1;

namespace Skedular.SystemTests.Gateway.GraphQl;

[Trait(CategoryNames.Key, CategoryNames.System)]
[Collection("Gateway")]
public class GetVersionShould(IGetVersionQuery getVersionQuery)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await getVersionQuery.ExecuteAsync(cancellationToken);

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.BookingVersion.ShouldNotBeNull();
        result.Data.CoreVersion.ShouldNotBeNull();
        result.Data.CustomerVersion.ShouldNotBeNull();
        result.Data.LocationVersion.ShouldNotBeNull();
        result.Data.MarketplaceVersion.ShouldNotBeNull();
        result.Data.MsTeamsVersion.ShouldNotBeNull();
        result.Data.OrganizationVersion.ShouldNotBeNull();
        result.Data.SlackVersion.ShouldNotBeNull();
        result.Data.TeamVersion.ShouldNotBeNull();
    }
}
