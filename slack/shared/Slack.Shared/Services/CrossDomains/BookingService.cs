using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Grpc;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Caching.Memory;
using Slack.Shared.Mappers;
using Slack.Shared.Models;
using Booking = Slack.Shared.Models.Booking;
using BookingCategory = Api.Shared.Services.Models.BookingCategory;
using BookingEdge = Slack.Shared.Models.BookingEdge;
using BookingOrderField = Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingOrderField;
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
    Task<Booking> AddPrivateAsync(string workspaceMemberId, Booking booking, CancellationToken cancellationToken);
    Task<Booking> UpdatePrivateAsync(string workspaceMemberId, Booking booking, CancellationToken cancellationToken);
    Task DeletePrivateAsync(string workspaceMemberId, string bookingId, CancellationToken cancellationToken);

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
    IMemoryCache memoryCache,
    IOrganizationService organizationService,
    ICustomerService customerService,
    ILocationService locationService,
    ITeamService teamService,
    ILocationResourceService locationResourceService)
    : IBookingService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new() { SlidingExpiration = TimeSpan.FromSeconds(30) };

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
                Category = bookingSearchCriteria.Category.ToNullableBookingCategory().ToSafeString(),
                IncludeMineOnly = bookingSearchCriteria.IncludeMineOnly ?? false,
                IncludeFutureBookingsOnly = bookingSearchCriteria.IncludeFutureBookingsOnly ?? false,
                OrganizationId = bookingSearchCriteria.OrganizationId.ToSafeString()
            }
        };

        adminGetPaginatedBookingsInput.Where.PaymentStatuses.Add(bookingSearchCriteria.PaymentStatuses.Select(item => item.ToPaymentStatus()));
        adminGetPaginatedBookingsInput.Where.LocationIds.Add(bookingSearchCriteria.LocationIds);
        adminGetPaginatedBookingsInput.Where.TeamIds.Add(bookingSearchCriteria.TeamIds);
        adminGetPaginatedBookingsInput.Where.CustomerIds.Add(bookingSearchCriteria.CustomerIds);

        adminGetPaginatedBookingsInput.OrderBy.Add(new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From });

        var connection = await bookingServiceClient.Admin_GetPaginatedBookingsAsync(
            adminGetPaginatedBookingsInput,
            bookingConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        var edges = connection.Edges.Select(item => new BookingEdge(mapper.MapTo(item.Node), item.Cursor)).ToList();

        Cache(edges.Select(item => item.Node).ToList());

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
            (await memoryCache.GetOrCreateAsync(
                CreateKeyById(bookingId),
                async _ => mapper.MapTo(
                    await bookingServiceClient.GetAsync(
                        new GetInput { Id = bookingId },
                        bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                        cancellationToken: cancellationToken)),
                _cacheEntryOptions))!,
            cancellationToken);

    public async Task<Booking> AddPrivateAsync(string workspaceMemberId, Booking booking, CancellationToken cancellationToken)
    {
        var addInput = new AddPrivateInput
        {
            Id = booking.Id,
            From = booking.From.ToTimestamp(),
            Until = booking.Until.ToTimestamp(),
            Category = booking.Category switch
            {
                BookingCategory.WorkingFromHome => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WorkingFromHome,
                BookingCategory.WorkingFromOffice => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WorkingFromOffice,
                BookingCategory.WorkingFromCoworkingSpace => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WorkingFromCoworkingSpace,
                BookingCategory.SickLeave => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.SickLeave,
                BookingCategory.AnnualLeave => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.AnnualLeave,
                BookingCategory.WellbeingLeave => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WellbeingLeave,
                BookingCategory.ClientOffice => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.ClientOffice,
                BookingCategory.Vacation => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.Vacation,
                BookingCategory.TravelingForWork => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.TravelingForWork,
                BookingCategory.NonWorkingDay => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Notes = booking.Notes.ToSafeString()
        };

        addInput.CustomerIds.AddRange(booking.InvolvedCustomers.Select(item => item.Id));
        addInput.OrganizationIds.AddRange(booking.InvolvedOrganizations.Select(item => item.Id));
        addInput.TeamIds.AddRange(booking.InvolvedTeams.Select(item => item.Id));
        addInput.ResourceIds.AddRange(booking.Resources.Select(item => item.Id));

        var mappedBooking = mapper.MapTo(
            await bookingServiceClient.AddPrivateAsync(
                addInput,
                bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        Cache([mappedBooking]);

        return await EnrichAsync(workspaceMemberId, mappedBooking, cancellationToken);
    }

    public async Task<Booking> UpdatePrivateAsync(string workspaceMemberId, Booking booking, CancellationToken cancellationToken)
    {
        var updateInput = new UpdatePrivateInput
        {
            Id = booking.Id,
            From = booking.From.ToTimestamp(),
            Until = booking.Until.ToTimestamp(),
            Category = booking.Category switch
            {
                BookingCategory.WorkingFromHome => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WorkingFromHome,
                BookingCategory.WorkingFromOffice => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WorkingFromOffice,
                BookingCategory.WorkingFromCoworkingSpace => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WorkingFromCoworkingSpace,
                BookingCategory.SickLeave => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.SickLeave,
                BookingCategory.AnnualLeave => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.AnnualLeave,
                BookingCategory.WellbeingLeave => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WellbeingLeave,
                BookingCategory.ClientOffice => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.ClientOffice,
                BookingCategory.Vacation => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.Vacation,
                BookingCategory.TravelingForWork => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.TravelingForWork,
                BookingCategory.NonWorkingDay => Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Notes = booking.Notes.ToSafeString()
        };

        updateInput.CustomerIds.AddRange(booking.InvolvedCustomers.Select(item => item.Id));
        updateInput.OrganizationIds.AddRange(booking.InvolvedOrganizations.Select(item => item.Id));
        updateInput.TeamIds.AddRange(booking.InvolvedTeams.Select(item => item.Id));
        updateInput.ResourceIds.AddRange(booking.Resources.Select(item => item.Id));

        var mappedBooking = mapper.MapTo(
            await bookingServiceClient.UpdatePrivateAsync(
                updateInput,
                bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        Cache([mappedBooking]);

        return await EnrichAsync(workspaceMemberId, mappedBooking, cancellationToken);
    }

    public async Task DeletePrivateAsync(string workspaceMemberId, string bookingId, CancellationToken cancellationToken)
    {
        await bookingServiceClient.DeletePrivateAsync(
            new DeletePrivateInput { Id = bookingId },
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(bookingId);

        memoryCache.Remove(key);
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
                Category = bookingSearchCriteria.Category.ToNullableBookingCategory().ToSafeString(),
                IncludeMineOnly = bookingSearchCriteria.IncludeMineOnly ?? false,
                IncludeFutureBookingsOnly = bookingSearchCriteria.IncludeFutureBookingsOnly ?? false,
                OrganizationId = bookingSearchCriteria.OrganizationId.ToSafeString()
            }
        };

        getPaginatedBookingsInput.Where.PaymentStatuses.Add(bookingSearchCriteria.PaymentStatuses.Select(item => item.ToPaymentStatus()));
        getPaginatedBookingsInput.Where.LocationIds.Add(bookingSearchCriteria.LocationIds);
        getPaginatedBookingsInput.Where.TeamIds.Add(bookingSearchCriteria.TeamIds);
        getPaginatedBookingsInput.Where.CustomerIds.Add(bookingSearchCriteria.CustomerIds);

        getPaginatedBookingsInput.OrderBy.Add(new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From });

        var connection = await bookingServiceClient.GetPaginatedBookingsAsync(
            getPaginatedBookingsInput,
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);
        var edges = connection.Edges.Select(item => new BookingEdge(mapper.MapTo(item.Node), item.Cursor)).ToList();

        Cache(edges.Select(item => item.Node).ToList());

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

    private void Cache(ICollection<Booking> bookings)
    {
        foreach (var booking in bookings)
        {
            var key = CreateKeyById(booking.Id);

            memoryCache.Remove(key);
            memoryCache.Set(key, booking, _cacheEntryOptions);
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
            booking.InvolvedLocations
                .Select(item => item.Id)
                .Distinct()
                .Select(item => locationService.AdminGetAsync(item, cancellationToken)));

        var teams = await Task.WhenAll(
            booking.InvolvedTeams
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
            booking.InvolvedLocations
                .Select(item => item.Id)
                .Distinct()
                .Select(item => locationService.GetAsync(workspaceMemberId, item, cancellationToken)));

        var teams = await Task.WhenAll(
            booking.InvolvedTeams
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
