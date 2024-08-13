using Api.Shared.Services.Grpc.UnityHub.Location.V1;
using Api.Shared.Services.Grpc.UnityHub.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using MsTeams.Shared.Database.Entities;
using MsTeams.Shared.Publishers;
using MsTeams.Shared.Repositories;
using OrganizationConfiguration = MsTeams.Shared.Configurations.OrganizationConfiguration;
using LocationConfiguration = MsTeams.Shared.Configurations.LocationConfiguration;
using Admin_AddInput = Api.Shared.Services.Grpc.UnityHub.Organization.V1.Admin_AddInput;
using Location = MsTeams.Shared.Database.Entities.Location;
using Organization = MsTeams.Shared.Database.Entities.Organization;

namespace MsTeams.Api.Services;

public interface ITenantOnboardingService
{
    Task OnboardAsync(
        string tenantId,
        InstallStateUserIdLookup installStateUserIdLookup,
        CancellationToken cancellationToken);
}

public class TenantOnboardingService(
    OrganizationConfiguration organizationConfiguration,
    LocationConfiguration locationConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IMsTeamsInternalOutboxPublisher msTeamsInternalOutboxPublisher,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    LocationService.LocationServiceClient locationServiceClient) : ITenantOnboardingService
{
    public async Task OnboardAsync(
        string tenantId,
        InstallStateUserIdLookup installStateUserIdLookup,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(
                repositoryFactory.OrganizationRepository.UnitOfWork,
                cancellationToken);

        var organization =
            await repositoryFactory.OrganizationRepository.UpsertNakedAsync(randomHelper.Generate(), cancellationToken);

        var tenant = repositoryFactory.TenantRepository.Add(new Tenant
        {
            Id = tenantId,
            InstalledByUserId = installStateUserIdLookup.InstalledByUserId,
            Organization = organization
        });

        repositoryFactory.InstallStateUserIdLookupRepository.Remove(installStateUserIdLookup);

        await Task.WhenAll([
            CreateOrganizationAsync("No name set!!!", organization, cancellationToken),
            CreateLocationAsync(organization, cancellationToken)
        ]);

        await msTeamsInternalOutboxPublisher.PublishRefreshTenantMembersAsync(
            [tenant.Id],
            repositoryFactory.TenantRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.TenantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<Organization> CreateOrganizationAsync(
        string? name,
        Organization organization,
        CancellationToken cancellationToken)
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
                TermsOfUseId = activeTermsOfUse.Id
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        return organization;
    }

    private async Task<Location> CreateLocationAsync(
        Organization organization,
        CancellationToken cancellationToken)
    {
        var location =
            await repositoryFactory.LocationRepository.UpsertNakedAsync(randomHelper.Generate(), cancellationToken);

        await locationServiceClient.Admin_AddAsync(
            new global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Admin_AddInput
            {
                Id = location.Id, Name = "Office", OrganizationId = organization.Id
            },
            locationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        return location;
    }
}
