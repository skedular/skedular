using AutoFixture.Xunit3;
using Customer.Api.Controllers;
using Customer.Api.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.UnitTests.Controllers.CustomerControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RepublishAllShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Call_RepublishAllCustomersAsync(
        [Frozen] IWorkaroundService workaroundService,
        [NoAutoProperties] CustomerController sut,
        CancellationToken cancellationToken)
    {
        _ = await sut.RepublishAll(cancellationToken);

        A.CallTo(() => workaroundService.RepublishAllCustomersAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_OkResult([NoAutoProperties] CustomerController sut, CancellationToken cancellationToken)
    {
        var result = await sut.RepublishAll(cancellationToken);

        result.ShouldBeOfType<OkResult>();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_Exception_When_Workaround_Service_Throws(
        [Frozen] IWorkaroundService workaroundService,
        [NoAutoProperties] CustomerController sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => workaroundService.RepublishAllCustomersAsync(A<CancellationToken>._)).Throws<Exception>();

        var action = async () => await sut.RepublishAll(cancellationToken);

        await action.ShouldThrowAsync<Exception>();
    }
}
