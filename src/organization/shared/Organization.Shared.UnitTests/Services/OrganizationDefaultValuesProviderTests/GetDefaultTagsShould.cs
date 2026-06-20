using Api.Shared.Services.Models;
using Enterprise.Shared.Random;
using Organization.Shared.Services;

namespace Organization.Shared.UnitTests.Services.OrganizationDefaultValuesProviderTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetDefaultTagsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Include_Entire_Location_Resource_Type(
        [Frozen] IRandomHelper randomHelper,
        OrganizationDefaultValuesProvider sut,
        Database.Entities.Organization organization,
        string generatedId)
    {
        A.CallTo(() => randomHelper.Generate()).Returns(generatedId);

        var result = sut.GetDefaultTags(organization);

        result.ShouldContain(tag =>
            tag.Type == OrganizationTagTypeConstants.ResourceEntireLocation &&
            tag.Name == "Entire Location" &&
            tag.Organization == organization);
    }
}
