using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.MsTeams.V1;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Version;
using Grpc.Core;
using HotChocolate.Subscriptions;
using Version = Api.Shared.Services.Grpc.Skedular.MsTeams.V1.Version;

namespace MsTeams.Api.Grpc;

public class MsTeamsGrpcService(
    MsTeamsConfiguration msTeamsConfiguration,
    IVersionService versionService,
    ITopicEventSender topicEventSender,
    IGrpcAuthenticator grpcAuthenticator) : MsTeamsService.MsTeamsServiceBase
{
    public override async Task<RaiseGraphqlChangeResponse> RaiseGraphqlChange(RaiseGraphqlChangeInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(msTeamsConfiguration.ApiKey);

        await topicEventSender.SendAsync(request.TopicName, request.Id, context.CancellationToken);

        return new RaiseGraphqlChangeResponse();
    }

    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }
}
