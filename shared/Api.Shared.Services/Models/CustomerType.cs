namespace Api.Shared.Services.Models;

public enum CustomerType
{
    Guest,
    Registered
}

public static class CustomerTypeConstants
{
    public const string Guest = "GUEST";
    public const string Registered = "REGISTERED";
}

public static class CustomerTypeExtensions
{
    extension(CustomerType? src)
    {
        public string? ToNullableCustomerType() =>
            src is null
                ? null
                : src switch
                {
                    CustomerType.Guest => CustomerTypeConstants.Guest,
                    CustomerType.Registered => CustomerTypeConstants.Registered,
                    _ => throw new ArgumentOutOfRangeException()
                };

        public string? ToNullableCustomerTypeName() =>
            src is null
                ? null
                : src switch
                {
                    CustomerType.Guest => "Guest",
                    CustomerType.Registered => "Registered",
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(CustomerType src)
    {
        public string ToCustomerType() =>
            src switch
            {
                CustomerType.Guest => CustomerTypeConstants.Guest,
                CustomerType.Registered => CustomerTypeConstants.Registered,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToCustomerTypeName() =>
            src switch
            {
                CustomerType.Guest => "Guest",
                CustomerType.Registered => "Registered",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string? src)
    {
        public string? ToNullableCustomerTypeName() =>
            src is null
                ? null
                : src switch
                {
                    CustomerTypeConstants.Guest => "Guest",
                    CustomerTypeConstants.Registered => "Registered",
                    _ => throw new ArgumentOutOfRangeException()
                };

        public CustomerType? ToNullableCustomerType() =>
            src is null
                ? null
                : src switch
                {
                    CustomerTypeConstants.Guest => CustomerType.Guest,
                    CustomerTypeConstants.Registered => CustomerType.Registered,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(string src)
    {
        public string ToCustomerTypeName() =>
            src switch
            {
                CustomerTypeConstants.Guest => "Guest",
                CustomerTypeConstants.Registered => "Registered",
                _ => throw new ArgumentOutOfRangeException()
            };

        public CustomerType ToCustomerType() =>
            src switch
            {
                CustomerTypeConstants.Guest => CustomerType.Guest,
                CustomerTypeConstants.Registered => CustomerType.Registered,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
