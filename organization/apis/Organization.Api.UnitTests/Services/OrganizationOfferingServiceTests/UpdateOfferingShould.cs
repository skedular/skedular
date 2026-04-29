using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;

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
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IMapper mapper,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        IRandomHelper randomHelper,
        ICustomerService customerService,
        IOrganizationAuthorizationService organizationAuthorizationService,
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
            UnitPrice = 0
        };
        var mappedOrganization = new Shared.Models.Organization { Id = organization.Id, Name = organization.Name };
        var stripeUrl = new Uri("https://example.test/authorize");
        var timeProvider = A.Fake<TimeProvider>();

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
        A.CallTo(() => mapper.MapTo(organization, stripeUrl)).Returns(mappedOrganization);

        var sut = new OrganizationOfferingService(
            transactionBuilder,
            repositoryFactory,
            randomHelper,
            customerService,
            organizationAuthorizationService,
            organizationOutboxPublisher,
            temporalOutboxService,
            organizationStripeConnectAccountService,
            mapper,
            timeProvider);

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
}
