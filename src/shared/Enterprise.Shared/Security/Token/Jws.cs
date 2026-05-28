using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace Enterprise.Shared.Security.Token;

public class Jws
{
    [JsonPropertyName("keys")] public JsonWebKey[] Keys { get; } = [];
}
