using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Grpc;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Caching.Hybrid;
using Slack.Shared.Mappers;
using Booking = Slack.Shared.Models.Booking;
using BookingEdge = Slack.Shared.Models.BookingEdge;
using BookingType = Api.Shared.Services.Models.BookingType;
using PageInfo = Enterprise.Shared.GraphQL.Types.PageInfo;

namespace Slack.Shared.Services.CrossDomains;

public interface IBookingService
{
    Task<Booking> GetAsync(string workspaceMemberId, string bookingId, CancellationToken cancellationToken);
    Task<Booking> AddAsync(string workspaceMemberId, Booking booking, CancellationToken cancellationToken);
    Task<Booking> UpdateAsync(string workspaceMemberId, Booking booking, CancellationToken cancellationToken);
    Task RemoveAsync(string workspaceMemberId, string bookingId, CancellationToken cancellationToken);

    Task<Connection<BookingEdge>> GetPaginatedBookingsAsync(
        string workspaceMemberId,
        string organizationId,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken);
}

public class BookingService(
    ApplicationConfiguration applicationConfiguration,
    BookingConfiguration bookingConfiguration,
    Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingService.BookingServiceClient bookingServiceClient,
    IMapper mapper,
    HybridCache hybridCache)
    : IBookingService
{
    public async Task<Booking> GetAsync(string workspaceMemberId, string bookingId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(bookingId),
            async ct => mapper.MapTo(
                await bookingServiceClient.GetAsync(
                    new GetInput { Id = bookingId },
                    bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

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

        return mappedBooking;
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

        return mappedBooking;
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
        string organizationId,
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
            Where = new BookingWhereInput()
        };

        getPaginatedBookingsInput.Where.OrganizationIds.Add(organizationId);

        getPaginatedBookingsInput.OrderBy.Add(new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From });

        var connection = await bookingServiceClient.GetPaginatedBookingsAsync(
            getPaginatedBookingsInput,
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

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
            Edges = connection.Edges.Select(item => new BookingEdge(mapper.MapTo(item.Node), item.Cursor)).ToList()
        };

        await CacheAsync(result.Edges.Select(item => item.Node).ToList(), cancellationToken);

        return result;
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

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:booking-id:{id}";
}
