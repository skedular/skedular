namespace Api.Shared.Services.Models;

public enum PersonalInformationVisibility
{
    Visible,
    Redacted,
}

public static class PersonalInformationVisibilityConstants
{
    public const string Visible = "VISIBLE";
    public const string Redacted = "REDACTED";
}

public static class PersonalInformationVisibilityExtensions
{
    extension(string src)
    {
        public PersonalInformationVisibility ToPersonalInformationVisibility() =>
            src switch
            {
                PersonalInformationVisibilityConstants.Visible => PersonalInformationVisibility.Visible,
                PersonalInformationVisibilityConstants.Redacted => PersonalInformationVisibility.Redacted,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
            };
    }

    extension(PersonalInformationVisibility src)
    {
        public string ToPersonalInformationVisibility() =>
            src switch
            {
                PersonalInformationVisibility.Visible => PersonalInformationVisibilityConstants.Visible,
                PersonalInformationVisibility.Redacted => PersonalInformationVisibilityConstants.Redacted,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
            };

        public string ToPersonalInformationVisibilityName() =>
            src switch
            {
                PersonalInformationVisibility.Visible => "Visible",
                PersonalInformationVisibility.Redacted => "Redacted",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input."),
            };
    }
}
