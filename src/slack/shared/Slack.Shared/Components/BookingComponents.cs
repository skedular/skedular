using Slack.Shared.Constants;
using Slack.Shared.Models;
using SlackNet.Blocks;

namespace Slack.Shared.Components;

public interface IBookingComponents
{
    public IReadOnlyList<Block> GetResourcesLines(Booking booking);
}

public class BookingComponents : IBookingComponents
{
    public IReadOnlyList<Block> GetResourcesLines(Booking booking)
    {
        if (booking.Resources.Count == 0)
        {
            return
            [
                new SectionBlock
                {
                    Text = "No resource booked!".ToPlainTextWithIcon(Icons.Resource),
                },
            ];
        }

        return
        [
            .. booking.Resources.Select(Block (item) =>
            {
                var resourceLabel = $"{Icons.Resources} {item.Name}";

                if (item.Zones.Count != 0)
                {
                    resourceLabel += $" {Icons.Zones} {string.Join(",", item.Zones.Select(tag => tag.Name))}";
                }

                if (item.CustomTags.Count != 0)
                {
                    resourceLabel += $" {Icons.CustomTags} {string.Join(",", item.CustomTags.Select(tag => tag.Name))}";
                }

                return new SectionBlock
                {
                    Text = resourceLabel.ToMarkdown(),
                };
            }),
        ];
    }
}
