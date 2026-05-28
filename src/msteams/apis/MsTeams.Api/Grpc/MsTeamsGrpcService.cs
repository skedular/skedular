using Api.Shared.Grpc.Skedular.MsTeams.Core.V1;
using Enterprise.Shared.Version;
using Grpc.Core;
using Version = Api.Shared.Grpc.Skedular.MsTeams.Core.V1.Version;

namespace MsTeams.Api.Grpc;

public class MsTeamsGrpcService(IVersionService versionService) : MsTeamsService.MsTeamsServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }
}
