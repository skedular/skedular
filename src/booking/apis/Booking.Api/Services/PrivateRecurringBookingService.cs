using Api.Shared.Services;
using Booking.Api.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using Enterprise.Shared.Random;
using Customer = Booking.Shared.Database.Entities.Customer;

namespace Booking.Api.Services;

public interface IPrivateRecurringBookingService
{
    Task<RecurringBooking> AddAsync(RecurringBooking recurringBooking, CancellationToken cancellationToken);
    Task<RecurringBooking> UpdateAsync(PrivateRecurringBookingPatchRequest request, CancellationToken cancellationToken);
    Task<RecurringBooking> DeleteAsync(string id, CancellationToken cancellationToken);
}

public class PrivateRecurringBookingService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IContext context,
    Shared.Services.IPrivateRecurringBookingService sharedPrivateRecurringBookingService,
    IEntityMapper entityMapper,
    ILogger<PrivateRecurringBookingService> logger) : IPrivateRecurringBookingService
{
    public async Task<RecurringBooking> AddAsync(RecurringBooking recurringBooking, CancellationToken cancellationToken)
    {
        if (recurringBooking.InvolvedCustomers.Count == 0)
        {
            throw new ArgumentException(nameof(recurringBooking.InvolvedCustomers));
        }

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        if (string.IsNullOrWhiteSpace(recurringBooking.Id))
        {
            recurringBooking.Id = randomHelper.Generate();
        }
        else
        {
            var existingRecurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(recurringBooking.Id, cancellationToken);
            if (existingRecurringBooking is not null)
            {
                return await UpdateInternalAsync(recurringBooking, existingRecurringBooking, customer, cancellationToken);
            }
        }

        var organizations = await organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(
            recurringBooking.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                .Select(item => item.Id)
                .Distinct()
                .ToList(),
            recurringBooking.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.CustomDomain))
                .Select(item => item.CustomDomain!)
                .Distinct()
                .ToList(),
            customer.Id,
            false,
            cancellationToken);

        var teams = await teamAuthorizationService.GetBookingInvolvedTeamAndValidatePermissionsAsync(
            recurringBooking.InvolvedTeams.Select(item => item.Id).Distinct().ToList(),
            customer.Id,
            false,
            cancellationToken);

        return await sharedPrivateRecurringBookingService.AddAsync(recurringBooking, customer, organizations, teams, cancellationToken);
    }

    public async Task<RecurringBooking> UpdateAsync(PrivateRecurringBookingPatchRequest request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.RecurringBooking.Id);

        var editUnits = string.Join(",", request.FieldsToUpdate);
        logger.LogInformation(
            "Private recurring booking patch autosave started. RecurringBookingId: {RecurringBookingId}, EditUnits: {EditUnits}",
            request.RecurringBooking.Id,
            editUnits);

        try
        {
            var existingRecurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(
                request.RecurringBooking.Id,
                cancellationToken) ?? throw new RecurringBookingNotFound();
            var recurringBooking = entityMapper.MapTo(existingRecurringBooking);
            Apply(request, recurringBooking);

            var updatedBooking = await UpdateAsync(recurringBooking, cancellationToken);
            logger.LogInformation(
                "Private recurring booking patch autosave completed. RecurringBookingId: {RecurringBookingId}, EditUnits: {EditUnits}",
                updatedBooking.Id,
                editUnits);
            return updatedBooking;
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(
                exception,
                "Private recurring booking patch autosave rejected by authorization. RecurringBookingId: {RecurringBookingId}, EditUnits: {EditUnits}",
                request.RecurringBooking.Id,
                editUnits);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Private recurring booking patch autosave failed. RecurringBookingId: {RecurringBookingId}, EditUnits: {EditUnits}",
                request.RecurringBooking.Id,
                editUnits);
            throw;
        }
    }

    public async Task<RecurringBooking> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var existingRecurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(id, cancellationToken) ??
                                       throw new RecurringBookingNotFound();
        var organizationIds = existingRecurringBooking.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
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

        var teamIds = existingRecurringBooking.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
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

        return await sharedPrivateRecurringBookingService.DeleteAsync(existingRecurringBooking, customer, cancellationToken);
    }

    private async Task<RecurringBooking> UpdateAsync(RecurringBooking recurringBooking, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(recurringBooking.Id);

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var existingRecurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(recurringBooking.Id, cancellationToken) ??
                                       throw new RecurringBookingNotFound();

        return await UpdateInternalAsync(recurringBooking, existingRecurringBooking, customer, cancellationToken);
    }

    private async Task<RecurringBooking> UpdateInternalAsync(
        RecurringBooking recurringBooking,
        Shared.Database.Entities.RecurringBooking existingRecurringBooking,
        Customer callingCustomer,
        CancellationToken cancellationToken)
    {
        var organizations = await organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(
            recurringBooking.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                .Select(item => item.Id)
                .Distinct()
                .ToList(),
            recurringBooking.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.CustomDomain))
                .Select(item => item.CustomDomain!)
                .Distinct()
                .ToList(),
            callingCustomer.Id,
            true,
            cancellationToken);
        var teams = await teamAuthorizationService.GetBookingInvolvedTeamAndValidatePermissionsAsync(
            recurringBooking.InvolvedTeams.Select(item => item.Id).Distinct().ToList(),
            callingCustomer.Id,
            true,
            cancellationToken);

        return await sharedPrivateRecurringBookingService.UpdateAsync(
            recurringBooking,
            existingRecurringBooking,
            callingCustomer,
            organizations,
            teams,
            cancellationToken);
    }

    private static void Apply(PrivateRecurringBookingPatchRequest request, RecurringBooking recurringBooking)
    {
        foreach (var field in request.FieldsToUpdate)
        {
            switch (field)
            {
                case PrivateRecurringBookingPatchField.Participants:
                    recurringBooking.InvolvedCustomers = request.RecurringBooking.InvolvedCustomers;
                    recurringBooking.InvolvedOrganizations = request.RecurringBooking.InvolvedOrganizations;
                    recurringBooking.InvolvedTeams = request.RecurringBooking.InvolvedTeams;
                    break;
                case PrivateRecurringBookingPatchField.RequestedResources:
                    recurringBooking.RequestedResources = request.RecurringBooking.RequestedResources;
                    break;
                case PrivateRecurringBookingPatchField.Schedule:
                    recurringBooking.From = request.RecurringBooking.From;
                    recurringBooking.Until = request.RecurringBooking.Until;
                    break;
                case PrivateRecurringBookingPatchField.Recurrence:
                    recurringBooking.Frequency = request.RecurringBooking.Frequency;
                    recurringBooking.Interval = request.RecurringBooking.Interval;
                    recurringBooking.ByMonthDay = request.RecurringBooking.ByMonthDay;
                    recurringBooking.BySetPosition = request.RecurringBooking.BySetPosition;
                    recurringBooking.ByWeekDays = request.RecurringBooking.ByWeekDays;
                    recurringBooking.EndType = request.RecurringBooking.EndType;
                    recurringBooking.StartDate = request.RecurringBooking.StartDate;
                    recurringBooking.EndDate = request.RecurringBooking.EndDate;
                    recurringBooking.OccurrenceCount = request.RecurringBooking.OccurrenceCount;
                    break;
                case PrivateRecurringBookingPatchField.SkippedDates:
                    recurringBooking.SkippedDates = request.RecurringBooking.SkippedDates;
                    break;
                case PrivateRecurringBookingPatchField.Category:
                    recurringBooking.Category = request.RecurringBooking.Category;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(request.FieldsToUpdate), field, null);
            }
        }
    }
}
