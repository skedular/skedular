using Api.Shared.Services.Offering;
using Organization.Api.Models;
using Organization.Api.Services;

namespace Organization.Api.UnitTests.Services.OrganizationOfferingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateOfferingPatchShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Patch_When_Offering_Code_Field_Is_Not_Selected(
        OrganizationOfferingService sut,
        CancellationToken cancellationToken)
    {
        var request = new OrganizationOfferingPatchRequest(
            "org-1",
            "acme",
            new HashSet<OrganizationOfferingPatchField>(),
            OfferingCode.FreeTierV1);

        await Should.ThrowAsync<ArgumentException>(() => sut.UpdateOfferingPatchAsync(request, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Patch_When_Offering_Code_Is_Missing(
        OrganizationOfferingService sut,
        CancellationToken cancellationToken)
    {
        var request = new OrganizationOfferingPatchRequest(
            "org-1",
            "acme",
            new HashSet<OrganizationOfferingPatchField>
            {
                OrganizationOfferingPatchField.OfferingCode,
            },
            null);

        await Should.ThrowAsync<ArgumentException>(() => sut.UpdateOfferingPatchAsync(request, cancellationToken));
    }
}
