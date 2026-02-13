using Api.Shared.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using Enterprise.Shared.Random;
using Customer = Booking.Shared.Database.Entities.Customer;
using Location = Booking.Shared.Database.Entities.Location;
using Organization = Booking.Shared.Database.Entities.Organization;
using Resource = Booking.Shared.Database.Entities.Resource;

namespace Booking.Api.Services;

public interface IMarketplaceBookingService
{
    Task<Shared.Models.Booking> BookProductAsync(Shared.Models.Booking booking, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> UpdateAsync(Shared.Models.Booking booking, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> DeleteAsync(string id, CancellationToken cancellationToken);
}

public class MarketplaceBookingService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    IContext context,
    Shared.Services.IMarketplaceBookingService sharedMarketplaceBookingService) : IMarketplaceBookingService
{
    public async Task<Shared.Models.Booking> BookProductAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        if (booking.InvolvedCustomers.Count == 0)
        {
            throw new ArgumentException(nameof(booking.InvolvedCustomers));
        }

        if (booking.LineItems.Count == 0)
        {
            throw new ArgumentException(nameof(booking.LineItems));
        }

        if (booking.LineItems.Any(item => item.Quantity <= 0 || string.IsNullOrWhiteSpace(item.ProductVersionId)))
        {
            throw new ArgumentException(nameof(booking.LineItems));
        }

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        if (!string.IsNullOrWhiteSpace(booking.Id))
        {
            var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
            if (existingBooking is not null)
            {
                return await UpdateInternalAsync(booking, existingBooking, customer, cancellationToken);
            }
        }
        else
        {
            booking.Id = randomHelper.Generate();
        }

        var organizations = await GetOrganizationsAndValidatePermissionsAsync(booking, customer.Id, false, cancellationToken);

        return await sharedMarketplaceBookingService.BookProductAsync(booking, customer, organizations, cancellationToken);
    }

    public async Task<Shared.Models.Booking> UpdateAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(booking.Id);

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken) ?? throw new BookingNotFound();

        return await UpdateInternalAsync(booking, existingBooking, customer, cancellationToken);
    }

    public async Task<Shared.Models.Booking> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(id, cancellationToken) ?? throw new BookingNotFound();
        var organizationIds = existingBooking.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
        if (organizationIds.Count != 0)
        {
            var organizations = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
                organizationIds,
                null,
                false,
                false,
                cancellationToken);

            foreach (var organization in organizations)
            {
                if (!await organizationAuthorizationService.CanDeleteBookingAsync(organization.Id, customer.Id, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }

        return await sharedMarketplaceBookingService.DeleteAsync(existingBooking, customer, cancellationToken);
    }

    private async Task<ICollection<Organization>> GetOrganizationsAndValidatePermissionsAsync(
        Shared.Models.Booking booking,
        string customerId,
        bool existing,
        CancellationToken cancellationToken)
    {
        var organizationIds = booking.InvolvedOrganizations
            .Where(item => !string.IsNullOrWhiteSpace(item.Id))
            .Select(item => item.Id)
            .Distinct()
            .ToList();
        var uniqueAlphanumericNames = booking.InvolvedOrganizations
            .Where(item => !string.IsNullOrWhiteSpace(item.UniqueAlphanumericName))
            .Select(item => item.UniqueAlphanumericName!)
            .Distinct()
            .ToList();

        if (organizationIds.Count == 0 && uniqueAlphanumericNames.Count == 0)
        {
            return [];
        }

        var organizationEntities = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
            organizationIds,
            uniqueAlphanumericNames,
            false,
            false,
            cancellationToken);
        if (organizationIds.Count + uniqueAlphanumericNames.Count != organizationEntities.Count)
        {
            throw new OrganizationNotFound();
        }

        var result = new List<Organization>();
        foreach (var organization in booking.InvolvedOrganizations)
        {
            var organizationEntity = organizationEntities.First(item =>
                item.Id == organization.Id || item.UniqueAlphanumericName == organization.UniqueAlphanumericName);
            if (existing)
            {
                if (!await organizationAuthorizationService.CanUpdateBookingAsync(organizationEntity.Id, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                if (!await organizationAuthorizationService.CanAddBookingAsync(organizationEntity.Id, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }

                if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(organizationEntity.Id, customerId, cancellationToken))
                {
                    throw new NoMoreInteractionAllowed();
                }
            }

            result.Add(organizationEntity);
        }

        return result;
    }

    private async Task<Shared.Models.Booking> UpdateInternalAsync(
        Shared.Models.Booking booking,
        Shared.Database.Entities.Booking existingBooking,
        Customer callingCustomer,
        CancellationToken cancellationToken)
    {
        var organizations = await GetOrganizationsAndValidatePermissionsAsync(booking, callingCustomer.Id, true, cancellationToken);

        return await sharedMarketplaceBookingService.UpdateAsync(booking, existingBooking, callingCustomer, organizations, [], cancellationToken);
    }

    private static List<Location> ResourcesToLocations(ICollection<Resource> resources) =>
        resources
            .Where(item => item.Location is not null)
            .Select(item => item.Location)
            .GroupBy(item => item!.Id)
            .Select(item => item.First())
            .ToList()!;
}
