using System.Security.Claims;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace Booking.Api.UnitTests.Services.Authorization;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class PlatformOperationsAuthorizationServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Allow_The_Explicit_Platform_Operator_Role(
        [Frozen]
        IHttpContextAccessor httpContextAccessor,
        PlatformOperationsAuthorizationService sut)
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.Role, PlatformOperationsRoleConstants.Operator)],
                "test")),
        };
        A.CallTo(() => httpContextAccessor.HttpContext).Returns(context);

        sut.IsAuthorized().ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Deny_Organization_Roles(
        [Frozen]
        IHttpContextAccessor httpContextAccessor,
        PlatformOperationsAuthorizationService sut)
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.Role, "Administrator")],
                "test")),
        };
        A.CallTo(() => httpContextAccessor.HttpContext).Returns(context);

        sut.IsAuthorized().ShouldBeFalse();
    }
}
