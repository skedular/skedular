using System.Data;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Customer = Booking.Shared.Database.Entities.Customer;

namespace Booking.Api.Services;

/// <summary>
///     Result of a marketplace booking add operation; exactly one of <see cref="Booking" /> or
///     <see cref="Failure" /> is non-null for a completed immediate submission.
/// </summary>
public sealed record MarketplaceBookingAddResult(
    Shared.Models.Booking? Booking = null,
    MarketplaceBookingFailureSummary? Failure = null);

public interface IMarketplaceBookingService
{
    Task<string?> GetBookingIdAsync(string marketplaceBookingId, CancellationToken cancellationToken);
    Task<MarketplaceBookingAddResult> AddAsync(Shared.Models.Booking booking, CancellationToken cancellationToken);

    Task<IReadOnlyList<Shared.Models.Booking>> AddCreditBookingsAsync(Shared.Models.Booking booking, int quantity,
        CancellationToken cancellationToken);

    Task<Shared.Models.Booking> UpdateAsync(MarketplaceBookingPatchRequest request, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> DeleteAsync(string id, string? cancellationOverrideReason, CancellationToken cancellationToken);
}

public class MarketplaceBookingService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICancellationDecisionService cancellationDecisionService,
    ITeamAuthorizationService teamAuthorizationService,
    IContext context,
    Shared.Services.IMarketplaceBookingService sharedMarketplaceBookingService,
    IDbTransactionBuilder transactionBuilder,
    IEntityMapper entityMapper,
    ILogger<MarketplaceBookingService> logger) : IMarketplaceBookingService
{
    public async Task<string?> GetBookingIdAsync(string marketplaceBookingId, CancellationToken cancellationToken) =>
        (await repositoryFactory.BookingRepository.GetByMarketplaceBookingIdAsync(marketplaceBookingId, cancellationToken))?.Id;

    public async Task<MarketplaceBookingAddResult> AddAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
        => await AddAsync(booking, false, cancellationToken);

    public async Task<IReadOnlyList<Shared.Models.Booking>> AddCreditBookingsAsync(
        Shared.Models.Booking booking,
        int quantity,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (booking.MarketplaceBooking?.EntitlementId is null)
        {
            throw new InvalidOperationException("Credit booking batches require an entitlement.");
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.UnitOfWork, IsolationLevel.Serializable, cancellationToken);
        var bookings = new List<Shared.Models.Booking>(quantity);
        for (var index = 0; index < quantity; index++)
        {
            var instance = CloneCreditBooking(booking, randomHelper.Generate());
            var result = await AddAsync(instance, true, cancellationToken);
            if (result.Failure is not null || result.Booking is null)
            {
                throw new MarketplaceBookingAvailabilityConflict([]);
            }

            bookings.Add(result.Booking);
        }

        await transaction.CommitAsync(cancellationToken);
        return bookings;
    }

    public async Task<Shared.Models.Booking> UpdateAsync(MarketplaceBookingPatchRequest request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Booking.Id);

        var editUnits = string.Join(",", request.FieldsToUpdate);
        logger.LogInformation(
            "Marketplace booking patch autosave started. BookingId: {BookingId}, EditUnits: {EditUnits}",
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
                "Marketplace booking patch autosave completed. BookingId: {BookingId}, EditUnits: {EditUnits}",
                updatedBooking.Id,
                editUnits);
            return updatedBooking;
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(
                exception,
                "Marketplace booking patch autosave rejected by authorization. BookingId: {BookingId}, EditUnits: {EditUnits}",
                request.Booking.Id,
                editUnits);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Marketplace booking patch autosave failed. BookingId: {BookingId}, EditUnits: {EditUnits}",
                request.Booking.Id,
                editUnits);
            throw;
        }
    }

    public async Task<Shared.Models.Booking> DeleteAsync(string id, string? cancellationOverrideReason, CancellationToken cancellationToken)
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

        ArgumentNullException.ThrowIfNull(existingBooking.MarketplaceBooking);

        var productVersionId = existingBooking.MarketplaceBooking.ProductVersion.Id;
        ArgumentException.ThrowIfNullOrWhiteSpace(productVersionId);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(productVersionId, cancellationToken) ??
                             throw new ProductVersionNotFound();
        var productOwnerOrganizationId = productVersion.Product.Organization.Id;
        var canManageProduct =
            await organizationAuthorizationService.CanOverrideCancellationPolicyAsync(productOwnerOrganizationId, customer.Id, cancellationToken);
        var cancellationDecision = cancellationDecisionService.ResolveCustomerDecision(
            customer.Id,
            productOwnerOrganizationId,
            canManageProduct,
            cancellationOverrideReason);

        var deletedBooking = await sharedMarketplaceBookingService.DeleteAsync(
            existingBooking,
            customer,
            cancellationDecision.CanOverridePolicy,
            cancellationDecision.OverrideReason,
            true,
            cancellationToken);
        return deletedBooking;
    }

    private async Task<MarketplaceBookingAddResult> AddAsync(Shared.Models.Booking booking, bool useExistingTransaction,
        CancellationToken cancellationToken)
    {
        var marketplaceBooking = booking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        if (booking.InvolvedCustomers.Count == 0)
        {
            throw new ArgumentException(nameof(booking.InvolvedCustomers));
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
                var updated = await UpdateInternalAsync(booking, existingBooking, customer, cancellationToken);
                return new MarketplaceBookingAddResult(updated);
            }
        }
        else
        {
            booking.Id = randomHelper.Generate();
        }

        var organizations = await organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(
            [
                .. booking.InvolvedOrganizations
                    .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                    .Select(item => item.Id)
                    .Distinct(),
            ],
            [
                .. booking.InvolvedOrganizations
                    .Where(item => !string.IsNullOrWhiteSpace(item.CustomDomain))
                    .Select(item => item.CustomDomain!)
                    .Distinct(),
            ],
            customer.Id,
            false,
            cancellationToken);
        var teams = await teamAuthorizationService.GetBookingInvolvedTeamAndValidatePermissionsAsync(
            [.. booking.InvolvedTeams.Select(item => item.Id).Distinct()],
            customer.Id,
            false,
            cancellationToken);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        if (productVersion.Product.Organization.Type == OrganizationTypeConstants.Host &&
            productVersion.Type != ProductTypeConstants.Event)
        {
            logger.LogWarning(
                "Host booking rejected because the Product is not configured for full-place booking. ProductId: {ProductId}, ProductVersionId: {ProductVersionId}",
                productVersion.ProductId,
                productVersion.Id);
            throw new InvalidOperationException("Host products only support full-place booking.");
        }

        if (productVersion.Product.Organization.Type == OrganizationTypeConstants.Host &&
            marketplaceBooking.PaymentMethod != PaymentMethod.Card)
        {
            logger.LogWarning(
                "Host booking rejected because only Stripe card payments are supported. ProductId: {ProductId}, PaymentMethod: {PaymentMethod}",
                productVersion.ProductId,
                marketplaceBooking.PaymentMethod);
            throw new InvalidOperationException("Host bookings currently support card payment only.");
        }

        if (productVersion.Type == ProductTypeConstants.Event)
        {
            marketplaceBooking.Quantity = 1;
        }
        else if (marketplaceBooking.Quantity <= 0)
        {
            throw new ArgumentException(nameof(marketplaceBooking.Quantity));
        }

        marketplaceBooking.ProductPricing =
            productVersion.PricingOptions.ToSafeCollection().First(item => item.Id == marketplaceBooking.ProductPricing.Id);

        try
        {
            var isCreditFunded = marketplaceBooking.EntitlementId is not null;
            await using var transaction = isCreditFunded && !useExistingTransaction
                ? await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, IsolationLevel.Serializable, cancellationToken)
                : null;
            var added = await sharedMarketplaceBookingService.AddAsync(
                booking,
                customer,
                organizations,
                teams,
                null,
                true,
                true,
                isCreditFunded,
                cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return new MarketplaceBookingAddResult(added);
        }
        catch (MarketplaceBookingAvailabilityConflict ex) when (!string.IsNullOrEmpty(ex.FailureId))
        {
            var failure = await repositoryFactory.MarketplaceBookingFailureRepository.GetByIdAsync(ex.FailureId, cancellationToken);
            if (failure is null)
            {
                throw;
            }

            return new MarketplaceBookingAddResult(Failure: new MarketplaceBookingFailureSummary(
                failure.Id, failure.Category, failure.Scope, failure.FinalizedAt, failure.RequestedFrom, failure.RequestedUntil,
                failure.CustomerAction ?? string.Empty));
        }
    }

    private static Shared.Models.Booking CloneCreditBooking(Shared.Models.Booking source, string id) => new()
    {
        Id = id,
        From = source.From,
        Until = source.Until,
        Notes = source.Notes,
        Category = source.Category,
        Schedules = [new BookingSchedule(source.From, source.Until)],
        InvolvedCustomers = source.InvolvedCustomers,
        InvolvedOrganizations = source.InvolvedOrganizations,
        InvolvedTeams = source.InvolvedTeams,
        Resources = source.Resources,
        MarketplaceBooking = new MarketplaceBooking
        {
            Quantity = 1,
            ProductVersion = source.MarketplaceBooking!.ProductVersion,
            ProductPricing = source.MarketplaceBooking.ProductPricing,
            PaymentMethod = source.MarketplaceBooking.PaymentMethod,
            EntitlementId = source.MarketplaceBooking.EntitlementId,
        },
    };

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
        // Marketplace subscription instances are reconciled by the scheduler until an
        // administrator changes an individual occurrence. Keep that change local to
        // the instance; the parent subscription's selected weekday pattern is unchanged.
        if (existingBooking.RecurringBooking is not null && booking.HasRecurringInstanceOverrides != true)
        {
            booking.HasRecurringInstanceOverrides = true;
        }

        var organizations = await organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(
            [
                .. booking.InvolvedOrganizations
                    .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                    .Select(item => item.Id)
                    .Distinct(),
            ],
            [
                .. booking.InvolvedOrganizations
                    .Where(item => !string.IsNullOrWhiteSpace(item.CustomDomain))
                    .Select(item => item.CustomDomain!)
                    .Distinct(),
            ],
            callingCustomer.Id,
            true,
            cancellationToken);
        var teams = await teamAuthorizationService.GetBookingInvolvedTeamAndValidatePermissionsAsync(
            [.. booking.InvolvedTeams.Select(item => item.Id).Distinct()],
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

    private static void Apply(MarketplaceBookingPatchRequest request, Shared.Models.Booking booking)
    {
        foreach (var field in request.FieldsToUpdate)
        {
            switch (field)
            {
                case MarketplaceBookingPatchField.Participants:
                    booking.InvolvedCustomers = request.Booking.InvolvedCustomers;
                    booking.InvolvedOrganizations = request.Booking.InvolvedOrganizations;
                    booking.InvolvedTeams = request.Booking.InvolvedTeams;
                    break;
                case MarketplaceBookingPatchField.Notes:
                    booking.Notes = request.Booking.Notes;
                    break;
                case MarketplaceBookingPatchField.Category:
                    booking.Category = request.Booking.Category;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(request.FieldsToUpdate), field,
                        $"Unexpected value for {nameof(request.FieldsToUpdate)}: {field}. Update enum mapping or caller input.");
            }
        }
    }
}
