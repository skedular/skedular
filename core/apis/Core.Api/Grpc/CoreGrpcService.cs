using Api.Shared.Services.Grpc.Skedular.Core.V1;
using Enterprise.Shared.Version;
using Grpc.Core;
using Version = Api.Shared.Services.Grpc.Skedular.Core.V1.Version;

namespace Core.Api.Grpc;

public class CoreGrpcService(IVersionService versionService) : CoreService.CoreServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }
}
