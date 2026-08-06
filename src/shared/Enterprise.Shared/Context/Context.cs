using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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

public class Context(IHttpContextAccessor httpContextAccessor, ILogger<Context> logger) : IContext
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

    public void SetCorrelationId(string value) => SetStringValue(CorrelationIdKey, value);

    public string GetCorrelationId() =>
        httpContextAccessor.HttpContext is null
            ? string.Empty
            : GetStringValue(CorrelationIdKey);

    public void SetVerifiableToken(string value) => SetStringValue(VerifiableTokenKey, value);

    public string GetVerifiableToken() => GetStringValue(VerifiableTokenKey);

    public void SetDesignation(string value) => SetStringValue(DesignationKey, value);

    public string GetDesignation() => GetStringValue(DesignationKey);

    public void SetTitle(string value) => SetStringValue(TitleKey, value);

    public string GetTitle() => GetStringValue(TitleKey);

    public void SetName(string value) => SetStringValue(NameKey, value);

    public string GetName() => GetStringValue(NameKey);

    public void SetGivenName(string value) => SetStringValue(GivenNameKey, value);

    public string GetGivenName() => GetStringValue(GivenNameKey);

    public void SetMiddleName(string value) => SetStringValue(MiddleNameKey, value);

    public string GetMiddleName() => GetStringValue(MiddleNameKey);

    public void SetFamilyName(string value) => SetStringValue(FamilyNameKey, value);

    public string GetFamilyName() => GetStringValue(FamilyNameKey);

    public void SetPhotoUrl(string value) => SetStringValue(PhotoUrlKey, value);

    public string GetPhotoUrl() => GetStringValue(PhotoUrlKey);

    public void SetPhotoUrl24(string value) => SetStringValue(PhotoUrl24Key, value);

    public string GetPhotoUrl24() => GetStringValue(PhotoUrl24Key);

    public void SetPhotoUrl32(string value) => SetStringValue(PhotoUrl32Key, value);

    public string GetPhotoUrl32() => GetStringValue(PhotoUrl32Key);

    public void SetPhotoUrl48(string value) => SetStringValue(PhotoUrl48Key, value);

    public string GetPhotoUrl48() => GetStringValue(PhotoUrl48Key);

    public void SetPhotoUrl72(string value) => SetStringValue(PhotoUrl72Key, value);

    public string GetPhotoUrl72() => GetStringValue(PhotoUrl72Key);

    public void SetPhotoUrl192(string value) => SetStringValue(PhotoUrl192Key, value);

    public string GetPhotoUrl192() => GetStringValue(PhotoUrl192Key);

    public void SetPhotoUrl512(string value) => SetStringValue(PhotoUrl512Key, value);

    public string GetPhotoUrl512() => GetStringValue(PhotoUrl512Key);

    public void SetEmail(string value) => SetStringValue(EmailKey, value);

    public string GetEmail() => GetStringValue(EmailKey);

    public void SetEmailVerified(bool value) => SetBoolValue(EmailVerifiedKey, value);

    public bool GetEmailVerified() => GetBoolValue(EmailVerifiedKey);

    public void SetTimezone(string value) => SetStringValue(TimezoneKey, value);

    public string GetTimezone() => GetStringValue(TimezoneKey);

    public void SetLocale(string value) => SetStringValue(LocaleKey, value);

    public string GetLocale() => GetStringValue(LocaleKey);

    public void SetAzureTenantId(Guid value) => SetGuidValue(AzureTenantIdKey, value);

    public Guid GetAzureTenantId() => GetGuidValue(AzureTenantIdKey);

    public void SetAzureTenantAudience(string value) => SetStringValue(AzureTenantAudienceKey, value);

    public string GetAzureTenantAudience() => GetStringValue(AzureTenantAudienceKey);

    public void AddUserSsoContext(string organizationId, UserSsoContext userSsoContext)
    {
        if (GetHttpContext().Items.TryGetValue(UserSsoContextKey, out var value))
        {
            if (value is ConcurrentDictionary<string, UserSsoContext> organizationUserSso)
            {
                organizationUserSso[organizationId] = userSsoContext;
                logger.LogDebug("Stored user SSO context in request context");
            }
        }
        else
        {
            GetHttpContext().Items[UserSsoContextKey] = new ConcurrentDictionary<string, UserSsoContext>
            {
                [organizationId] = userSsoContext,
            };
            logger.LogDebug("Created user SSO context collection in request context");
        }
    }

    public UserSsoContext? GetUserSsoContext(string organizationId)
    {
        if (GetHttpContext().Items.TryGetValue(UserSsoContextKey, out var value) &&
            value is ConcurrentDictionary<string, UserSsoContext> organizationUserSso &&
            organizationUserSso.TryGetValue(organizationId, out var userSsoContext))
        {
            logger.LogDebug("Resolved user SSO context from request context");
            return userSsoContext;
        }

        logger.LogDebug("User SSO context was not present in request context");
        return null;
    }

    private void SetStringValue(string key, string value)
    {
        GetHttpContext().Items[key] = value;
        logger.LogDebug("Stored context item {ContextKey}", key);
    }

    private string GetStringValue(string key)
    {
        var found = GetHttpContext().Items.TryGetValue(key, out var value);
        logger.LogDebug(found ? "Resolved context item {ContextKey}" : "Context item {ContextKey} was not present", key);

        return found ? value as string ?? string.Empty : string.Empty;
    }

    private void SetBoolValue(string key, bool value)
    {
        GetHttpContext().Items[key] = value;
        logger.LogDebug("Stored boolean context item {ContextKey}", key);
    }

    private bool GetBoolValue(string key)
    {
        var found = GetHttpContext().Items.TryGetValue(key, out var value) && value is true;
        logger.LogDebug(found ? "Resolved boolean context item {ContextKey}" : "Boolean context item {ContextKey} was not present", key);

        return found;
    }

    private void SetGuidValue(string key, Guid value)
    {
        GetHttpContext().Items[key] = value.ToString();
        logger.LogDebug("Stored GUID context item {ContextKey}", key);
    }

    private Guid GetGuidValue(string key)
    {
        if (GetHttpContext().Items.TryGetValue(key, out var value) && value is string strValue)
        {
            logger.LogDebug("Resolved GUID context item {ContextKey}", key);
            return Guid.Parse(strValue);
        }

        logger.LogDebug("GUID context item {ContextKey} was not present", key);
        return Guid.Empty;
    }

    private HttpContext GetHttpContext()
    {
        if (httpContextAccessor.HttpContext is null)
        {
            logger.LogWarning("Request context was unavailable");
        }

        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        return httpContextAccessor.HttpContext;
    }
}
