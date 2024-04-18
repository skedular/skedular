using Newtonsoft.Json.Linq;

namespace Testing.Shared.IntegrationTests.Pact;

public static class PactProviderDataHelper
{
    public static void UpdatePactHeader(
        JToken interaction,
        string name,
        string newValue)
    {
        //get and update request headers with token and auth
        var request = interaction["request"];

        ArgumentNullException.ThrowIfNull(request);

        foreach (var jToken in request["headers"]!)
        {
            var header = jToken as JProperty;

            ArgumentNullException.ThrowIfNull(header);

            if (header.Name != name)
            {
                continue;
            }

            header.Value = newValue;

            return;
        }
    }

    public static List<string> GetProviderStates(
        JToken interaction)
    {
        var providerStates = interaction["providerStates"];
        if (providerStates is null)
        {
            return [];
        }

        return providerStates.Select(providerState =>
        {
            var name = providerState["name"];

            ArgumentNullException.ThrowIfNull(name);

            return name.ToString();
        }).ToList();
    }

    public static string GetRequestMethod(JToken interaction)
    {
        var request = interaction["request"];

        ArgumentNullException.ThrowIfNull(request);

        var method = request["method"];

        ArgumentNullException.ThrowIfNull(method);

        return method.ToString();
    }
}
