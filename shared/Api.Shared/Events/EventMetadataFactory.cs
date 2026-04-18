using Google.Protobuf.WellKnownTypes;

namespace Api.Shared.Events;

public interface IEventMetadata<TType>
{
    string Id { get; set; }
    string DomainSource { get; set; }
    string AppSource { get; set; }
    TType Type { get; set; }
    Timestamp Time { get; set; }
    string CorrelationId { get; set; }
}

public static class EventMetadataFactory
{
    public static TMetadata NewMetadata<TMetadata, TType>(
        string domainSource,
        string appSource,
        TType type,
        string? correlationId,
        Guid? id = null)
        where TMetadata : class, IEventMetadata<TType>, new() =>
        new()
        {
            Id = id.HasValue ? id.Value.ToString() : Guid.NewGuid().ToString(),
            DomainSource = domainSource,
            AppSource = appSource,
            Type = type!,
            Time = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow),
            CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString() : correlationId!
        };
}
