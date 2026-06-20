using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Shared.Activities;
using Organization.Shared.Database.Entities;
using Organization.Shared.Mappers;
using Organization.Shared.Repositories;
using Temporalio.Testing;
using OrganizationModel = Organization.Shared.Models.Organization;
using OrganizationOfferingModel = Organization.Shared.Models.OrganizationOffering;

namespace Organization.Shared.UnitTests.Activities.OrganizationOfferingsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RenewAutoRenewableOrganizationOfferingAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_The_Next_Full_Month_And_Return_Its_Id(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] TimeProvider timeProvider,
        OrganizationOfferings sut,
        IOrganizationRepository organizationRepository,
        IOrganizationOfferingRepository offeringRepository,
        IDbContextTransaction transaction,
        string organizationId,
        string bridgeOfferingId,
        string renewedOfferingId,
        DateTimeOffset billingBoundary)
    {
        var environment = new ActivityEnvironment();
        var renewalBoundary = billingBoundary.GetOfferingPeriodStart();
        var organization = new Database.Entities.Organization { Id = organizationId, OrganizationOfferings = [] };
        var bridgeOffering = new OrganizationOffering
        {
            Id = bridgeOfferingId,
            Code = OfferingCode.SpacesGrowthV1,
            Start = renewalBoundary.AddDays(-5),
            End = renewalBoundary,
            AutoRenew = true,
            Currency = Currency.Usd.ToString(),
            Organization = organization
        };
        organization.OrganizationOfferings = [bridgeOffering];
        OrganizationOffering? addedOffering = null;

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationOfferingRepository).Returns(offeringRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(
                organizationId, null, environment.CancellationTokenSource.Token))
            .Returns(organization);
        A.CallTo(() => offeringRepository.GetByIdAsync(
                bridgeOfferingId, environment.CancellationTokenSource.Token))
            .Returns(bridgeOffering);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(renewalBoundary);
        A.CallTo(() => randomHelper.Generate()).Returns(renewedOfferingId);
        A.CallTo(() => offeringRepository.Add(A<OrganizationOffering>._))
            .Invokes((OrganizationOffering offering) => addedOffering = offering);
        A.CallTo(() => entityMapper.MapTo(organization)).ReturnsLazily(() => new OrganizationModel
        {
            Id = organizationId,
            OrganizationOfferings =
            [
                new OrganizationOfferingModel
                {
                    Id = renewedOfferingId, Code = OfferingCode.SpacesGrowthV1, Start = addedOffering!.Start, End = addedOffering.End
                }
            ]
        });
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(
                repositoryFactory.UnitOfWork, environment.CancellationTokenSource.Token))
            .Returns(transaction);

        var result = await environment.RunAsync(() => sut.RenewAutoRenewableOrganizationOfferingAsync(
            new RenewAutoRenewableOrganizationOfferingAsyncInput(organizationId, bridgeOfferingId)));

        result.ShouldBe(renewedOfferingId);
        addedOffering.ShouldNotBeNull();
        addedOffering.Start.ShouldBe(renewalBoundary);
        addedOffering.End.ShouldBe(renewalBoundary.AddMonths(1));
        A.CallTo(() => offeringRepository.Remove(bridgeOffering)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
    }
}
