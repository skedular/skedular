using Enterprise.Shared.Configurations;

namespace Enterprise.Shared.UnitTests.Configurations.ApplicationConfigurationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetSourceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_formatted_source_string(string environment, string domainSource, string appSource)
    {
        var config = new ApplicationConfiguration { Environment = environment, DomainSource = domainSource, AppSource = appSource };

        config.GetSource().ShouldBe($"{environment}::{domainSource}::{appSource}");
    }

    [Fact]
    public void Return_empty_segments_when_properties_are_empty()
    {
        var config = new ApplicationConfiguration();

        config.GetSource().ShouldBe("::::");
    }
}
