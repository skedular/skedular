using Booking.Api.Mappers;
using Booking.Api.Services;
using Enterprise.Shared.Version;
using HotChocolate;
using HotChocolate.Types;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Booking.Api.GraphQL;

[QueryType]
public class RootQuery(IMapper mapper, IVersionService versionService)
{
    [UseResolverScope]
    public Version BookingVersion()
    {
        var version = versionService.GetVersion();

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> BookingCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);
}
