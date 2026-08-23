using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
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
using OrganizationOfferingEntity = Organization.Shared.Database.Entities.OrganizationOffering;
using PaymentMethod = Stripe.PaymentMethod;

namespace Organization.Api.UnitTests.Services.PaymentServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RemovePaymentMethodAsyncShould
{
    [Theory]
    [InlineAutoFakeItEasyData([], OrganizationTypeConstants.Private, OfferingCode.FreeTierV1)]
    [InlineAutoFakeItEasyData([], OrganizationTypeConstants.Marketplace, OfferingCode.SpacesFreeTierV1)]
    [InlineAutoFakeItEasyData([], OrganizationTypeConstants.Host, OfferingCode.HostStandardV1)]
    public async Task Restore_Organization_Type_Specific_Fallback_Offering_When_Last_Payment_Method_Is_Removed(
        string organizationType,
        OfferingCode expectedFreeOfferingCode,
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IOrganizationStripePaymentMethodRepository organizationStripePaymentMethodRepository,
        [Frozen]
        IOrganizationOfferingRepository organizationOfferingRepository,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IRetrievable<PaymentMethod, PaymentMethodGetOptions> paymentMethodRetrievableService,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        PaymentService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            CustomDomain = "acme",
            Name = "Acme",
            Type = organizationType,
        };
        var paymentMethod = new PaymentMethodEntity
        {
            Id = "payment-method-1",
            PaymentMethodId = "stripe-payment-method-1",
            Organization = organization,
            DeletedAt = TimeProvider.System.GetUtcNow(),
        };
        var customer = new CustomerModel
        {
            Id = "customer-1",
        };
        var customerEntity = new Customer
        {
            Id = customer.Id,
        };
        var mappedOrganization = new Shared.Models.Organization
        {
            Id = organization.Id,
            Name = organization.Name,
        };
        organization.OrganizationStripePaymentMethods.Add(paymentMethod);

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationStripePaymentMethodRepository).Returns(organizationStripePaymentMethodRepository);
        A.CallTo(() => repositoryFactory.OrganizationOfferingRepository).Returns(organizationOfferingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationStripePaymentMethodRepository.GetByIdAsync(paymentMethod.Id, cancellationToken)).Returns(paymentMethod);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanManagePaymentMethodAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => entityMapper.MapTo(organization)).Returns(mappedOrganization);
        A.CallTo(() => paymentMethodRetrievableService.GetAsync(paymentMethod.PaymentMethodId, null!, null, cancellationToken))
            .Returns<PaymentMethod>(null!);
        A.CallTo(() => organizationOfferingRepository.GetCurrentActiveByOrganizationIdAsync(A<string>._, A<DateTimeOffset>._, cancellationToken))
            .Returns((OrganizationOfferingEntity)null!);
        A.CallTo(() => organizationOfferingRepository.GetCurrentByOrganizationIdAndCodeAsync(organization.Id, expectedFreeOfferingCode,
            A<DateTimeOffset>._, true, cancellationToken)).Returns((OrganizationOfferingEntity)null!);

        await sut.RemovePaymentMethodAsync(paymentMethod.Id, cancellationToken);

        A.CallTo(() =>
                organizationOfferingRepository.Add(A<OrganizationOfferingEntity>.That.Matches(offering => offering.Code == expectedFreeOfferingCode)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Publish_Organization_When_Payment_Method_Is_Removed(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IOrganizationStripePaymentMethodRepository organizationStripePaymentMethodRepository,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen]
        IRetrievable<PaymentMethod, PaymentMethodGetOptions> paymentMethodRetrievableService,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        PaymentService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            CustomDomain = "acme",
            Name = "Acme",
        };
        var paymentMethod = new PaymentMethodEntity
        {
            Id = "payment-method-1",
            PaymentMethodId = "stripe-payment-method-1",
            Organization = organization,
        };
        var customer = new CustomerModel
        {
            Id = "customer-1",
        };
        var customerEntity = new Customer
        {
            Id = customer.Id,
        };
        var mappedOrganization = new Shared.Models.Organization
        {
            Id = organization.Id,
            Name = organization.Name,
        };

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
            .Returns<PaymentMethod>(null!);

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
