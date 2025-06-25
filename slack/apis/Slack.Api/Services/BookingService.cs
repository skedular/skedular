using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Shared.Models;

namespace Slack.Api.Services;

public interface IBookingService
{
    ValueTask<OrganizationBookingPermissions> GetOrganizationPermissionsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken);

    ValueTask<TeamBookingPermissions> GetTeamPermissionsAsync(
        string teamId,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken);
}

public class BookingService(
    BookingConfiguration bookingConfiguration,
    IMapper mapper,
    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingService.BookingServiceClient bookingServiceClient)
    : IBookingService, IDisposable
{
    private readonly SemaphoreSlim _cachedLocationPermissionsLock = new(1, 1);
    private readonly SemaphoreSlim _cachedOrganizationPermissionsLock = new(1, 1);
    private readonly SemaphoreSlim _cachedTeamPermissionsLock = new(1, 1);
    private OrganizationBookingPermissions? _cachedOrganizationBookingPermissions;
    private TeamBookingPermissions? _cachedTeamBookingPermissions;
    private bool _disposed;

    public async ValueTask<OrganizationBookingPermissions> GetOrganizationPermissionsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        if (_cachedOrganizationBookingPermissions is not null)
        {
            return _cachedOrganizationBookingPermissions;
        }

        try
        {
            await _cachedOrganizationPermissionsLock.WaitAsync(cancellationToken);
            _cachedOrganizationBookingPermissions = mapper.MapTo(
                await bookingServiceClient.GetOrganizationPermissionsAsync(
                    new GetOrganizationPermissionsInput { OrganizationId = workspace.Organization.Id },
                    bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                    cancellationToken: cancellationToken));

            return _cachedOrganizationBookingPermissions;
        }
        finally
        {
            _cachedOrganizationPermissionsLock.Release();
        }
    }

    public async ValueTask<TeamBookingPermissions> GetTeamPermissionsAsync(
        string teamId,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        if (_cachedTeamBookingPermissions is not null)
        {
            return _cachedTeamBookingPermissions;
        }

        try
        {
            await _cachedTeamPermissionsLock.WaitAsync(cancellationToken);
            _cachedTeamBookingPermissions = mapper.MapTo(
                await bookingServiceClient.GetTeamPermissionsAsync(
                    new GetTeamPermissionsInput { TeamId = teamId },
                    bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                    cancellationToken: cancellationToken));

            return _cachedTeamBookingPermissions;
        }
        finally
        {
            _cachedTeamPermissionsLock.Release();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~BookingService() => Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _cachedOrganizationPermissionsLock.Dispose();
            _cachedLocationPermissionsLock.Dispose();
            _cachedTeamPermissionsLock.Dispose();
        }

        _disposed = true;
    }
}
