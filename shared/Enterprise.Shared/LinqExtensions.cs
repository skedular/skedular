namespace Enterprise.Shared;

public static class LinqExtensions
{
    public static async Task ForEachAsync<T>(this IEnumerable<T> list, Func<T, CancellationToken, Task> action,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var value in list)
        {
            await action(value, cancellationToken);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in list)
        {
            action(item);
        }
    }
}
