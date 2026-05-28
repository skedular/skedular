using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using BankAccountEntity = Organization.Shared.Database.Entities.OrganizationBankAccount;

namespace Organization.Api.UnitTests.Services.OrganizationBankAccountServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Only_Selected_Bank_Account_Fields(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationBankAccountRepository organizationBankAccountRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationBankAccountService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization { Id = "org-1", CustomDomain = "acme", Name = "Acme" };
        var bankAccount = new BankAccountEntity
        {
            Id = "bank-account-1",
            Name = "Old account",
            BankName = "Old bank",
            AccountHolderName = "Old holder",
            AccountNumber = "000",
            Country = "New Zealand",
            Organization = organization
        };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var updatedBankAccount = new OrganizationBankAccount
        {
            Id = bankAccount.Id,
            Name = "New account",
            BankName = bankAccount.BankName,
            AccountHolderName = bankAccount.AccountHolderName,
            AccountNumber = "111",
            Country = bankAccount.Country
        };
        var mappedOrganization =
            new Shared.Models.Organization { Id = organization.Id, Name = organization.Name, CustomDomain = organization.CustomDomain };
        var stripeAuthorizeUrl = new Uri($"https://example.test/{organization.Id}");
        var request = new OrganizationBankAccountPatchRequest(
            bankAccount.Id,
            new HashSet<OrganizationBankAccountPatchField>
            {
                OrganizationBankAccountPatchField.Name, OrganizationBankAccountPatchField.AccountNumber
            },
            updatedBankAccount.Name,
            "Ignored bank",
            "Ignored holder",
            updatedBankAccount.AccountNumber,
            "Ignored country");

        A.CallTo(() => repositoryFactory.OrganizationBankAccountRepository).Returns(organizationBankAccountRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationBankAccountRepository.GetByIdAsync(bankAccount.Id, cancellationToken)).Returns(bankAccount);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationBankAccountRepository.Update(bankAccount)).Returns(bankAccount);
        A.CallTo(() => graphQlMapper.MapTo(bankAccount)).Returns(updatedBankAccount);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(mappedOrganization);

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.ShouldBeSameAs(updatedBankAccount);
        bankAccount.Name.ShouldBe(updatedBankAccount.Name);
        bankAccount.AccountNumber.ShouldBe(updatedBankAccount.AccountNumber);
        bankAccount.BankName.ShouldBe("Old bank");
        bankAccount.AccountHolderName.ShouldBe("Old holder");
        bankAccount.Country.ShouldBe("New Zealand");
        A.CallTo(() => organizationBankAccountRepository.Update(bankAccount)).MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(items => items.SequenceEqual(new[] { mappedOrganization })),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
