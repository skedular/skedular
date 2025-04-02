using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
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
    IMapper mapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IOrganizationInternalOutboxPublisher organizationInternalOutboxPublisher,
    IOrganizationTermsOfUseService organizationTermsOfUseService,
    LocationService.LocationServiceClient locationServiceClient,
    TimeProvider timeProvider) : IAzureTenantOnboardingService
{
    public async Task OnboardAsync(
        string tenantId,
        AzureInstallStateUserIdLookup azureInstallStateUserIdLookup,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var location = new Location { Id = randomHelper.Generate() };
        var now = timeProvider.GetUtcNow();
        var organization = new Shared.Database.Entities.Organization
        {
            Id = randomHelper.Generate(),
            Name = "No name set!!!",
            AgreedToTermsOfUse = true,
            Type = OrganizationTypeConstants.Private,
            TermsOfUse = await organizationTermsOfUseService.GetActiveTermsOfUseEntityAsync(cancellationToken),
            HasAttachedPaymentMethod = false,
            Locations = [location],
            OrganizationOfferings =
            [
                new OrganizationOffering
                {
                    Id = randomHelper.Generate(),
                    CreatedAt = now,
                    Code = OfferingCode.FreeTierV1,
                    Start = now,
                    End = now.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
                    AutoRenew = true,
                    UnitPrice = OfferingCode.FreeTierV1.GetOffering().UnitPrice
                }
            ]
        };
        var azureTenant = new AzureTenant
        {
            Id = tenantId, InstalledByUserId = azureInstallStateUserIdLookup.InstalledByUserId, Organization = organization
        };
        organization.AzureTenants = [azureTenant];

        var tenant = repositoryFactory.AzureTenantRepository.Add(azureTenant);

        await locationServiceClient.Admin_AddAsync(
            new Admin_AddInput { Id = location.Id, Name = "Office", OrganizationId = organization.Id },
            locationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        repositoryFactory.AzureInstallStateUserIdLookupRepository.Remove(azureInstallStateUserIdLookup);

        await organizationOutboxPublisher.PublishOrganizationsAsync([mapper.MapTo(organization)], repositoryFactory.UnitOfWork, cancellationToken);
        await organizationInternalOutboxPublisher.PublishRefreshAzureTenantMembersAsync([tenant.Id], repositoryFactory.UnitOfWork, cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
