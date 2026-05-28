namespace Location.Shared.Models;

public enum ResourceAvailabilityClassification
{
    Available,
    Unavailable,
    Booked
}

public static class ResourceAvailabilityClassificationConstants
{
    public const string Available = "AVAILABLE";
    public const string Unavailable = "UNAVAILABLE";
    public const string Booked = "BOOKED";
}

public static class ResourceAvailabilityClassificationExtensions
{
    extension(ResourceAvailabilityClassification src)
    {
        public string ToResourceAvailabilityClassification() =>
            src switch
            {
                ResourceAvailabilityClassification.Available => ResourceAvailabilityClassificationConstants.Available,
                ResourceAvailabilityClassification.Unavailable => ResourceAvailabilityClassificationConstants.Unavailable,
                ResourceAvailabilityClassification.Booked => ResourceAvailabilityClassificationConstants.Booked,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string src)
    {
        public ResourceAvailabilityClassification ToResourceAvailabilityClassification() =>
            src switch
            {
                ResourceAvailabilityClassificationConstants.Available => ResourceAvailabilityClassification.Available,
                ResourceAvailabilityClassificationConstants.Unavailable => ResourceAvailabilityClassification.Unavailable,
                ResourceAvailabilityClassificationConstants.Booked => ResourceAvailabilityClassification.Booked,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
