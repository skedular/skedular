namespace Api.Shared.Services.Models;

public enum PersonalInformationVisibility
{
    Visible,
    Redacted
}

public static class PersonalInformationVisibilityConstants
{
    public const string Visible = "VISIBLE";
    public const string Redacted = "REDACTED";
}

public static class PersonalInformationVisibilityExtensions
{
    public static PersonalInformationVisibility ToPersonalInformationVisibility(this string src) =>
        src switch
        {
            PersonalInformationVisibilityConstants.Visible => PersonalInformationVisibility.Visible,
            PersonalInformationVisibilityConstants.Redacted => PersonalInformationVisibility.Redacted,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToPersonalInformationVisibility(this PersonalInformationVisibility src) =>
        src switch
        {
            PersonalInformationVisibility.Visible => PersonalInformationVisibilityConstants.Visible,
            PersonalInformationVisibility.Redacted => PersonalInformationVisibilityConstants.Redacted,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToPersonalInformationVisibilityName(this PersonalInformationVisibility src) =>
        src switch
        {
            PersonalInformationVisibility.Visible => "Visible",
            PersonalInformationVisibility.Redacted => "Redacted",
            _ => throw new ArgumentOutOfRangeException()
        };
}
