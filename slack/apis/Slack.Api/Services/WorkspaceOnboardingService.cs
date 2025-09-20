using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Slack.Api.Mappers;
using Slack.Shared.Models;
using Slack.Shared.Publishers;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using Slack.Shared.Workflows.NewSlackWorkspaceJoined;
using Slack.Shared.Workflows.ReSyncSlackWorkspace;
using SlackNet.WebApi;
using Organization = Slack.Shared.Database.Entities.Organization;

namespace Slack.Api.Services;

public interface IWorkspaceOnboardingService
{
    public Task OnboardAsync(OauthV2AccessResponse oauthV2AccessResponse, CancellationToken cancellationToken);
}

public class WorkspaceOnboardingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IMapper mapper,
    ITemporalOutboxPublisher temporalOutboxPublisher,
    IOrganizationService organizationService,
    ILocationService locationService)
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

        temporalOutboxPublisher.StartWorkflowReSyncSlackWorkspace(new ReSyncSlackWorkspaceInput(workspace.Id, null), repositoryFactory.UnitOfWork);
        temporalOutboxPublisher.StartWorkflowNewSlackWorkspaceJoined(new NewSlackWorkspaceJoinedInput(workspace.Id), repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task CreateOrganizationAsync(string? name, Organization organization, CancellationToken cancellationToken) =>
        await organizationService.AdminAddAsync(new Shared.Models.Organization { Id = organization.Id, Name = name }, cancellationToken);

    private async Task CreateLocationAsync(string? name, Organization organization, CancellationToken cancellationToken)
    {
        var location = await repositoryFactory.LocationRepository.UpsertNakedAsync(randomHelper.Generate(), cancellationToken);

        _ = await locationService.AdminAddAsync(
            new Location
            {
                Id = location.Id, Name = $"{name.ToSafeString()} Office", Organization = new Shared.Models.Organization { Id = organization.Id }
            }, cancellationToken);
    }
}
