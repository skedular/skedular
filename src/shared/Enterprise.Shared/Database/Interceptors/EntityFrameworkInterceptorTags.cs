namespace Enterprise.Shared.Database.Interceptors;

/// <summary>
///     Tags used to trigger interceptor actions
/// </summary>
public static class EntityFrameworkInterceptorTags
{
    public const string ForUpdate = "ForUpdate";
    public const string ForUpdateSkipLocked = "ForUpdateSkipLocked";
}
