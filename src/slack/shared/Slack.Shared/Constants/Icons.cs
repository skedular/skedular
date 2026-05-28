using Enterprise.Shared;
using SlackNet.Blocks;

namespace Slack.Shared.Constants;

public static class Icons
{
    public const string Settings = ":gear:";
    public const string Calendar = ":calendar:";
    public const string Locations = ":office:";
    public const string Location = ":office:";
    public const string Office = ":office:";
    public const string Teams = ":busts_in_silhouette:";
    public const string Team = ":busts_in_silhouette:";
    public const string Zone = ":world_map:";
    public const string Zones = ":world_map:";
    public const string Resource = ":desktop_computer:";
    public const string Resources = ":desktop_computer:";
    public const string CustomTags = ":pushpin:";
    public const string Back = ":back:";
    public const string New = ":heavy_plus_sign:";
    public const string Edit = ":pencil2:";
    public const string Update = ":floppy_disk:";
    public const string Booking = ":calendar:";
    public const string Bookings = ":calendar:";
    public const string Add = ":heavy_check_mark:";
    public const string Remove = ":heavy_minus_sign:";
    public const string Cancel = ":heavy_minus_sign:";
    public const string SetAsDefault = ":heavy_check_mark:";
    public const string ClearDefault = ":broom:";
    public const string Upgrade = ":rocket:";
    public const string Billing = ":receipt:";
    public const string PreviousPage = ":arrow_backward:";
    public const string FirstPage = ":black_left_pointing_double_triangle_with_vertical_bar:";
    public const string NextPage = ":arrow_forward:";
    public const string LastPage = ":black_right_pointing_double_triangle_with_vertical_bar:";
    public const string Feedback = ":first_place_medal:";
    public const string ThankYou = ":pray:";
    public const string People = ":office_worker:";
    public const string Information = ":information_desk_person:";
    public const string Home = ":house:";
    public const string Goto = ":star2:";
    public const string Activate = ":heavy_check_mark:";
    public const string Deactivate = ":heavy_minus_sign:";
    public const string Join = ":heavy_plus_sign:";
    public const string Email = ":e-mail:";
}

public enum IconPosition
{
    Start,
    End
}

public static class IconExtensions
{
    extension(string? text)
    {
        public string ToOptionText()
        {
            var str = string.IsNullOrWhiteSpace(text) ? string.Empty : text;

            return str.Truncate(Commons.MaxOptionTextLength);
        }

        public PlainText ToPlainText(int? maxLength = Commons.MaxOptionTextLength) =>
            new(maxLength is null ? text.Truncate(Commons.MaxOptionTextLength) : text.Truncate(maxLength.Value));

        public Markdown ToMarkdown(int? maxLength = Commons.MaxOptionTextLength) =>
            new(maxLength is null ? text.Truncate(Commons.MaxOptionTextLength) : text.Truncate(maxLength.Value));

        public PlainText ToOptionPlainTextWithIcon(string icon, IconPosition iconPosition = IconPosition.Start) =>
            text.ToPlainTextWithIcon(icon, Commons.MaxOptionTextLength, iconPosition);

        public PlainText ToPlainTextWithIcon(string icon, int? maxLength = null, IconPosition iconPosition = IconPosition.Start) =>
            new(text.ToTextWithIcon(icon, maxLength, iconPosition));

        public Markdown ToOptionMarkdownWithIcon(string icon, IconPosition iconPosition = IconPosition.Start) =>
            text.ToMarkdownWithIcon(icon, Commons.MaxOptionTextLength, iconPosition);

        public Markdown ToMarkdownWithIcon(string icon, int? maxLength = null,
            IconPosition iconPosition = IconPosition.Start) =>
            new(text.ToTextWithIcon(icon, maxLength, iconPosition));

        public string ToOptionTextWithIcon(string icon, IconPosition iconPosition = IconPosition.Start) =>
            text.ToTextWithIcon(icon, Commons.MaxOptionTextLength, iconPosition);

        public string ToTextWithIcon(string icon, int? maxLength = null, IconPosition iconPosition = IconPosition.Start)
        {
            var str = iconPosition == IconPosition.Start ? $"{icon} {text}" : $"{text} {icon}";
            return maxLength is null ? str : str.Truncate(maxLength.Value);
        }
    }
}
