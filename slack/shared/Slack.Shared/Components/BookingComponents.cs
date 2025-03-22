using Slack.Shared.Constants;
using Slack.Shared.Models;
using SlackNet.Blocks;

namespace Slack.Shared.Components;

public interface IBookingComponents
{
    public ICollection<Block> GetResourcesLines(Booking booking);
}

public class BookingComponents : IBookingComponents
{
    public ICollection<Block> GetResourcesLines(Booking booking)
    {
        if (booking.Resources.Count == 0)
        {
            return [new SectionBlock { Text = "No resource booked!".ToPlainTextWithIcon(Icons.Resource) }];
        }

        return booking.Resources.Select(Block (item) =>
        {
            var resourceLabel = $"{Icons.Resources} {item.Name}";

            if (item.OrganizationZones.Count != 0)
            {
                resourceLabel += $" {Icons.Zones} {string.Join(",", item.OrganizationZones.Select(tag => tag.Name))}";
            }

            if (item.OrganizationCustomTags.Count != 0)
            {
                resourceLabel += $" {Icons.CustomTags} {string.Join(",", item.OrganizationCustomTags.Select(tag => tag.Name))}";
            }

            return new SectionBlock { Text = resourceLabel.ToMarkdown() };
        }).ToList();
    }
}
