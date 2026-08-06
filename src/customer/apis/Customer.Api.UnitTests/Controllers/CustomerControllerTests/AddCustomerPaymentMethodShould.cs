using Customer.Api.Controllers;
using Customer.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.UnitTests.Controllers.CustomerControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddCustomerPaymentMethodShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Call_HandleStripePaymentMethodEventAsync(
        [Frozen]
        IPaymentService paymentService,
        [NoAutoProperties]
        CustomerStripeController sut,
        string setupIntent,
        string setupIntentClientSecret,
        string redirectStatus,
        string url,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => paymentService.HandleStripePaymentMethodEventAsync(A<string>._, A<string>._, A<string?>._, A<CancellationToken>._))
            .Returns(url);

        _ = await sut.AddCustomerPaymentMethod(setupIntent, setupIntentClientSecret, redirectStatus, cancellationToken: cancellationToken);

        A.CallTo(() => paymentService.HandleStripePaymentMethodEventAsync(setupIntentClientSecret, redirectStatus, null, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Redirect(
        [Frozen]
        IPaymentService paymentService,
        [NoAutoProperties]
        CustomerStripeController sut,
        string setupIntent,
        string setupIntentClientSecret,
        string redirectStatus,
        string url,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => paymentService.HandleStripePaymentMethodEventAsync(A<string>._, A<string>._, A<string?>._, A<CancellationToken>._))
            .Returns(url);

        var result = await sut.AddCustomerPaymentMethod(setupIntent, setupIntentClientSecret, redirectStatus, cancellationToken: cancellationToken);

        result.ShouldBeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.ShouldNotBeNull();
        redirectResult.Url.ShouldBe(url);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_Exception_When_Payment_Service_Throws(
        [Frozen]
        IPaymentService paymentService,
        [NoAutoProperties]
        CustomerStripeController sut,
        string setupIntent,
        string setupIntentClientSecret,
        string redirectStatus,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => paymentService.HandleStripePaymentMethodEventAsync(A<string>._, A<string>._, A<string?>._, A<CancellationToken>._))
            .Throws<Exception>();

        var action = async () =>
            await sut.AddCustomerPaymentMethod(setupIntent, setupIntentClientSecret, redirectStatus, cancellationToken: cancellationToken);

        await action.ShouldThrowAsync<Exception>();
    }
}
