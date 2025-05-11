using Api.Shared.Services.Grpc.Skedular.Slack.V1;
using Enterprise.Shared.Version;
using Grpc.Core;
using Version = Api.Shared.Services.Grpc.Skedular.Slack.V1.Version;

namespace Slack.Api.Grpc;

public class SlackGrpcService(IVersionService versionService) : SlackService.SlackServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }
}
