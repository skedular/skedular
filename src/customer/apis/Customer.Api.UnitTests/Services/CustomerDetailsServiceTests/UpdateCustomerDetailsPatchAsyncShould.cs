using Api.Shared.Services.Models;
using Customer.Api.Models;
using Customer.Api.Services;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;
using CustomerEntity = Customer.Shared.Database.Entities.Customer;
using CustomerModel = Customer.Shared.Models.Customer;

namespace Customer.Api.UnitTests.Services.CustomerDetailsServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateCustomerDetailsPatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Apply_Only_Selected_Designation_And_Preserve_Other_Fields(
        [Frozen] ICustomerHelperService customerHelperService,
        CustomerDetailsService sut,
        CancellationToken cancellationToken)
    {
        var entity = new CustomerEntity { Id = "cust-1", Designation = "Engineer", Timezone = "Europe/London" };
        var request = new CustomerDetailsPatchRequest(
            new HashSet<CustomerDetailsPatchField> { CustomerDetailsPatchField.Designation },
            "America/New_York",
            "Manager",
            null,
            null,
            null,
            null,
            null,
            null,
            PersonalInformationVisibility.Redacted);
        var updatedModel = new CustomerModel { Id = "cust-1" };

        A.CallTo(() => customerHelperService.GetCustomerAsync(cancellationToken)).Returns(entity);
        A.CallTo(() => customerHelperService.UpdateAndPublishEventAsync(entity, cancellationToken))
            .Returns(updatedModel);

        var result = await sut.UpdateCustomerDetailsAsync("cust-1", request, cancellationToken);

        result.ShouldBe(updatedModel);
        entity.Designation.ShouldBe("Manager");
        entity.Timezone.ShouldBe("Europe/London");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Autosave_Started_And_Completed(
        [Frozen] ICustomerHelperService customerHelperService,
        [Frozen] ILogger<CustomerDetailsService> logger,
        CustomerDetailsService sut,
        CancellationToken cancellationToken)
    {
        var entity = new CustomerEntity { Id = "cust-1" };
        var request = new CustomerDetailsPatchRequest(
            new HashSet<CustomerDetailsPatchField> { CustomerDetailsPatchField.Designation },
            null,
            "Manager",
            null,
            null,
            null,
            null,
            null,
            null,
            PersonalInformationVisibility.Redacted);
        var updatedModel = new CustomerModel { Id = "cust-1" };

        A.CallTo(() => customerHelperService.GetCustomerAsync(cancellationToken)).Returns(entity);
        A.CallTo(() => customerHelperService.UpdateAndPublishEventAsync(entity, cancellationToken))
            .Returns(updatedModel);

        await sut.UpdateCustomerDetailsAsync("cust-1", request, cancellationToken);

        LogAssertions.ACallToLogInfoContaining(logger, "Customer details patch autosave started")
            .MustHaveHappenedOnceExactly();
        LogAssertions.ACallToLogInfoContaining(logger, "Customer details patch autosave completed")
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Authorization_Rejection_When_Customer_Id_Mismatches(
        [Frozen] ICustomerHelperService customerHelperService,
        [Frozen] ILogger<CustomerDetailsService> logger,
        CustomerDetailsService sut,
        CancellationToken cancellationToken)
    {
        var entity = new CustomerEntity { Id = "cust-1" };
        var request = new CustomerDetailsPatchRequest(
            new HashSet<CustomerDetailsPatchField> { CustomerDetailsPatchField.Designation },
            null,
            "Manager",
            null,
            null,
            null,
            null,
            null,
            null,
            PersonalInformationVisibility.Redacted);

        A.CallTo(() => customerHelperService.GetCustomerAsync(cancellationToken)).Returns(entity);

        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            sut.UpdateCustomerDetailsAsync("other-cust", request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Warning)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("rejected by authorization"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Error_And_Rethrow_On_General_Failure(
        [Frozen] ICustomerHelperService customerHelperService,
        [Frozen] ILogger<CustomerDetailsService> logger,
        CustomerDetailsService sut,
        CancellationToken cancellationToken)
    {
        var entity = new CustomerEntity { Id = "cust-1" };
        var request = new CustomerDetailsPatchRequest(
            new HashSet<CustomerDetailsPatchField> { CustomerDetailsPatchField.Designation },
            null,
            "Manager",
            null,
            null,
            null,
            null,
            null,
            null,
            PersonalInformationVisibility.Redacted);

        A.CallTo(() => customerHelperService.GetCustomerAsync(cancellationToken)).Returns(entity);
        A.CallTo(() => customerHelperService.UpdateAndPublishEventAsync(entity, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("publish failed"));

        await Should.ThrowAsync<InvalidOperationException>(() =>
            sut.UpdateCustomerDetailsAsync("cust-1", request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Customer details patch autosave failed"))
            .MustHaveHappenedOnceExactly();
    }
}
