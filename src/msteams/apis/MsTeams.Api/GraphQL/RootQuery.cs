using Enterprise.Shared.Version;
using HotChocolate.Types;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace MsTeams.Api.GraphQL;

[QueryType]
public class RootQuery(IVersionService versionService)
{
    public Version MsTeamsVersion()
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
}
