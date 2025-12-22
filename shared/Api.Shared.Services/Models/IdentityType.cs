namespace Api.Shared.Services.Models;

public enum IdentityType
{
    Guest,
    Registered
}

public static class IdentityTypeConstants
{
    public const string Guest = "GUEST";
    public const string Registered = "REGISTERED";
}

public static class IdentityTypeExtensions
{
    extension(IdentityType? src)
    {
        public string? ToNullableIdentityType() =>
            src is null
                ? null
                : src switch
                {
                    IdentityType.Guest => IdentityTypeConstants.Guest,
                    IdentityType.Registered => IdentityTypeConstants.Registered,
                    _ => throw new ArgumentOutOfRangeException()
                };

        public string? ToNullableIdentityTypeName() =>
            src is null
                ? null
                : src switch
                {
                    IdentityType.Guest => "Guest",
                    IdentityType.Registered => "Registered",
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(IdentityType src)
    {
        public string ToIdentityType() =>
            src switch
            {
                IdentityType.Guest => IdentityTypeConstants.Guest,
                IdentityType.Registered => IdentityTypeConstants.Registered,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToIdentityTypeName() =>
            src switch
            {
                IdentityType.Guest => "Guest",
                IdentityType.Registered => "Registered",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string? src)
    {
        public string? ToNullableIdentityTypeName() =>
            src is null
                ? null
                : src switch
                {
                    IdentityTypeConstants.Guest => "Guest",
                    IdentityTypeConstants.Registered => "Registered",
                    _ => throw new ArgumentOutOfRangeException()
                };

        public IdentityType? ToNullableIdentityType() =>
            src is null
                ? null
                : src switch
                {
                    IdentityTypeConstants.Guest => IdentityType.Guest,
                    IdentityTypeConstants.Registered => IdentityType.Registered,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(string src)
    {
        public string ToIdentityTypeName() =>
            src switch
            {
                IdentityTypeConstants.Guest => "Guest",
                IdentityTypeConstants.Registered => "Registered",
                _ => throw new ArgumentOutOfRangeException()
            };

        public IdentityType ToIdentityType() =>
            src switch
            {
                IdentityTypeConstants.Guest => IdentityType.Guest,
                IdentityTypeConstants.Registered => IdentityType.Registered,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
