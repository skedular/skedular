using Api.Shared.Services.Offering;
using AutoFixture;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using OfferingModel = Api.Shared.Services.Models.Offering;

namespace Booking.Shared.UnitTests.Services.SpacesBookingQuotaServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CanCreateBookingInstancesShould
{
    private static Organization CreateOrganization(
        int planCode,
        int? quotaLimit = null,
        int? customCapacity = null,
        DateTimeOffset? periodStart = null,
        DateTimeOffset? periodEnd = null)
    {
        var now = DateTimeOffset.UtcNow;
        var code = planCode switch
        {
            1 => OfferingCode.SpacesFreeTierV1,
            4 => OfferingCode.EarlyBirdV1,
            5 => OfferingCode.SpacesGrowthV1,
            6 => OfferingCode.SpacesBusinessV1,
            7 => OfferingCode.SpacesContactUsV1,
            _ => (OfferingCode)999
        };
        return new Organization
        {
            Id = "org-1",
            Offering = new OfferingModel
            {
                Code = code,
                Start = now.AddDays(-1),
                End = now.AddMonths(1),
                SpacesPlanCode = planCode,
                SpacesQuotaLimit = quotaLimit,
                SpacesCustomCapacity = customCapacity,
                SpacesPeriodStart = periodStart ?? new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero),
                SpacesPeriodEnd = periodEnd ?? new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero).AddMonths(1),
                SpacesProductEnabled = true,
                SpacesTrialStartedAt = planCode == 1 ? now.AddDays(-1) : null,
                SpacesTrialEndsAt = planCode == 1 ? now.AddDays(13) : null
            }
        };
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SpacesAccessFixtureCustomizer)])]
    public async Task Allow_Active_Trial_Within_Existing_Quota(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ISpacesBookingInstanceCounter counter,
        SpacesBookingQuotaService sut,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var org = CreateOrganization(1);
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(organizationId, A<CancellationToken>._))
            .Returns(Task.FromResult<Organization?>(org));
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
                organizationId, A<DateTimeOffset>._, A<DateTimeOffset>._, A<CancellationToken>._))
            .Returns(Task.FromResult(50));
        A.CallTo(() => counter.CountCurrentPeriodInstances(
                A<IReadOnlyList<DateTimeOffset>>._,
                A<DateTimeOffset>._, A<DateTimeOffset>._))
            .Returns(new SpacesBookingInstanceCount(1, 0));

        var result = await sut.CanCreateBookingInstanceAsync(organizationId, DateTimeOffset.UtcNow, cancellationToken);

        result.CanCreate.ShouldBeTrue();
        result.QuotaLimit.ShouldBe(100);
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
                A<string>._, A<DateTimeOffset>._, A<DateTimeOffset>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SpacesAccessFixtureCustomizer)])]
    public async Task Block_Active_Trial_When_Existing_Quota_Is_Exceeded(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ISpacesBookingInstanceCounter counter,
        SpacesBookingQuotaService sut,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var org = CreateOrganization(1);
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(organizationId, A<CancellationToken>._))
            .Returns(Task.FromResult<Organization?>(org));
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
                organizationId, A<DateTimeOffset>._, A<DateTimeOffset>._, A<CancellationToken>._))
            .Returns(Task.FromResult(100));
        A.CallTo(() => counter.CountCurrentPeriodInstances(
                A<IReadOnlyList<DateTimeOffset>>._,
                A<DateTimeOffset>._, A<DateTimeOffset>._))
            .Returns(new SpacesBookingInstanceCount(1, 0));

        var result = await sut.CanCreateBookingInstanceAsync(organizationId, DateTimeOffset.UtcNow, cancellationToken);

        result.CanCreate.ShouldBeFalse();
        result.ReasonCode.ShouldBe(SpacesQuotaReasonCode.FreeTierLimitExceeded);
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
                A<string>._, A<DateTimeOffset>._, A<DateTimeOffset>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SpacesAccessFixtureCustomizer)])]
    public async Task Allow_Growth_Plan_Within_Quota(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ISpacesBookingInstanceCounter counter,
        SpacesBookingQuotaService sut,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var org = CreateOrganization(5);
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(organizationId, A<CancellationToken>._))
            .Returns(Task.FromResult<Organization?>(org));
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
                organizationId, A<DateTimeOffset>._, A<DateTimeOffset>._, A<CancellationToken>._))
            .Returns(Task.FromResult(400));
        A.CallTo(() => counter.CountCurrentPeriodInstances(
                A<IReadOnlyList<DateTimeOffset>>._,
                A<DateTimeOffset>._, A<DateTimeOffset>._))
            .Returns(new SpacesBookingInstanceCount(1, 0));

        var result = await sut.CanCreateBookingInstanceAsync(organizationId, DateTimeOffset.UtcNow, cancellationToken);

        result.CanCreate.ShouldBeTrue();
        result.QuotaLimit.ShouldBe(500);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SpacesAccessFixtureCustomizer)])]
    public async Task Allow_Legacy_Early_Bird_Plan_As_Unlimited(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ISpacesBookingInstanceCounter counter,
        SpacesBookingQuotaService sut,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var org = CreateOrganization(4, 100, 100);
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(organizationId, A<CancellationToken>._))
            .Returns(Task.FromResult<Organization?>(org));
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
                organizationId, A<DateTimeOffset>._, A<DateTimeOffset>._, A<CancellationToken>._))
            .Returns(Task.FromResult(10_000));
        A.CallTo(() => counter.CountCurrentPeriodInstances(
                A<IReadOnlyList<DateTimeOffset>>._,
                A<DateTimeOffset>._, A<DateTimeOffset>._))
            .Returns(new SpacesBookingInstanceCount(1, 0));

        var result = await sut.CanCreateBookingInstanceAsync(organizationId, DateTimeOffset.UtcNow, cancellationToken);

        result.CanCreate.ShouldBeTrue();
        result.PlanCode.ShouldBe(4);
        result.QuotaLimit.ShouldBe(-1);
        result.RemainingQuota.ShouldBe(int.MaxValue);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SpacesAccessFixtureCustomizer)])]
    public async Task Block_Growth_Plan_When_Quota_Exceeded(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ISpacesBookingInstanceCounter counter,
        SpacesBookingQuotaService sut,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var org = CreateOrganization(5);
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(organizationId, A<CancellationToken>._))
            .Returns(Task.FromResult<Organization?>(org));
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
                organizationId, A<DateTimeOffset>._, A<DateTimeOffset>._, A<CancellationToken>._))
            .Returns(Task.FromResult(500));
        A.CallTo(() => counter.CountCurrentPeriodInstances(
                A<IReadOnlyList<DateTimeOffset>>._,
                A<DateTimeOffset>._, A<DateTimeOffset>._))
            .Returns(new SpacesBookingInstanceCount(1, 0));

        var result = await sut.CanCreateBookingInstanceAsync(organizationId, DateTimeOffset.UtcNow, cancellationToken);

        result.CanCreate.ShouldBeFalse();
        result.ReasonCode.ShouldBe(SpacesQuotaReasonCode.PaidTierLimitExceeded);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SpacesAccessFixtureCustomizer)])]
    public async Task Reject_When_Missing_Offering_State(
        [Frozen] IRepositoryFactory repositoryFactory,
        SpacesBookingQuotaService sut,
        string organizationId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(organizationId, A<CancellationToken>._))
            .Returns(Task.FromResult<Organization?>(null));

        var result = await sut.CanCreateBookingInstanceAsync(organizationId, DateTimeOffset.UtcNow, cancellationToken);

        result.CanCreate.ShouldBeFalse();
        result.ReasonCode.ShouldBe(SpacesQuotaReasonCode.MissingOfferingState);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SpacesAccessFixtureCustomizer)])]
    public async Task Return_Status_Without_Counting_Phantom_Attempt(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ISpacesBookingInstanceCounter counter,
        SpacesBookingQuotaService sut,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var org = CreateOrganization(1);
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(organizationId, A<CancellationToken>._))
            .Returns(Task.FromResult<Organization?>(org));
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
                organizationId, A<DateTimeOffset>._, A<DateTimeOffset>._, A<CancellationToken>._))
            .Returns(Task.FromResult(95));

        var result = await sut.GetQuotaStatusAsync(organizationId, cancellationToken);

        result.CanCreate.ShouldBeTrue();
        result.PlanCode.ShouldBe(1);
        result.CurrentUsage.ShouldBe(95);
        result.QuotaLimit.ShouldBe(100);
        result.AttemptedCurrentPeriodCount.ShouldBe(0);
        result.ExcludedOutOfPeriodCount.ShouldBe(0);
        A.CallTo(() => counter.CountCurrentPeriodInstances(
                A<IReadOnlyList<DateTimeOffset>>._,
                A<DateTimeOffset>._,
                A<DateTimeOffset>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SpacesAccessFixtureCustomizer)])]
    public async Task Enforce_Configured_Free_Quota_During_Active_Trial(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ISpacesBookingInstanceCounter counter,
        SpacesBookingQuotaService sut,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var org = CreateOrganization(1, 25);
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(organizationId, A<CancellationToken>._))
            .Returns(Task.FromResult<Organization?>(org));
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
                organizationId, A<DateTimeOffset>._, A<DateTimeOffset>._, A<CancellationToken>._))
            .Returns(Task.FromResult(25));
        A.CallTo(() => counter.CountCurrentPeriodInstances(
                A<IReadOnlyList<DateTimeOffset>>._,
                A<DateTimeOffset>._,
                A<DateTimeOffset>._))
            .Returns(new SpacesBookingInstanceCount(1, 0));

        var result = await sut.CanCreateBookingInstanceAsync(organizationId, DateTimeOffset.UtcNow, cancellationToken);

        result.CanCreate.ShouldBeFalse();
        result.ReasonCode.ShouldBe(SpacesQuotaReasonCode.FreeTierLimitExceeded);
        result.QuotaLimit.ShouldBe(25);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SpacesAccessFixtureCustomizer)])]
    public async Task Reject_Unknown_Plan_Code(
        [Frozen] IRepositoryFactory repositoryFactory,
        SpacesBookingQuotaService sut,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var org = CreateOrganization(999);
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(organizationId, A<CancellationToken>._))
            .Returns(Task.FromResult<Organization?>(org));
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
                organizationId, A<DateTimeOffset>._, A<DateTimeOffset>._, A<CancellationToken>._))
            .Returns(Task.FromResult(0));

        var result = await sut.CanCreateBookingInstanceAsync(organizationId, DateTimeOffset.UtcNow, cancellationToken);

        result.CanCreate.ShouldBeFalse();
        result.ReasonCode.ShouldBe(SpacesQuotaReasonCode.MissingOfferingState);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SpacesAccessFixtureCustomizer)])]
    public async Task Reject_Invalid_Period(
        [Frozen] IRepositoryFactory repositoryFactory,
        SpacesBookingQuotaService sut,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var start = new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero);
        var org = CreateOrganization(5, periodStart: start, periodEnd: start);
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(organizationId, A<CancellationToken>._))
            .Returns(Task.FromResult<Organization?>(org));
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
                organizationId, A<DateTimeOffset>._, A<DateTimeOffset>._, A<CancellationToken>._))
            .Returns(Task.FromResult(0));

        var result = await sut.CanCreateBookingInstanceAsync(organizationId, DateTimeOffset.UtcNow, cancellationToken);

        result.CanCreate.ShouldBeFalse();
        result.ReasonCode.ShouldBe(SpacesQuotaReasonCode.MissingOfferingState);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(SpacesAccessFixtureCustomizer)])]
    public async Task Reject_At_Exact_Trial_Expiry_Without_Querying_Usage(
        [Frozen] IRepositoryFactory repositoryFactory,
        SpacesBookingQuotaService sut,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var organization = CreateOrganization(1);
        organization.Offering!.SpacesTrialStartedAt = now.AddDays(-14);
        organization.Offering.SpacesTrialEndsAt = now;
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(
                organizationId, A<CancellationToken>._))
            .Returns(organization);

        var result = await sut.CanCreateBookingInstanceAsync(organizationId, now, cancellationToken);

        result.CanCreate.ShouldBeFalse();
        result.ReasonCode.ShouldBe(SpacesQuotaReasonCode.TrialExpired);
        result.AccessDecision.ShouldNotBeNull();
        result.AccessDecision.Status.ShouldBe(SpacesSubscriptionStatus.TrialExpired);
        A.CallTo(() => repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
                A<string>._, A<DateTimeOffset>._, A<DateTimeOffset>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}

public sealed class SpacesAccessFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture)
    {
        fixture.Register<ISpacesAccessEvaluator>(() => new SpacesAccessEvaluator());
        fixture.Register(() => TimeProvider.System);
    }
}
