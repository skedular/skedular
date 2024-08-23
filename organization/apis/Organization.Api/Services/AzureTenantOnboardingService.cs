using Api.Shared.Services.Grpc.UnityHub.Location.V1;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using LocationConfiguration = Organization.Shared.Configurations.LocationConfiguration;
using Location = Organization.Shared.Database.Entities.Location;

namespace Organization.Api.Services;

public interface IAzureTenantOnboardingService
{
    Task OnboardAsync(
        string tenantId,
        AzureInstallStateUserIdLookup azureInstallStateUserIdLookup,
        CancellationToken cancellationToken);
}

public class AzureTenantOnboardingService(
    LocationConfiguration locationConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationInternalOutboxPublisher organizationInternalOutboxPublisher,
    IOrganizationTermsOfUseService organizationTermsOfUseService,
    IOrganizationService organizationService,
    LocationService.LocationServiceClient locationServiceClient) : IAzureTenantOnboardingService
{
    public async Task OnboardAsync(
        string tenantId,
        AzureInstallStateUserIdLookup azureInstallStateUserIdLookup,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(
                repositoryFactory.OrganizationRepository.UnitOfWork,
                cancellationToken);

        var organization =
            await repositoryFactory.OrganizationRepository.UpsertNakedAsync(randomHelper.Generate(), cancellationToken);

        var tenant = repositoryFactory.AzureTenantRepository.Add(new AzureTenant
        {
            Id = tenantId,
            InstalledByUserId = azureInstallStateUserIdLookup.InstalledByUserId,
            Organization = organization
        });

        repositoryFactory.AzureInstallStateUserIdLookupRepository.Remove(azureInstallStateUserIdLookup);

        await Task.WhenAll(CreateOrganizationAsync("No name set!!!", organization, cancellationToken),
            CreateLocationAsync(organization, cancellationToken));

        await organizationInternalOutboxPublisher.PublishRefreshAzureTenantMembersAsync(
            [tenant.Id],
            repositoryFactory.AzureTenantRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.AzureTenantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<Shared.Database.Entities.Organization> CreateOrganizationAsync(
        string? name,
        Shared.Database.Entities.Organization organization,
        CancellationToken cancellationToken)
    {
        var activeTermsOfUse = await organizationTermsOfUseService.GetActiveTermsOfUseAsync(cancellationToken);

        await organizationService.AddAsync(
            new Shared.Models.Organization
            {
                Id = organization.Id,
                Name = name.ToSafeString(),
                AgreedToTermsOfUse = true,
                TermsOfUse = activeTermsOfUse
            },
            null,
            true,
            cancellationToken);

        return organization;
    }

    private async Task<Location> CreateLocationAsync(
        Shared.Database.Entities.Organization organization,
        CancellationToken cancellationToken)
    {
        var location =
            await repositoryFactory.LocationRepository.UpsertNakedAsync(
                randomHelper.Generate(),
                organization,
                cancellationToken);

        await locationServiceClient.Admin_AddAsync(
            new Admin_AddInput { Id = location.Id, Name = "Office", OrganizationId = organization.Id },
            locationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        return location;
    }
}
