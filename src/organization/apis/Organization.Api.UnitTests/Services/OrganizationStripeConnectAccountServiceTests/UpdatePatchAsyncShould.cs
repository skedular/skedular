using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Repositories;
using StripeAccountEntity = Organization.Shared.Database.Entities.OrganizationStripeConnectAccount;

namespace Organization.Api.UnitTests.Services.OrganizationStripeConnectAccountServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Selected_Name(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IOrganizationStripeConnectAccountRepository organizationStripeConnectAccountRepository,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        IGraphQlMapper graphQlMapper,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        OrganizationStripeConnectAccountService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            Name = "Acme",
            CustomDomain = "acme",
        };
        var account = new StripeAccountEntity
        {
            Id = "stripe-1",
            Name = "Old name",
            Organization = organization,
        };
        var customer = new Customer
        {
            Id = "customer-1",
        };
        var customerEntity = new Shared.Database.Entities.Customer
        {
            Id = customer.Id,
        };
        var updatedAccount = new OrganizationStripeConnectAccount
        {
            Id = account.Id,
            Name = "New name",
        };
        var request = new OrganizationStripeConnectAccountPatchRequest(
            account.Id,
            new HashSet<OrganizationStripeConnectAccountPatchField>
            {
                OrganizationStripeConnectAccountPatchField.Name,
            },
            updatedAccount.Name);

        A.CallTo(() => repositoryFactory.OrganizationStripeConnectAccountRepository).Returns(organizationStripeConnectAccountRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationStripeConnectAccountRepository.GetByIdAsync(account.Id, cancellationToken)).Returns(account);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanManageStripeConnectAccountAsync(organization, customer.Id, cancellationToken))
            .Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountRepository.Update(account)).Returns(account);
        A.CallTo(() => graphQlMapper.MapTo(account)).Returns(updatedAccount);

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.ShouldBeSameAs(updatedAccount);
        account.Name.ShouldBe(updatedAccount.Name);
        A.CallTo(() => organizationStripeConnectAccountRepository.Update(account)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
