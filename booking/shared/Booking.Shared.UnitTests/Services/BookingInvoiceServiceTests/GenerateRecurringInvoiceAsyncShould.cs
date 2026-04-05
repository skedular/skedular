using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using AutoFixture.Xunit3;
using Booking.Shared.Configurations;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Grpc;
using FakeItEasy;
using Grpc.Core;
using QuestPDF.Infrastructure;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;
using ProductEntity = Booking.Shared.Database.Entities.Product;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using OrganizationBillingCycleModel = Api.Shared.Services.Models.OrganizationBillingCycle;

namespace Booking.Shared.UnitTests.Services.BookingInvoiceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GenerateRecurringInvoiceAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Use_Shared_Recurring_Billing_Schedule_For_Non_Xero_Recurring_Invoices(
        [Frozen] OrganizationConfiguration organizationConfiguration,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IProductVersionRepository productVersionRepository,
        [Frozen] IRecurringInvoiceBillingScheduleService recurringInvoiceBillingScheduleService,
        [Frozen] IInvoicePaymentTermsService invoicePaymentTermsService,
        IProductVersionHelperService productVersionHelperService,
        IMapper mapper,
        IOrganizationArrearsBillingPlannerService organizationArrearsBillingPlannerService,
        CallInvoker callInvoker,
        string recurringBookingId,
        string productVersionId,
        string pricingId,
        string organizationId)
    {
        organizationConfiguration.ApiKey = "api-key";
        var sut = new BookingInvoiceService(
            repositoryFactory,
            organizationConfiguration,
            new OrganizationService.OrganizationServiceClient(callInvoker),
            invoicePaymentTermsService,
            recurringInvoiceBillingScheduleService,
            productVersionHelperService,
            mapper,
            organizationArrearsBillingPlannerService);

        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            CreatedAt = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2026, 6, 30, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                Quantity = 1,
                PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod(),
                BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
                ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Quarterly },
                ProductVersion = new ProductVersionEntity { Id = productVersionId }
            }
        };
        var productVersion = new ProductVersionEntity
        {
            Id = productVersionId,
            Product = new ProductEntity
            {
                Organization = new OrganizationEntity
                {
                    Id = organizationId,
                    BillingCycle = OrganizationBillingCycleModel.Weekly.ToOrganizationBillingCycle()
                }
            }
        };
        var organization = new Organization
        {
            Id = organizationId,
            BillingDetails = new BillingDetails { InvoiceDueInDays = 7 }
        };
        var bankAccountConnection = new BankAccountConnection
        {
            Edges =
            {
                new BankAccountEdge
                {
                    Node = new BankAccount
                    {
                        Id = "bank-1",
                        IsDefault = true,
                        BankName = "Test Bank",
                        AccountHolderName = "Operations",
                        AccountNumber = "12-1234-1234567-00",
                        Country = "NZ"
                    }
                }
            }
        };
        var billingDefinition = new RecurringInvoiceBillingDefinition(
            Booking.Shared.Models.XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            ProductPricingCadence.Weekly,
            23.0769m);

        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBookingId, A<CancellationToken>._)).Returns(recurringBooking);
        A.CallTo(() => productVersionRepository.GetByIdAsync(productVersionId, A<CancellationToken>._)).Returns(productVersion);
        A.CallTo(() => recurringInvoiceBillingScheduleService.GetSchedule(
                recurringBooking,
                recurringBooking.MarketplaceBooking,
                OrganizationBillingCycleModel.Weekly))
            .Returns(billingDefinition);
        A.CallTo(() => invoicePaymentTermsService.GetDueDate(
                recurringBooking.CreatedAt,
                organization.BillingDetails.InvoiceDueInDays))
            .Returns(recurringBooking.CreatedAt.AddDays(7));
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetBankAccountsInput, BankAccountConnection>>._,
                A<string?>._,
                A<CallOptions>.That.Matches(options => options.Headers != null &&
                    options.Headers.Any(item => item.Key == Constants.ApiKey && item.Value == organizationConfiguration.ApiKey)),
                A<Admin_GetBankAccountsInput>.That.Matches(input => input.Where.OrganizationId == organizationId)))
            .Returns(CreateResponse(bankAccountConnection));
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetInput, Organization>>._,
                A<string?>._,
                A<CallOptions>.That.Matches(options => options.Headers != null &&
                    options.Headers.Any(item => item.Key == Constants.ApiKey && item.Value == organizationConfiguration.ApiKey)),
                A<Admin_GetInput>.That.Matches(input => input.Id == organizationId)))
            .Returns(CreateResponse(organization));

        var result = await sut.GenerateRecurringInvoiceAsync(recurringBookingId, false, CancellationToken.None);

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IDocument>();
        A.CallTo(() => recurringInvoiceBillingScheduleService.GetSchedule(
                recurringBooking,
                recurringBooking.MarketplaceBooking,
                OrganizationBillingCycleModel.Weekly))
            .MustHaveHappenedOnceExactly();
    }

    private static AsyncUnaryCall<T> CreateResponse<T>(T response) where T : class =>
        new(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });
}
