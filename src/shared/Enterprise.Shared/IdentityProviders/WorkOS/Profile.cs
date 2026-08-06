using Newtonsoft.Json;

namespace Enterprise.Shared.IdentityProviders.WorkOS;

public class Profile : global::WorkOS.Profile
{
    [JsonProperty("profile_picture_url")]
    public string? PhotoUrl { get; set; }

    [JsonProperty("email_verified")]
    public bool EmailVerified { get; set; }
}
