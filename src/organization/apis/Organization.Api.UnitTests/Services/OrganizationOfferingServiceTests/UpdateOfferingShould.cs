using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;

namespace Organization.Api.UnitTests.Services.OrganizationOfferingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateOfferingShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Use_The_Organization_Id_Lookup_When_An_Organization_Id_Is_Provided(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationOfferingRepository organizationOfferingRepository,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] TimeProvider timeProvider,
        OrganizationOfferingService sut,
        CancellationToken cancellationToken)
    {
        var now = new DateTimeOffset(2026, 4, 19, 10, 0, 0, TimeSpan.Zero);
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            Name = "Org 1",
            CustomDomain = "org-1",
            Type = OrganizationTypeConstants.Private,
            OrganizationOfferings = []
        };
        var matchingOffering = new OrganizationOffering
        {
            Id = "offer-1",
            Organization = organization,
            Code = OfferingCode.FreeTierV1,
            Start = now.AddDays(-1),
            End = now.AddDays(1),
            AutoRenew = true,
            UnitPrice = null
        };
        var mappedOrganization = new Shared.Models.Organization { Id = organization.Id, Name = organization.Name };
        var stripeUrl = new Uri("https://example.test/authorize");

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationOfferingRepository).Returns(organizationOfferingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken)).Returns(organization);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => organizationOfferingRepository.GetCurrentByOrganizationIdAndCodeAsync(
                organization.Id,
                OfferingCode.FreeTierV1,
                now,
                true,
                cancellationToken))
            .Returns(matchingOffering);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id)).Returns(stripeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeUrl)).Returns(mappedOrganization);

        await sut.UpdateOfferingAsync(organization.Id, null, OfferingCode.FreeTierV1, true, cancellationToken);

        A.CallTo(() => organizationOfferingRepository.GetCurrentByOrganizationIdAndCodeAsync(
            organization.Id,
            OfferingCode.FreeTierV1,
            now,
            true,
            cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOfferingRepository.GetCurrentByCustomDomainAndCodeAsync(
            A<string>._,
            A<OfferingCode>._,
            A<DateTimeOffset>._,
            A<bool>._,
            A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => organizationOfferingRepository.Undelete(matchingOffering)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Chargeable_Offering_When_Organization_Has_No_Payment_Method_Even_When_Authorization_Is_Ignored(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        OrganizationOfferingService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            Name = "Org 1",
            CustomDomain = "org-1",
            Type = OrganizationTypeConstants.Private,
            OrganizationOfferings = []
        };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken)).Returns(organization);

        await Should.ThrowAsync<PaymentMethodRequired>(() => sut.UpdateOfferingAsync(
            organization.Id,
            null,
            OfferingCode.PayAsYouGoV1,
            true,
            cancellationToken));

        A.CallTo(() => transactionBuilder.BeginTransactionAsync(A<IUnitOfWork>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Resolve_Marketplace_Free_Tier_To_Spaces_Free_Tier(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationOfferingRepository organizationOfferingRepository,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] TimeProvider timeProvider,
        OrganizationOfferingService sut,
        CancellationToken cancellationToken)
    {
        var now = new DateTimeOffset(2026, 6, 23, 10, 0, 0, TimeSpan.Zero);
        var activeOffering = new OrganizationOffering
        {
            Id = "offer-growth",
            Code = OfferingCode.SpacesGrowthV1,
            Currency = Currency.Usd.ToCurrency(),
            Start = now.AddDays(-1),
            End = now.AddDays(1),
            AutoRenew = true
        };
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            Name = "Co Work",
            CustomDomain = "co-work",
            Type = OrganizationTypeConstants.Marketplace,
            OrganizationOfferings = [activeOffering]
        };
        activeOffering.Organization = organization;
        var mappedOrganization = new Shared.Models.Organization { Id = organization.Id, Name = organization.Name };
        var stripeUrl = new Uri("https://example.test/authorize");

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationOfferingRepository).Returns(organizationOfferingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken)).Returns(organization);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => organizationOfferingRepository.GetCurrentByOrganizationIdAndCodeAsync(
                organization.Id,
                OfferingCode.SpacesFreeTierV1,
                now,
                true,
                cancellationToken))
            .Returns(Task.FromResult<OrganizationOffering?>(null));
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id)).Returns(stripeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeUrl)).Returns(mappedOrganization);

        await sut.UpdateOfferingAsync(organization.Id, null, OfferingCode.FreeTierV1, true, cancellationToken);

        A.CallTo(() => organizationOfferingRepository.GetCurrentByOrganizationIdAndCodeAsync(
            organization.Id,
            OfferingCode.SpacesFreeTierV1,
            now,
            true,
            cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOfferingRepository.GetCurrentByOrganizationIdAndCodeAsync(
            organization.Id,
            OfferingCode.FreeTierV1,
            A<DateTimeOffset>._,
            A<bool>._,
            A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => organizationOfferingRepository.Remove(activeOffering)).MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOfferingRepository.Add(
                A<OrganizationOffering>.That.Matches(offering => offering.Code == OfferingCode.SpacesFreeTierV1)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
