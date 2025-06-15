namespace Enterprise.Shared;

public static class Constants
{
    public const int MaxUniqueIdLength = 100;
    public const int MaxKafkaTopicNameLength = 249;
    public const int MaxOutboxProcessingErrorsLength = 102400;
    public const int MaxWorkflowTypeLength = 1024;
    public const int MaxWorkflowExecutionArgsLength = 10240;
    public const string OrganizationSsoCookiePrefix = "organization-sso";
    public const string OrganizationSsoCookieHeader = "X-SSO-Cookies";
    public static readonly Uri EmptyUri = new("about:blank");
}
