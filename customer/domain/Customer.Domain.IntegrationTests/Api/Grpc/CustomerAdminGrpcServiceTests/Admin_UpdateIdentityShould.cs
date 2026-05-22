using Api.Shared.Grpc.Skedular.Customer.Admin.V1;
using Api.Shared.Services.Models;
using Customer.Shared.Database.Entities;
using Customer.Shared.Repositories;
using Enterprise.Shared.Grpc;
using CustomerGrpcConfig = Api.Shared.Services.Configurations.Grpc.CustomerConfiguration;

namespace Customer.Domain.IntegrationTests.Api.Grpc.CustomerAdminGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Customer.Api")]
public class Admin_UpdateIdentityShould(
    CustomerAdminService.CustomerAdminServiceClient customerAdminServiceClient,
    IRepositoryFactory repositoryFactory,
    CustomerGrpcConfig customerConfiguration)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Omitted_Details(
        string customerId,
        string identityId,
        string originalEmail,
        string updatedEmail,
        bool originalEmailVerified,
        CancellationToken cancellationToken)
    {
        await SeedCustomerAsync(customerId, identityId, originalEmail, originalEmailVerified, cancellationToken);

        var result = await customerAdminServiceClient.Admin_UpdateIdentityAsync(
            new Admin_UpdateIdentityInput
            {
                CustomerId = customerId, Id = identityId, Email = updatedEmail, FieldsToUpdate = { IdentityPatchField.Email }
            },
            customerConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        result.ShouldNotBeNull();

        var customer = await repositoryFactory.CustomerRepository.GetByIdUntrackedAsync(customerId, cancellationToken);
        customer.ShouldNotBeNull();
        var identity = customer.Identities.Single(i => i.Id == identityId);
        identity.Email.ShouldBe(updatedEmail);
        identity.EmailVerified.ShouldBe(originalEmailVerified);
    }

    private async Task SeedCustomerAsync(
        string customerId,
        string identityId,
        string email,
        bool emailVerified,
        CancellationToken cancellationToken)
    {
        var customer = repositoryFactory.CustomerRepository.Add(new Shared.Database.Entities.Customer
        {
            Id = customerId,
            Type = CustomerTypeConstants.Registered,
            PersonalInformationVisibility = PersonalInformationVisibilityConstants.Visible
        });
        repositoryFactory.IdentityRepository.Add(new Identity { Id = identityId, Customer = customer, Email = email, EmailVerified = emailVerified });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
