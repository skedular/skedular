using Api.Shared.Services.Models;
using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.GraphQL;
using Temporalio.Testing;
using Constants = Booking.Shared.GraphQL.Constants;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Shared.UnitTests.Activities.InvoiceIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GenerateAndSendRecurringInvoiceAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Publish_Changes_And_Stop_When_Xero_Service_Handles_Recurring_Invoice(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IProductVersionRepository productVersionRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IGraphQlTopicEventSender graphQlTopicEventSender,
        [Frozen] IXeroInvoiceService xeroInvoiceService,
        [Frozen] ISkedularInvoiceService skedularInvoiceService,
        InvoiceIntegrations sut,
        string recurringBookingId,
        string productVersionId,
        string pricingId,
        string organizationId,
        string relatedBookingId,
        string subscriptionId)
    {
        var environment = new ActivityEnvironment();

        var recurringBooking = new RecurringBooking
        {
            Id = recurringBookingId,
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBookingSubscription = new MarketplaceBookingSubscription { Id = subscriptionId },
            MarketplaceBooking = new MarketplaceBooking
            {
                InvoiceNumber = "INV-001",
                ProductPricing = ProductPricing.Empty(pricingId),
                ProductVersion = new ProductVersion { Id = productVersionId }
            }
        };
        var productVersion = new ProductVersion
        {
            Id = productVersionId, Product = new Product { Organization = new Organization { Id = organizationId } }
        };
        var relatedBookings = (IReadOnlyList<BookingEntity>)new List<BookingEntity> { new() { Id = relatedBookingId } };

        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBookingId, environment.CancellationTokenSource.Token))
            .Returns(recurringBooking);
        A.CallTo(() => productVersionRepository.GetByIdAsync(productVersionId, environment.CancellationTokenSource.Token))
            .Returns(productVersion);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdUntrackedAsync(
                recurringBookingId,
                recurringBooking.StartDate,
                null,
                environment.CancellationTokenSource.Token))
            .Returns(relatedBookings);
        A.CallTo(() => xeroInvoiceService.HandleRecurringBookingInvoiceAsync(
                organizationId,
                recurringBooking,
                recurringBooking.MarketplaceBooking,
                productVersion,
                environment.CancellationTokenSource.Token))
            .Returns(RecurringInvoiceHandlingDisposition.StopAndPublish);

        await environment.RunAsync(() =>
            sut.GenerateAndSendRecurringInvoiceAsync(new GenerateAndSendRecurringInvoiceInput(recurringBookingId, [])));

        A.CallTo(() => skedularInvoiceService.GenerateAndSendRecurringInvoiceAsync(
                A<GenerateAndSendRecurringInvoiceInput>._,
                A<RecurringBooking>._,
                A<string>._,
                A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                subscriptionId,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.BookingTopicName,
                relatedBookingId,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Fall_Back_To_Skedular_And_Publish_Changes_When_Xero_Service_Does_Not_Handle_Recurring_Invoice(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IProductVersionRepository productVersionRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IGraphQlTopicEventSender graphQlTopicEventSender,
        [Frozen] IXeroInvoiceService xeroInvoiceService,
        [Frozen] ISkedularInvoiceService skedularInvoiceService,
        InvoiceIntegrations sut,
        string recurringBookingId,
        string productVersionId,
        string pricingId,
        string organizationId,
        string relatedBookingId,
        string subscriptionId)
    {
        var environment = new ActivityEnvironment();

        var recurringBooking = new RecurringBooking
        {
            Id = recurringBookingId,
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBookingSubscription = new MarketplaceBookingSubscription { Id = subscriptionId },
            MarketplaceBooking = new MarketplaceBooking
            {
                InvoiceNumber = "INV-001",
                ProductPricing = ProductPricing.Empty(pricingId),
                ProductVersion = new ProductVersion { Id = productVersionId }
            }
        };
        var productVersion = new ProductVersion
        {
            Id = productVersionId, Product = new Product { Organization = new Organization { Id = organizationId } }
        };
        var relatedBookings = (IReadOnlyList<BookingEntity>)new List<BookingEntity> { new() { Id = relatedBookingId } };

        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBookingId, environment.CancellationTokenSource.Token))
            .Returns(recurringBooking);
        A.CallTo(() => productVersionRepository.GetByIdAsync(productVersionId, environment.CancellationTokenSource.Token))
            .Returns(productVersion);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdUntrackedAsync(
                recurringBookingId,
                recurringBooking.StartDate,
                null,
                environment.CancellationTokenSource.Token))
            .Returns(relatedBookings);
        A.CallTo(() => xeroInvoiceService.HandleRecurringBookingInvoiceAsync(
                organizationId,
                recurringBooking,
                recurringBooking.MarketplaceBooking,
                productVersion,
                environment.CancellationTokenSource.Token))
            .Returns(RecurringInvoiceHandlingDisposition.ContinueToSkedular);

        await environment.RunAsync(() =>
            sut.GenerateAndSendRecurringInvoiceAsync(new GenerateAndSendRecurringInvoiceInput(recurringBookingId, [])));

        A.CallTo(() => skedularInvoiceService.GenerateAndSendRecurringInvoiceAsync(
                A<GenerateAndSendRecurringInvoiceInput>.That.Matches(input => input.RecurringBookingId == recurringBookingId),
                recurringBooking,
                organizationId,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                subscriptionId,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.BookingTopicName,
                relatedBookingId,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }
}
