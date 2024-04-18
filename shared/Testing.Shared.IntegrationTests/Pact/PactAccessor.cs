using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PactNet;

namespace Testing.Shared.IntegrationTests.Pact;

public interface IPactAccessor : IAsyncDisposable
{
    IPactBuilderV3 PactBuilder { get; }
    int PactPort { get; }
}

public class PactAccessor : IPactAccessor
{
    private readonly PactSettings _pactSettings;

    public PactAccessor(PactSettings pactSettings)
    {
        _pactSettings = pactSettings;

        var config = new PactConfig
        {
            DefaultJsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            },
            PactDir = _pactSettings.TempPactDirectory
        };

        PactPort = _pactSettings.Port;
        PactBuilder = PactNet.Pact
            .V3(_pactSettings.ConsumerName, _pactSettings.ProviderName, config)
            .WithHttpInteractions(_pactSettings.Port);
    }

    public IPactBuilderV3 PactBuilder { get; }
    public int PactPort { get; }

    public async ValueTask DisposeAsync()
    {
        var tempPactFilePath = Path.Join(_pactSettings.TempPactDirectory,
            $"{_pactSettings.ConsumerName}-{_pactSettings.ProviderName}.json");

        if (!File.Exists(tempPactFilePath))
        {
            return;
        }

        var pactFileContent = await File.ReadAllTextAsync(tempPactFilePath);
        var interactions = new Dictionary<string, IList<dynamic>>();

        dynamic parsedContent = JsonConvert.DeserializeObject(pactFileContent) ?? throw new InvalidOperationException();

        foreach (var interaction in parsedContent["interactions"])
        {
            string description = interaction["description"];

            if (!interactions.TryGetValue(description, out var value))
            {
                interactions.Add(description, new List<dynamic> { interaction });
            }
            else
            {
                value.Add(interaction);
            }
        }

        if (!Directory.Exists(_pactSettings.PactDirectory))
        {
            Directory.CreateDirectory(_pactSettings.PactDirectory);
        }

        foreach (var interaction in interactions)
        {
            var filename = interaction.Key.Replace(" ", "_");
            var filePath = Path.Join(_pactSettings.PactDirectory, $"{filename}.json");

            var clonedContent =
                JsonConvert.DeserializeObject<dynamic>(
                    JsonConvert.SerializeObject(parsedContent));

            clonedContent["interactions"] =
                JsonConvert.DeserializeObject<dynamic>(
                    JsonConvert.SerializeObject(interaction.Value));

            // Write out singleton file
            await File.WriteAllTextAsync(filePath,
                JsonConvert.SerializeObject(clonedContent, Formatting.Indented));
        }
    }
}
