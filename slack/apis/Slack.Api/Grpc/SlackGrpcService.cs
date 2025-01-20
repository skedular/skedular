using System.Reflection;
using Api.Shared.Services.Grpc.Skedular.Slack.V1;
using Enterprise.Shared.Grpc;
using Grpc.Core;
using Slack.Api.Mappers;
using Slack.Api.Services;
using Slack.Shared.Configurations;
using Version = Api.Shared.Services.Grpc.Skedular.Slack.V1.Version;

namespace Slack.Api.Grpc;

public class SlackGrpcService(
    SlackConfiguration slackConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    IWorkspaceService workspaceService,
    IMapper mapper) : SlackService.SlackServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }

    public override async Task<Workspace> Admin_AddWorkspace(Admin_AddWorkspaceInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(slackConfiguration.ApiKey);

        return mapper.MapTo(await workspaceService.AddAsync(mapper.MapTo(request), context.CancellationToken));
    }
}
