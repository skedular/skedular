using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Hybrid;
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
    BookingService.BookingServiceClient bookingServiceClient,
    IMapper mapper,
    HybridCache hybridCache) : IBookingPermissionsService
{
    public async Task<OrganizationBookingPermissions> GetOrganizationPermissionsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateOrganizationKeyById(workspaceMemberId, organizationId),
            async ct => mapper.MapTo(
                await bookingServiceClient.GetOrganizationPermissionsAsync(
                    new GetOrganizationPermissionsInput { OrganizationId = organizationId },
                    bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    public async Task<TeamBookingPermissions> GetTeamPermissionsAsync(string workspaceMemberId, string teamId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateTeamKeyById(workspaceMemberId, teamId),
            async ct => mapper.MapTo(
                await bookingServiceClient.GetTeamPermissionsAsync(
                    new GetTeamPermissionsInput { TeamId = teamId },
                    bookingConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    private string CreateOrganizationKeyById(string workspaceMemberId, string organizationId) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:bookingorganizationpermissions-id:{workspaceMemberId}:{organizationId}";

    private string CreateTeamKeyById(string workspaceMemberId, string teamId) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:bookingteampermissions-id:{workspaceMemberId}:{teamId}";
}
