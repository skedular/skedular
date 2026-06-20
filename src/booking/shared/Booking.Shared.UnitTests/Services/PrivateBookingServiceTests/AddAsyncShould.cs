using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Offering = Api.Shared.Services.Models.Offering;
using Organization = Booking.Shared.Database.Entities.Organization;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Shared.UnitTests.Services.PrivateBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_SpacesBookingQuotaExceeded_When_Quota_Exceeded(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] ISpacesBookingQuotaService spacesBookingQuotaService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        PrivateBookingService sut,
        CancellationToken cancellationToken)
    {
        var booking = new Shared.Models.Booking
        {
            From = new DateTimeOffset(2026, 6, 15, 9, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero),
            InvolvedCustomers = [new Customer { Id = "customer-1" }],
            ResourceBookingSlots =
            [
                new ResourceBookingSlot { Resource = new Resource { Id = "resource-1" }, Customers = [new Customer { Id = "customer-1" }] }
            ]
        };
        var organizations = new List<Organization>
        {
            new() { Id = "org-1", Type = OrganizationTypeConstants.Marketplace, Offering = new Offering { Code = OfferingCode.SpacesFreeTierV1 } }
        };
        var teams = new List<Team>();

        A.CallTo(() => repositoryFactory.CustomerRepository.GetByIdsAsync(
                A<IReadOnlyList<string>>._, true, A<CancellationToken>._))
            .Returns(Task.FromResult<IReadOnlyList<Database.Entities.Customer>>([new Database.Entities.Customer { Id = "customer-1" }]));
        A.CallTo(() => repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
                null, null, A<DateTimeOffset>._, A<DateTimeOffset>._,
                A<IReadOnlyList<string>>._, A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._, A<CancellationToken>._))
            .Returns(Task.FromResult<IReadOnlyList<Database.Entities.Resource>>([
                new Database.Entities.Resource { Id = "resource-1", ResourceBookingSlots = [] }
            ]));
        A.CallTo(() => repositoryFactory.BookingRepository.Add(A<Database.Entities.Booking>._))
            .Returns(new Database.Entities.Booking { Id = "booking-1" });
        A.CallTo(() => spacesBookingQuotaService.TryReserveBookingInstancesAsync(
                "org-1", A<IReadOnlyList<DateTimeOffset>>._, A<CancellationToken>._))
            .Returns(Task.FromResult(new SpacesQuotaDecision(
                false, SpacesQuotaReasonCode.FreeTierLimitExceeded, 1, 100, 100, 1, 0, 0,
                new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 7, 1, 0, 0, 0, TimeSpan.Zero))));
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, A<CancellationToken>._))
            .Returns(Task.FromResult(A.Fake<IDbContextTransaction>()));
        A.CallTo(() => entityMapper.MapTo(A<Database.Entities.Booking>._))
            .Returns(booking);

        await Should.ThrowAsync<SpacesBookingQuotaExceeded>(() =>
            sut.AddAsync(booking, new Database.Entities.Customer { Id = "customer-1" }, organizations, teams,
                null, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Succeed_When_Quota_Allows(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] ISpacesBookingQuotaService spacesBookingQuotaService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        PrivateBookingService sut,
        CancellationToken cancellationToken)
    {
        var booking = new Shared.Models.Booking
        {
            From = new DateTimeOffset(2026, 6, 15, 9, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero),
            InvolvedCustomers = [new Customer { Id = "customer-1" }],
            ResourceBookingSlots =
            [
                new ResourceBookingSlot { Resource = new Resource { Id = "resource-1" }, Customers = [new Customer { Id = "customer-1" }] }
            ]
        };
        var organizations = new List<Organization>
        {
            new() { Id = "org-1", Type = OrganizationTypeConstants.Marketplace, Offering = new Offering { Code = OfferingCode.SpacesFreeTierV1 } }
        };
        var teams = new List<Team>();

        A.CallTo(() => repositoryFactory.CustomerRepository.GetByIdsAsync(
                A<IReadOnlyList<string>>._, true, A<CancellationToken>._))
            .Returns(Task.FromResult<IReadOnlyList<Database.Entities.Customer>>([new Database.Entities.Customer { Id = "customer-1" }]));
        A.CallTo(() => repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
                null, null, A<DateTimeOffset>._, A<DateTimeOffset>._,
                A<IReadOnlyList<string>>._, A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._, A<CancellationToken>._))
            .Returns(Task.FromResult<IReadOnlyList<Database.Entities.Resource>>([
                new Database.Entities.Resource { Id = "resource-1", ResourceBookingSlots = [] }
            ]));
        A.CallTo(() => repositoryFactory.BookingRepository.Add(A<Database.Entities.Booking>._))
            .Returns(new Database.Entities.Booking { Id = "booking-1" });
        A.CallTo(() => spacesBookingQuotaService.TryReserveBookingInstancesAsync(
                "org-1", A<IReadOnlyList<DateTimeOffset>>._, A<CancellationToken>._))
            .Returns(Task.FromResult(new SpacesQuotaDecision(
                true, SpacesQuotaReasonCode.WithinQuota, 1, 50, 100, 1, 0, 50,
                new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 7, 1, 0, 0, 0, TimeSpan.Zero))));
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, A<CancellationToken>._))
            .Returns(Task.FromResult(A.Fake<IDbContextTransaction>()));
        A.CallTo(() => entityMapper.MapTo(A<Database.Entities.Booking>._))
            .Returns(booking);

        var result = await sut.AddAsync(booking, new Database.Entities.Customer { Id = "customer-1" }, organizations, teams,
            null, cancellationToken);

        result.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Not_Check_Spaces_Quota_For_Private_Organization(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] ISpacesBookingQuotaService spacesBookingQuotaService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        PrivateBookingService sut,
        CancellationToken cancellationToken)
    {
        var booking = new Shared.Models.Booking
        {
            From = new DateTimeOffset(2026, 6, 15, 9, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero),
            InvolvedCustomers = [new Customer { Id = "customer-1" }],
            ResourceBookingSlots =
            [
                new ResourceBookingSlot { Resource = new Resource { Id = "resource-1" }, Customers = [new Customer { Id = "customer-1" }] }
            ]
        };
        var organizations = new List<Organization> { new() { Id = "org-1", Type = OrganizationTypeConstants.Private } };
        var teams = new List<Team>();

        A.CallTo(() => repositoryFactory.CustomerRepository.GetByIdsAsync(
                A<IReadOnlyList<string>>._, true, A<CancellationToken>._))
            .Returns(Task.FromResult<IReadOnlyList<Database.Entities.Customer>>([new Database.Entities.Customer { Id = "customer-1" }]));
        A.CallTo(() => repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
                null, null, A<DateTimeOffset>._, A<DateTimeOffset>._,
                A<IReadOnlyList<string>>._, A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._, A<CancellationToken>._))
            .Returns(Task.FromResult<IReadOnlyList<Database.Entities.Resource>>([
                new Database.Entities.Resource { Id = "resource-1", ResourceBookingSlots = [] }
            ]));
        A.CallTo(() => repositoryFactory.BookingRepository.Add(A<Database.Entities.Booking>._))
            .Returns(new Database.Entities.Booking { Id = "booking-1" });
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, A<CancellationToken>._))
            .Returns(Task.FromResult(A.Fake<IDbContextTransaction>()));
        A.CallTo(() => entityMapper.MapTo(A<Database.Entities.Booking>._))
            .Returns(booking);

        var result = await sut.AddAsync(booking, new Database.Entities.Customer { Id = "customer-1" }, organizations, teams,
            null, cancellationToken);

        result.ShouldNotBeNull();
        A.CallTo(() => spacesBookingQuotaService.TryReserveBookingInstancesAsync(
                A<string>._, A<IReadOnlyList<DateTimeOffset>>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}
