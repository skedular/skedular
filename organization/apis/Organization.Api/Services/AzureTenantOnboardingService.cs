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
using Organization.Shared.Services;
using Organization.Shared.Workflows;
using LocationConfiguration = Api.Shared.Clients.Configurations.Grpc.LocationConfiguration;

namespace Organization.Api.Services;

public interface IAzureTenantOnboardingService
{
    Task OnboardAsync(string tenantId, AzureInstallStateUserIdLookup azureInstallStateUserIdLookup, CancellationToken cancellationToken);
}

public class AzureTenantOnboardingService(
    LocationConfiguration locationConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IMapper mapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    IOrganizationTermsOfUseService organizationTermsOfUseService,
    LocationService.LocationServiceClient locationServiceClient,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    TimeProvider timeProvider) : IAzureTenantOnboardingService
{
    public async Task OnboardAsync(string tenantId, AzureInstallStateUserIdLookup azureInstallStateUserIdLookup, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var start = timeProvider.GetUtcNow();
        var organizationOffering = new OrganizationOffering
        {
            Id = randomHelper.Generate(),
            CreatedAt = start,
            Code = OfferingCode.FreeTierV1,
            Start = start,
            End = start.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
            AutoRenew = true,
            UnitPrice = OfferingCode.FreeTierV1.GetOffering().UnitPrice
        };
        var organization = new Shared.Database.Entities.Organization
        {
            Id = randomHelper.Generate(),
            Name = "No name set!!!",
            AgreedToTermsOfUse = true,
            Type = OrganizationTypeConstants.Private,
            IsOwnershipVerified = false,
            TermsOfUse = await organizationTermsOfUseService.GetActiveTermsOfUseEntityAsync(cancellationToken),
            OrganizationOfferings = [organizationOffering]
        };
        var azureTenant = new AzureTenant
        {
            Id = tenantId, InstalledByUserId = azureInstallStateUserIdLookup.InstalledByUserId, Organization = organization
        };
        organization.AzureTenants = [azureTenant];

        var tenant = repositoryFactory.AzureTenantRepository.Add(azureTenant);

        await locationServiceClient.Admin_AddAsync(
            new Admin_AddInput { Id = randomHelper.Generate(), Name = "Office", OrganizationId = organization.Id },
            locationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        repositoryFactory.AzureInstallStateUserIdLookupRepository.Remove(azureInstallStateUserIdLookup);

        organizationOutboxPublisher.PublishOrganizations(
            [mapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))],
            repositoryFactory.UnitOfWork);
        temporalOutboxService.StartWorkflowReSyncAzureTenant(new ReSyncAzureTenantInput(tenant.Id, null), repositoryFactory.UnitOfWork);
        temporalOutboxService.StartWorkflowScheduleRenewOrganizationOffering(
            new ScheduleRenewOrganizationOfferingInput(
                organization.Id,
                organizationOffering.Id,
                organizationOffering.End.GetNextOfferingPeriodStart()),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
