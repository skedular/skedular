using Newtonsoft.Json;
using PactNet;

namespace Testing.Shared.IntegrationTests.Pact;

public interface IPactAccessor : IAsyncDisposable
{
    IPactBuilderV3 PactBuilder { get; }
    int PactPort { get; }
}

public class PactAccessor(PactSettings pactSettings) : IPactAccessor
{
    public int PactPort { get; } = pactSettings.Port;

    public IPactBuilderV3 PactBuilder { get; } = PactNet.Pact
        .V3(pactSettings.ConsumerName, pactSettings.ProviderName,
            new PactConfig { PactDir = pactSettings.TempPactDirectory })
        .WithHttpInteractions(pactSettings.Port);

    public async ValueTask DisposeAsync()
    {
        var tempPactFilePath = Path.Join(pactSettings.TempPactDirectory,
            $"{pactSettings.ConsumerName}-{pactSettings.ProviderName}.json");

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

        if (!Directory.Exists(pactSettings.PactDirectory))
        {
            Directory.CreateDirectory(pactSettings.PactDirectory);
        }

        foreach (var interaction in interactions)
        {
            var filename = interaction.Key.Replace(" ", "_");
            var filePath = Path.Join(pactSettings.PactDirectory, $"{filename}.json");

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
