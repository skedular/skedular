using System.Diagnostics;
using System.Net;
using AutoFixture.Xunit3;
using Enterprise.Shared.Infrastructure.ActionResults;
using Enterprise.Shared.Infrastructure.Filters;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Infrastructure.Filters.HttpGlobalExceptionFilterTests;

public class OnExceptionShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Set_Result_If_HandleException_Returns_False(
        [Frozen] IGlobalHttpExceptionHandler fakeGlobalHttpExceptionHandler,
        HttpGlobalExceptionFilter sut,
        ExceptionContext exceptionContext)
    {
        var call = A.CallTo(() =>
            fakeGlobalHttpExceptionHandler.HandleException(exceptionContext));

        call.Returns(false);

        sut.OnException(exceptionContext);

        call.MustHaveHappenedOnceExactly();

        exceptionContext.Result.Should().NotBeNull();

        var result = exceptionContext.Result as InternalServerErrorObjectResult;

        result.Should().NotBeNull();
        result.StatusCode.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        result.Value.Should().NotBeNull();

        var jsonErrorResponse = result.Value as HttpGlobalExceptionFilter.JsonErrorResponse;

        jsonErrorResponse.Should().NotBeNull();
        jsonErrorResponse.Messages.Should().Contain("An error occurred.");
        jsonErrorResponse.Messages.Should().Contain($"TraceId: {Activity.Current?.TraceId}");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_StatusCode_To_InternalServerError_If_HandleException_Returns_False(
        [Frozen] IGlobalHttpExceptionHandler fakeGlobalHttpExceptionHandler,
        HttpGlobalExceptionFilter sut,
        ExceptionContext exceptionContext)
    {
        A.CallTo(() => fakeGlobalHttpExceptionHandler.HandleException(exceptionContext))
            .Returns(false);

        sut.OnException(exceptionContext);

        exceptionContext.Result.Should().NotBeNull();

        exceptionContext.HttpContext.Response.StatusCode.Should()
            .Be((int)HttpStatusCode.InternalServerError);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_ExceptionHandled_To_True_If_HandleException_Returns_False(
        [Frozen] IGlobalHttpExceptionHandler fakeGlobalHttpExceptionHandler,
        HttpGlobalExceptionFilter sut,
        ExceptionContext exceptionContext)
    {
        A.CallTo(() => fakeGlobalHttpExceptionHandler.HandleException(exceptionContext))
            .Returns(false);

        sut.OnException(exceptionContext);

        exceptionContext.ExceptionHandled.Should().BeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_ExceptionHandled_To_True_If_HandleException_Returns_True(
        [Frozen] IGlobalHttpExceptionHandler fakeGlobalHttpExceptionHandler,
        HttpGlobalExceptionFilter sut,
        ExceptionContext exceptionContext)
    {
        A.CallTo(() => fakeGlobalHttpExceptionHandler.HandleException(exceptionContext))
            .Returns(true);

        sut.OnException(exceptionContext);

        exceptionContext.ExceptionHandled.Should().BeTrue();
    }
}
