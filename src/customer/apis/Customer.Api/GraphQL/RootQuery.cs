using Customer.Api.Services;
using Enterprise.Shared.Version;
using HotChocolate;
using HotChocolate.Types;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Customer.Api.GraphQL;

[QueryType]
public class RootQuery(IVersionService versionService)
{
    public Version CustomerVersion()
    {
        var version = versionService.GetVersion();

        return new Version
        {
            Major = version.Major,
            Minor = version.Minor,
            Build = version.Build,
            Revision = version.Revision,
        };
    }

    [UseResolverScope]
    public async Task<bool> CustomerReadinessSyncedAsync(
        [Service]
        ICustomerReadinessAccessService customerReadinessAccessService,
        CancellationToken cancellationToken) =>
        await customerReadinessAccessService.IsReadyAsync(cancellationToken);
}
