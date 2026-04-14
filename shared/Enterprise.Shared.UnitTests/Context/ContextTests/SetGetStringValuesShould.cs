using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.UnitTests.Context.ContextTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SetGetStringValuesShould
{
    private static Shared.Context.Context BuildSut(out DefaultHttpContext httpContext)
    {
        httpContext = new DefaultHttpContext();
        var accessor = A.Fake<IHttpContextAccessor>();
        var logger = A.Fake<ILogger<Shared.Context.Context>>();
        A.CallTo(() => accessor.HttpContext).Returns(httpContext);
        return new Shared.Context.Context(accessor, logger);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_correlation_id(string value)
    {
        var sut = BuildSut(out _);
        sut.SetCorrelationId(value);
        sut.GetCorrelationId().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_verifiable_token(string value)
    {
        var sut = BuildSut(out _);
        sut.SetVerifiableToken(value);
        sut.GetVerifiableToken().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_designation(string value)
    {
        var sut = BuildSut(out _);
        sut.SetDesignation(value);
        sut.GetDesignation().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_title(string value)
    {
        var sut = BuildSut(out _);
        sut.SetTitle(value);
        sut.GetTitle().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_name(string value)
    {
        var sut = BuildSut(out _);
        sut.SetName(value);
        sut.GetName().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_given_name(string value)
    {
        var sut = BuildSut(out _);
        sut.SetGivenName(value);
        sut.GetGivenName().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_middle_name(string value)
    {
        var sut = BuildSut(out _);
        sut.SetMiddleName(value);
        sut.GetMiddleName().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_family_name(string value)
    {
        var sut = BuildSut(out _);
        sut.SetFamilyName(value);
        sut.GetFamilyName().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_email(string value)
    {
        var sut = BuildSut(out _);
        sut.SetEmail(value);
        sut.GetEmail().ShouldBe(value);
    }

    [Fact]
    public void Set_and_get_email_verified()
    {
        var sut = BuildSut(out _);
        sut.SetEmailVerified(true);
        sut.GetEmailVerified().ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_timezone(string value)
    {
        var sut = BuildSut(out _);
        sut.SetTimezone(value);
        sut.GetTimezone().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_locale(string value)
    {
        var sut = BuildSut(out _);
        sut.SetLocale(value);
        sut.GetLocale().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_photo_urls(string url)
    {
        var sut = BuildSut(out _);

        sut.SetPhotoUrl(url);
        sut.GetPhotoUrl().ShouldBe(url);

        sut.SetPhotoUrl24(url);
        sut.GetPhotoUrl24().ShouldBe(url);

        sut.SetPhotoUrl32(url);
        sut.GetPhotoUrl32().ShouldBe(url);

        sut.SetPhotoUrl48(url);
        sut.GetPhotoUrl48().ShouldBe(url);

        sut.SetPhotoUrl72(url);
        sut.GetPhotoUrl72().ShouldBe(url);

        sut.SetPhotoUrl192(url);
        sut.GetPhotoUrl192().ShouldBe(url);

        sut.SetPhotoUrl512(url);
        sut.GetPhotoUrl512().ShouldBe(url);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_azure_tenant_id(Guid tenantId)
    {
        var sut = BuildSut(out _);
        sut.SetAzureTenantId(tenantId);
        sut.GetAzureTenantId().ShouldBe(tenantId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_azure_tenant_audience(string value)
    {
        var sut = BuildSut(out _);
        sut.SetAzureTenantAudience(value);
        sut.GetAzureTenantAudience().ShouldBe(value);
    }
}
