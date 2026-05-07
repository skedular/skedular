using Api.Shared.Services;
using Enterprise.Shared.Database;
using Location.Api.Services.Authorization;
using Location.Shared.Mappers;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Location.Shared.Services.Cache;
using Location.Shared.Workflows;
using Constants = Api.Shared.Services.Constants;

namespace Location.Api.Services;

public interface ILocationOwnershipService
{
    Task<Shared.Models.Location> ClaimOwnershipAsync(
        string uniqueClaimCode,
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken);
}

public class LocationOwnershipService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    TimeProvider timeProvider,
    IEntityMapper entityMapper,
    ILocationOutboxPublisher locationOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    ICachedLocationService cachedLocationService) : ILocationOwnershipService
{
    public async Task<Shared.Models.Location> ClaimOwnershipAsync(
        string uniqueClaimCode,
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uniqueClaimCode);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByUniqueClaimCodeAsync(
            uniqueClaimCode.ToUpperInvariant(),
            cancellationToken) ?? throw new LocationUniqueClaimCodeNotFound();
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
            organizationId,
            organizationCustomDomain,
            false,
            false,
            cancellationToken);
        if (existingOrganization is null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);
            existingOrganization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        }
        else
        {
            if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization.Id, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        var ownedBySkedularPublicLocationsOrganization =
            existingOrganization.CustomDomain == Constants.SkedularPublicLocationsCustomDomainName;

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingLocation.Organization = existingOrganization;
        existingLocation.UniqueClaimCode = null;

        var location = entityMapper.MapTo(existingLocation);

        locationOutboxPublisher.PublishLocations([location], repositoryFactory.UnitOfWork);

        if (ownedBySkedularPublicLocationsOrganization)
        {
            temporalOutboxService.StartWorkflowLocationDailyAnalytics(
                new GenerateLocationDailyAnalyticsInput(location.Id, timeProvider.GetUtcNow().AddDays(1), null),
                repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(existingLocation.Id, cancellationToken);

        return location;
    }
}
