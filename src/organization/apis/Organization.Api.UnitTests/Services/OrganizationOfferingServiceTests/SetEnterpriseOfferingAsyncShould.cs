using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Database.Entities;
using Organization.Shared.Models.PricingCatalog;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.UnitTests.Services.OrganizationOfferingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SetEnterpriseOfferingAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Use_Spaces_Catalog_For_Existing_Marketplace_Organization_Regardless_Of_Offering_Code(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationOfferingRepository organizationOfferingRepository,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] TimeProvider timeProvider,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        OrganizationOfferingService sut,
        CancellationToken cancellationToken)
    {
        var now = new DateTimeOffset(2026, 6, 19, 10, 0, 0, TimeSpan.Zero);
        var existingOffering = new OrganizationOffering
        {
            Id = "offering-1",
            Code = OfferingCode.FreeTierV1,
            Currency = Currency.Usd.ToCurrency(),
            Start = now.AddMonths(-1),
            End = now,
            AutoRenew = true
        };
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            Name = "Co Work",
            CustomDomain = "co-work",
            Type = OrganizationTypeConstants.Marketplace,
            OrganizationOfferings = [existingOffering],
            OrganizationStripePaymentMethods = [new OrganizationStripePaymentMethod { Id = "payment-method-1" }]
        };
        existingOffering.Organization = organization;
        var stripeUrl = new Uri("https://example.test/authorize");
        var mappedOrganization = new Shared.Models.Organization { Id = organization.Id, Name = organization.Name };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationOfferingRepository).Returns(organizationOfferingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(null, organization.CustomDomain, cancellationToken)).Returns(organization);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id)).Returns(stripeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeUrl)).Returns(mappedOrganization);

        await sut.SetEnterpriseOfferingAsync(
            null,
            organization.CustomDomain,
            OfferingCode.EarlyBirdV1,
            0,
            Currency.Usd,
            null,
            null,
            null,
            100,
            null,
            cancellationToken);

        existingOffering.Code.ShouldBe(OfferingCode.EarlyBirdV1);
        existingOffering.CatalogVersion.ShouldBe(PricingCatalogConstants.CurrentSpacesCatalogVersion);
        existingOffering.FixedPrice.ShouldBe(0);
        existingOffering.UnitPrice.ShouldBeNull();
        existingOffering.PurchasedTeamCapacity.ShouldBe(100);
        existingOffering.Start.ShouldBe(now);
        existingOffering.End.ShouldBe(now.GetOfferingPeriodStart().GetOfferingPeriodEnd());
        existingOffering.AutoRenew.ShouldBeTrue();
        A.CallTo(() => organizationOfferingRepository.Update(existingOffering)).MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>._,
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Chargeable_Offering_When_Organization_Has_No_Payment_Method(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        OrganizationOfferingService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            Name = "Co Work",
            CustomDomain = "co-work",
            Type = OrganizationTypeConstants.Marketplace,
            OrganizationOfferings = []
        };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(null, organization.CustomDomain, cancellationToken)).Returns(organization);

        await Should.ThrowAsync<PaymentMethodRequired>(() => sut.SetEnterpriseOfferingAsync(
            null,
            organization.CustomDomain,
            OfferingCode.SpacesGrowthV1,
            4900,
            Currency.Usd,
            null,
            null,
            null,
            500,
            null,
            cancellationToken));

        A.CallTo(() => transactionBuilder.BeginTransactionAsync(A<IUnitOfWork>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Store_Temporary_Discount_For_Paid_Catalog_Offering(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationOfferingRepository organizationOfferingRepository,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] TimeProvider timeProvider,
        OrganizationOfferingService sut,
        CancellationToken cancellationToken)
    {
        var now = new DateTimeOffset(2026, 6, 19, 10, 0, 0, TimeSpan.Zero);
        var existingOffering = new OrganizationOffering
        {
            Id = "offering-1",
            Code = OfferingCode.SpacesFreeTierV1,
            Currency = Currency.Usd.ToCurrency(),
            Start = now.AddMonths(-1),
            End = now,
            AutoRenew = true
        };
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            Name = "Co Work",
            CustomDomain = "co-work",
            Type = OrganizationTypeConstants.Marketplace,
            OrganizationOfferings = [existingOffering],
            OrganizationStripePaymentMethods = [new OrganizationStripePaymentMethod { Id = "payment-method-1" }]
        };
        existingOffering.Organization = organization;

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationOfferingRepository).Returns(organizationOfferingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(null, organization.CustomDomain, cancellationToken)).Returns(organization);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        await sut.SetEnterpriseOfferingAsync(
            null,
            organization.CustomDomain,
            OfferingCode.SpacesGrowthV1,
            4900,
            Currency.Usd,
            null,
            null,
            null,
            500,
            50,
            cancellationToken);

        existingOffering.Code.ShouldBe(OfferingCode.SpacesGrowthV1);
        existingOffering.DiscountPercentage.ShouldBe(50);
        A.CallTo(() => organizationOfferingRepository.Update(existingOffering)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Allow_Discount_For_Custom_Offering(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationOfferingRepository organizationOfferingRepository,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] TimeProvider timeProvider,
        OrganizationOfferingService sut,
        CancellationToken cancellationToken)
    {
        var now = new DateTimeOffset(2026, 6, 19, 10, 0, 0, TimeSpan.Zero);
        var existingOffering = new OrganizationOffering
        {
            Id = "offering-1",
            Code = OfferingCode.FreeTierV1,
            Currency = Currency.Usd.ToCurrency(),
            Start = now.AddMonths(-1),
            End = now,
            AutoRenew = true
        };
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            Name = "Org 1",
            CustomDomain = "org-1",
            Type = OrganizationTypeConstants.Private,
            OrganizationOfferings = [existingOffering],
            OrganizationStripePaymentMethods = [new OrganizationStripePaymentMethod { Id = "payment-method-1" }]
        };
        existingOffering.Organization = organization;

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationOfferingRepository).Returns(organizationOfferingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(null, organization.CustomDomain, cancellationToken)).Returns(organization);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        await sut.SetEnterpriseOfferingAsync(
            null,
            organization.Id,
            OfferingCode.EnterpriseCustomV1,
            1000,
            Currency.Usd,
            null,
            null,
            null,
            null,
            100,
            cancellationToken);

        existingOffering.DiscountPercentage.ShouldBe(100);
        existingOffering.CatalogVersion.ShouldBe(PricingCatalogConstants.CurrentTeamsCatalogVersion);
        A.CallTo(() => organizationOfferingRepository.Update(existingOffering)).MustHaveHappenedOnceExactly();
    }
}
