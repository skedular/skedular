using Api.Shared.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Repositories;
using Enterprise.Shared;
using Enterprise.Shared.Context;
using Enterprise.Shared.Random;
using Customer = Booking.Shared.Database.Entities.Customer;

namespace Booking.Api.Services;

public interface IMarketplaceBookingService
{
    Task<Shared.Models.Booking> AddAsync(Shared.Models.Booking booking, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> UpdateAsync(Shared.Models.Booking booking, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> DeleteAsync(string id, CancellationToken cancellationToken);
}

public class MarketplaceBookingService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IContext context,
    Shared.Services.IMarketplaceBookingService sharedMarketplaceBookingService) : IMarketplaceBookingService
{
    public async Task<Shared.Models.Booking> AddAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        var marketplaceBooking = booking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        if (booking.InvolvedCustomers.Count == 0)
        {
            throw new ArgumentException(nameof(booking.InvolvedCustomers));
        }

        if (marketplaceBooking.Quantity <= 0)
        {
            throw new ArgumentException(nameof(marketplaceBooking.Quantity));
        }

        if (string.IsNullOrWhiteSpace(marketplaceBooking.ProductVersion.Id))
        {
            throw new ArgumentException(nameof(marketplaceBooking.ProductVersion));
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

        var organizations = await organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(
            booking.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                .Select(item => item.Id)
                .Distinct()
                .ToList(),
            booking.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.UniqueAlphanumericName))
                .Select(item => item.UniqueAlphanumericName!)
                .Distinct()
                .ToList(),
            customer.Id,
            false,
            cancellationToken);
        var teams = await teamAuthorizationService.GetBookingInvolvedTeamAndValidatePermissionsAsync(
            booking.InvolvedTeams.Select(item => item.Id).Distinct().ToList(),
            customer.Id,
            false,
            cancellationToken);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        marketplaceBooking.ProductPricing =
            productVersion.PricingOptions.ToSafeCollection().First(item => item.Id == marketplaceBooking.ProductPricing.Id);

        return await sharedMarketplaceBookingService.AddAsync(booking, customer, organizations, teams, null, cancellationToken);
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

    private async Task<Shared.Models.Booking> UpdateInternalAsync(
        Shared.Models.Booking booking,
        Shared.Database.Entities.Booking existingBooking,
        Customer callingCustomer,
        CancellationToken cancellationToken)
    {
        var organizations = await organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(
            booking.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                .Select(item => item.Id)
                .Distinct()
                .ToList(),
            booking.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.UniqueAlphanumericName))
                .Select(item => item.UniqueAlphanumericName!)
                .Distinct()
                .ToList(),
            callingCustomer.Id,
            true,
            cancellationToken);
        var teams = await teamAuthorizationService.GetBookingInvolvedTeamAndValidatePermissionsAsync(
            booking.InvolvedTeams.Select(item => item.Id).Distinct().ToList(),
            callingCustomer.Id,
            true,
            cancellationToken);

        return await sharedMarketplaceBookingService.UpdateAsync(
            booking,
            existingBooking,
            callingCustomer,
            organizations,
            teams,
            null,
            false,
            cancellationToken);
    }
}
