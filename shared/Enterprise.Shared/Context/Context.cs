using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.Context;

public record UserSsoContext(string Email);

public interface IContext
{
    void SetCorrelationId(string value);
    string GetCorrelationId();
    void SetVerifiableToken(string value);
    string GetVerifiableToken();
    void SetDesignation(string value);
    string GetDesignation();
    void SetTitle(string value);
    string GetTitle();
    void SetName(string value);
    string GetName();
    void SetGivenName(string value);
    string GetGivenName();
    void SetMiddleName(string value);
    string GetMiddleName();
    void SetFamilyName(string value);
    string GetFamilyName();
    void SetPhotoUrl(string value);
    string GetPhotoUrl();
    void SetPhotoUrl24(string value);
    string GetPhotoUrl24();
    void SetPhotoUrl32(string value);
    string GetPhotoUrl32();
    void SetPhotoUrl48(string value);
    string GetPhotoUrl48();
    void SetPhotoUrl72(string value);
    string GetPhotoUrl72();
    void SetPhotoUrl192(string value);
    string GetPhotoUrl192();
    void SetPhotoUrl512(string value);
    string GetPhotoUrl512();
    void SetEmail(string value);
    string GetEmail();
    void SetEmailVerified(bool value);
    bool GetEmailVerified();
    void SetTimezone(string value);
    string GetTimezone();
    void SetLocale(string value);
    string GetLocale();
    void SetAzureTenantId(Guid value);
    Guid GetAzureTenantId();
    void SetAzureTenantAudience(string value);
    string GetAzureTenantAudience();
    void AddUserSsoContext(string organizationId, UserSsoContext userSsoContext);
    UserSsoContext? GetUserSsoContext(string organizationId);
}

public class Context(IHttpContextAccessor httpContextAccessor) : IContext
{
    private const string CorrelationIdKey = "CorrelationId";
    private const string VerifiableTokenKey = "VerifiableToken";
    private const string DesignationKey = "Designation";
    private const string TitleKey = "Title";
    private const string NameKey = "Name";
    private const string GivenNameKey = "GivenName";
    private const string MiddleNameKey = "MiddleName";
    private const string FamilyNameKey = "FamilyName";
    private const string PhotoUrlKey = "PhotoUrl";
    private const string PhotoUrl24Key = "PhotoUrl24";
    private const string PhotoUrl32Key = "PhotoUrl32";
    private const string PhotoUrl48Key = "PhotoUrl48";
    private const string PhotoUrl72Key = "PhotoUrl72";
    private const string PhotoUrl192Key = "PhotoUrl192";
    private const string PhotoUrl512Key = "PhotoUrl512";
    private const string EmailKey = "Email";
    private const string EmailVerifiedKey = "EmailVerified";
    private const string TimezoneKey = "Timezone";
    private const string LocaleKey = "Locale";
    private const string AzureTenantIdKey = "AzureTenantId";
    private const string AzureTenantAudienceKey = "AzureTenantAudience";
    private const string UserSsoContextKey = "UserSsoContext";

    public void SetCorrelationId(string value) => GetHttpContext().Items[CorrelationIdKey] = value;

    public string GetCorrelationId() =>
        httpContextAccessor.HttpContext is null
            ? string.Empty
            : GetHttpContext().Items.TryGetValue(CorrelationIdKey, out var value)
                ? value as string ?? string.Empty
                : string.Empty;

    public void SetVerifiableToken(string value) => GetHttpContext().Items[VerifiableTokenKey] = value;

    public string GetVerifiableToken() =>
        GetHttpContext().Items.TryGetValue(VerifiableTokenKey, out var value)
            ? value as string ?? "user_01JJDG8QK13PRT3ANFWH5HJRRK"
            : "user_01JJDG8QK13PRT3ANFWH5HJRRK";

    public void SetDesignation(string value) => GetHttpContext().Items[DesignationKey] = value;

    public string GetDesignation() =>
        GetHttpContext().Items.TryGetValue(DesignationKey, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetTitle(string value) => GetHttpContext().Items[TitleKey] = value;

    public string GetTitle() =>
        GetHttpContext().Items.TryGetValue(TitleKey, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetName(string value) => GetHttpContext().Items[NameKey] = value;

    public string GetName() =>
        GetHttpContext().Items.TryGetValue(NameKey, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetGivenName(string value) => GetHttpContext().Items[GivenNameKey] = value;

    public string GetGivenName() =>
        GetHttpContext().Items.TryGetValue(GivenNameKey, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetMiddleName(string value) => GetHttpContext().Items[MiddleNameKey] = value;

    public string GetMiddleName() =>
        GetHttpContext().Items.TryGetValue(MiddleNameKey, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetFamilyName(string value) => GetHttpContext().Items[FamilyNameKey] = value;

    public string GetFamilyName() =>
        GetHttpContext().Items.TryGetValue(FamilyNameKey, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetPhotoUrl(string value) => GetHttpContext().Items[PhotoUrlKey] = value;

    public string GetPhotoUrl() =>
        GetHttpContext().Items.TryGetValue(PhotoUrlKey, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetPhotoUrl24(string value) => GetHttpContext().Items[PhotoUrl24Key] = value;

    public string GetPhotoUrl24() =>
        GetHttpContext().Items.TryGetValue(PhotoUrl24Key, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetPhotoUrl32(string value) => GetHttpContext().Items[PhotoUrl32Key] = value;

    public string GetPhotoUrl32() =>
        GetHttpContext().Items.TryGetValue(PhotoUrl32Key, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetPhotoUrl48(string value) => GetHttpContext().Items[PhotoUrl48Key] = value;

    public string GetPhotoUrl48() =>
        GetHttpContext().Items.TryGetValue(PhotoUrl48Key, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetPhotoUrl72(string value) => GetHttpContext().Items[PhotoUrl72Key] = value;

    public string GetPhotoUrl72() =>
        GetHttpContext().Items.TryGetValue(PhotoUrl72Key, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetPhotoUrl192(string value) => GetHttpContext().Items[PhotoUrl192Key] = value;

    public string GetPhotoUrl192() =>
        GetHttpContext().Items.TryGetValue(PhotoUrl192Key, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetPhotoUrl512(string value) => GetHttpContext().Items[PhotoUrl512Key] = value;

    public string GetPhotoUrl512() =>
        GetHttpContext().Items.TryGetValue(PhotoUrl512Key, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetEmail(string value) => GetHttpContext().Items[EmailKey] = value;

    public string GetEmail() =>
        GetHttpContext().Items.TryGetValue(EmailKey, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetEmailVerified(bool value) => GetHttpContext().Items[EmailVerifiedKey] = value;

    public bool GetEmailVerified() =>
        GetHttpContext().Items.TryGetValue(EmailVerifiedKey, out var value) &&
        value is true;

    public void SetTimezone(string value) => GetHttpContext().Items[TimezoneKey] = value;

    public string GetTimezone() =>
        GetHttpContext().Items.TryGetValue(TimezoneKey, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetLocale(string value) => GetHttpContext().Items[LocaleKey] = value;

    public string GetLocale() =>
        GetHttpContext().Items.TryGetValue(LocaleKey, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void SetAzureTenantId(Guid value) => GetHttpContext().Items[AzureTenantIdKey] = value.ToString();

    public Guid GetAzureTenantId() =>
        GetHttpContext().Items.TryGetValue(AzureTenantIdKey, out var value)
            ? value is string strValue ? Guid.Parse(strValue) : Guid.Empty
            : Guid.Empty;

    public void SetAzureTenantAudience(string value) => GetHttpContext().Items[AzureTenantAudienceKey] = value;

    public string GetAzureTenantAudience() =>
        GetHttpContext().Items.TryGetValue(AzureTenantAudienceKey, out var value)
            ? value as string ?? string.Empty
            : string.Empty;

    public void AddUserSsoContext(string organizationId, UserSsoContext userSsoContext)
    {
        if (GetHttpContext().Items.TryGetValue(UserSsoContextKey, out var value))
        {
            if (value is ConcurrentDictionary<string, UserSsoContext> organizationUserSso)
            {
                organizationUserSso[organizationId] = userSsoContext;
            }
        }
        else
        {
            GetHttpContext().Items[UserSsoContextKey] = new ConcurrentDictionary<string, UserSsoContext> { [organizationId] = userSsoContext };
        }
    }

    public UserSsoContext? GetUserSsoContext(string organizationId) =>
        GetHttpContext().Items.TryGetValue(UserSsoContextKey, out var value) &&
        value is ConcurrentDictionary<string, UserSsoContext> organizationUserSso &&
        organizationUserSso.TryGetValue(organizationId, out var userSsoContext)
            ? userSsoContext
            : null;

    private HttpContext GetHttpContext()
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        return httpContextAccessor.HttpContext;
    }
}
