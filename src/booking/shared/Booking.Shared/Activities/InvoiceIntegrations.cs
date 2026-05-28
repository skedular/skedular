using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Temporalio.Activities;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Shared.Activities;

public record GenerateAndSendInvoiceInput(string BookingId, IReadOnlyList<string> InvoiceEmailList);

public record GenerateAndSendRecurringInvoiceInput(string RecurringBookingId, IReadOnlyList<string> InvoiceEmailList);

public record SyncAccountingInvoiceStateInput(
    string OrganizationId,
    string LocalEntityType,
    string LocalEntityId,
    string? ExternalInvoiceIdHint = null);

public record SyncAccountingInvoiceStateResult(bool IsTerminal, DateTimeOffset? NextSyncAt);

public class InvoiceIntegrations(
    IRepositoryFactory repositoryFactory,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationInvoiceCounterService organizationInvoiceCounterService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    ISkedularInvoiceService skedularInvoiceService,
    IXeroInvoiceService xeroInvoiceService)
{
    [Activity]
    public async Task GenerateAndSendInvoiceAsync(GenerateAndSendInvoiceInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(args.BookingId, cancellationToken);
        if (booking is null || booking.IsDeleted())
        {
            return;
        }

        var marketplaceBooking = booking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        var organizationId = productVersion.Product.Organization.Id;

        await EnsureInvoiceNumberAsync(marketplaceBooking, organizationId, cancellationToken);

        var sentViaXero = await xeroInvoiceService.TryHandleMarketplaceBookingInvoiceAsync(
            organizationId,
            booking,
            marketplaceBooking,
            productVersion,
            cancellationToken);

        if (sentViaXero)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);
            return;
        }

        await skedularInvoiceService.GenerateAndSendInvoiceAsync(args, booking, organizationId, cancellationToken);
        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);
    }

    [Activity]
    public async Task GenerateAndSendRecurringInvoiceAsync(GenerateAndSendRecurringInvoiceInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(args.RecurringBookingId, cancellationToken);
        if (recurringBooking is null || recurringBooking.IsDeleted())
        {
            return;
        }

        var marketplaceBooking = recurringBooking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        var organizationId = productVersion.Product.Organization.Id;

        await EnsureInvoiceNumberAsync(marketplaceBooking, organizationId, cancellationToken);

        var disposition = await xeroInvoiceService.HandleRecurringBookingInvoiceAsync(
            organizationId,
            recurringBooking,
            marketplaceBooking,
            productVersion,
            cancellationToken);

        if (disposition == RecurringInvoiceHandlingDisposition.StopAndPublish)
        {
            await PublishRecurringBookingInvoiceChangesAsync(recurringBooking, cancellationToken);
            return;
        }

        await skedularInvoiceService.GenerateAndSendRecurringInvoiceAsync(args, recurringBooking, organizationId, cancellationToken);
        await PublishRecurringBookingInvoiceChangesAsync(recurringBooking, cancellationToken);
    }

    [Activity]
    public Task<SyncAccountingInvoiceStateResult> SyncAccountingInvoiceStateAsync(SyncAccountingInvoiceStateInput input) =>
        xeroInvoiceService.SyncAccountingInvoiceStateAsync(input, ActivityExecutionContext.Current.CancellationToken);

    private async Task EnsureInvoiceNumberAsync(
        MarketplaceBooking marketplaceBooking,
        string organizationId,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(marketplaceBooking.InvoiceNumber))
        {
            return;
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
        marketplaceBooking.InvoiceNumber = await organizationInvoiceCounterService.GetNextInvoiceNumberIdAsync(organizationId, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task PublishRecurringBookingInvoiceChangesAsync(RecurringBooking recurringBooking, CancellationToken cancellationToken)
    {
        if (recurringBooking.MarketplaceBookingSubscription is not null)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                recurringBooking.MarketplaceBookingSubscription.Id,
                cancellationToken);
        }

        var relatedBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdUntrackedAsync(
            recurringBooking.Id,
            recurringBooking.StartDate,
            null,
            cancellationToken);
        foreach (var booking in relatedBookings)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);
        }
    }
}
