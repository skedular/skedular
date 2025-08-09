using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Slack.Api.Mappers;
using Slack.Shared.Publishers;
using Slack.Shared.Repositories;
using SlackNet.WebApi;
using Admin_AddInput = Api.Shared.Services.Grpc.Skedular.Organization.V1.Admin_AddInput;
using LocationConfiguration = Api.Shared.Clients.Configurations.Grpc.LocationConfiguration;
using Organization = Slack.Shared.Database.Entities.Organization;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;

namespace Slack.Api.Services;

public interface IWorkspaceOnboardingService
{
    public Task OnboardAsync(OauthV2AccessResponse oauthV2AccessResponse, CancellationToken cancellationToken);
}

public class WorkspaceOnboardingService(
    OrganizationConfiguration organizationConfiguration,
    LocationConfiguration locationConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IMapper mapper,
    ISlackInternalOutboxPublisher slackInternalOutboxPublisher,
    ITemporalOutboxPublisher temporalOutboxPublisher,
    global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient
        organizationServiceClient,
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService.LocationServiceClient locationServiceClient)
    : IWorkspaceOnboardingService
{
    public async Task OnboardAsync(OauthV2AccessResponse oauthV2AccessResponse, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(oauthV2AccessResponse.Team);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(randomHelper.Generate(), cancellationToken);
        var exitingWorkspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(oauthV2AccessResponse.Team!.Id, cancellationToken);
        var workspace = exitingWorkspace is null
            ? repositoryFactory.WorkspaceRepository.Add(mapper.MapTo(oauthV2AccessResponse, organization))
            : repositoryFactory.WorkspaceRepository.Update(mapper.MergeTo(oauthV2AccessResponse, exitingWorkspace, organization));

        await CreateOrganizationAsync(oauthV2AccessResponse.Team.Name, organization, cancellationToken);
        await CreateLocationAsync(oauthV2AccessResponse.Team.Name, organization, cancellationToken);

        slackInternalOutboxPublisher.PublishRefreshWorkspaceMembers([workspace.Id], repositoryFactory.UnitOfWork);
        slackInternalOutboxPublisher.PublishRefreshWorkspaceChannels([workspace.Id], repositoryFactory.UnitOfWork);
        temporalOutboxPublisher.StartWorkflowNewSlackWorkspaceJoined(workspace.Id, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task CreateOrganizationAsync(string? name, Organization organization, CancellationToken cancellationToken)
    {
        var activeTermsOfUse = await organizationServiceClient.GetActiveOrganizationTermsOfUseAsync(
            new GetActiveOrganizationTermsOfUseInput(),
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        await organizationServiceClient.Admin_AddAsync(
            new Admin_AddInput
            {
                Id = organization.Id,
                Name = name.ToSafeString(),
                AgreedToTermsOfUse = true,
                TermsOfUseId = activeTermsOfUse.Id,
                Type = OrganizationTypeConstants.Private,
                MemberVisibilityPolicy = OrganizationMemberVisibilityPolicyConstants.FullAccess
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
    }

    private async Task CreateLocationAsync(string? name, Organization organization, CancellationToken cancellationToken)
    {
        var location = await repositoryFactory.LocationRepository.UpsertNakedAsync(randomHelper.Generate(), cancellationToken);

        await locationServiceClient.Admin_AddAsync(
            new global::Api.Shared.Services.Grpc.Skedular.Location.V1.Admin_AddInput
            {
                Id = location.Id, Name = $"{name.ToSafeString()} Office", OrganizationId = organization.Id
            },
            locationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
    }
}
