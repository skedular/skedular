using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Testing.Shared.IntegrationTests.Pact;

public class PactProviderContract
{
    private readonly JObject _contract;
    private readonly JToken _interaction;

    public PactProviderContract(string fileText)
    {
        _contract = JObject.Parse(fileText);
        var interaction = _contract["interactions"]![0];

        ArgumentNullException.ThrowIfNull(interaction);

        _interaction = interaction;

        // Only requests that have a body
        if (GetRequestMethod() == HttpMethod.Post ||
            GetRequestMethod() == HttpMethod.Put ||
            GetRequestMethod() == HttpMethod.Patch)
        {
            _ = JsonConvert.DeserializeObject(_interaction!["request"]!["body"]!.ToString());
        }
    }

    public override string ToString() => _contract.ToString();

    public List<string> GetProviderStates()
    {
        var providerStates = _interaction["providerStates"];

        return providerStates is null ? [] : providerStates.Select(state => state["name"]!.ToString()).ToList();
    }

    public void UpdateHeader(string headerName, string newValue)
    {
        foreach (var jToken in _interaction["request"]?["headers"]!)
        {
            var header = (JProperty)jToken;

            if (header.Name != headerName)
            {
                continue;
            }

            header.Value = newValue;

            return;
        }

        throw new Exception("The header " + headerName + " was not found in the Pact interaction.");
    }

    public HttpMethod GetRequestMethod() => new(_interaction["request"]!["method"]!.ToString());

    public void ReplaceInUrl(string originalValue, string newValue)
    {
        var originalPath = _interaction["request"]!["path"]!.ToString();
        var newPath = originalPath.Replace(originalValue, newValue);
        _interaction["request"]!["path"] = newPath;
    }

    public void UpdateQueryString(string queryName, string newValue)
    {
        foreach (var jToken in _interaction["request"]?["query"]!)
        {
            var query = (JProperty)jToken;

            if (query.Name != queryName)
            {
                continue;
            }

            query.Value = newValue;

            return;
        }

        throw new Exception("The query " + queryName + " was not found in the Pact interaction.");
    }
}
