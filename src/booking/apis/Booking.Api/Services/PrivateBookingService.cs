using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Context;
using Enterprise.Shared.Random;
using Customer = Booking.Shared.Database.Entities.Customer;

namespace Booking.Api.Services;

public interface IPrivateBookingService
{
    Task<Shared.Models.Booking> AddAsync(Shared.Models.Booking booking, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> AddAsync(Shared.Models.Booking booking, DateOnly? fullOpeningHoursDate, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> UpdateAsync(PrivateBookingPatchRequest request, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> DeleteAsync(string id, CancellationToken cancellationToken);
}

public class PrivateBookingService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IContext context,
    Shared.Services.IPrivateBookingService sharedPrivateBookingService,
    IMarketplaceBookingOpeningHoursService marketplaceBookingOpeningHoursService,
    IEntityMapper entityMapper,
    ILogger<PrivateBookingService> logger) : IPrivateBookingService
{
    public Task<Shared.Models.Booking> AddAsync(Shared.Models.Booking booking, CancellationToken cancellationToken) =>
        AddAsync(booking, null, cancellationToken);

    public async Task<Shared.Models.Booking> AddAsync(
        Shared.Models.Booking booking,
        DateOnly? fullOpeningHoursDate,
        CancellationToken cancellationToken)
    {
        if (booking.InvolvedCustomers.Count == 0)
        {
            throw new ArgumentException(nameof(booking.InvolvedCustomers));
        }

        if (fullOpeningHoursDate.HasValue)
        {
            booking = await ResolveFullOpeningHoursBookingWindowAsync(booking, fullOpeningHoursDate.Value, cancellationToken);
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
                .Where(item => !string.IsNullOrWhiteSpace(item.CustomDomain))
                .Select(item => item.CustomDomain!)
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

        return await sharedPrivateBookingService.AddAsync(booking, customer, organizations, teams, null, cancellationToken);
    }

    public async Task<Shared.Models.Booking> UpdateAsync(PrivateBookingPatchRequest request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Booking.Id);

        var editUnits = string.Join(",", request.FieldsToUpdate);
        logger.LogInformation(
            "Private booking patch autosave started. BookingId: {BookingId}, EditUnits: {EditUnits}",
            request.Booking.Id,
            editUnits);

        try
        {
            var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(request.Booking.Id, cancellationToken) ??
                                  throw new BookingNotFound();
            var booking = entityMapper.MapTo(existingBooking);
            Apply(request, booking);

            var updatedBooking = await UpdateAsync(booking, cancellationToken);
            logger.LogInformation(
                "Private booking patch autosave completed. BookingId: {BookingId}, EditUnits: {EditUnits}",
                updatedBooking.Id,
                editUnits);
            return updatedBooking;
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(
                exception,
                "Private booking patch autosave rejected by authorization. BookingId: {BookingId}, EditUnits: {EditUnits}",
                request.Booking.Id,
                editUnits);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Private booking patch autosave failed. BookingId: {BookingId}, EditUnits: {EditUnits}",
                request.Booking.Id,
                editUnits);
            throw;
        }
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
            var organizations = await repositoryFactory.OrganizationRepository.GetByIdsOrCustomDomainsAsync(
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

        var teamIds = existingBooking.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
        if (teamIds.Count != 0)
        {
            var teams = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, false, cancellationToken);
            foreach (var team in teams)
            {
                if (!await teamAuthorizationService.CanDeleteBookingAsync(team, customer.Id, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }

        return await sharedPrivateBookingService.DeleteAsync(existingBooking, customer, true, cancellationToken);
    }

    private async Task<Shared.Models.Booking> ResolveFullOpeningHoursBookingWindowAsync(
        Shared.Models.Booking booking,
        DateOnly bookingDate,
        CancellationToken cancellationToken)
    {
        var resourceIds = booking.Resources.Select(item => item.Resource.Id).Where(item => !string.IsNullOrWhiteSpace(item)).Distinct().ToList();
        if (resourceIds.Count == 0)
        {
            throw new ArgumentException("Full opening-hours bookings require at least one resource.", nameof(booking.Resources));
        }

        var resources = await repositoryFactory.ResourceRepository.GetByIdsAsync(resourceIds, false, cancellationToken);
        if (resources.Count != resourceIds.Count)
        {
            throw new ResourceNotFound();
        }

        var windows = resources
            .Select(resource => marketplaceBookingOpeningHoursService.ResolveDailyBookingWindow(resource, bookingDate))
            .ToList();
        if (windows.Any(window => window is null))
        {
            throw new ResourceNotAvailable();
        }

        var firstWindow = windows.First()!.Value;
        if (windows.Any(window => window!.Value.From != firstWindow.From || window.Value.Until != firstWindow.Until))
        {
            throw new ResourceNotAvailable();
        }

        booking.From = firstWindow.From;
        booking.Until = firstWindow.Until;
        booking.Schedules = [new BookingSchedule(firstWindow.From, firstWindow.Until)];

        return booking;
    }

    private async Task<Shared.Models.Booking> UpdateAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(booking.Id);

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken) ?? throw new BookingNotFound();

        return await UpdateInternalAsync(booking, existingBooking, customer, cancellationToken);
    }

    private async Task<Shared.Models.Booking> UpdateInternalAsync(
        Shared.Models.Booking booking,
        Shared.Database.Entities.Booking existingBooking,
        Customer callingCustomer,
        CancellationToken cancellationToken)
    {
        if (booking.HasRecurringInstanceOverrides == true)
        {
            // Do nothing
        }
        else if (existingBooking.RecurringBooking is not null && (existingBooking.From != booking.From || existingBooking.Until != booking.Until))
        {
            booking.HasRecurringInstanceOverrides = true;
        }

        var organizations = await organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(
            booking.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                .Select(item => item.Id)
                .Distinct()
                .ToList(),
            booking.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.CustomDomain))
                .Select(item => item.CustomDomain!)
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

        return await sharedPrivateBookingService.UpdateAsync(
            booking,
            existingBooking,
            callingCustomer,
            organizations,
            teams,
            null,
            false,
            cancellationToken);
    }

    private static void Apply(PrivateBookingPatchRequest request, Shared.Models.Booking booking)
    {
        foreach (var field in request.FieldsToUpdate)
        {
            switch (field)
            {
                case PrivateBookingPatchField.Participants:
                    booking.InvolvedCustomers = request.Booking.InvolvedCustomers;
                    booking.InvolvedOrganizations = request.Booking.InvolvedOrganizations;
                    booking.InvolvedTeams = request.Booking.InvolvedTeams;
                    break;
                case PrivateBookingPatchField.Schedule:
                    booking.From = request.Booking.From;
                    booking.Until = request.Booking.Until;
                    booking.Schedules = request.Booking.Schedules;
                    break;
                case PrivateBookingPatchField.Notes:
                    booking.Notes = request.Booking.Notes;
                    break;
                case PrivateBookingPatchField.Category:
                    booking.Category = request.Booking.Category;
                    break;
                case PrivateBookingPatchField.Resources:
                    booking.Resources = request.Booking.Resources;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(request.FieldsToUpdate), field,
                        $"Unexpected value for {nameof(request.FieldsToUpdate)}: {field}. Update enum mapping or caller input.");
            }
        }
    }
}
