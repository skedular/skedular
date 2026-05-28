using CountryData;
using Slack.Shared.Constants;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.OptionProviders;

public class CountryOptionProvider : IBlockOptionProvider
{
    public Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request) =>
        Task.FromResult(new BlockOptionsResponse
        {
            Options = CountryLoader.CountryInfo
                .Where(item => item.Name.Contains(request.Value, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(item => item.Name)
                .Take(100)
                .Select(item => new Option { Text = item.Name.ToOptionText(), Value = item.Name }).ToList()
        });
}
