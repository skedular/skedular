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

            if (item.OrganizationZones.Count != 0)
            {
                deskLabel += $" {Icons.Zones} {string.Join(",", item.OrganizationZones.Select(tag => tag.Name))}";
            }

            if (item.OrganizationDeskTypes.Count != 0)
            {
                deskLabel +=
                    $" {Icons.DeskTypes} {string.Join(",", item.OrganizationDeskTypes.Select(tag => tag.Name))}";
            }

            return (Block)new SectionBlock { Text = deskLabel.ToMarkdown() };
        }).ToList();
    }
}
