using Api.Shared.Services;
using Enterprise.Shared.Database;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Location.Shared.Workflows.GenerateLocationDailyAnalytics;

namespace Location.Api.Services;

public interface ILocationOwnershipService
{
    Task<Shared.Models.Location> ClaimOwnershipAsync(
        string uniqueClaimCode,
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken);
}

public class LocationOwnershipService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    TimeProvider timeProvider,
    IMapper mapper,
    ILocationOutboxPublisher locationOutboxPublisher,
    ITemporalOutboxPublisher temporalOutboxPublisher,
    ICachedLocationService cachedLocationService) : ILocationOwnershipService
{
    public async Task<Shared.Models.Location> ClaimOwnershipAsync(
        string uniqueClaimCode,
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);

        ArgumentException.ThrowIfNullOrWhiteSpace(uniqueClaimCode);

        var existingLocation = await repositoryFactory.LocationRepository.GetByUniqueClaimCodeAsync(
            uniqueClaimCode.ToLowerInvariant(),
            cancellationToken) ?? throw new LocationUniqueClaimCodeNotFound();

        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                                       organizationId,
                                       organizationUniqueAlphanumericName,
                                       false,
                                       false,
                                       cancellationToken) ??
                                   throw new LocationUniqueClaimCodeNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization.Id, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var ownedBySkedularPublicLocationsOrganization =
            existingOrganization.UniqueAlphanumericName == Constants.SkedularPublicLocationsUniqueAlphanumericName;

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingLocation.Organization = existingOrganization;
        existingLocation.UniqueClaimCode = null;

        var location = mapper.MapTo(existingLocation);

        locationOutboxPublisher.PublishLocations([location], repositoryFactory.UnitOfWork);

        if (ownedBySkedularPublicLocationsOrganization)
        {
            temporalOutboxPublisher.StartWorkflowLocationDailyAnalytics(
                new GenerateLocationDailyAnalyticsInput(location.Id, timeProvider.GetUtcNow().AddDays(1)),
                repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(existingLocation.Id, cancellationToken);

        return location;
    }
}
