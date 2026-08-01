using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Location = Booking.Shared.Database.Entities.Location;
using Organization = Booking.Shared.Database.Entities.Organization;
using RecurringBooking = Booking.Shared.Database.Entities.RecurringBooking;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Shared.UnitTests.Services.PrivateBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Succeed_Without_Quota_Increment(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] ISpacesBookingQuotaService spacesBookingQuotaService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        PrivateBookingService sut,
        IDbContextTransaction dbContextTransaction,
        CancellationToken cancellationToken)
    {
        var booking = new Shared.Models.Booking
        {
            Id = "booking-1",
            From = new DateTimeOffset(2026, 6, 15, 9, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero),
            InvolvedCustomers = [new Customer { Id = "customer-1" }],
            ResourceBookingSlots =
            [
                new ResourceBookingSlot { Resource = new Resource { Id = "resource-1" }, Customers = [new Customer { Id = "customer-1" }] }
            ]
        };
        var existingBooking = new Database.Entities.Booking { Id = "booking-1", Channel = BookingChannelConstants.Private };
        var organizations = new List<Organization> { new() { Id = "org-1" } };
        var teams = new List<Team>();

        A.CallTo(() => repositoryFactory.CustomerRepository.GetByIdsAsync(
                A<IReadOnlyList<string>>._, true, A<CancellationToken>._))
            .Returns(Task.FromResult<IReadOnlyList<Database.Entities.Customer>>([new Database.Entities.Customer { Id = "customer-1" }]));
        A.CallTo(() => entityMapper.MergeTo(
                booking, existingBooking,
                A<IReadOnlyList<Database.Entities.Customer>>._, A<IReadOnlyList<Organization>>._,
                A<IReadOnlyList<Location>>._, A<IReadOnlyList<Team>>._,
                A<IReadOnlyList<Database.Entities.Resource>>._, A<Database.Entities.Customer?>._, A<Database.Entities.Customer?>._,
                null, null, A<RecurringBooking?>._))
            .Returns(existingBooking);
        A.CallTo(() => repositoryFactory.BookingRepository.Update(existingBooking))
            .Returns(existingBooking);
        A.CallTo(() => entityMapper.MapTo(existingBooking)).Returns(booking);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, A<CancellationToken>._))
            .Returns(Task.FromResult(dbContextTransaction));

        var result = await sut.UpdateAsync(booking, existingBooking, null, organizations, teams,
            null, false, cancellationToken);

        result.ShouldNotBeNull();
        result.Id.ShouldBe("booking-1");
        A.CallTo(() => spacesBookingQuotaService.CanCreateBookingInstanceAsync(
                A<string>._, A<DateTimeOffset>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}
