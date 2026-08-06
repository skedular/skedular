using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Grpc;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using BookingGrpcConfig = Api.Shared.Services.Configurations.Grpc.BookingConfiguration;

namespace Booking.Domain.IntegrationTests.Api.Grpc.BookingGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class UpdatePrivateShould(
    BookingService.BookingServiceClient bookingServiceClient,
    IRepositoryFactory repositoryFactory,
    BookingGrpcConfig bookingConfiguration,
    TimeProvider timeProvider)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Omitted_Details(
        string bookingId,
        string customerId,
        string identityId,
        string originalNotes,
        string updatedNotes,
        CancellationToken cancellationToken)
    {
        var from = timeProvider.GetUtcNow().AddDays(1);
        var until = from.AddHours(1);
        await SeedPrivateBookingAsync(bookingId, customerId, identityId, originalNotes, from, until, cancellationToken);

        var result = await bookingServiceClient.UpdatePrivateAsync(
            new UpdatePrivateInput
            {
                Id = bookingId,
                Notes = updatedNotes,
                FieldsToUpdate =
                {
                    PrivateBookingPatchField.Notes,
                },
            },
            bookingConfiguration.ApiKey.CreateMetadata(identityId),
            cancellationToken: cancellationToken);

        result.ShouldNotBeNull();
        result.Notes.ShouldBe(updatedNotes);

        var booking = await repositoryFactory.BookingRepository.GetByIdUntrackedAsync(bookingId, cancellationToken);
        booking.ShouldNotBeNull();
        booking.Notes.ShouldBe(updatedNotes);
        booking.From.ShouldBe(from);
        booking.Until.ShouldBe(until);
    }

    private async Task SeedPrivateBookingAsync(
        string bookingId,
        string customerId,
        string identityId,
        string notes,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, true, cancellationToken);

        repositoryFactory.IdentityRepository.Add(new Identity
        {
            Id = identityId,
            Customer = customer,
        });
        repositoryFactory.BookingRepository.Add(new BookingEntity
        {
            Id = bookingId,
            From = from,
            Until = until,
            Notes = notes,
            Category = BookingCategoryConstants.WorkingFromOffice,
            Channel = BookingChannelConstants.Private,
            Schedules = [],
            InvolvedCustomers = [customer],
            CreatedByCustomer = customer,
            LastModifiedByCustomer = customer,
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
