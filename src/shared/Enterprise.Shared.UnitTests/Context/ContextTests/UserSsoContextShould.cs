using Enterprise.Shared.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.UnitTests.Context.ContextTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UserSsoContextShould
{
    private static Shared.Context.Context BuildSut(IHttpContextAccessor accessor, ILogger<Shared.Context.Context> logger)
    {
        var httpContext = new DefaultHttpContext();
        A.CallTo(() => accessor.HttpContext).Returns(httpContext);
        return new Shared.Context.Context(accessor, logger);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Add_and_retrieve_user_sso_context(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string organizationId, string email)
    {
        var sut = BuildSut(accessor, logger);
        var userSso = new UserSsoContext(email);

        sut.AddUserSsoContext(organizationId, userSso);

        var result = sut.GetUserSsoContext(organizationId);
        result.ShouldNotBeNull();
        result.Email.ShouldBe(email);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_null_when_organization_not_found(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string organizationId)
    {
        var sut = BuildSut(accessor, logger);

        sut.GetUserSsoContext(organizationId).ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Overwrite_existing_sso_context(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string organizationId, string email1, string email2)
    {
        var sut = BuildSut(accessor, logger);

        sut.AddUserSsoContext(organizationId, new UserSsoContext(email1));
        sut.AddUserSsoContext(organizationId, new UserSsoContext(email2));

        sut.GetUserSsoContext(organizationId)!.Email.ShouldBe(email2);
    }
}
