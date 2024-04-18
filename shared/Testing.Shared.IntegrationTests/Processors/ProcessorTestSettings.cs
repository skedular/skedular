namespace Testing.Shared.IntegrationTests.Processors;

public class ProcessorTestSettings
{
    public IEnumerable<string> Topics { get; set; } = [];
    public IEnumerable<string> ConsumerGroups { get; set; } = [];
}
