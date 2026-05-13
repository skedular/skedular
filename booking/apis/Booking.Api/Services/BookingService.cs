using Api.Shared.Services;
using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;

namespace Booking.Api.Services;

public interface IBookingService
{
    Task<Shared.Models.Booking> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<OrganizationArrearsInvoice>> GetArrearsInvoicesAsync(string bookingId, CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<Edge<Shared.Models.Booking>>, int)> GetPaginatedBookingsAsync(
        PaginationInputParam paginationInputParam,
        BookingSearchCriteria searchCriteria,
        IReadOnlyList<BookingOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);
}

public class BookingService(
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IGraphQlMapper graphQlMapper,
    IEntityMapper sharedEntityMapper,
    ICachedBookingService cachedBookingService) : IBookingService
{
    public async Task<Shared.Models.Booking> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var booking = await cachedBookingService.GetByIdAsync(id, cancellationToken) ?? throw new BookingNotFound();

        await EnsureCustomerCanViewBookingAsync(booking, customerId, cancellationToken);

        return sharedEntityMapper.MapTo(booking);
    }

    public async Task<IReadOnlyList<OrganizationArrearsInvoice>> GetArrearsInvoicesAsync(string bookingId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bookingId);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var booking = await cachedBookingService.GetByIdAsync(bookingId, cancellationToken) ?? throw new BookingNotFound();
        await EnsureCustomerCanViewBookingAsync(booking, customerId, cancellationToken);

        var invoices = await repositoryFactory.OrganizationArrearsInvoiceRepository.GetByBookingIdUntrackedAsync(bookingId, cancellationToken);
        return invoices.Select(sharedEntityMapper.MapTo).ToList();
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<Shared.Models.Booking>>, int)> GetPaginatedBookingsAsync(
        PaginationInputParam paginationInputParam,
        BookingSearchCriteria searchCriteria,
        IReadOnlyList<BookingOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        string? customerId = null;
        if (!ignoreAuthorizationCheck)
        {
            customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customerId) && searchCriteria.IncludeMineOnly == true)
        {
            searchCriteria = searchCriteria with { CustomerIds = [customerId] };
        }

        BookingAccessScope? accessScope = null;
        List<string>? organizationIds = null;
        List<string>? locationIds = null;
        List<string>? teamIds = null;

        var requestedOwnBookingsOnly = IsRequestedOwnBookingsOnly(searchCriteria, customerId);
        var requestedOtherCustomersBookings = IsRequestedOtherCustomersBookings(searchCriteria, customerId);
        var scopedOrganization = await GetScopedOrganizationAsync(searchCriteria, cancellationToken);

        if (requestedOtherCustomersBookings && scopedOrganization is null)
        {
            throw new InvalidOperationException("To view other people's bookings, narrow your search to a specific organisation first.");
        }

        if (!string.IsNullOrWhiteSpace(customerId) &&
            scopedOrganization is not null &&
            (requestedOtherCustomersBookings || !requestedOwnBookingsOnly))
        {
            if (!await organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(scopedOrganization.Id, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        if (scopedOrganization is not null)
        {
            accessScope = new BookingAccessScope(
                [scopedOrganization.Id],
                scopedOrganization.Locations.Where(item => !item.DeletedAt.HasValue).Select(item => item.Id).ToList(),
                scopedOrganization.Teams.Where(item => !item.DeletedAt.HasValue).Select(item => item.Id).ToList());

            searchCriteria = searchCriteria with
            {
                OrganizationId = scopedOrganization.Id, OrganizationCustomDomain = scopedOrganization.CustomDomain
            };
        }

        if (!string.IsNullOrWhiteSpace(customerId) && searchCriteria.LocationIds.Count != 0)
        {
            var locations =
                await repositoryFactory.LocationRepository.GetActiveByIdsAsync(searchCriteria.LocationIds.Distinct().ToList(), cancellationToken);

            foreach (var location in locations)
            {
                if (organizationIds is null)
                {
                    organizationIds = await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
                }

                if (location.Organization is null || !organizationIds.Contains(location.Organization.Id))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            locationIds = searchCriteria.LocationIds.Distinct().ToList();
        }

        if (!string.IsNullOrWhiteSpace(customerId) && searchCriteria.TeamIds.Count != 0)
        {
            var teams = await repositoryFactory.TeamRepository.GetActiveByIdsAsync(searchCriteria.TeamIds.Distinct().ToList(), cancellationToken);

            foreach (var team in teams)
            {
                if (organizationIds is null)
                {
                    organizationIds = await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
                }

                if (team.Organization is null || !organizationIds.Contains(team.Organization.Id))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            teamIds = searchCriteria.TeamIds.Distinct().ToList();
        }

        if (accessScope is null &&
            !string.IsNullOrWhiteSpace(customerId) &&
            (organizationIds?.Count > 0 || locationIds?.Count > 0 || teamIds?.Count > 0))
        {
            accessScope = new BookingAccessScope(
                organizationIds ?? [],
                locationIds ?? [],
                teamIds ?? []);
        }

        if (!string.IsNullOrWhiteSpace(customerId) &&
            (!searchCriteria.IncludeMineOnly.HasValue || !searchCriteria.IncludeMineOnly.Value) &&
            string.IsNullOrWhiteSpace(searchCriteria.OrganizationId) &&
            string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain) &&
            searchCriteria.LocationIds.Count == 0 &&
            searchCriteria.TeamIds.Count == 0)
        {
            organizationIds ??= await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
            locationIds ??= await GetCustomerLocationIdsAsync(customerId, cancellationToken);
            teamIds ??= await GetCustomerTeamIdsAsync(customerId, cancellationToken);

            if (organizationIds.Count == 0 && locationIds.Count == 0 && teamIds.Count == 0)
            {
                return (new PaginatedInfo(false, false, null, null), [], 0);
            }

            accessScope = new BookingAccessScope(organizationIds, locationIds, teamIds);
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.BookingRepository.GetPaginatedBookingsUntrackedAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            accessScope,
            cancellationToken);

        return (paginatedInfo, edges.Select(graphQlMapper.MapTo).ToList(), totalCount);
    }

    private async Task<List<string>> GetCustomerOrganizationIdsAsync(
        string customerId,
        CancellationToken cancellationToken)
    {
        var organizations = await repositoryFactory.OrganizationRepository.GetByCustomerIdAsync(customerId, false, false, cancellationToken);
        var result = new List<string>();
        foreach (var organization in organizations)
        {
            if (await organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(organization.Id, customerId, cancellationToken))
            {
                result.Add(organization.Id);
            }
        }

        return result;
    }

    private async Task<List<string>> GetCustomerLocationIdsAsync(string customerId, CancellationToken cancellationToken)
    {
        var locations = await repositoryFactory.LocationRepository.GetByCustomerIdAsync(customerId, false, cancellationToken);
        return locations.Select(item => item.Id).ToList();
    }

    private async Task<List<string>> GetCustomerTeamIdsAsync(string customerId, CancellationToken cancellationToken)
    {
        var teams = await repositoryFactory.TeamRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return teams.Select(item => item.Id).ToList();
    }

    private async Task EnsureCustomerCanViewBookingAsync(
        Shared.Database.Entities.Booking booking,
        string customerId,
        CancellationToken cancellationToken)
    {
        if (booking.InvolvedCustomers.Any(item => item.Id == customerId))
        {
            return;
        }

        var organizationIds = booking.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
        if (organizationIds.Count != 0)
        {
            var organizationEntities = await repositoryFactory.OrganizationRepository.GetByIdsOrCustomDomainsAsync(
                organizationIds,
                null,
                false,
                false,
                cancellationToken);
            foreach (var organization in organizationEntities)
            {
                if (await organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(organization.Id, customerId, cancellationToken))
                {
                    return;
                }
            }
        }

        var teamIds = booking.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
        if (teamIds.Count != 0)
        {
            var teamEntities = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, false, cancellationToken);
            foreach (var team in teamEntities)
            {
                if (team.Organization is not null &&
                    await organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(team.Organization.Id, customerId, cancellationToken))
                {
                    return;
                }
            }
        }

        throw new UnauthorizedAccessException();
    }

    private async Task<OrganizationEntity?> GetScopedOrganizationAsync(BookingSearchCriteria searchCriteria, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(searchCriteria.OrganizationId) && string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain))
        {
            return null;
        }

        return await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                   searchCriteria.OrganizationId,
                   searchCriteria.OrganizationCustomDomain,
                   false,
                   false,
                   cancellationToken) ??
               throw new OrganizationNotFound();
    }

    private static bool IsRequestedOwnBookingsOnly(BookingSearchCriteria searchCriteria, string? customerId) =>
        !string.IsNullOrWhiteSpace(customerId) &&
        searchCriteria.CustomerIds.Count != 0 &&
        searchCriteria.CustomerIds.All(item => item == customerId);

    private static bool IsRequestedOtherCustomersBookings(BookingSearchCriteria searchCriteria, string? customerId) =>
        !string.IsNullOrWhiteSpace(customerId) &&
        searchCriteria.CustomerIds.Count != 0 &&
        searchCriteria.CustomerIds.Any(item => item != customerId);
}
