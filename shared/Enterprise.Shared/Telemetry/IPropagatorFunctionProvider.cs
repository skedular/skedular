namespace Enterprise.Shared.Telemetry;

/// <summary>
///     Supporting functions for propagating context to custom types
///     This provides a simple way of setting up a place to propagate trace context.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPropagatorFunctionProvider<in T>
{
    /// <summary>
    ///     Inject the context field into provided destination type
    /// </summary>
    /// <param name="destination">Entity to inject value</param>
    /// <param name="contextFieldName">Field name</param>
    /// <param name="contextFieldValue">Field value</param>
    void Inject(
        T destination,
        string contextFieldName,
        string contextFieldValue);

    /// <summary>
    ///     Extract the field value from the provided location type
    /// </summary>
    /// <param name="location">Entity to extract field from</param>
    /// <param name="contextFieldName">Field name</param>
    /// <returns></returns>
    IEnumerable<string> Extract(T location, string contextFieldName);
}
