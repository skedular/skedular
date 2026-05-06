using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Memory;
using Slack.Shared.Mappers;
using Slack.Shared.Models;

namespace Slack.Shared.Services.CrossDomains;

public interface IBookingPermissionsService
{
    Task<OrganizationBookingPermissions> GetOrganizationPermissionsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken);

    Task<TeamBookingPermissions> GetTeamPermissionsAsync(string workspaceMemberId, string teamId, CancellationToken cancellationToken);
}

public class BookingPermissionsService(
    ApplicationConfiguration applicationConfiguration,
    BookingConfiguration bookingConfiguration,
    Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingService.BookingServiceClient bookingServiceClient,
    IMapper mapper,
    IMemoryCache memoryCache) : IBookingPermissionsService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new() { SlidingExpiration = TimeSpan.FromSeconds(30) };

    public async Task<OrganizationBookingPermissions> GetOrganizationPermissionsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateOrganizationKeyById(workspaceMemberId, organizationId),
            async _ => mapper.MapTo(
                await bookingServiceClient.GetOrganizationPermissionsAsync(
                    new GetOrganizationPermissionsInput { OrganizationId = organizationId },
                    bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    public async Task<TeamBookingPermissions> GetTeamPermissionsAsync(string workspaceMemberId, string teamId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateTeamKeyById(workspaceMemberId, teamId),
            async _ => mapper.MapTo(
                await bookingServiceClient.GetTeamPermissionsAsync(
                    new GetTeamPermissionsInput { TeamId = teamId },
                    bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    private string CreateOrganizationKeyById(string workspaceMemberId, string organizationId) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:bookingorganizationpermissions-id:{workspaceMemberId}:{organizationId}";

    private string CreateTeamKeyById(string workspaceMemberId, string teamId) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:bookingteampermissions-id:{workspaceMemberId}:{teamId}";
}
