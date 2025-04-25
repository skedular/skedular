namespace Api.Shared.Services.Models;

public interface ICustomerPersonalDetails
{
    string? Designation { get; set; }
    string? Title { get; set; }
    string? Timezone { get; set; }
    string? Locale { get; set; }
    string? Name { get; set; }
    string? GivenName { get; set; }
    string? MiddleName { get; set; }
    string? FamilyName { get; set; }
    string? PhotoUrl { get; set; }
    string? PhotoUrl24 { get; set; }
    string? PhotoUrl32 { get; set; }
    string? PhotoUrl48 { get; set; }
    string? PhotoUrl72 { get; set; }
    string? PhotoUrl192 { get; set; }
    string? PhotoUrl512 { get; set; }
    string? PhoneNumber { get; set; }
}

public static class CustomerPersonalDetailsExtensions
{
    public static string ToDisplayableName<T>(this T src) where T : ICustomerPersonalDetails
    {
        if (!string.IsNullOrWhiteSpace(src.Name))
        {
            return src.Name;
        }

        List<string?> allNames = [src.GivenName, src.MiddleName, src.FamilyName];
        return allNames.Aggregate(string.Empty, (acc, name) => string.IsNullOrWhiteSpace(name) ? acc : $"{acc} {name}");
    }

    public static T Redact<T>(this T src, OrganizationMemberVisibilityPolicy policy) where T : ICustomerPersonalDetails
    {
        src.Designation = src.Designation.FullRedact(policy);
        src.Title = src.Title.FullRedact(policy);
        src.Timezone = src.Timezone.FullRedact(policy);
        src.Locale = src.Locale.FullRedact(policy);
        src.Name = src.Name.Redact(policy);
        src.GivenName = src.GivenName.Redact(policy);
        src.MiddleName = src.MiddleName.Redact(policy);
        src.FamilyName = src.FamilyName.Redact(policy);
        src.PhotoUrl = src.PhotoUrl.FullRedact(policy);
        src.PhotoUrl24 = src.PhotoUrl24.FullRedact(policy);
        src.PhotoUrl32 = src.PhotoUrl32.FullRedact(policy);
        src.PhotoUrl48 = src.PhotoUrl48.FullRedact(policy);
        src.PhotoUrl72 = src.PhotoUrl72.FullRedact(policy);
        src.PhotoUrl192 = src.PhotoUrl192.FullRedact(policy);
        src.PhotoUrl512 = src.PhotoUrl512.FullRedact(policy);
        src.PhoneNumber = src.PhoneNumber.FullRedact(policy);

        return src;
    }

    public static string? FullRedact(this string? src, OrganizationMemberVisibilityPolicy policy) =>
        policy == OrganizationMemberVisibilityPolicy.FullAccess ? src : string.Empty;

    private static string? Redact(this string? src, OrganizationMemberVisibilityPolicy policy) =>
        policy == OrganizationMemberVisibilityPolicy.FullAccess ? src : src?.Length > 1 ? $"{src[..1]}[*****]" : src;
}
