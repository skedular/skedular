using AutoFixture.Xunit3;
using Customer.Api.Controllers;
using Customer.Api.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Testing.Shared;

namespace Customer.Api.UnitTests.Controllers.CustomerControllerTests;

public class RepublishShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Call_RepublishCustomerAsync(
        [Frozen] IWorkaroundService workaroundService,
        [NoAutoProperties] CustomerController sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        _ = await sut.Republish(customerId, cancellationToken);

        A.CallTo(() => workaroundService.RepublishCustomerAsync(customerId, cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_OkResult(
        [NoAutoProperties] CustomerController sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var result = await sut.Republish(customerId, cancellationToken);

        result.ShouldBeOfType<OkResult>();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_Exception_When_Workaround_Service_Throws(
        [Frozen] IWorkaroundService workaroundService,
        [NoAutoProperties] CustomerController sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => workaroundService.RepublishCustomerAsync(A<string>._, A<CancellationToken>._)).Throws<Exception>();

        var action = async () => await sut.Republish(customerId, cancellationToken);

        await action.ShouldThrowAsync<Exception>();
    }
}
