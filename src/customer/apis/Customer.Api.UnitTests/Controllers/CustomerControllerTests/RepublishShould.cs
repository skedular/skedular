using Customer.Api.Controllers;
using Customer.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.UnitTests.Controllers.CustomerControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RepublishShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Call_RepublishCustomerAsync(
        [Frozen]
        IWorkaroundService workaroundService,
        [NoAutoProperties]
        CustomerWorkaroundController sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        _ = await sut.Republish(customerId, cancellationToken);

        A.CallTo(() => workaroundService.RepublishCustomerAsync(customerId, cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_OkResult(
        [NoAutoProperties]
        CustomerWorkaroundController sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var result = await sut.Republish(customerId, cancellationToken);

        result.ShouldBeOfType<OkResult>();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_Exception_When_Workaround_Service_Throws(
        [Frozen]
        IWorkaroundService workaroundService,
        [NoAutoProperties]
        CustomerWorkaroundController sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => workaroundService.RepublishCustomerAsync(A<string>._, A<CancellationToken>._)).Throws<Exception>();

        var action = async () => await sut.Republish(customerId, cancellationToken);

        await action.ShouldThrowAsync<Exception>();
    }
}
