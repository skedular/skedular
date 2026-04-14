using Enterprise.Shared.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.UnitTests.Context.ContextTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UserSsoContextShould
{
    private static Shared.Context.Context BuildSut()
    {
        var httpContext = new DefaultHttpContext();
        var accessor = A.Fake<IHttpContextAccessor>();
        var logger = A.Fake<ILogger<Shared.Context.Context>>();
        A.CallTo(() => accessor.HttpContext).Returns(httpContext);
        return new Shared.Context.Context(accessor, logger);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Add_and_retrieve_user_sso_context(string organizationId, string email)
    {
        var sut = BuildSut();
        var userSso = new UserSsoContext(email);

        sut.AddUserSsoContext(organizationId, userSso);

        var result = sut.GetUserSsoContext(organizationId);
        result.ShouldNotBeNull();
        result.Email.ShouldBe(email);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_null_when_organization_not_found(string organizationId)
    {
        var sut = BuildSut();

        sut.GetUserSsoContext(organizationId).ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Overwrite_existing_sso_context(string organizationId, string email1, string email2)
    {
        var sut = BuildSut();

        sut.AddUserSsoContext(organizationId, new UserSsoContext(email1));
        sut.AddUserSsoContext(organizationId, new UserSsoContext(email2));

        sut.GetUserSsoContext(organizationId)!.Email.ShouldBe(email2);
    }
}
