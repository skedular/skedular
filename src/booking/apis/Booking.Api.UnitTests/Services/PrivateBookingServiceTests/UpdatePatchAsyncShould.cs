using Api.Shared.Services.Models;
using Booking.Api.Models;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;
using Organization = Booking.Shared.Database.Entities.Organization;
using Team = Booking.Shared.Database.Entities.Team;
using SharedPrivateBookingService = Booking.Shared.Services.IPrivateBookingService;

namespace Booking.Api.UnitTests.Services.PrivateBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Apply_Only_Selected_Notes_And_Preserve_Other_Fields(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
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
        SharedPrivateBookingService sharedPrivateBookingService,
        PrivateBookingService sut,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var existingEntity = new Shared.Database.Entities.Booking
        {
            Id = "booking-1",
            InvolvedOrganizations = [],
            InvolvedTeams = [],
        };
        var mappedBooking = new Shared.Models.Booking
        {
            Id = "booking-1",
            Notes = "old-notes",
            Category = BookingCategory.WorkingFromOffice,
            InvolvedCustomers = [],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            Resources = [],
        };
        var request = new PrivateBookingPatchRequest(
            new Shared.Models.Booking
            {
                Id = "booking-1",
                Notes = "new-notes",
            },
            new HashSet<PrivateBookingPatchField>
            {
                PrivateBookingPatchField.Notes,
            });
        var updatedBooking = new Shared.Models.Booking
        {
            Id = "booking-1",
        };
        IReadOnlyList<string> emptyIds = [];
        IReadOnlyList<Organization> emptyOrganizations = [];
        IReadOnlyList<Team> emptyTeams = [];

        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => bookingRepository.GetByIdAsync("booking-1", cancellationToken)).Returns(existingEntity);
        A.CallTo(() => entityMapper.MapTo(existingEntity)).Returns(mappedBooking);
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
        A.CallTo(() => sharedPrivateBookingService.UpdateAsync(
                A<Shared.Models.Booking>.That.Matches(b => b.Notes == "new-notes"),
                existingEntity, A<CustomerEntity>._, emptyOrganizations, emptyTeams, null, false, cancellationToken))
            .Returns(updatedBooking);

        var result = await sut.UpdateAsync(request, cancellationToken);

        result.ShouldBe(updatedBooking);
        mappedBooking.Notes.ShouldBe("new-notes");
        mappedBooking.Category.ShouldBe(BookingCategory.WorkingFromOffice);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Existing_Booking_Without_Quota_Reservation(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
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
        SharedPrivateBookingService sharedPrivateBookingService,
        PrivateBookingService sut,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var existingEntity = new Shared.Database.Entities.Booking
        {
            Id = "booking-1",
            InvolvedOrganizations = [],
            InvolvedTeams = [],
        };
        var mappedBooking = new Shared.Models.Booking
        {
            Id = "booking-1",
            Category = BookingCategory.WorkingFromOffice,
            InvolvedCustomers = [],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            Resources = [],
        };
        var request = new PrivateBookingPatchRequest(
            new Shared.Models.Booking
            {
                Id = "booking-1",
                Notes = "changed",
            },
            new HashSet<PrivateBookingPatchField>
            {
                PrivateBookingPatchField.Notes,
            });
        var updatedBooking = new Shared.Models.Booking
        {
            Id = "booking-1",
            Notes = "changed",
        };
        IReadOnlyList<string> emptyIds = [];
        IReadOnlyList<Organization> emptyOrganizations = [];
        IReadOnlyList<Team> emptyTeams = [];

        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => bookingRepository.GetByIdAsync("booking-1", cancellationToken)).Returns(existingEntity);
        A.CallTo(() => entityMapper.MapTo(existingEntity)).Returns(mappedBooking);
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
        A.CallTo(() => sharedPrivateBookingService.UpdateAsync(
                A<Shared.Models.Booking>._, existingEntity, A<CustomerEntity>._, emptyOrganizations, emptyTeams, null, false, cancellationToken))
            .Returns(updatedBooking);

        var result = await sut.UpdateAsync(request, cancellationToken);

        result.ShouldBe(updatedBooking);
        A.CallTo(() => sharedPrivateBookingService.UpdateAsync(
                A<Shared.Models.Booking>._, existingEntity, A<CustomerEntity>._, emptyOrganizations, emptyTeams, null, false, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Autosave_Started_And_Completed(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
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
        SharedPrivateBookingService sharedPrivateBookingService,
        [Frozen]
        ILogger<PrivateBookingService> logger,
        PrivateBookingService sut,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var existingEntity = new Shared.Database.Entities.Booking
        {
            Id = "booking-1",
            InvolvedOrganizations = [],
            InvolvedTeams = [],
        };
        var mappedBooking = new Shared.Models.Booking
        {
            Id = "booking-1",
            Category = BookingCategory.WorkingFromOffice,
            InvolvedCustomers = [],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            Resources = [],
        };
        var request = new PrivateBookingPatchRequest(
            new Shared.Models.Booking
            {
                Id = "booking-1",
                Notes = "new-notes",
            },
            new HashSet<PrivateBookingPatchField>
            {
                PrivateBookingPatchField.Notes,
            });
        var updatedBooking = new Shared.Models.Booking
        {
            Id = "booking-1",
        };
        IReadOnlyList<string> emptyIds = [];
        IReadOnlyList<Organization> emptyOrganizations = [];
        IReadOnlyList<Team> emptyTeams = [];

        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => bookingRepository.GetByIdAsync("booking-1", cancellationToken)).Returns(existingEntity);
        A.CallTo(() => entityMapper.MapTo(existingEntity)).Returns(mappedBooking);
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
        A.CallTo(() => sharedPrivateBookingService.UpdateAsync(
                A<Shared.Models.Booking>._, existingEntity, A<CustomerEntity>._, emptyOrganizations, emptyTeams, null, false, cancellationToken))
            .Returns(updatedBooking);

        await sut.UpdateAsync(request, cancellationToken);

        LogAssertions.ACallToLogInfoContaining(logger, "Private booking patch autosave started")
            .MustHaveHappenedOnceExactly();
        LogAssertions.ACallToLogInfoContaining(logger, "Private booking patch autosave completed")
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Authorization_Rejection_And_Rethrow(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        ICustomerRepository customerRepository,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IContext context,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        ILogger<PrivateBookingService> logger,
        PrivateBookingService sut,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var existingEntity = new Shared.Database.Entities.Booking
        {
            Id = "booking-1",
            InvolvedOrganizations = [],
            InvolvedTeams = [],
        };
        var mappedBooking = new Shared.Models.Booking
        {
            Id = "booking-1",
            InvolvedCustomers = [],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            Resources = [],
        };
        var request = new PrivateBookingPatchRequest(
            new Shared.Models.Booking
            {
                Id = "booking-1",
                Notes = "new-notes",
            },
            new HashSet<PrivateBookingPatchField>
            {
                PrivateBookingPatchField.Notes,
            });

        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => bookingRepository.GetByIdAsync("booking-1", cancellationToken)).Returns(existingEntity);
        A.CallTo(() => entityMapper.MapTo(existingEntity)).Returns(mappedBooking);
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
        IBookingRepository bookingRepository,
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
        SharedPrivateBookingService sharedPrivateBookingService,
        [Frozen]
        ILogger<PrivateBookingService> logger,
        PrivateBookingService sut,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var existingEntity = new Shared.Database.Entities.Booking
        {
            Id = "booking-1",
            InvolvedOrganizations = [],
            InvolvedTeams = [],
        };
        var mappedBooking = new Shared.Models.Booking
        {
            Id = "booking-1",
            InvolvedCustomers = [],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            Resources = [],
        };
        var request = new PrivateBookingPatchRequest(
            new Shared.Models.Booking
            {
                Id = "booking-1",
                Notes = "new-notes",
            },
            new HashSet<PrivateBookingPatchField>
            {
                PrivateBookingPatchField.Notes,
            });
        IReadOnlyList<string> emptyIds = [];
        IReadOnlyList<Organization> emptyOrganizations = [];
        IReadOnlyList<Team> emptyTeams = [];

        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => bookingRepository.GetByIdAsync("booking-1", cancellationToken)).Returns(existingEntity);
        A.CallTo(() => entityMapper.MapTo(existingEntity)).Returns(mappedBooking);
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
        A.CallTo(() => sharedPrivateBookingService.UpdateAsync(
                A<Shared.Models.Booking>._, existingEntity, A<CustomerEntity>._, emptyOrganizations, emptyTeams, null, false, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("save failed"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Private booking patch autosave failed"))
            .MustHaveHappenedOnceExactly();
    }
}
