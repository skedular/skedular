using Organization.Api.Mappers;
using Organization.Shared.Models;

namespace Organization.Api.UnitTests.Mappers.MapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapToBillingDetailsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Include_Invoice_Due_Days_In_Grpc_Response(
        Mapper sut,
        string id,
        string email,
        string addressLine1,
        string city,
        string zipcode,
        string country,
        string countryCode,
        int invoiceDueInDays)
    {
        invoiceDueInDays = Math.Abs(invoiceDueInDays % 365) + 1;
        var billingDetails = new OrganizationBillingDetails
        {
            Id = id,
            Email = email,
            AddressLine1 = addressLine1,
            City = city,
            Zipcode = zipcode,
            Country = country,
            CountryCode = countryCode,
            InvoiceDueInDays = invoiceDueInDays
        };

        var result = sut.MapToGrpcResponse(billingDetails);

        result.InvoiceDueInDays.ShouldBe(invoiceDueInDays);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Normalize_Invoice_Due_Days_In_Grpc_Response_When_Value_Is_Not_Positive(
        Mapper sut,
        string id,
        string email,
        string addressLine1,
        string city,
        string zipcode,
        string country,
        string countryCode)
    {
        var billingDetails = new OrganizationBillingDetails
        {
            Id = id,
            Email = email,
            AddressLine1 = addressLine1,
            City = city,
            Zipcode = zipcode,
            Country = country,
            CountryCode = countryCode,
            InvoiceDueInDays = 0
        };

        var result = sut.MapToGrpcResponse(billingDetails);

        result.InvoiceDueInDays.ShouldBe(OrganizationBillingDetails.DefaultInvoiceDueInDays);
    }
}
