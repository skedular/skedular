using Enterprise.Shared.Events;
using Google.Protobuf.WellKnownTypes;

namespace Enterprise.Shared.UnitTests.Events.EventMetadataFactoryTests;

public enum SampleEventType
{
    Created = 1
}

public class SampleMetadata : IEventMetadata<SampleEventType>
{
    public string Id { get; set; } = string.Empty;
    public string DomainSource { get; set; } = string.Empty;
    public string AppSource { get; set; } = string.Empty;
    public SampleEventType Type { get; set; }
    public Timestamp Time { get; set; } = new();
    public string CorrelationId { get; set; } = string.Empty;
}

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class NewMetadataShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Populate_all_fields(string domainSource, string appSource, string correlationId)
    {
        var metadata = EventMetadataFactory.NewMetadata<SampleMetadata, SampleEventType>(
            domainSource, appSource, SampleEventType.Created, correlationId);

        metadata.DomainSource.ShouldBe(domainSource);
        metadata.AppSource.ShouldBe(appSource);
        metadata.Type.ShouldBe(SampleEventType.Created);
        metadata.CorrelationId.ShouldBe(correlationId);
        metadata.Id.ShouldNotBeNullOrWhiteSpace();
        metadata.Time.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_provided_id_when_given(string domainSource, string appSource)
    {
        var id = Guid.NewGuid();

        var metadata = EventMetadataFactory.NewMetadata<SampleMetadata, SampleEventType>(
            domainSource, appSource, SampleEventType.Created, null, id);

        metadata.Id.ShouldBe(id.ToString());
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Generate_correlation_id_when_null(string domainSource, string appSource)
    {
        var metadata = EventMetadataFactory.NewMetadata<SampleMetadata, SampleEventType>(
            domainSource, appSource, SampleEventType.Created, null);

        metadata.CorrelationId.ShouldNotBeNullOrWhiteSpace();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Generate_correlation_id_when_whitespace(string domainSource, string appSource)
    {
        var metadata = EventMetadataFactory.NewMetadata<SampleMetadata, SampleEventType>(
            domainSource, appSource, SampleEventType.Created, "   ");

        metadata.CorrelationId.ShouldNotBeNullOrWhiteSpace();
        metadata.CorrelationId.Trim().ShouldNotBe("   ");
    }
}
