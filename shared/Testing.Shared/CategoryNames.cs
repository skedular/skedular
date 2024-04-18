namespace Testing.Shared;

/// <summary>
///     Hardcoded values for use with the Xunit Trait attribute.
/// </summary>
/// <remarks>
///     A custom attribute would be preferable, but there are issues with those currently
///     https://youtrack.jetbrains.com/issue/RSRP-458779
/// </remarks>
public static class CategoryNames
{
    public const string Key = "Category";

    public const string Integration = nameof(Integration);
    public const string PactProvider = nameof(PactProvider);
    public const string PactConsumer = nameof(PactConsumer);
    public const string SystemIntegration = nameof(SystemIntegration);
}
