namespace Enterprise.Shared;

public static class IntExtensions
{
    public static int ToNullInt(this int? value) => value ?? -1;
    public static int? FromNullInt(this int value) => value == -1 ? null : value;
}
