using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Database;

namespace Booking.Api.UnitTests.Services.MarketplaceRefundAdminServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ConfirmBankTransferReceivedAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Complete_An_Approved_Bank_Transfer_Workflow(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        [Frozen] IMarketplaceRefundTransitionService refundTransitionService,
        [Frozen] IUnitOfWork unitOfWork,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1",
            Status = MarketplaceRefundStatusConstants.Approved,
            PaymentAllocations =
            [
                new MarketplaceRefundPaymentAllocation
                {
                    SourcePaymentProvider = "BANK_TRANSFER",
                    SourcePaymentReference = "bank-payment-1",
                    SourceCapturedAmount = 100,
                    AllocatedRefundAmount = 0,
                    IsSourcePayment = true,
                    Currency = "USD"
                }
            ]
        };
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("operator-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "operator-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.HasConfirmedPaymentAsync(refund, cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);
        A.CallTo(() => refundTransitionService.TransitionAsync(refund, A<string>._, A<string?>._, A<string?>._, A<string?>._, cancellationToken))
            .ReturnsLazily(call =>
            {
                var value = call.GetArgument<MarketplaceRefund>(0)!;
                value.Status = call.GetArgument<string>(1)!;
                return Task.FromResult(value);
            });

        await sut.RecordBankTransferSentAsync(refund.Id, "bank-reference-1", cancellationToken);
        var result = await sut.ConfirmBankTransferReceivedAsync(refund.Id, cancellationToken);

        result.Status.ShouldBe(MarketplaceRefundStatusConstants.Completed.ToMarketplaceRefundStatus());
        result.BankTransferReference.ShouldBe("bank-reference-1");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_When_The_Bank_Transfer_Reference_Is_Missing(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1",
            Status = MarketplaceRefundStatusConstants.Processing
        };
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("operator-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "operator-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.HasConfirmedPaymentAsync(refund, cancellationToken)).Returns(true);

        await Should.ThrowAsync<InvalidOperationException>(() => sut.ConfirmBankTransferReceivedAsync(refund.Id, cancellationToken));
    }
}
