using Api.Shared.Services;
using Organization.Api.Mappers;
using Organization.Api.Models;

namespace Organization.Api.UnitTests.Mappers.OrganizationPatchMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ValidateShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Reject_Invalid_Field_Selection(OrganizationPatchMapper sut)
    {
        var request = new OrganizationPatchRequest(
            "org-1",
            null,
            new HashSet<OrganizationPatchField>
            {
                (OrganizationPatchField)999,
            },
            "Name");

        Should.Throw<ArgumentOutOfRangeException>(() => sut.Validate(request));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Reject_Blank_Selected_Name(OrganizationPatchMapper sut)
    {
        var request = new OrganizationPatchRequest(
            "org-1",
            null,
            new HashSet<OrganizationPatchField>
            {
                OrganizationPatchField.Name,
            },
            " ");

        Should.Throw<ArgumentException>(() => sut.Validate(request));
    }

    [Theory]
    [InlineAutoFakeItEasyData(new Type[] { }, 0)]
    [InlineAutoFakeItEasyData(new Type[] { }, 1000)]
    public void Reject_Out_Of_Range_Selected_Invoice_Due_Days(int invoiceDueInDays, OrganizationPatchMapper sut)
    {
        var request = new OrganizationPatchRequest(
            "org-1",
            null,
            new HashSet<OrganizationPatchField>
            {
                OrganizationPatchField.InvoiceDueInDays,
            },
            "Name",
            InvoiceDueInDays: invoiceDueInDays);

        Should.Throw<InvoiceDueInDaysMustBeBetween1And999>(() => sut.Validate(request));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Reject_Missing_Selected_Physical_Address(OrganizationPatchMapper sut)
    {
        var request = new OrganizationPatchRequest(
            "org-1",
            null,
            new HashSet<OrganizationPatchField>
            {
                OrganizationPatchField.PhysicalAddress,
            },
            "Name");

        Should.Throw<ArgumentException>(() => sut.Validate(request));
    }
}
