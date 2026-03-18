using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Api.Services;

public interface IRecurringBookingPaymentService
{
    Task<RecurringBooking> ConfirmPaymentAsync(string id, CancellationToken cancellationToken);
    Task<RecurringBooking> RejectPaymentAsync(string id, CancellationToken cancellationToken);
    Task<RecurringBooking> MakePaymentNotRequiredAsync(string id, CancellationToken cancellationToken);
}

public class RecurringBookingPaymentService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITemporalOutboxService temporalOutboxService,
    IMapper sharedMapper,
    IGraphQlTopicEventSender graphQlTopicEventSender) : IRecurringBookingPaymentService
{
    public Task<RecurringBooking> ConfirmPaymentAsync(string id, CancellationToken cancellationToken) =>
        UpdatePaymentStatusInternalAsync(id, PaymentStatus.Confirmed, cancellationToken);

    public Task<RecurringBooking> RejectPaymentAsync(string id, CancellationToken cancellationToken) =>
        UpdatePaymentStatusInternalAsync(id, PaymentStatus.Rejected, cancellationToken);

    public Task<RecurringBooking> MakePaymentNotRequiredAsync(string id, CancellationToken cancellationToken) =>
        UpdatePaymentStatusInternalAsync(id, PaymentStatus.NoPaymentRequired, cancellationToken);

    private async Task<RecurringBooking> UpdatePaymentStatusInternalAsync(
        string id,
        PaymentStatus paymentStatus,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(id, cancellationToken) ??
                               throw new RecurringBookingNotFound();
        if (recurringBooking.Channel.ToBookingChannel() != BookingChannel.Marketplace || recurringBooking.MarketplaceBooking is null)
        {
            throw new RecurringBookingIsNotMarketplace();
        }

        await EnsureCustomerCanModifyPaymentAsync(recurringBooking, customer.Id, cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        recurringBooking.MarketplaceBooking.PaymentStatus = paymentStatus.ToPaymentStatus();
        repositoryFactory.MarketplaceBookingRepository.Update(recurringBooking.MarketplaceBooking);

        if (recurringBooking.MarketplaceBooking.PaymentMethod.ToPaymentMethod() == PaymentMethod.Card)
        {
            temporalOutboxService.SignalWorkflowPayRecurringBookingViaCardSetPaymentStatus(
                recurringBooking.Id,
                new SetPaymentStatusArgs(recurringBooking.MarketplaceBooking.PaymentStatus),
                repositoryFactory.UnitOfWork);
        }
        else if (recurringBooking.MarketplaceBooking.PaymentMethod.ToPaymentMethod() == PaymentMethod.BankTransfer)
        {
            temporalOutboxService.SignalWorkflowPayRecurringBookingViaBankTransferSetPaymentStatus(
                recurringBooking.Id,
                new SetPaymentStatusArgs(recurringBooking.MarketplaceBooking.PaymentStatus),
                repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if (recurringBooking.MarketplaceBookingSubscription is not null)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                recurringBooking.MarketplaceBookingSubscription.Id,
                cancellationToken);
        }

        var relatedBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdAsync(
            recurringBooking.Id,
            recurringBooking.StartDate,
            null,
            cancellationToken);
        foreach (var booking in relatedBookings)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);
        }

        return sharedMapper.MapTo(recurringBooking);
    }

    private async Task EnsureCustomerCanModifyPaymentAsync(
        Shared.Database.Entities.RecurringBooking recurringBooking,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organizationIds = recurringBooking.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
        if (organizationIds.Count != 0)
        {
            foreach (var organizationId in organizationIds)
            {
                if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(organizationId, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            return;
        }

        var teamIds = recurringBooking.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
        if (teamIds.Count == 0)
        {
            throw new UnauthorizedAccessException();
        }

        var teams = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, true, cancellationToken);
        foreach (var team in teams)
        {
            if (team.Organization is null ||
                !await organizationAuthorizationService.CanModifyPaymentMethodAsync(team.Organization.Id, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
