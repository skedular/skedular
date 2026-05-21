using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Stripe;
using Customer = Organization.Shared.Database.Entities.Customer;
using CustomerModel = Organization.Shared.Models.Customer;
using PaymentMethodEntity = Organization.Shared.Database.Entities.OrganizationStripePaymentMethod;

namespace Organization.Api.UnitTests.Services.PaymentServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RemovePaymentMethodAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Publish_Organization_When_Payment_Method_Is_Removed(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationStripePaymentMethodRepository organizationStripePaymentMethodRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] IRetrievable<PaymentMethod, PaymentMethodGetOptions> paymentMethodRetrievableService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        PaymentService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization { Id = "org-1", CustomDomain = "acme", Name = "Acme" };
        var paymentMethod = new PaymentMethodEntity
        {
            Id = "payment-method-1", PaymentMethodId = "stripe-payment-method-1", Organization = organization
        };
        var customer = new CustomerModel { Id = "customer-1" };
        var customerEntity = new Customer { Id = customer.Id };
        var mappedOrganization = new Shared.Models.Organization { Id = organization.Id, Name = organization.Name };

        organization.OrganizationStripePaymentMethods.Add(paymentMethod);

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationStripePaymentMethodRepository).Returns(organizationStripePaymentMethodRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationStripePaymentMethodRepository.GetByIdAsync(paymentMethod.Id, cancellationToken)).Returns(paymentMethod);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanManagePaymentMethodAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => entityMapper.MapTo(organization)).Returns(mappedOrganization);
        A.CallTo(() => paymentMethodRetrievableService.GetAsync(paymentMethod.PaymentMethodId, null!, null, cancellationToken))
            .Returns((PaymentMethod)null!);

        await sut.RemovePaymentMethodAsync(paymentMethod.Id, cancellationToken);

        A.CallTo(() => organizationStripePaymentMethodRepository.Remove(paymentMethod)).MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(items => items.SequenceEqual(new[] { mappedOrganization })),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
