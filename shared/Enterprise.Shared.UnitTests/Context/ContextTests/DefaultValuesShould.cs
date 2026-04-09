using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.UnitTests.Context.ContextTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DefaultValuesShould
{
    [Fact]
    public void Return_empty_string_for_correlation_id_when_no_http_context()
    {
        var accessor = A.Fake<IHttpContextAccessor>();
        A.CallTo(() => accessor.HttpContext).Returns(null);
        var sut = new Shared.Context.Context(accessor);

        sut.GetCorrelationId().ShouldBe(string.Empty);
    }

    [Fact]
    public void Return_empty_string_for_missing_string_keys()
    {
        var httpContext = new DefaultHttpContext();
        var accessor = A.Fake<IHttpContextAccessor>();
        A.CallTo(() => accessor.HttpContext).Returns(httpContext);
        var sut = new Shared.Context.Context(accessor);

        sut.GetDesignation().ShouldBe(string.Empty);
        sut.GetTitle().ShouldBe(string.Empty);
        sut.GetName().ShouldBe(string.Empty);
        sut.GetGivenName().ShouldBe(string.Empty);
        sut.GetMiddleName().ShouldBe(string.Empty);
        sut.GetFamilyName().ShouldBe(string.Empty);
        sut.GetPhotoUrl().ShouldBe(string.Empty);
        sut.GetPhotoUrl24().ShouldBe(string.Empty);
        sut.GetPhotoUrl32().ShouldBe(string.Empty);
        sut.GetPhotoUrl48().ShouldBe(string.Empty);
        sut.GetPhotoUrl72().ShouldBe(string.Empty);
        sut.GetPhotoUrl192().ShouldBe(string.Empty);
        sut.GetPhotoUrl512().ShouldBe(string.Empty);
        sut.GetEmail().ShouldBe(string.Empty);
        sut.GetEmailVerified().ShouldBeFalse();
        sut.GetTimezone().ShouldBe(string.Empty);
        sut.GetLocale().ShouldBe(string.Empty);
        sut.GetAzureTenantId().ShouldBe(Guid.Empty);
        sut.GetAzureTenantAudience().ShouldBe(string.Empty);
        sut.GetVerifiableToken().ShouldBe(string.Empty);
    }
}
