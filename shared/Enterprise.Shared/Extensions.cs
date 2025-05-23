namespace Enterprise.Shared;

public static class Extensions
{
    public static string ToFullName(this Type type) => type.FullName ?? type.Name;
}
