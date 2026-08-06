using Api.Shared.Services.Models;
using Customer.Domain.IntegrationTests.Skedular.GraphQL.V1;
using Customer.Shared.Database.Entities;
using Customer.Shared.Repositories;

namespace Customer.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Customer.Api")]
public class UpdateMyCustomerPatchSaveShould(
    IUpdateMyCustomerPatchSaveMutation updateMyCustomerPatchSaveMutation,
    IRepositoryFactory repositoryFactory)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Omitted_Details_For_Single_And_Grouped_Saves(
        string customerId,
        string identityId,
        string originalName,
        string updatedName,
        string givenName,
        string familyName,
        string phoneNumber,
        CancellationToken cancellationToken)
    {
        await SeedCustomerAsync(customerId, identityId, originalName, phoneNumber, cancellationToken);

        TestBearerTokenHandler.SetToken(identityId);
        try
        {
            var nameResult = await updateMyCustomerPatchSaveMutation.ExecuteAsync(
                [CustomerDetailsPatchField.Name],
                updatedName,
                null,
                null,
                cancellationToken);

            nameResult.Errors.Select(error => error.Message).ShouldBeEmpty();
            nameResult.Data.ShouldNotBeNull();
            nameResult.Data.UpdateMyCustomerDetails.Customer.Name.ShouldBe(updatedName);
            nameResult.Data.UpdateMyCustomerDetails.Customer.PhoneNumber.ShouldBe(phoneNumber);

            var personalDetailsResult = await updateMyCustomerPatchSaveMutation.ExecuteAsync(
                [CustomerDetailsPatchField.GivenName, CustomerDetailsPatchField.FamilyName],
                null,
                givenName,
                familyName,
                cancellationToken);

            personalDetailsResult.Errors.Select(error => error.Message).ShouldBeEmpty();
            personalDetailsResult.Data.ShouldNotBeNull();
            personalDetailsResult.Data.UpdateMyCustomerDetails.Customer.Name.ShouldBe(updatedName);
            personalDetailsResult.Data.UpdateMyCustomerDetails.Customer.GivenName.ShouldBe(givenName);
            personalDetailsResult.Data.UpdateMyCustomerDetails.Customer.FamilyName.ShouldBe(familyName);

            var customer = await repositoryFactory.CustomerRepository.GetByIdUntrackedAsync(customerId, cancellationToken);
            customer.ShouldNotBeNull();
            customer.Name.ShouldBe(updatedName);
            customer.GivenName.ShouldBe(givenName);
            customer.FamilyName.ShouldBe(familyName);
            customer.PhoneNumber.ShouldBe(phoneNumber);
        }
        finally
        {
            TestBearerTokenHandler.ClearToken();
        }
    }

    private async Task SeedCustomerAsync(
        string customerId,
        string identityId,
        string originalName,
        string phoneNumber,
        CancellationToken cancellationToken)
    {
        var customer = repositoryFactory.CustomerRepository.Add(new Shared.Database.Entities.Customer
        {
            Id = customerId,
            Name = originalName,
            PhoneNumber = phoneNumber,
            Type = CustomerTypeConstants.Registered,
            PersonalInformationVisibility = PersonalInformationVisibilityConstants.Visible,
        });
        repositoryFactory.IdentityRepository.Add(new Identity
        {
            Id = identityId,
            Customer = customer,
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
