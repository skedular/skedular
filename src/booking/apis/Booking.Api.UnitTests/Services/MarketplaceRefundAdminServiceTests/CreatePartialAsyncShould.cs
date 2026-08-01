using System.Data;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;

namespace Booking.Api.UnitTests.Services.MarketplaceRefundAdminServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CreatePartialAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_And_Automatically_Process_A_Refund_Within_The_Source_Balance(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IDbContextTransaction transaction,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var allocation = new MarketplaceRefundPaymentAllocation
        {
            Id = "allocation-1",
            MarketplaceRefundId = "parent-1",
            SourcePaymentProvider = "STRIPE",
            SourcePaymentReference = "payment-1",
            SourceCapturedAmount = 100m,
            AllocatedRefundAmount = 0m,
            IsSourcePayment = true,
            Currency = "NZD"
        };
        var parent = new MarketplaceRefund
        {
            Id = "parent-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1",
            ReferenceTime = TimeProvider.System.GetUtcNow(),
            IdempotencyKey = "cancellation:booking-1"
        };
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("operator-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, IsolationLevel.Serializable, cancellationToken)).Returns(transaction);
        A.CallTo(() => marketplaceRefundRepository.GetAllocationByIdAsync(allocation.Id, cancellationToken)).Returns(allocation);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(parent.Id, cancellationToken)).Returns(parent);
        A.CallTo(() => marketplaceRefundRepository.GetByIdempotencyKeyAsync("partial:org-1:request-1", cancellationToken))
            .Returns((MarketplaceRefund?)null);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "operator-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.HasConfirmedPaymentAsync(parent, cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundRepository.Add(A<MarketplaceRefund>._)).ReturnsLazily(call => call.GetArgument<MarketplaceRefund>(0)!);
        A.CallTo(() => marketplaceRefundRepository.ReserveAllocationAsync(A<string>._, allocation.Id, 25m, cancellationToken))
            .Returns(new MarketplaceRefundPaymentAllocation());

        var result = await sut.CreatePartialAsync(allocation.Id, 25m, "Service issue", "request-1", cancellationToken);

        result.RefundKind.ShouldBe(MarketplaceRefundKind.Partial);
        result.RefundAmount.ShouldBe(25m);
        result.IdempotencyKey.ShouldBe("partial:org-1:request-1");
        A.CallTo(() => marketplaceRefundRepository.ReserveAllocationAsync(result.Id, allocation.Id, 25m, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalOutboxService.StartWorkflowProcessMarketplaceRefund(
                A<ProcessMarketplaceRefundInput>.That.Matches(input => input.RefundId == result.Id && input.ActorCustomerId == "operator-1"),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_When_The_Requested_Amount_Exceeds_The_Remaining_Source_Balance(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IDbContextTransaction transaction,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var allocation = new MarketplaceRefundPaymentAllocation
        {
            Id = "allocation-1", MarketplaceRefundId = "parent-1", SourceCapturedAmount = 100m, Currency = "NZD"
        };
        var parent = new MarketplaceRefund
        {
            Id = "parent-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1"
        };
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("operator-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, IsolationLevel.Serializable, cancellationToken)).Returns(transaction);
        A.CallTo(() => marketplaceRefundRepository.GetAllocationByIdAsync(allocation.Id, cancellationToken)).Returns(allocation);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(parent.Id, cancellationToken)).Returns(parent);
        A.CallTo(() => marketplaceRefundRepository.GetByIdempotencyKeyAsync(A<string>._, cancellationToken)).Returns((MarketplaceRefund?)null);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "operator-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.HasConfirmedPaymentAsync(parent, cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundRepository.Add(A<MarketplaceRefund>._)).ReturnsLazily(call => call.GetArgument<MarketplaceRefund>(0)!);
        A.CallTo(() => marketplaceRefundRepository.ReserveAllocationAsync(A<string>._, allocation.Id, 101m, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("The refund exceeds the remaining source-payment balance."));

        var exception =
            await Should.ThrowAsync<InvalidOperationException>(() =>
                sut.CreatePartialAsync(allocation.Id, 101m, "Service issue", "request-2", cancellationToken));

        exception.Message.ShouldContain("remaining source-payment balance");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_When_Multiple_Partial_Refunds_Exceed_The_Original_Source_Amount(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IDbContextTransaction transaction,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var allocation = new MarketplaceRefundPaymentAllocation
        {
            Id = "allocation-1", MarketplaceRefundId = "parent-1", SourceCapturedAmount = 100m, Currency = "NZD"
        };
        var parent = new MarketplaceRefund
        {
            Id = "parent-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1"
        };
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("operator-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, IsolationLevel.Serializable, cancellationToken)).Returns(transaction);
        A.CallTo(() => marketplaceRefundRepository.GetAllocationByIdAsync(allocation.Id, cancellationToken)).Returns(allocation);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(parent.Id, cancellationToken)).Returns(parent);
        A.CallTo(() => marketplaceRefundRepository.GetByIdempotencyKeyAsync(A<string>._, cancellationToken)).Returns((MarketplaceRefund?)null);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "operator-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.HasConfirmedPaymentAsync(parent, cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundRepository.Add(A<MarketplaceRefund>._))
            .ReturnsLazily(call => call.GetArgument<MarketplaceRefund>(0)!);
        A.CallTo(() => marketplaceRefundRepository.ReserveAllocationAsync(A<string>._, allocation.Id, 60m, cancellationToken))
            .Returns(new MarketplaceRefundPaymentAllocation());
        A.CallTo(() => marketplaceRefundRepository.ReserveAllocationAsync(A<string>._, allocation.Id, 50m, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("The refund exceeds the remaining source-payment balance."));

        await sut.CreatePartialAsync(allocation.Id, 60m, "First issue", "request-3", cancellationToken);
        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            sut.CreatePartialAsync(allocation.Id, 50m, "Second issue", "request-4", cancellationToken));

        exception.Message.ShouldContain("remaining source-payment balance");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_The_Existing_Operation_When_The_Same_Partial_Refund_Is_Replayed(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IDbContextTransaction transaction,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var allocation = new MarketplaceRefundPaymentAllocation { Id = "allocation-1", MarketplaceRefundId = "parent-1", Currency = "NZD" };
        var parent = new MarketplaceRefund
        {
            Id = "parent-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1"
        };
        var existing = new MarketplaceRefund { Id = "partial-1", IdempotencyKey = "partial:org-1:request-5" };
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("operator-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, IsolationLevel.Serializable, cancellationToken)).Returns(transaction);
        A.CallTo(() => marketplaceRefundRepository.GetAllocationByIdAsync(allocation.Id, cancellationToken)).Returns(allocation);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(parent.Id, cancellationToken)).Returns(parent);
        A.CallTo(() => marketplaceRefundRepository.GetByIdempotencyKeyAsync(existing.IdempotencyKey, cancellationToken)).Returns(existing);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "operator-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.HasConfirmedPaymentAsync(parent, cancellationToken)).Returns(true);

        var result = await sut.CreatePartialAsync(allocation.Id, 25m, "Service issue", "request-5", cancellationToken);

        result.Id.ShouldBe(existing.Id);
        A.CallTo(() => marketplaceRefundRepository.ReserveAllocationAsync(A<string>._, A<string>._, A<decimal>._, cancellationToken))
            .MustNotHaveHappened();
    }
}
