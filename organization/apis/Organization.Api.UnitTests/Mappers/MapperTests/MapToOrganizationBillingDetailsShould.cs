using Organization.Api.GraphQL.Billing;
using Organization.Api.Mappers;
using OrganizationBillingDetails = Organization.Shared.Models.OrganizationBillingDetails;

namespace Organization.Api.UnitTests.Mappers.MapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapToOrganizationBillingDetailsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Use_Default_Invoice_Due_Days_When_Add_Input_Value_Is_Not_Positive(
        Mapper sut,
        string email,
        string addressLine1,
        string zipcode,
        string country)
    {
        var input = new AddOrganizationBillingDetailsInput
        {
            Email = email,
            AddressLine1 = addressLine1,
            Zipcode = zipcode,
            Country = country,
            InvoiceDueInDays = 0
        };

        var result = sut.MapTo(input);

        result.InvoiceDueInDays.ShouldBe(OrganizationBillingDetails.DefaultInvoiceDueInDays);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Keep_Positive_Invoice_Due_Days_When_Add_Input_Value_Is_Valid(
        Mapper sut,
        string email,
        string addressLine1,
        string zipcode,
        string country,
        int invoiceDueInDays)
    {
        invoiceDueInDays = Math.Abs(invoiceDueInDays % 365) + 1;
        var input = new AddOrganizationBillingDetailsInput
        {
            Email = email,
            AddressLine1 = addressLine1,
            Zipcode = zipcode,
            Country = country,
            InvoiceDueInDays = invoiceDueInDays
        };

        var result = sut.MapTo(input);

        result.InvoiceDueInDays.ShouldBe(invoiceDueInDays);
    }
}
