using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Grpc;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Caching.Hybrid;
using Slack.Shared.Mappers;
using Slack.Shared.Models;
using Booking = Slack.Shared.Models.Booking;
using BookingEdge = Slack.Shared.Models.BookingEdge;
using BookingOrderField = Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingOrderField;
using BookingType = Api.Shared.Services.Models.BookingType;
using PageInfo = Enterprise.Shared.GraphQL.Types.PageInfo;
using Resource = Slack.Shared.Models.Resource;

namespace Slack.Shared.Services.CrossDomains;

public interface IBookingService
{
    Task<Connection<BookingEdge>> Admin_GetPaginatedBookingsAsync(
        BookingSearchCriteria bookingSearchCriteria,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken);

    Task<Booking> GetAsync(string workspaceMemberId, string bookingId, CancellationToken cancellationToken);
    Task<Booking> AddAsync(string workspaceMemberId, Booking booking, CancellationToken cancellationToken);
    Task<Booking> UpdateAsync(string workspaceMemberId, Booking booking, CancellationToken cancellationToken);
    Task RemoveAsync(string workspaceMemberId, string bookingId, CancellationToken cancellationToken);

    Task<Connection<BookingEdge>> GetPaginatedBookingsAsync(
        string workspaceMemberId,
        BookingSearchCriteria bookingSearchCriteria,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken);

    Task<ICollection<Resource>> GetAvailableResourcesAsync(
        string workspaceMemberId,
        string organizationId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> resourceIdsToInclude,
        CancellationToken cancellationToken);
}

public class BookingService(
    ApplicationConfiguration applicationConfiguration,
    BookingConfiguration bookingConfiguration,
    Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingService.BookingServiceClient bookingServiceClient,
    IMapper mapper,
    HybridCache hybridCache,
    IOrganizationService organizationService,
    ICustomerService customerService,
    ILocationService locationService,
    ITeamService teamService,
    ILocationResourceService locationResourceService)
    : IBookingService
{
    public async Task<Connection<BookingEdge>> Admin_GetPaginatedBookingsAsync(
        BookingSearchCriteria bookingSearchCriteria,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken)
    {
        var adminGetPaginatedBookingsInput = new Admin_GetPaginatedBookingsInput
        {
            First = first.ToNullInt(),
            After = after.ToSafeString(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new BookingWhereInput
            {
                FromGt = bookingSearchCriteria.FromGt?.ToTimestamp(),
                FromGte = bookingSearchCriteria.FromGte?.ToTimestamp(),
                FromLt = bookingSearchCriteria.FromLt?.ToTimestamp(),
                FromLte = bookingSearchCriteria.FromLte?.ToTimestamp(),
                ToGt = bookingSearchCriteria.ToGt?.ToTimestamp(),
                ToGte = bookingSearchCriteria.ToGte?.ToTimestamp(),
                ToLt = bookingSearchCriteria.ToLt?.ToTimestamp(),
                ToLte = bookingSearchCriteria.ToLte?.ToTimestamp(),
                NotesContains = bookingSearchCriteria.NotesContains.ToSafeString(),
                NameContains = bookingSearchCriteria.NameContains.ToSafeString(),
                Type = bookingSearchCriteria.Type.ToNullableBookingType().ToSafeString(),
                IncludeMineOnly = bookingSearchCriteria.IncludeMineOnly ?? false,
                IncludeFutureBookingsOnly = bookingSearchCriteria.IncludeFutureBookingsOnly ?? false
            }
        };

        adminGetPaginatedBookingsInput.Where.PaymentStatuses.Add(bookingSearchCriteria.PaymentStatuses.Select(item => item.ToPaymentStatus()));
        adminGetPaginatedBookingsInput.Where.OrganizationIds.Add(bookingSearchCriteria.OrganizationIds);
        adminGetPaginatedBookingsInput.Where.LocationIds.Add(bookingSearchCriteria.LocationIds);
        adminGetPaginatedBookingsInput.Where.TeamIds.Add(bookingSearchCriteria.TeamIds);
        adminGetPaginatedBookingsInput.Where.CustomerIds.Add(bookingSearchCriteria.CustomerIds);

        adminGetPaginatedBookingsInput.OrderBy.Add(new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From });

        var connection = await bookingServiceClient.Admin_GetPaginatedBookingsAsync(
            adminGetPaginatedBookingsInput,
            bookingConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        var edges = connection.Edges.Select(item => new BookingEdge(mapper.MapTo(item.Node), item.Cursor)).ToList();

        await CacheAsync(edges.Select(item => item.Node).ToList(), cancellationToken);

        var enrichedEdges = new List<BookingEdge>();
        foreach (var item in edges)
        {
            enrichedEdges.Add(new BookingEdge(await AdminEnrichAsync(item.Node, cancellationToken), item.Cursor));
        }

        var result = new Connection<BookingEdge>
        {
            PageInfo = new PageInfo
            {
                StartCursor = connection.PageInfo.StartCursor,
                EndCursor = connection.PageInfo.EndCursor,
                HasNextPage = connection.PageInfo.HasNextPage,
                HasPreviousPage = connection.PageInfo.HasPreviousPage
            },
            TotalCount = connection.TotalCount,
            Edges = enrichedEdges
        };

        return result;
    }

    public async Task<Booking> GetAsync(string workspaceMemberId, string bookingId, CancellationToken cancellationToken) =>
        await EnrichAsync(
            workspaceMemberId,
            await hybridCache.GetOrCreateAsync(
                CreateKeyById(bookingId),
                async ct => mapper.MapTo(
                    await bookingServiceClient.GetAsync(
                        new GetInput { Id = bookingId },
                        bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                        cancellationToken: ct)),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
                cancellationToken: cancellationToken),
            cancellationToken);

    public async Task<Booking> AddAsync(string workspaceMemberId, Booking booking, CancellationToken cancellationToken)
    {
        var addInput = new AddInput
        {
            Id = booking.Id,
            From = booking.From.ToTimestamp(),
            Until = booking.Until.ToTimestamp(),
            Type = booking.Type switch
            {
                BookingType.WorkingFromHome => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromHome,
                BookingType.WorkingFromOffice => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromOffice,
                BookingType.WorkingFromCoworkingSpace => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromCoworkingSpace,
                BookingType.SickLeave => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.SickLeave,
                BookingType.AnnualLeave => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.AnnualLeave,
                BookingType.WellbeingLeave => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WellbeingLeave,
                BookingType.ClientOffice => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.ClientOffice,
                BookingType.Vacation => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.Vacation,
                BookingType.TravelingForWork => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.TravelingForWork,
                BookingType.NonWorkingDay => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Notes = booking.Notes.ToSafeString()
        };

        addInput.CustomerIds.AddRange(booking.InvolvedCustomers.Select(item => item.Id));
        addInput.OrganizationIds.AddRange(booking.InvolvedOrganizations.Select(item => item.Id));
        addInput.TeamIds.AddRange(booking.InvolvedTeams.Select(item => item.Id));

        var mappedBooking = mapper.MapTo(
            await bookingServiceClient.AddAsync(
                addInput,
                bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedBooking], cancellationToken);

        return await EnrichAsync(workspaceMemberId, mappedBooking, cancellationToken);
    }

    public async Task<Booking> UpdateAsync(string workspaceMemberId, Booking booking, CancellationToken cancellationToken)
    {
        var updateInput = new UpdateInput
        {
            Id = booking.Id,
            From = booking.From.ToTimestamp(),
            Until = booking.Until.ToTimestamp(),
            Type = booking.Type switch
            {
                BookingType.WorkingFromHome => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromHome,
                BookingType.WorkingFromOffice => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromOffice,
                BookingType.WorkingFromCoworkingSpace => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromCoworkingSpace,
                BookingType.SickLeave => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.SickLeave,
                BookingType.AnnualLeave => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.AnnualLeave,
                BookingType.WellbeingLeave => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WellbeingLeave,
                BookingType.ClientOffice => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.ClientOffice,
                BookingType.Vacation => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.Vacation,
                BookingType.TravelingForWork => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.TravelingForWork,
                BookingType.NonWorkingDay => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Notes = booking.Notes.ToSafeString()
        };

        updateInput.CustomerIds.AddRange(booking.InvolvedCustomers.Select(item => item.Id));
        updateInput.OrganizationIds.AddRange(booking.InvolvedOrganizations.Select(item => item.Id));
        updateInput.TeamIds.AddRange(booking.InvolvedTeams.Select(item => item.Id));

        var mappedBooking = mapper.MapTo(
            await bookingServiceClient.UpdateAsync(
                updateInput,
                bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedBooking], cancellationToken);

        return await EnrichAsync(workspaceMemberId, mappedBooking, cancellationToken);
    }

    public async Task RemoveAsync(string workspaceMemberId, string bookingId, CancellationToken cancellationToken)
    {
        await bookingServiceClient.DeleteAsync(
            new DeleteInput { Id = bookingId },
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(bookingId);

        await hybridCache.RemoveAsync(key, cancellationToken);
    }

    public async Task<Connection<BookingEdge>> GetPaginatedBookingsAsync(
        string workspaceMemberId,
        BookingSearchCriteria bookingSearchCriteria,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken)
    {
        var getPaginatedBookingsInput = new GetPaginatedBookingsInput
        {
            First = first.ToNullInt(),
            After = after.ToSafeString(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new BookingWhereInput
            {
                FromGt = bookingSearchCriteria.FromGt?.ToTimestamp(),
                FromGte = bookingSearchCriteria.FromGte?.ToTimestamp(),
                FromLt = bookingSearchCriteria.FromLt?.ToTimestamp(),
                FromLte = bookingSearchCriteria.FromLte?.ToTimestamp(),
                ToGt = bookingSearchCriteria.ToGt?.ToTimestamp(),
                ToGte = bookingSearchCriteria.ToGte?.ToTimestamp(),
                ToLt = bookingSearchCriteria.ToLt?.ToTimestamp(),
                ToLte = bookingSearchCriteria.ToLte?.ToTimestamp(),
                NotesContains = bookingSearchCriteria.NotesContains.ToSafeString(),
                NameContains = bookingSearchCriteria.NameContains.ToSafeString(),
                Type = bookingSearchCriteria.Type.ToNullableBookingType().ToSafeString(),
                IncludeMineOnly = bookingSearchCriteria.IncludeMineOnly ?? false,
                IncludeFutureBookingsOnly = bookingSearchCriteria.IncludeFutureBookingsOnly ?? false
            }
        };

        getPaginatedBookingsInput.Where.PaymentStatuses.Add(bookingSearchCriteria.PaymentStatuses.Select(item => item.ToPaymentStatus()));
        getPaginatedBookingsInput.Where.OrganizationIds.Add(bookingSearchCriteria.OrganizationIds);
        getPaginatedBookingsInput.Where.LocationIds.Add(bookingSearchCriteria.LocationIds);
        getPaginatedBookingsInput.Where.TeamIds.Add(bookingSearchCriteria.TeamIds);
        getPaginatedBookingsInput.Where.CustomerIds.Add(bookingSearchCriteria.CustomerIds);

        getPaginatedBookingsInput.OrderBy.Add(new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From });

        var connection = await bookingServiceClient.GetPaginatedBookingsAsync(
            getPaginatedBookingsInput,
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);
        var edges = connection.Edges.Select(item => new BookingEdge(mapper.MapTo(item.Node), item.Cursor)).ToList();

        await CacheAsync(edges.Select(item => item.Node).ToList(), cancellationToken);

        var enrichedEdges = new List<BookingEdge>();
        foreach (var item in edges)
        {
            enrichedEdges.Add(new BookingEdge(await EnrichAsync(workspaceMemberId, item.Node, cancellationToken), item.Cursor));
        }

        var result = new Connection<BookingEdge>
        {
            PageInfo = new PageInfo
            {
                StartCursor = connection.PageInfo.StartCursor,
                EndCursor = connection.PageInfo.EndCursor,
                HasNextPage = connection.PageInfo.HasNextPage,
                HasPreviousPage = connection.PageInfo.HasPreviousPage
            },
            TotalCount = connection.TotalCount,
            Edges = enrichedEdges
        };

        return result;
    }

    public async Task<ICollection<Resource>> GetAvailableResourcesAsync(
        string workspaceMemberId,
        string organizationId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> resourceIdsToInclude,
        CancellationToken cancellationToken)
    {
        var getAvailableResourcesInput = new GetAvailableResourcesInput
        {
            OrganizationId = organizationId, From = from.ToTimestamp(), Until = until.ToTimestamp()
        };

        getAvailableResourcesInput.ResourceIdsToInclude.AddRange(resourceIdsToInclude);

        var availableResourceIds = (await bookingServiceClient.GetAvailableResourcesAsync(
            getAvailableResourcesInput,
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken)).ResourceIds;

        return await Task.WhenAll(
            availableResourceIds
                .Distinct()
                .Select(item => locationResourceService.GetAsync(workspaceMemberId, item, cancellationToken)));
    }

    private async Task CacheAsync(ICollection<Booking> bookings, CancellationToken cancellationToken)
    {
        foreach (var booking in bookings)
        {
            var key = CreateKeyById(booking.Id);

            await hybridCache.RemoveAsync(key, cancellationToken);
            await hybridCache.SetAsync(
                key,
                booking,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
                cancellationToken: cancellationToken);
        }
    }

    private async Task<Booking> AdminEnrichAsync(Booking booking, CancellationToken cancellationToken)
    {
        var resources = await Task.WhenAll(
            booking.Resources
                .Select(item => item.Id)
                .Distinct()
                .Select(item => locationResourceService.AdminGetAsync(item, cancellationToken)));

        var customers = await Task.WhenAll(
            booking.InvolvedCustomers
                .Select(item => item.Id)
                .Distinct()
                .Select(item => customerService.AdminGetAsync(item, cancellationToken)));

        var organizations = await Task.WhenAll(
            booking.InvolvedOrganizations
                .Select(item => item.Id)
                .Distinct()
                .Select(item => organizationService.AdminGetAsync(item, cancellationToken)));

        var locations = await Task.WhenAll(
            booking.Resources
                .Select(item => item.Id)
                .Distinct()
                .Select(item => locationService.AdminGetAsync(item, cancellationToken)));

        var teams = await Task.WhenAll(
            booking.Resources
                .Select(item => item.Id)
                .Distinct()
                .Select(item => teamService.AdminGetAsync(item, cancellationToken)));

        booking.Resources = booking.Resources
            .Select(item => resources.FirstOrDefault(resource => resource.Id == item.Id) ?? item)
            .ToList();

        booking.InvolvedCustomers = booking.InvolvedCustomers
            .Select(item => customers.FirstOrDefault(resource => resource.Id == item.Id) ?? item)
            .ToList();

        booking.InvolvedOrganizations = booking.InvolvedOrganizations
            .Select(item => organizations.FirstOrDefault(resource => resource.Id == item.Id) ?? item)
            .ToList();

        booking.InvolvedLocations = booking.InvolvedLocations
            .Select(item => locations.FirstOrDefault(resource => resource.Id == item.Id) ?? item)
            .ToList();

        booking.InvolvedTeams = booking.InvolvedTeams
            .Select(item => teams.FirstOrDefault(resource => resource.Id == item.Id) ?? item)
            .ToList();

        return booking;
    }

    private async Task<Booking> EnrichAsync(string workspaceMemberId, Booking booking, CancellationToken cancellationToken)
    {
        var resources = await Task.WhenAll(
            booking.Resources
                .Select(item => item.Id)
                .Distinct()
                .Select(item => locationResourceService.GetAsync(workspaceMemberId, item, cancellationToken)));

        var customers = await Task.WhenAll(
            booking.InvolvedCustomers
                .Select(item => item.Id)
                .Distinct()
                .Select(item => customerService.GetByIdAsync(workspaceMemberId, item, cancellationToken)));

        var organizations = await Task.WhenAll(
            booking.InvolvedOrganizations
                .Select(item => item.Id)
                .Distinct()
                .Select(item => organizationService.GetAsync(workspaceMemberId, item, cancellationToken)));

        var locations = await Task.WhenAll(
            booking.Resources
                .Select(item => item.Id)
                .Distinct()
                .Select(item => locationService.GetAsync(workspaceMemberId, item, cancellationToken)));

        var teams = await Task.WhenAll(
            booking.Resources
                .Select(item => item.Id)
                .Distinct()
                .Select(item => teamService.GetAsync(workspaceMemberId, item, cancellationToken)));

        booking.Resources = booking.Resources
            .Select(item => resources.FirstOrDefault(resource => resource.Id == item.Id) ?? item)
            .ToList();

        booking.InvolvedCustomers = booking.InvolvedCustomers
            .Select(item => customers.FirstOrDefault(resource => resource.Id == item.Id) ?? item)
            .ToList();

        booking.InvolvedOrganizations = booking.InvolvedOrganizations
            .Select(item => organizations.FirstOrDefault(resource => resource.Id == item.Id) ?? item)
            .ToList();

        booking.InvolvedLocations = booking.InvolvedLocations
            .Select(item => locations.FirstOrDefault(resource => resource.Id == item.Id) ?? item)
            .ToList();

        booking.InvolvedTeams = booking.InvolvedTeams
            .Select(item => teams.FirstOrDefault(resource => resource.Id == item.Id) ?? item)
            .ToList();

        return booking;
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:booking-id:{id}";
}
