using Api.Shared.Services.Models;
using Booking.Domain.IntegrationTests.Skedular.GraphQL.V1;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using BookingCategory = Booking.Domain.IntegrationTests.Skedular.GraphQL.V1.BookingCategory;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class UpdatePrivateBookingPatchSaveShould(
    IUpdatePrivateBookingPatchSaveMutation updatePrivateBookingPatchSaveMutation,
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Omitted_Details_For_Single_And_Grouped_Saves(
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

        TestBearerTokenHandler.SetToken(identityId);
        try
        {
            var notesResult = await updatePrivateBookingPatchSaveMutation.ExecuteAsync(
                bookingId,
                [PrivateBookingPatchField.Notes],
                [customerId],
                from,
                until,
                updatedNotes,
                cancellationToken);

            notesResult.Errors.Select(error => error.Message).ShouldBeEmpty();
            notesResult.Data.ShouldNotBeNull();
            notesResult.Data.UpdatePrivateBooking.Booking.Notes.ShouldBe(updatedNotes);
            notesResult.Data.UpdatePrivateBooking.Booking.From.ShouldBe(from);

            var groupedResult = await updatePrivateBookingPatchSaveMutation.ExecuteAsync(
                bookingId,
                [PrivateBookingPatchField.Notes, PrivateBookingPatchField.Category],
                [customerId],
                from,
                until,
                updatedNotes,
                cancellationToken);

            groupedResult.Errors.Select(error => error.Message).ShouldBeEmpty();
            groupedResult.Data.ShouldNotBeNull();
            groupedResult.Data.UpdatePrivateBooking.Booking.Notes.ShouldBe(updatedNotes);
            groupedResult.Data.UpdatePrivateBooking.Booking.Category.Category.ShouldBe(
                BookingCategory.AnnualLeave);

            var booking = await repositoryFactory.BookingRepository.GetByIdUntrackedAsync(bookingId, cancellationToken);
            booking.ShouldNotBeNull();
            booking.Notes.ShouldBe(updatedNotes);
            booking.From.ShouldBe(from);
            booking.Until.ShouldBe(until);
            booking.Category.ShouldBe(BookingCategoryConstants.AnnualLeave);
        }
        finally
        {
            TestBearerTokenHandler.ClearToken();
        }
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

        repositoryFactory.IdentityRepository.Add(new Identity { Id = identityId, Customer = customer });
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
            LastModifiedByCustomer = customer
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
