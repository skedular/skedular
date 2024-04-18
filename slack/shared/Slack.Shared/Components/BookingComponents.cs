using Api.Shared.Models;
using Slack.Shared.Constants;
using Slack.Shared.Models;
using SlackNet.Blocks;

namespace Slack.Shared.Components;

public interface IBookingComponents
{
    public ICollection<Block> GetDesksLines(Booking booking);
}

public class BookingComponents : IBookingComponents
{
    public ICollection<Block> GetDesksLines(Booking booking)
    {
        if (booking.Desks.Count == 0)
        {
            return
            [
                new SectionBlock { Text = "No desk booked!".ToPlainTextWithIcon(Icons.Desk) }
            ];
        }

        return booking.Desks.Select(item =>
        {
            var deskLabel = $"{Icons.Desks} {item.Name}";
            var zones = item.Tags.Where(tag => tag.Type == LocationTagType.Zone).Select(tag => tag.Name).ToList();
            if (zones.Count != 0)
            {
                deskLabel += $" {Icons.Zones} {string.Join(",", zones)}";
            }

            return (Block)new SectionBlock { Text = deskLabel.ToMarkdown() };
        }).ToList();
    }
}
