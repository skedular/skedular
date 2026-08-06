using Api.Shared.Services.Models;
using Booking.Api.Models;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;
using Organization = Booking.Shared.Database.Entities.Organization;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;
using Team = Booking.Shared.Database.Entities.Team;
using SharedPrivateRecurringBookingService = Booking.Shared.Services.IPrivateRecurringBookingService;

namespace Booking.Api.UnitTests.Services.PrivateRecurringBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Apply_Only_Selected_Category_And_Preserve_Other_Fields(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRecurringBookingRepository recurringBookingRepository,
        [Frozen]
        ICustomerRepository customerRepository,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IContext context,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        ITeamAuthorizationService teamAuthorizationService,
        [Frozen]
        SharedPrivateRecurringBookingService sharedPrivateRecurringBookingService,
        PrivateRecurringBookingService sut,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var existingEntity = new RecurringBookingEntity
        {
            Id = "recurring-1",
            InvolvedOrganizations = [],
            InvolvedTeams = [],
        };
        var mappedRecurringBooking = new RecurringBooking
        {
            Id = "recurring-1",
            Category = BookingCategory.WorkingFromOffice,
            SkippedDates = [new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero)],
            InvolvedCustomers = [],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            RequestedResources = [],
        };
        var request = new PrivateRecurringBookingPatchRequest(
            new RecurringBooking
            {
                Id = "recurring-1",
                Category = BookingCategory.WorkingFromHome,
            },
            new HashSet<PrivateRecurringBookingPatchField>
            {
                PrivateRecurringBookingPatchField.Category,
            });
        var updatedRecurringBooking = new RecurringBooking
        {
            Id = "recurring-1",
        };
        IReadOnlyList<string> emptyIds = [];
        IReadOnlyList<Organization> emptyOrganizations = [];
        IReadOnlyList<Team> emptyTeams = [];

        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync("recurring-1", cancellationToken)).Returns(existingEntity);
        A.CallTo(() => entityMapper.MapTo(existingEntity)).Returns(mappedRecurringBooking);
        A.CallTo(() => context.GetVerifiableToken()).Returns(verifiableToken);
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken))
            .Returns(new CustomerEntity
            {
                Id = "customer-1",
            });
        A.CallTo(() => organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(
                emptyIds, emptyIds, "customer-1", true, cancellationToken))
            .Returns(emptyOrganizations);
        A.CallTo(() => teamAuthorizationService.GetBookingInvolvedTeamAndValidatePermissionsAsync(
                emptyIds, "customer-1", true, cancellationToken))
            .Returns(emptyTeams);
        A.CallTo(() => sharedPrivateRecurringBookingService.UpdateAsync(
                A<RecurringBooking>.That.Matches(b => b.Category == BookingCategory.WorkingFromHome),
                existingEntity, A<CustomerEntity>._, emptyOrganizations, emptyTeams, cancellationToken))
            .Returns(updatedRecurringBooking);

        var result = await sut.UpdateAsync(request, cancellationToken);

        result.ShouldBe(updatedRecurringBooking);
        mappedRecurringBooking.Category.ShouldBe(BookingCategory.WorkingFromHome);
        mappedRecurringBooking.SkippedDates.ShouldBe([new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero)]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Autosave_Started_And_Completed(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRecurringBookingRepository recurringBookingRepository,
        [Frozen]
        ICustomerRepository customerRepository,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IContext context,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        ITeamAuthorizationService teamAuthorizationService,
        [Frozen]
        SharedPrivateRecurringBookingService sharedPrivateRecurringBookingService,
        [Frozen]
        ILogger<PrivateRecurringBookingService> logger,
        PrivateRecurringBookingService sut,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var existingEntity = new RecurringBookingEntity
        {
            Id = "recurring-1",
            InvolvedOrganizations = [],
            InvolvedTeams = [],
        };
        var mappedRecurringBooking = new RecurringBooking
        {
            Id = "recurring-1",
            InvolvedCustomers = [],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            RequestedResources = [],
        };
        var request = new PrivateRecurringBookingPatchRequest(
            new RecurringBooking
            {
                Id = "recurring-1",
                Category = BookingCategory.WorkingFromHome,
            },
            new HashSet<PrivateRecurringBookingPatchField>
            {
                PrivateRecurringBookingPatchField.Category,
            });
        var updatedRecurringBooking = new RecurringBooking
        {
            Id = "recurring-1",
        };
        IReadOnlyList<string> emptyIds = [];
        IReadOnlyList<Organization> emptyOrganizations = [];
        IReadOnlyList<Team> emptyTeams = [];

        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync("recurring-1", cancellationToken)).Returns(existingEntity);
        A.CallTo(() => entityMapper.MapTo(existingEntity)).Returns(mappedRecurringBooking);
        A.CallTo(() => context.GetVerifiableToken()).Returns(verifiableToken);
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken))
            .Returns(new CustomerEntity
            {
                Id = "customer-1",
            });
        A.CallTo(() => organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(
                emptyIds, emptyIds, "customer-1", true, cancellationToken))
            .Returns(emptyOrganizations);
        A.CallTo(() => teamAuthorizationService.GetBookingInvolvedTeamAndValidatePermissionsAsync(
                emptyIds, "customer-1", true, cancellationToken))
            .Returns(emptyTeams);
        A.CallTo(() => sharedPrivateRecurringBookingService.UpdateAsync(
                A<RecurringBooking>._, existingEntity, A<CustomerEntity>._, emptyOrganizations, emptyTeams, cancellationToken))
            .Returns(updatedRecurringBooking);

        await sut.UpdateAsync(request, cancellationToken);

        LogAssertions.ACallToLogInfoContaining(logger, "Private recurring booking patch autosave started")
            .MustHaveHappenedOnceExactly();
        LogAssertions.ACallToLogInfoContaining(logger, "Private recurring booking patch autosave completed")
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Authorization_Rejection_And_Rethrow(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRecurringBookingRepository recurringBookingRepository,
        [Frozen]
        ICustomerRepository customerRepository,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IContext context,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        ILogger<PrivateRecurringBookingService> logger,
        PrivateRecurringBookingService sut,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var existingEntity = new RecurringBookingEntity
        {
            Id = "recurring-1",
            InvolvedOrganizations = [],
            InvolvedTeams = [],
        };
        var mappedRecurringBooking = new RecurringBooking
        {
            Id = "recurring-1",
            InvolvedCustomers = [],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            RequestedResources = [],
        };
        var request = new PrivateRecurringBookingPatchRequest(
            new RecurringBooking
            {
                Id = "recurring-1",
                Category = BookingCategory.WorkingFromHome,
            },
            new HashSet<PrivateRecurringBookingPatchField>
            {
                PrivateRecurringBookingPatchField.Category,
            });

        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync("recurring-1", cancellationToken)).Returns(existingEntity);
        A.CallTo(() => entityMapper.MapTo(existingEntity)).Returns(mappedRecurringBooking);
        A.CallTo(() => context.GetVerifiableToken()).Returns(verifiableToken);
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken))
            .Returns(new CustomerEntity
            {
                Id = "customer-1",
            });
        A.CallTo(() => organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(
                A<IReadOnlyList<string>>._, A<IReadOnlyList<string>>._, A<string>._, A<bool>._, cancellationToken))
            .Throws<UnauthorizedAccessException>();

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Warning)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("rejected by authorization"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Error_And_Rethrow_On_General_Failure(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRecurringBookingRepository recurringBookingRepository,
        [Frozen]
        ICustomerRepository customerRepository,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IContext context,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        ITeamAuthorizationService teamAuthorizationService,
        [Frozen]
        SharedPrivateRecurringBookingService sharedPrivateRecurringBookingService,
        [Frozen]
        ILogger<PrivateRecurringBookingService> logger,
        PrivateRecurringBookingService sut,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var existingEntity = new RecurringBookingEntity
        {
            Id = "recurring-1",
            InvolvedOrganizations = [],
            InvolvedTeams = [],
        };
        var mappedRecurringBooking = new RecurringBooking
        {
            Id = "recurring-1",
            InvolvedCustomers = [],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            RequestedResources = [],
        };
        var request = new PrivateRecurringBookingPatchRequest(
            new RecurringBooking
            {
                Id = "recurring-1",
                Category = BookingCategory.WorkingFromHome,
            },
            new HashSet<PrivateRecurringBookingPatchField>
            {
                PrivateRecurringBookingPatchField.Category,
            });
        IReadOnlyList<string> emptyIds = [];
        IReadOnlyList<Organization> emptyOrganizations = [];
        IReadOnlyList<Team> emptyTeams = [];

        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync("recurring-1", cancellationToken)).Returns(existingEntity);
        A.CallTo(() => entityMapper.MapTo(existingEntity)).Returns(mappedRecurringBooking);
        A.CallTo(() => context.GetVerifiableToken()).Returns(verifiableToken);
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken))
            .Returns(new CustomerEntity
            {
                Id = "customer-1",
            });
        A.CallTo(() => organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(
                emptyIds, emptyIds, "customer-1", true, cancellationToken))
            .Returns(emptyOrganizations);
        A.CallTo(() => teamAuthorizationService.GetBookingInvolvedTeamAndValidatePermissionsAsync(
                emptyIds, "customer-1", true, cancellationToken))
            .Returns(emptyTeams);
        A.CallTo(() => sharedPrivateRecurringBookingService.UpdateAsync(
                A<RecurringBooking>._, existingEntity, A<CustomerEntity>._, emptyOrganizations, emptyTeams, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("save failed"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Private recurring booking patch autosave failed"))
            .MustHaveHappenedOnceExactly();
    }
}
