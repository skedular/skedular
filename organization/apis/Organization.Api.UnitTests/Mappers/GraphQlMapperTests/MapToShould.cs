using Api.Shared.Services.Models;
using Organization.Api.GraphQL.Organization;
using Organization.Api.Mappers;
using Organization.Api.Models;

namespace Organization.Api.UnitTests.Mappers.GraphQlMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapToShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Preserve_Selected_Fields_And_Explicit_Clear_Values(
        GraphQlMapper sut,
        string id,
        string customDomain,
        string ignoredName)
    {
        var input = new UpdateOrganizationInput
        {
            Id = id,
            CustomDomain = customDomain,
            FieldsToUpdate =
            [
                OrganizationPatchField.Description,
                OrganizationPatchField.ContactPhone,
                OrganizationPatchField.BillingCycle
            ],
            Name = ignoredName,
            Description = string.Empty,
            ContactPhone = null,
            BillingCycle = OrganizationBillingCycle.Monthly
        };

        var result = sut.MapTo(input);

        result.Id.ShouldBe(input.Id);
        result.CustomDomain.ShouldBe(input.CustomDomain);
        result.FieldsToUpdate.ShouldBe(input.FieldsToUpdate.ToHashSet());
        result.Name.ShouldBe(input.Name);
        result.Description.ShouldBe(string.Empty);
        result.ContactPhone.ShouldBeNull();
        result.BillingCycle.ShouldBe(input.BillingCycle);
    }
}
