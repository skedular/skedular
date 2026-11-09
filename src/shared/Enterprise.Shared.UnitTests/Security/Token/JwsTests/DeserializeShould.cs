using System.Text.Json;
using Enterprise.Shared.Security.Token;

namespace Enterprise.Shared.UnitTests.Security.Token.JwsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DeserializeShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Read_signing_keys_from_the_JWKS_response(string keyId)
    {
        var json = JsonSerializer.Serialize(new
        {
            keys = new[]
            {
                new
                {
                    kty = "RSA",
                    kid = keyId,
                    n = "AQAB",
                    e = "AQAB",
                },
            },
        });

        var result = JsonSerializer.Deserialize<Jws>(json);

        result.ShouldNotBeNull();
        result.Keys.ShouldHaveSingleItem().KeyId.ShouldBe(keyId);
    }
}
