using Organization.Shared.Activities;
using Organization.Shared.Database.Entities;
using Organization.Shared.Mappers;
using Organization.Shared.Repositories;
using Stripe;
using Temporalio.Testing;
using OrganizationEntity = Organization.Shared.Database.Entities.Organization;

namespace Organization.Shared.UnitTests.Activities.StripeIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SetOrganizationPaymentMethodAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_The_Plan_Profile_Route_When_Payment_Method_Setup_Fails(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        StripeIntegrations sut)
    {
        var environment = new ActivityEnvironment();
        var organization = new OrganizationEntity
        {
            Id = "org-1",
            CustomDomain = "acme",
        };
        const string redirectTo = "https://app.example.test/organizations/acme/admin?section=subscriptions";

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, environment.CancellationTokenSource.Token))
            .Returns(organization);

        var result = await environment.RunAsync(() => sut.SetOrganizationPaymentMethodAsync(
            new SetOrganizationPaymentMethodInput(organization.Id, "seti-1", "failed", redirectTo)));

        result.ShouldBe("https://app.example.test/organizations/acme/admin?section=setup&profileSection=plan&add-payment-method-status=failed");
        A.CallTo(() => repositoryFactory.OrganizationStripePaymentMethodRepository).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_The_Plan_Profile_Route_And_Persist_Payment_Method_When_Setup_Succeeds(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IRetrievable<SetupIntent, SetupIntentGetOptions> setupIntentRetrievableService,
        [Frozen]
        IRetrievable<PaymentMethod, PaymentMethodGetOptions> paymentMethodRetrievableService,
        [Frozen]
        IOrganizationStripePaymentMethodRepository organizationStripePaymentMethodRepository,
        StripeIntegrations sut)
    {
        var environment = new ActivityEnvironment();
        var organization = new OrganizationEntity
        {
            Id = "org-1",
            CustomDomain = "acme",
        };
        const string redirectTo = "https://app.example.test/organizations/acme/admin?section=subscriptions";
        var setupIntent = new SetupIntent
        {
            PaymentMethodId = "pm-1",
        };
        var paymentMethod = new PaymentMethod
        {
            Id = "pm-1",
            Card = new PaymentMethodCard(),
        };
        var mappedPaymentMethod = new OrganizationStripePaymentMethod();

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, environment.CancellationTokenSource.Token))
            .Returns(organization);
        A.CallTo(() => setupIntentRetrievableService.GetAsync(setupIntent.Id, A<SetupIntentGetOptions>._, A<RequestOptions>._,
            environment.CancellationTokenSource.Token)).Returns(setupIntent);
        A.CallTo(() => paymentMethodRetrievableService.GetAsync(setupIntent.PaymentMethodId, A<PaymentMethodGetOptions>._, A<RequestOptions>._,
            environment.CancellationTokenSource.Token)).Returns(paymentMethod);
        A.CallTo(() => entityMapper.MapTo(paymentMethod, setupIntent.Id, organization)).Returns(mappedPaymentMethod);
        A.CallTo(() => repositoryFactory.OrganizationStripePaymentMethodRepository).Returns(organizationStripePaymentMethodRepository);

        var result = await environment.RunAsync(() => sut.SetOrganizationPaymentMethodAsync(
            new SetOrganizationPaymentMethodInput(organization.Id, setupIntent.Id, "succeeded", redirectTo)));

        result.ShouldBe("https://app.example.test/organizations/acme/admin?section=setup&profileSection=plan&add-payment-method-status=added");
        A.CallTo(() => organizationStripePaymentMethodRepository.Add(mappedPaymentMethod)).MustHaveHappenedOnceExactly();
    }
}
