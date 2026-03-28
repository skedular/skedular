using Api.Shared.Services.Models;
using AutoFixture.Xunit3;
using Booking.Api.Services;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using FakeItEasy;
using Organization = Booking.Shared.Database.Entities.Organization;

namespace Booking.Api.UnitTests.Services.WorkaroundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class WorkaroundServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Signal_Existing_Organization_Arrears_Billing_Workflow_Run_Now(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ITemporalService temporalService,
        WorkaroundService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Organization { Id = "org-1", BillingCycle = OrganizationBillingCycle.Monthly.ToOrganizationBillingCycle() };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);

        await sut.GenerateOrganizationArrearsInvoicesAsync("org-1", cancellationToken);

        A.CallTo(() => temporalService.SignalRunOrganizationArrearsBillingWorkflowRunNowAsync(
                A<OrganizationArrearsBillingConfiguration>.That.Matches(input =>
                    input.OrganizationId == "org-1" &&
                    input.BillingCycle == OrganizationBillingCycle.Monthly),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalService.StartWorkflowRunOrganizationArrearsBillingAsync(
                A<RunOrganizationArrearsBillingInput>._,
                cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Signal_Organization_Arrears_Billing_Workflow_Run_Now_With_Current_Configuration(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ITemporalService temporalService,
        WorkaroundService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Organization { Id = "org-1", BillingCycle = OrganizationBillingCycle.Weekly.ToOrganizationBillingCycle() };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);

        await sut.GenerateOrganizationArrearsInvoicesAsync("org-1", cancellationToken);

        A.CallTo(() => temporalService.SignalRunOrganizationArrearsBillingWorkflowRunNowAsync(
                A<OrganizationArrearsBillingConfiguration>.That.Matches(input =>
                    input.OrganizationId == "org-1" &&
                    input.BillingCycle == OrganizationBillingCycle.Weekly),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalService.StartWorkflowRunOrganizationArrearsBillingAsync(
                A<RunOrganizationArrearsBillingInput>._,
                cancellationToken))
            .MustNotHaveHappened();
    }
}
