using Enterprise.Shared.Security.Sso;

namespace Enterprise.Shared.UnitTests.Security.Sso;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SamlMetadataExceptionShould
{
    [Fact]
    public void Have_default_message()
    {
        var ex = new SamlMetadataException();

        ex.Message.ShouldBe("Signing certificate not found in IdP metadata");
    }
}
