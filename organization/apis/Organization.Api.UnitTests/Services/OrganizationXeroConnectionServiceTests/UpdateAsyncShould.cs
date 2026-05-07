using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Repositories;

namespace Organization.Api.UnitTests.Services.OrganizationXeroConnectionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Allow_Disabled_Billing_Mode(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationXeroConnectionRepository organizationXeroConnectionRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationXeroConnectionService sut,
        string customerId,
        string organizationId,
        string organizationName,
        string organizationCustomDomain,
        string tenantId,
        string tenantName,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = organizationId, Name = organizationName, CustomDomain = organizationCustomDomain
        };
        var customer = new Customer { Id = customerId };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customerId };
        var input = new OrganizationXeroConnection
        {
            Organization =
                new Shared.Models.Organization { Id = organizationId, Name = organizationName, CustomDomain = organizationCustomDomain },
            BillingMode = OrganizationXeroBillingMode.Disabled,
            TenantId = tenantId,
            TenantName = tenantName,
            IsActive = false,
            SendInvoicesViaXero = true,
            AutoReconcilePayments = true
        };
        var entity = new Shared.Database.Entities.OrganizationXeroConnection
        {
            Organization = organization,
            BillingMode = XeroBillingModeConstants.Disabled,
            TenantId = tenantId,
            TenantName = tenantName,
            IsActive = false,
            SendInvoicesViaXero = true,
            AutoReconcilePayments = true
        };
        var mappedOrganization = new Shared.Models.Organization
        {
            Id = organizationId, Name = organizationName, CustomDomain = organizationCustomDomain
        };
        var stripeAuthorizeUrl = new Uri($"https://example.test/{organizationId}");

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationXeroConnectionRepository).Returns(organizationXeroConnectionRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organizationId, organizationCustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customerId, cancellationToken)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => graphQlMapper.MapToEntity(input, organization)).Returns(entity);
        A.CallTo(() => organizationXeroConnectionRepository.Add(entity)).Returns(entity);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organizationId))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(mappedOrganization);


        var result = await sut.UpdateAsync(input, cancellationToken);

        result.ShouldBe(mappedOrganization);
        organization.OrganizationXeroConnection.ShouldNotBeNull();
        organization.OrganizationXeroConnection.ShouldBe(entity);
        organization.OrganizationXeroConnection.BillingMode.ShouldBe(XeroBillingModeConstants.Disabled);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Allow_Enabled_Billing_Mode(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationXeroConnectionRepository organizationXeroConnectionRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationXeroConnectionService sut,
        string customerId,
        string organizationId,
        string organizationName,
        string organizationCustomDomain,
        string tenantId,
        string tenantName,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = organizationId, Name = organizationName, CustomDomain = organizationCustomDomain
        };
        var customer = new Customer { Id = customerId };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customerId };
        var input = new OrganizationXeroConnection
        {
            Organization =
                new Shared.Models.Organization { Id = organizationId, Name = organizationName, CustomDomain = organizationCustomDomain },
            BillingMode = OrganizationXeroBillingMode.Enabled,
            TenantId = tenantId,
            TenantName = tenantName,
            IsActive = false,
            SendInvoicesViaXero = true,
            AutoReconcilePayments = true
        };
        var entity = new Shared.Database.Entities.OrganizationXeroConnection
        {
            Organization = organization,
            BillingMode = XeroBillingModeConstants.Enabled,
            TenantId = tenantId,
            TenantName = tenantName,
            IsActive = false,
            SendInvoicesViaXero = true,
            AutoReconcilePayments = true
        };
        var mappedOrganization = new Shared.Models.Organization
        {
            Id = organizationId, Name = organizationName, CustomDomain = organizationCustomDomain
        };
        var stripeAuthorizeUrl = new Uri($"https://example.test/{organizationId}");

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationXeroConnectionRepository).Returns(organizationXeroConnectionRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organizationId, organizationCustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customerId, cancellationToken)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => graphQlMapper.MapToEntity(input, organization)).Returns(entity);
        A.CallTo(() => organizationXeroConnectionRepository.Add(entity)).Returns(entity);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organizationId))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(mappedOrganization);


        var result = await sut.UpdateAsync(input, cancellationToken);

        result.ShouldBe(mappedOrganization);
        organization.OrganizationXeroConnection.ShouldNotBeNull();
        organization.OrganizationXeroConnection.ShouldBe(entity);
        organization.OrganizationXeroConnection.BillingMode.ShouldBe(XeroBillingModeConstants.Enabled);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Allow_Repeating_Invoices_Billing_Mode(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationXeroConnectionRepository organizationXeroConnectionRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationXeroConnectionService sut,
        string customerId,
        string organizationId,
        string organizationName,
        string organizationCustomDomain,
        string tenantId,
        string tenantName,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = organizationId, Name = organizationName, CustomDomain = organizationCustomDomain
        };
        var customer = new Customer { Id = customerId };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customerId };
        var input = new OrganizationXeroConnection
        {
            Organization =
                new Shared.Models.Organization { Id = organizationId, Name = organizationName, CustomDomain = organizationCustomDomain },
            BillingMode = OrganizationXeroBillingMode.RepeatingInvoices,
            TenantId = tenantId,
            TenantName = tenantName,
            IsActive = false,
            SendInvoicesViaXero = true,
            AutoReconcilePayments = true
        };
        var entity = new Shared.Database.Entities.OrganizationXeroConnection
        {
            Organization = organization,
            BillingMode = XeroBillingModeConstants.RepeatingInvoices,
            TenantId = tenantId,
            TenantName = tenantName,
            IsActive = false,
            SendInvoicesViaXero = true,
            AutoReconcilePayments = true
        };
        var mappedOrganization = new Shared.Models.Organization
        {
            Id = organizationId, Name = organizationName, CustomDomain = organizationCustomDomain
        };
        var stripeAuthorizeUrl = new Uri($"https://example.test/{organizationId}");

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationXeroConnectionRepository).Returns(organizationXeroConnectionRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organizationId, organizationCustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customerId, cancellationToken)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => graphQlMapper.MapToEntity(input, organization)).Returns(entity);
        A.CallTo(() => organizationXeroConnectionRepository.Add(entity)).Returns(entity);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organizationId))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(mappedOrganization);


        var result = await sut.UpdateAsync(input, cancellationToken);

        result.ShouldBe(mappedOrganization);
        organization.OrganizationXeroConnection.ShouldNotBeNull();
        organization.OrganizationXeroConnection.ShouldBe(entity);
        organization.OrganizationXeroConnection.BillingMode.ShouldBe(XeroBillingModeConstants.RepeatingInvoices);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_Billing_Mode_Is_Unsupported(
        OrganizationXeroConnectionService sut,
        string organizationId,
        string organizationName,
        string organizationCustomDomain,
        CancellationToken cancellationToken)
    {
        var input = new OrganizationXeroConnection
        {
            Organization = new Shared.Models.Organization
            {
                Id = organizationId, Name = organizationName, CustomDomain = organizationCustomDomain
            },
            BillingMode = (OrganizationXeroBillingMode)999
        };


        await Should.ThrowAsync<UnsupportedXeroBillingModeException>(() => sut.UpdateAsync(input, cancellationToken));
    }
}
