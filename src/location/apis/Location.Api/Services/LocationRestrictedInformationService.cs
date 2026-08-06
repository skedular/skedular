using Api.Shared.Services;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Location.Api.Models;
using Location.Api.Services.Authorization;
using Location.Shared.Mappers;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;

namespace Location.Api.Services;

public interface ILocationRestrictedInformationService
{
    Task<IReadOnlyList<LocationRestrictedInformation>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken);
    Task<Shared.Models.Location> AddAsync(LocationRestrictedInformation restrictedInformation, CancellationToken cancellationToken);
    Task<Shared.Models.Location> UpdateAsync(LocationRestrictedInformationPatchRequest request, CancellationToken cancellationToken);
    Task<Shared.Models.Location> DeleteAsync(string id, CancellationToken cancellationToken);
}

public class LocationRestrictedInformationService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ILocationBookingAccessService locationBookingAccessService,
    ILocationOutboxPublisher locationOutboxPublisher,
    IEntityMapper entityMapper,
    ICachedLocationService cachedLocationService,
    IContext context,
    ILogger<LocationRestrictedInformationService> logger) : ILocationRestrictedInformationService
{
    public async Task<IReadOnlyList<LocationRestrictedInformation>> GetByLocationIdAsync(
        string locationId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var location = await repositoryFactory.LocationRepository.GetByIdUntrackedAsync(locationId, cancellationToken) ??
                       throw new LocationNotFound();

        if (!await CanCurrentCustomerViewAsync(location.Id, location.OrganizationId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var restrictedInformation =
            await repositoryFactory.LocationRestrictedInformationRepository.GetActiveByLocationIdUntrackedAsync(locationId, cancellationToken);

        return restrictedInformation.Select(MapTo).ToList();
    }

    public async Task<Shared.Models.Location> AddAsync(
        LocationRestrictedInformation restrictedInformation,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(restrictedInformation.Location.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(restrictedInformation.Title);

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(restrictedInformation.Location.Id, cancellationToken) ??
                       throw new LocationNotFound();

        await EnsureCurrentCustomerCanModifyAsync(location.OrganizationId, cancellationToken);

        if (string.IsNullOrWhiteSpace(restrictedInformation.Id))
        {
            restrictedInformation.Id = randomHelper.Generate();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.LocationRestrictedInformationRepository.Add(MapTo(restrictedInformation, location));

        var mappedLocation = await PublishLocationChangedAsync(location.Id, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(location.Id, cancellationToken);

        return mappedLocation;
    }

    public async Task<Shared.Models.Location> UpdateAsync(
        LocationRestrictedInformationPatchRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RestrictedInformation.Id);

        var editUnits = string.Join(",", request.FieldsToUpdate);
        logger.LogInformation(
            "Location restricted information patch autosave started. RestrictedInformationId: {RestrictedInformationId}, EditUnits: {EditUnits}",
            request.RestrictedInformation.Id,
            editUnits);

        try
        {
            var existing = await repositoryFactory.LocationRestrictedInformationRepository.GetByIdAsync(
                               request.RestrictedInformation.Id,
                               cancellationToken) ??
                           throw new LocationRestrictedInformationNotFound();

            await EnsureCurrentCustomerCanModifyAsync(existing.Location.OrganizationId, cancellationToken);

            if (request.FieldsToUpdate.Count == 0)
            {
                var location = await repositoryFactory.LocationRepository.GetByIdAsync(existing.Location.Id, cancellationToken) ??
                               throw new LocationNotFound();
                var unchangedLocation = entityMapper.MapTo(location);
                logger.LogInformation(
                    "Location restricted information patch autosave completed with no changes. LocationId: {LocationId}, RestrictedInformationId: {RestrictedInformationId}, EditUnits: {EditUnits}",
                    unchangedLocation.Id,
                    request.RestrictedInformation.Id,
                    editUnits);
                return unchangedLocation;
            }

            await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

            foreach (var field in request.FieldsToUpdate)
            {
                switch (field)
                {
                    case LocationRestrictedInformationPatchField.Title:
                        ArgumentException.ThrowIfNullOrWhiteSpace(request.RestrictedInformation.Title);
                        existing.Title = request.RestrictedInformation.Title;
                        break;
                    case LocationRestrictedInformationPatchField.Category:
                        existing.Category = request.RestrictedInformation.Category.ToLocationRestrictedInformationCategory();
                        break;
                    case LocationRestrictedInformationPatchField.Content:
                        existing.Content = request.RestrictedInformation.Content;
                        break;
                    case LocationRestrictedInformationPatchField.Active:
                        existing.Active = request.RestrictedInformation.Active;
                        break;
                    case LocationRestrictedInformationPatchField.SortOrder:
                        existing.SortOrder = request.RestrictedInformation.SortOrder;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(request.FieldsToUpdate), field,
                            $"Unexpected value for {nameof(request.FieldsToUpdate)}: {field}. Update enum mapping or caller input.");
                }
            }

            repositoryFactory.LocationRestrictedInformationRepository.Update(existing);

            var mappedLocation = await PublishLocationChangedAsync(existing.Location.Id, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            await cachedLocationService.UpdateByIdAsync(existing.Location.Id, cancellationToken);

            logger.LogInformation(
                "Location restricted information patch autosave completed. LocationId: {LocationId}, RestrictedInformationId: {RestrictedInformationId}, EditUnits: {EditUnits}",
                mappedLocation.Id,
                request.RestrictedInformation.Id,
                editUnits);
            return mappedLocation;
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(
                exception,
                "Location restricted information patch autosave rejected by authorization. RestrictedInformationId: {RestrictedInformationId}, EditUnits: {EditUnits}",
                request.RestrictedInformation.Id,
                editUnits);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Location restricted information patch autosave failed. RestrictedInformationId: {RestrictedInformationId}, EditUnits: {EditUnits}",
                request.RestrictedInformation.Id,
                editUnits);
            throw;
        }
    }

    public async Task<Shared.Models.Location> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var existing = await repositoryFactory.LocationRestrictedInformationRepository.GetByIdAsync(id, cancellationToken) ??
                       throw new LocationRestrictedInformationNotFound();

        await EnsureCurrentCustomerCanModifyAsync(existing.Location.OrganizationId, cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.LocationRestrictedInformationRepository.Remove(existing);

        var mappedLocation = await PublishLocationChangedAsync(existing.Location.Id, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(existing.Location.Id, cancellationToken);

        return mappedLocation;
    }

    private async Task<bool> CanCurrentCustomerViewAsync(
        string locationId,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var customerToken = context.GetVerifiableToken();
        if (string.IsNullOrWhiteSpace(customerToken))
        {
            return false;
        }

        return await organizationAuthorizationService.CanViewAsync(organizationId, await GetCustomerIdAsync(cancellationToken), cancellationToken) ||
               await locationBookingAccessService.HasCurrentCustomerAccessToLocationAsync(locationId, cancellationToken);
    }

    private async Task EnsureCurrentCustomerCanModifyAsync(string organizationId, CancellationToken cancellationToken)
    {
        if (!await organizationAuthorizationService.CanModifyAsync(organizationId, await GetCustomerIdAsync(cancellationToken), cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }
    }

    private async Task<string> GetCustomerIdAsync(CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.GetMinimalByVerifiableTokenUntrackedAsync(
            context.GetVerifiableToken(),
            cancellationToken);

        return customer?.Id ?? throw new CustomerNotFound();
    }

    private static LocationRestrictedInformation MapTo(Shared.Database.Entities.LocationRestrictedInformation src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Title = src.Title,
            Category = src.Category.ToLocationRestrictedInformationCategory(),
            Content = src.Content,
            Active = src.Active,
            SortOrder = src.SortOrder,
            Location = new Shared.Models.Location
            {
                Id = src.Location.Id,
                Name = src.Location.Name,
                Organization = new Organization
                {
                    Id = src.Location.Organization.Id,
                    CustomDomain = src.Location.Organization.CustomDomain,
                },
            },
        };

    private static Shared.Database.Entities.LocationRestrictedInformation MapTo(
        LocationRestrictedInformation src,
        Shared.Database.Entities.Location location) =>
        new()
        {
            Id = src.Id,
            Title = src.Title,
            Category = src.Category.ToLocationRestrictedInformationCategory(),
            Content = src.Content,
            Active = src.Active,
            SortOrder = src.SortOrder,
            Location = location,
        };

    private async Task<Shared.Models.Location> PublishLocationChangedAsync(string locationId, CancellationToken cancellationToken)
    {
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken) ?? throw new LocationNotFound();
        var mappedLocation = entityMapper.MapTo(location);

        locationOutboxPublisher.PublishLocations([mappedLocation], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return mappedLocation;
    }
}
