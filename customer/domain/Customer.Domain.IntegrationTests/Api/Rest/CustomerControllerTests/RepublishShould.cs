using Api.Shared.Clients.OpenApi.Skedular.Customer.Workaround.V1;
using Customer.Domain.IntegrationTests.Fixtures;
using Customer.Shared.Repositories;

namespace Customer.Domain.IntegrationTests.Api.Rest.CustomerControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Customer.Api")]
public class RepublishShould(ICustomerWorkaroundClient customerClient, IRepositoryFactory repositoryFactory)
{
    [Theory]
    [AutoFakeItEasyData([typeof(BasicCustomerWithIdentityFixtureCustomizer)])]
    public async Task Republish_Customer(Shared.Database.Entities.Customer customer, CancellationToken cancellationToken)
    {
        repositoryFactory.CustomerRepository.Add(customer);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await customerClient.RepublishAsync(customer.Id, cancellationToken);
    }
}
