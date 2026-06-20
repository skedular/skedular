namespace Api.Shared.Services.Models;

public enum BookingChannel
{
    Private,
    Marketplace
}

public static class BookingChannelConstants
{
    public const string Private = "PRIVATE";
    public const string Marketplace = "MARKETPLACE";
}

public static class BookingChannelExtensions
{
    extension(string src)
    {
        public BookingChannel ToBookingChannel() =>
            src switch
            {
                BookingChannelConstants.Private => BookingChannel.Private,
                BookingChannelConstants.Marketplace => BookingChannel.Marketplace,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };
    }

    extension(string? src)
    {
        public BookingChannel? ToNullableBookingChannel() =>
            string.IsNullOrWhiteSpace(src)
                ? null
                : src switch
                {
                    BookingChannelConstants.Private => BookingChannel.Private,
                    BookingChannelConstants.Marketplace => BookingChannel.Marketplace,
                    _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                        $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
                };
    }

    extension(BookingChannel src)
    {
        public string ToBookingChannel() =>
            src switch
            {
                BookingChannel.Private => BookingChannelConstants.Private,
                BookingChannel.Marketplace => BookingChannelConstants.Marketplace,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };

        public string ToBookingChannelName() =>
            src switch
            {
                BookingChannel.Private => "Private",
                BookingChannel.Marketplace => "Marketplace",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };
    }
}
