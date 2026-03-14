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
    }

    extension(string? src)
    {
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
        public CustomerType ToCustomerType() =>
            src switch
            {
                CustomerTypeConstants.Guest => CustomerType.Guest,
                CustomerTypeConstants.Registered => CustomerType.Registered,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
