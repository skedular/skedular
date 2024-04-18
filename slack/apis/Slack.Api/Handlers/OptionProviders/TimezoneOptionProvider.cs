using Slack.Shared.Constants;
using SlackNet.Interaction;
using Option = SlackNet.Blocks.Option;

namespace Slack.Api.Handlers.OptionProviders;

public class TimezoneOptionProvider : IBlockOptionProvider
{
    public Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request)
    {
        var timezones = TimeZoneInfo.GetSystemTimeZones()
            .Where(item => item.Id.Contains(request.Value, StringComparison.InvariantCultureIgnoreCase) ||
                           item.DisplayName.Contains(request.Value, StringComparison.InvariantCultureIgnoreCase))
            .OrderBy(item => item.DisplayName)
            .Take(100)
            .ToList();

        return Task.FromResult(new BlockOptionsResponse
        {
            Options = timezones.Select(item => new Option { Text = item.Id.ToOptionText(), Value = item.Id })
                .ToList()
        });
    }
}
