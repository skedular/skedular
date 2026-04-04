namespace Enterprise.Shared;

public static class DomainAppHostEnvironmentVariables
{
    public const string UseFakeDependencies = "FAKE_DEPENDENCIES";

    public static bool IsFakeDependenciesEnabled() =>
        bool.TryParse(Environment.GetEnvironmentVariable(UseFakeDependencies), out var enabled) && enabled;

    public static void SetFakeDependencies(bool enabled) =>
        Environment.SetEnvironmentVariable(UseFakeDependencies, enabled.ToString());
}
