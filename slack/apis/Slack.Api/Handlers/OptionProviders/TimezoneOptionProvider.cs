using Slack.Shared.Constants;
using SlackNet.Interaction;
using Option = SlackNet.Blocks.Option;

namespace Slack.Api.Handlers.OptionProviders;

public class TimezoneOptionProvider : IBlockOptionProvider
{
    public Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request) =>
        Task.FromResult(new BlockOptionsResponse
        {
            Options = TimeZoneInfo.GetSystemTimeZones()
                .Where(item => item.Id.Contains(request.Value, StringComparison.InvariantCultureIgnoreCase) ||
                               item.DisplayName.Contains(request.Value, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(item => item.DisplayName)
                .Take(100).Select(item => new Option { Text = item.Id.ToOptionText(), Value = item.Id })
                .ToList()
        });
}
