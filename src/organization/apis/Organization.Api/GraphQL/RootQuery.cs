using Api.Shared.Services.Models;
using Enterprise.Shared.Version;
using HotChocolate.Types;
using Organization.Api.GraphQL.Tag;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Organization.Api.GraphQL;

[QueryType]
public class RootQuery(IVersionService versionService)
{
    [UseResolverScope]
    public Version OrganizationVersion()
    {
        var version = versionService.GetVersion();

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public int BookingSlotSizeInMinutes() => OpeningHoursDetails.BookingSlotSizeInMinutes;

    [UseResolverScope]
    public IEnumerable<OrganizationTagTypeDetails> ResourceTypes() =>
        OrganizationTagTypeConstants.ResourceTypes.Select(item =>
            new OrganizationTagTypeDetails { Type = item, Name = item.ToOrganizationTagTypeName() });

    [UseResolverScope]
    public IEnumerable<OrganizationTagTypeDetails> LocationSpaceTypes() =>
        OrganizationTagTypeConstants.LocationSpaceTypes.Select(item =>
            new OrganizationTagTypeDetails { Type = item, Name = item.ToOrganizationTagTypeName() });

    [UseResolverScope]
    public IEnumerable<string> EmailsToShowLatestCapabilities() => ["morteza.alizadeh@gmail.com", "leila.alavi78@gmail.com"];

    [UseResolverScope]
    public IEnumerable<string> EmailsToIgnoreObservability() => ["morteza.alizadeh@gmail.com", "leila.alavi78@gmail.com"];
}
