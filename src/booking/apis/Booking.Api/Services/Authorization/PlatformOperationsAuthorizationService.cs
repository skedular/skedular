using Booking.Shared.Models;

namespace Booking.Api.Services.Authorization;

public interface IPlatformOperationsAuthorizationService
{
    bool IsAuthorized();
}

public sealed class PlatformOperationsAuthorizationService(IHttpContextAccessor httpContextAccessor)
    : IPlatformOperationsAuthorizationService
{
    public bool IsAuthorized() =>
        httpContextAccessor.HttpContext?.User.IsInRole(PlatformOperationsRoleConstants.Operator) == true;
}
