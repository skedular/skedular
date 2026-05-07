using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.UnitTests.Context.ContextTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SetGetStringValuesShould
{
    private static Shared.Context.Context BuildSut(IHttpContextAccessor accessor, ILogger<Shared.Context.Context> logger)
    {
        var httpContext = new DefaultHttpContext();
        A.CallTo(() => accessor.HttpContext).Returns(httpContext);
        return new Shared.Context.Context(accessor, logger);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_correlation_id(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string value)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetCorrelationId(value);
        sut.GetCorrelationId().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_verifiable_token(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string value)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetVerifiableToken(value);
        sut.GetVerifiableToken().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_designation(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string value)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetDesignation(value);
        sut.GetDesignation().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_title(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string value)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetTitle(value);
        sut.GetTitle().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_name(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string value)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetName(value);
        sut.GetName().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_given_name(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string value)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetGivenName(value);
        sut.GetGivenName().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_middle_name(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string value)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetMiddleName(value);
        sut.GetMiddleName().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_family_name(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string value)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetFamilyName(value);
        sut.GetFamilyName().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_email(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string value)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetEmail(value);
        sut.GetEmail().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_email_verified(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetEmailVerified(true);
        sut.GetEmailVerified().ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_timezone(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string value)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetTimezone(value);
        sut.GetTimezone().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_locale(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string value)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetLocale(value);
        sut.GetLocale().ShouldBe(value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_photo_urls(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string url)
    {
        var sut = BuildSut(accessor, logger);

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
    public void Set_and_get_azure_tenant_id(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        Guid tenantId)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetAzureTenantId(tenantId);
        sut.GetAzureTenantId().ShouldBe(tenantId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_and_get_azure_tenant_audience(
        [Frozen] IHttpContextAccessor accessor,
        [Frozen] ILogger<Shared.Context.Context> logger,
        string value)
    {
        var sut = BuildSut(accessor, logger);
        sut.SetAzureTenantAudience(value);
        sut.GetAzureTenantAudience().ShouldBe(value);
    }
}
