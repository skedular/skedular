using Booking.Processors.Subscribers;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Stripe;
using MarketplaceRefundStatusConstants = Booking.Shared.Models.MarketplaceRefundStatusConstants;
using BookingInternalEvent = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Event;
using BookingInternalKey = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key;
using BookingInternalMetadata = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Metadata;
using BookingInternalType = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Type;

namespace Booking.Processors.UnitTests.Subscribers;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BookingInternalSubscriberStripePayoutShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Consume_Serialized_Charge_Webhook(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        EventContext eventContext,
        BookingInternalSubscriber sut,
        CancellationToken cancellationToken)
    {
        var checkout = new StripeCheckoutSession
        {
            PaymentIntentId = "pi_1",
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => checkoutRepository.GetByPaymentIntentIdAsync("pi_1", cancellationToken)).Returns(checkout);
        var payload =
            "{\"id\":\"evt_1\",\"type\":\"charge.succeeded\",\"data\":{\"object\":{\"id\":\"ch_1\",\"object\":\"charge\",\"payment_intent\":\"pi_1\",\"transfer\":\"tr_1\"}}}";
        var @event = new BookingInternalEvent
        {
            Metadata = new BookingInternalMetadata
            {
                Type = BookingInternalType.StripeConnectAccountWebhookEventReceived,
            },
            StripeConnectAccountWebhookEventPayload = payload,
        };

        await sut.HandleAsync(eventContext, new BookingInternalKey(), @event, cancellationToken);

        checkout.ChargeId.ShouldBe("ch_1");
        checkout.TransferId.ShouldBe("tr_1");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Consume_Serialized_Paid_Payout_Webhook(
        [Frozen]
        IStripePayoutReconciliationService payoutReconciliationService,
        [Frozen]
        EventContext eventContext,
        BookingInternalSubscriber sut,
        CancellationToken cancellationToken)
    {
        var payload =
            "{\"id\":\"evt_paid\",\"account\":\"acct_1\",\"created\":1785232800,\"type\":\"payout.paid\",\"data\":{\"object\":{\"id\":\"po_1\",\"object\":\"payout\",\"status\":\"paid\"}}}";
        var @event = new BookingInternalEvent
        {
            Metadata = new BookingInternalMetadata
            {
                Type = BookingInternalType.StripeConnectAccountWebhookEventReceived,
            },
            StripeConnectAccountWebhookEventPayload = payload,
        };

        await sut.HandleAsync(eventContext, new BookingInternalKey(), @event, cancellationToken);

        A.CallTo(() => payoutReconciliationService.HandlePaidAsync(
                A<Payout>.That.Matches(payout => payout.Id == "po_1" && payout.Status == "paid"),
                "acct_1", cancellationToken, A<DateTimeOffset?>._, "evt_paid"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Consume_Serialized_Failed_Payout_Webhook(
        [Frozen]
        IStripePayoutReconciliationService payoutReconciliationService,
        [Frozen]
        EventContext eventContext,
        BookingInternalSubscriber sut,
        CancellationToken cancellationToken)
    {
        var payload =
            "{\"id\":\"evt_failed\",\"account\":\"acct_1\",\"type\":\"payout.failed\",\"data\":{\"object\":{\"id\":\"po_1\",\"object\":\"payout\",\"status\":\"failed\",\"failure_message\":\"bank rejected\"}}}";
        var @event = new BookingInternalEvent
        {
            Metadata = new BookingInternalMetadata
            {
                Type = BookingInternalType.StripeConnectAccountWebhookEventReceived,
            },
            StripeConnectAccountWebhookEventPayload = payload,
        };

        await sut.HandleAsync(eventContext, new BookingInternalKey(), @event, cancellationToken);

        A.CallTo(() => payoutReconciliationService.HandleStateChangedAsync(
                A<Payout>.That.Matches(payout => payout.Id == "po_1" && payout.Status == "failed" &&
                                                 payout.FailureMessage == "bank rejected"),
                "payout.failed", cancellationToken, "acct_1", A<DateTimeOffset?>._, "evt_failed"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Consume_Serialized_Canceled_Payout_Webhook(
        [Frozen]
        IStripePayoutReconciliationService payoutReconciliationService,
        [Frozen]
        EventContext eventContext,
        BookingInternalSubscriber sut,
        CancellationToken cancellationToken)
    {
        var payload =
            "{\"id\":\"evt_canceled\",\"account\":\"acct_1\",\"type\":\"payout.canceled\",\"data\":{\"object\":{\"id\":\"po_1\",\"object\":\"payout\",\"status\":\"canceled\"}}}";
        var @event = new BookingInternalEvent
        {
            Metadata = new BookingInternalMetadata
            {
                Type = BookingInternalType.StripeConnectAccountWebhookEventReceived,
            },
            StripeConnectAccountWebhookEventPayload = payload,
        };

        await sut.HandleAsync(eventContext, new BookingInternalKey(), @event, cancellationToken);

        A.CallTo(() => payoutReconciliationService.HandleStateChangedAsync(
                A<Payout>.That.Matches(payout => payout.Id == "po_1" && payout.Status == "canceled"),
                "payout.canceled", cancellationToken, "acct_1", A<DateTimeOffset?>._, "evt_canceled"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Consume_Serialized_Refund_Webhook_With_Provider_Correlation(
        [Frozen]
        IStripeHostRefundService stripeHostRefundService,
        [Frozen]
        EventContext eventContext,
        BookingInternalSubscriber sut,
        CancellationToken cancellationToken)
    {
        var payload =
            "{\"id\":\"evt_refund\",\"account\":\"acct_1\",\"type\":\"refund.updated\",\"data\":{\"object\":{\"id\":\"re_1\",\"object\":\"refund\",\"status\":\"succeeded\"}}}";
        var @event = new BookingInternalEvent
        {
            Metadata = new BookingInternalMetadata
            {
                Type = BookingInternalType.StripeConnectAccountWebhookEventReceived,
            },
            StripeConnectAccountWebhookEventPayload = payload,
        };

        await sut.HandleAsync(eventContext, new BookingInternalKey(), @event, cancellationToken);

        A.CallTo(() => stripeHostRefundService.ReconcileAsync(
                A<Refund>.That.Matches(refund => refund.Id == "re_1"),
                cancellationToken,
                "acct_1",
                "evt_refund"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Persist_Charge_Context(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        BookingInternalSubscriber sut,
        CancellationToken cancellationToken)
    {
        var checkout = new StripeCheckoutSession
        {
            PaymentIntentId = "pi_1",
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByPaymentIntentIdAsync("pi_1", cancellationToken)).Returns(checkout);

        await sut.HandleChargeSucceededAsync(new Charge
        {
            Id = "ch_1",
            PaymentIntentId = "pi_1",
            TransferId = "tr_1",
        }, cancellationToken);

        checkout.ChargeId.ShouldBe("ch_1");
        checkout.TransferId.ShouldBe("tr_1");
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Persist_An_Exactly_Matched_Payout(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var checkout = new StripeCheckoutSession
        {
            ChargeType = "Destination",
            ChargeId = "ch_1",
            DestinationAccountId = "acct_1",
        };
        var eventCreatedAt = new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero);
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetDestinationChargeCandidatesAsync("acct_1", A<IReadOnlyCollection<string>>._, cancellationToken))
            .Returns([checkout]);
        A.CallTo(() => stripeClient.GetPayoutBalanceTransactionsAsync("po_1", "acct_1", cancellationToken))
            .Returns(new[]
            {
                new BalanceTransaction
                {
                    SourceId = "ch_1",
                    Source = null,
                },
            });

        await sut.HandlePaidAsync(new Payout
        {
            Id = "po_1",
            Status = "paid",
        }, "acct_1", cancellationToken, eventCreatedAt);

        checkout.PayoutId.ShouldBe("po_1");
        checkout.PayoutStatus.ShouldBe("paid");
        checkout.PayoutDisbursedAt.ShouldBe(eventCreatedAt);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Existing_Refund_Payout_Context_When_Payout_Is_Paid(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var checkout = new StripeCheckoutSession
        {
            ChargeType = "Destination",
            PaymentIntentId = "pi_1",
            ChargeId = "ch_1",
            TransferId = "tr_1",
        };
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            StripePaymentIntentId = "pi_1",
            StripeChargeType = "Destination",
            Status = MarketplaceRefundStatusConstants.Processing,
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByPaymentIntentIdAsync("pi_1", cancellationToken)).Returns(checkout);
        A.CallTo(() => refundRepository.GetByStripePaymentContextAsync("tr_1", "ch_1", "pi_1", cancellationToken))
            .Returns([refund]);

        await sut.HandlePaidAsync(
            new Payout
            {
                Id = "po_1",
                Status = "paid",
                Metadata = new Dictionary<string, string>
                {
                    ["payment_intent_id"] = "pi_1",
                },
            },
            "acct_1",
            cancellationToken);

        refund.StripeChargeId.ShouldBe("ch_1");
        refund.StripeTransferId.ShouldBe("tr_1");
        refund.PostPayoutRefund.ShouldBeTrue();
        A.CallTo(() => refundRepository.Update(refund)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Correlate_Payout_By_Authoritative_Payment_Intent_Metadata(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var checkout = new StripeCheckoutSession
        {
            PaymentIntentId = "pi_1",
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByPaymentIntentIdAsync("pi_1", cancellationToken)).Returns(checkout);

        await sut.HandlePaidAsync(
            new Payout
            {
                Id = "po_metadata_pi",
                Status = "paid",
                Metadata = new Dictionary<string, string>
                {
                    ["payment_intent_id"] = "pi_1",
                },
            },
            "acct_1",
            cancellationToken);

        checkout.PayoutId.ShouldBe("po_metadata_pi");
        checkout.PayoutStatus.ShouldBe("paid");
        checkout.PayoutDisbursedAt.ShouldNotBeNull();
        A.CallTo(() => stripeClient.GetPayoutBalanceTransactionsAsync(A<string>._, A<string>._, cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Correlate_Payout_By_Authoritative_Transfer_Metadata(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var checkout = new StripeCheckoutSession
        {
            TransferId = "tr_1",
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByTransferIdAsync("tr_1", cancellationToken)).Returns(checkout);

        await sut.HandlePaidAsync(
            new Payout
            {
                Id = "po_metadata_transfer",
                Status = "paid",
                Metadata = new Dictionary<string, string>
                {
                    ["transfer_id"] = "tr_1",
                },
            },
            "acct_1",
            cancellationToken);

        checkout.PayoutId.ShouldBe("po_metadata_transfer");
        checkout.PayoutStatus.ShouldBe("paid");
        checkout.PayoutDisbursedAt.ShouldNotBeNull();
        A.CallTo(() => stripeClient.GetPayoutBalanceTransactionsAsync(A<string>._, A<string>._, cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Correlate_Failed_Payout_By_Authoritative_Metadata(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var checkout = new StripeCheckoutSession
        {
            PaymentIntentId = "pi_failed",
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByPayoutIdAsync("po_failed_metadata", cancellationToken))
            .Returns(Task.FromResult<StripeCheckoutSession?>(null));
        A.CallTo(() => checkoutRepository.GetByPaymentIntentIdAsync("pi_failed", cancellationToken)).Returns(checkout);
        A.CallTo(() => refundRepository.GetByStripePaymentContextAsync(
                A<string?>._, A<string?>._, "pi_failed", cancellationToken))
            .Returns([]);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        await sut.HandleStateChangedAsync(
            new Payout
            {
                Id = "po_failed_metadata",
                Status = "failed",
                FailureMessage = "bank rejected",
                Metadata = new Dictionary<string, string>
                {
                    ["payment_intent_id"] = "pi_failed",
                },
            },
            "payout.failed",
            cancellationToken,
            "acct_1");

        checkout.PayoutId.ShouldBe("po_failed_metadata");
        checkout.PayoutStatus.ShouldBe("failed");
        checkout.PayoutFailureMessage.ShouldBe("bank rejected");
        checkout.PayoutDisbursedAt.ShouldBeNull();
        A.CallTo(() => refundRepository.AddExternalReconciliation(A<MarketplaceExternalRefundReconciliation>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Correlate_Canceled_Payout_By_Balance_Transaction(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var checkout = new StripeCheckoutSession
        {
            ChargeType = "Destination",
            ChargeId = "ch_canceled",
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByPayoutIdAsync("po_canceled_balance", cancellationToken))
            .Returns(Task.FromResult<StripeCheckoutSession?>(null));
        A.CallTo(() => stripeClient.GetPayoutBalanceTransactionsAsync("po_canceled_balance", "acct_1", cancellationToken))
            .Returns(new[]
            {
                new BalanceTransaction
                {
                    SourceId = "ch_canceled",
                    Source = null,
                },
            });
        A.CallTo(() => checkoutRepository.GetDestinationChargeCandidatesAsync(
                "acct_1", A<IReadOnlyCollection<string>>._, cancellationToken))
            .Returns([checkout]);
        A.CallTo(() => refundRepository.GetByStripePaymentContextAsync(
                A<string?>._, "ch_canceled", A<string?>._, cancellationToken))
            .Returns([]);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        await sut.HandleStateChangedAsync(
            new Payout
            {
                Id = "po_canceled_balance",
                Status = "canceled",
            },
            "payout.canceled",
            cancellationToken,
            "acct_1");

        checkout.PayoutId.ShouldBe("po_canceled_balance");
        checkout.PayoutStatus.ShouldBe("canceled");
        checkout.PayoutDisbursedAt.ShouldBeNull();
        A.CallTo(() => stripeClient.GetPayoutBalanceTransactionsAsync(
                "po_canceled_balance", "acct_1", cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Correlate_Canceled_Payout_By_Authoritative_Metadata(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var checkout = new StripeCheckoutSession
        {
            PaymentIntentId = "pi_canceled",
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByPayoutIdAsync("po_canceled_metadata", cancellationToken))
            .Returns(Task.FromResult<StripeCheckoutSession?>(null));
        A.CallTo(() => checkoutRepository.GetByPaymentIntentIdAsync("pi_canceled", cancellationToken)).Returns(checkout);
        A.CallTo(() => refundRepository.GetByStripePaymentContextAsync(
                A<string?>._, A<string?>._, "pi_canceled", cancellationToken))
            .Returns([]);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        await sut.HandleStateChangedAsync(
            new Payout
            {
                Id = "po_canceled_metadata",
                Status = "canceled",
                Metadata = new Dictionary<string, string>
                {
                    ["payment_intent_id"] = "pi_canceled",
                },
            },
            "payout.canceled",
            cancellationToken,
            "acct_1");

        checkout.PayoutId.ShouldBe("po_canceled_metadata");
        checkout.PayoutStatus.ShouldBe("canceled");
        checkout.PayoutDisbursedAt.ShouldBeNull();
        A.CallTo(() => refundRepository.AddExternalReconciliation(A<MarketplaceExternalRefundReconciliation>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Correlate_Failed_Payout_By_Balance_Transaction(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var checkout = new StripeCheckoutSession
        {
            ChargeType = "Destination",
            ChargeId = "ch_failed",
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByPayoutIdAsync("po_failed_balance", cancellationToken))
            .Returns(Task.FromResult<StripeCheckoutSession?>(null));
        A.CallTo(() => stripeClient.GetPayoutBalanceTransactionsAsync("po_failed_balance", "acct_1", cancellationToken))
            .Returns(new[]
            {
                new BalanceTransaction
                {
                    SourceId = "ch_failed",
                    Source = null,
                },
            });
        A.CallTo(() => checkoutRepository.GetDestinationChargeCandidatesAsync(
                "acct_1", A<IReadOnlyCollection<string>>._, cancellationToken))
            .Returns([checkout]);
        A.CallTo(() => refundRepository.GetByStripePaymentContextAsync(
                A<string?>._, "ch_failed", A<string?>._, cancellationToken))
            .Returns([]);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        await sut.HandleStateChangedAsync(
            new Payout
            {
                Id = "po_failed_balance",
                Status = "failed",
            },
            "payout.failed",
            cancellationToken,
            "acct_1");

        checkout.PayoutId.ShouldBe("po_failed_balance");
        checkout.PayoutStatus.ShouldBe("failed");
        checkout.PayoutDisbursedAt.ShouldBeNull();
        A.CallTo(() => stripeClient.GetPayoutBalanceTransactionsAsync(
                "po_failed_balance", "acct_1", cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Record_Unresolved_Metadata_Payout_For_Reconciliation(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByPaymentIntentIdAsync("pi_missing", cancellationToken))
            .Returns(Task.FromResult<StripeCheckoutSession?>(null));
        A.CallTo(() => refundRepository.GetExternalReconciliationAsync("STRIPE_PAYOUT", "po_unresolved", null, cancellationToken))
            .Returns(Task.FromResult<MarketplaceExternalRefundReconciliation?>(null));

        await sut.HandlePaidAsync(
            new Payout
            {
                Id = "po_unresolved",
                Status = "paid",
                Metadata = new Dictionary<string, string>
                {
                    ["payment_intent_id"] = "pi_missing",
                },
            },
            "acct_1",
            cancellationToken);

        A.CallTo(() => refundRepository.AddExternalReconciliation(A<MarketplaceExternalRefundReconciliation>.That.Matches(item =>
                item.Provider == "STRIPE_PAYOUT" && item.ExternalRefundId == "po_unresolved" &&
                item.ResolutionReason!.Contains("did not resolve"))))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => checkoutRepository.GetDestinationChargeCandidatesAsync(
                A<string>._, A<IReadOnlyCollection<string>>._, cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Route_Conflicting_Metadata_Payout_To_Reconciliation(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var paymentIntentCheckout = new StripeCheckoutSession
        {
            Id = "checkout_pi",
            PaymentIntentId = "pi_1",
        };
        var transferCheckout = new StripeCheckoutSession
        {
            Id = "checkout_transfer",
            TransferId = "tr_1",
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByPaymentIntentIdAsync("pi_1", cancellationToken)).Returns(paymentIntentCheckout);
        A.CallTo(() => checkoutRepository.GetByTransferIdAsync("tr_1", cancellationToken)).Returns(transferCheckout);
        A.CallTo(() => refundRepository.GetExternalReconciliationAsync("STRIPE_PAYOUT", "po_conflicting", null, cancellationToken))
            .Returns(Task.FromResult<MarketplaceExternalRefundReconciliation?>(null));

        await sut.HandlePaidAsync(
            new Payout
            {
                Id = "po_conflicting",
                Status = "paid",
                Metadata = new Dictionary<string, string>
                {
                    ["payment_intent_id"] = "pi_1",
                    ["transfer_id"] = "tr_1",
                },
            },
            null,
            cancellationToken);

        A.CallTo(() => refundRepository.AddExternalReconciliation(A<MarketplaceExternalRefundReconciliation>.That.Matches(item =>
                item.Provider == "STRIPE_PAYOUT" && item.ExternalRefundId == "po_conflicting" &&
                item.ResolutionReason!.Contains("conflicting", StringComparison.OrdinalIgnoreCase))))
            .MustHaveHappenedOnceExactly();
        paymentIntentCheckout.PayoutId.ShouldBeNull();
        transferCheckout.PayoutId.ShouldBeNull();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Match_Payout_When_Stripe_Leaves_Source_Unexpanded(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var checkout = new StripeCheckoutSession
        {
            ChargeType = "Destination",
            ChargeId = "ch_unexpanded",
            DestinationAccountId = "acct_1",
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetDestinationChargeCandidatesAsync("acct_1", A<IReadOnlyCollection<string>>._, cancellationToken))
            .Returns([checkout]);
        A.CallTo(() => stripeClient.GetPayoutBalanceTransactionsAsync("po_unexpanded", "acct_1", cancellationToken))
            .Returns(new[]
            {
                new BalanceTransaction
                {
                    SourceId = "ch_unexpanded",
                    Source = null,
                },
            });

        await sut.HandlePaidAsync(new Payout
        {
            Id = "po_unexpanded",
            Status = "paid",
        }, "acct_1", cancellationToken);

        checkout.PayoutId.ShouldBe("po_unexpanded");
        checkout.PayoutDisbursedAt.ShouldNotBeNull();
        A.CallTo(() => refundRepository.AddExternalReconciliation(A<MarketplaceExternalRefundReconciliation>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Match_Payout_When_Balance_Transaction_Result_Exceeds_One_Hundred_Items(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var checkout = new StripeCheckoutSession
        {
            ChargeType = "Destination",
            ChargeId = "ch_101",
            DestinationAccountId = "acct_1",
        };
        var transactions = Enumerable.Range(1, 100)
            .Select(index => new BalanceTransaction
            {
                SourceId = $"ch_{index}",
            })
            .Append(new BalanceTransaction
            {
                SourceId = "ch_101",
            })
            .ToArray();
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetDestinationChargeCandidatesAsync("acct_1", A<IReadOnlyCollection<string>>._, cancellationToken))
            .Returns([checkout]);
        A.CallTo(() => stripeClient.GetPayoutBalanceTransactionsAsync("po_101", "acct_1", cancellationToken))
            .Returns(transactions);

        await sut.HandlePaidAsync(new Payout
        {
            Id = "po_101",
            Status = "paid",
        }, "acct_1", cancellationToken);

        checkout.PayoutDisbursedAt.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Record_Unmatched_Payout_For_Reconciliation(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => stripeClient.GetPayoutBalanceTransactionsAsync("po_1", "acct_1", cancellationToken))
            .Returns(Array.Empty<BalanceTransaction>());
        A.CallTo(() => refundRepository.GetExternalReconciliationAsync("STRIPE_PAYOUT", "po_1", null, cancellationToken))
            .Returns(Task.FromResult<MarketplaceExternalRefundReconciliation?>(null));

        await sut.HandlePaidAsync(new Payout
        {
            Id = "po_1",
        }, "acct_1", cancellationToken);

        A.CallTo(() => refundRepository.AddExternalReconciliation(A<MarketplaceExternalRefundReconciliation>.That.Matches(item =>
            item.Provider == "STRIPE_PAYOUT" && item.ExternalRefundId == "po_1"))).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reconcile_All_Checkouts_In_A_Payout(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var firstCheckout = new StripeCheckoutSession
        {
            ChargeType = "Destination",
            ChargeId = "ch_1",
            DestinationAccountId = "acct_1",
        };
        var secondCheckout = new StripeCheckoutSession
        {
            ChargeType = "Destination",
            ChargeId = "ch_2",
            DestinationAccountId = "acct_1",
        };
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => checkoutRepository.GetDestinationChargeCandidatesAsync("acct_1", A<IReadOnlyCollection<string>>._, cancellationToken))
            .Returns([firstCheckout, secondCheckout]);
        A.CallTo(() => stripeClient.GetPayoutBalanceTransactionsAsync("po_1", "acct_1", cancellationToken))
            .Returns(new[]
            {
                new BalanceTransaction
                {
                    SourceId = "ch_1",
                },
                new BalanceTransaction
                {
                    SourceId = "ch_2",
                },
            });
        A.CallTo(() => refundRepository.GetExternalReconciliationAsync("STRIPE_PAYOUT", "po_1", null, cancellationToken))
            .Returns(Task.FromResult<MarketplaceExternalRefundReconciliation?>(null));

        await sut.HandlePaidAsync(new Payout
        {
            Id = "po_1",
        }, "acct_1", cancellationToken);

        firstCheckout.PayoutDisbursedAt.ShouldNotBeNull();
        secondCheckout.PayoutDisbursedAt.ShouldNotBeNull();
        A.CallTo(() => refundRepository.AddExternalReconciliation(A<MarketplaceExternalRefundReconciliation>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Clear_Disbursement_On_Failed_Or_Canceled_Payout(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken,
        string eventType)
    {
        eventType = eventType is "payout.failed" or "payout.canceled" ? eventType : "payout.failed";
        var checkout = new StripeCheckoutSession
        {
            PayoutId = "po_1",
            PayoutDisbursedAt = TimeProvider.System.GetUtcNow(),
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByPayoutIdAsync("po_1", cancellationToken)).Returns(checkout);

        await sut.HandleStateChangedAsync(new Payout
            {
                Id = "po_1",
                Status = "failed",
                FailureMessage = "bank rejected",
            }, eventType,
            cancellationToken);

        checkout.PayoutDisbursedAt.ShouldBeNull();
        checkout.PayoutFailureMessage.ShouldBe("bank rejected");
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Disbursement_On_NonTerminal_Payout_Update(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var disbursedAt = TimeProvider.System.GetUtcNow();
        var checkout = new StripeCheckoutSession
        {
            PayoutId = "po_1",
            PayoutDisbursedAt = disbursedAt,
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByPayoutIdAsync("po_1", cancellationToken)).Returns(checkout);

        await sut.HandleStateChangedAsync(
            new Payout
            {
                Id = "po_1",
                Status = "in_transit",
            }, "payout.updated", cancellationToken);

        checkout.PayoutDisbursedAt.ShouldBe(disbursedAt);
        checkout.PayoutStatus.ShouldBe("in_transit");
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Record_Disbursement_For_Paid_Update(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var eventCreatedAt = new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero);
        var checkout = new StripeCheckoutSession
        {
            PayoutId = "po_1",
        };
        var reconciliation = new MarketplaceExternalRefundReconciliation
        {
            Provider = "STRIPE_PAYOUT",
            ExternalRefundId = "po_1",
            Status = "Open",
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByPayoutIdAsync("po_1", cancellationToken)).Returns(checkout);
        A.CallTo(() => refundRepository.GetExternalReconciliationAsync(
                "STRIPE_PAYOUT", "po_1", null, cancellationToken))
            .Returns(reconciliation);
        A.CallTo(() => refundRepository.UpdateExternalReconciliation(reconciliation)).Returns(reconciliation);

        await sut.HandleStateChangedAsync(
            new Payout
            {
                Id = "po_1",
                Status = "paid",
            }, "payout.updated", cancellationToken,
            eventCreatedAt: eventCreatedAt);

        checkout.PayoutStatus.ShouldBe("paid");
        checkout.PayoutDisbursedAt.ShouldBe(eventCreatedAt);
        reconciliation.Status.ShouldBe("Resolved");
        reconciliation.ResolutionReason.ShouldNotBeNull();
        reconciliation.ResolutionReason.ShouldContain("matched");
        A.CallTo(() => refundRepository.UpdateExternalReconciliation(reconciliation))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Existing_Unmatched_Payout_Reconciliation_On_Later_State_Event(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var eventCreatedAt = TimeProvider.System.GetUtcNow();
        var existing = new MarketplaceExternalRefundReconciliation
        {
            Provider = "STRIPE_PAYOUT",
            ExternalRefundId = "po_1",
            Status = "Open",
            FirstSeenAt = TimeProvider.System.GetUtcNow().AddMinutes(-5),
            LastSeenAt = TimeProvider.System.GetUtcNow().AddMinutes(-5),
            ResolutionReason = "Initially unmatched payout.",
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => checkoutRepository.GetByPayoutIdAsync("po_1", cancellationToken))
            .Returns(Task.FromResult<StripeCheckoutSession?>(null));
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => refundRepository.GetExternalReconciliationAsync("STRIPE_PAYOUT", "po_1", null, cancellationToken))
            .Returns(existing);

        await sut.HandleStateChangedAsync(
            new Payout
            {
                Id = "po_1",
                Status = "failed",
                FailureMessage = "bank rejected",
            },
            "payout.failed", cancellationToken, eventCreatedAt: eventCreatedAt);

        existing.Status.ShouldBe("Open");
        existing.ResolutionReason.ShouldContain("payout.failed");
        existing.LastSeenAt.ShouldBeGreaterThan(existing.FirstSeenAt);
        A.CallTo(() => refundRepository.UpdateExternalReconciliation(existing)).MustHaveHappenedOnceExactly();
        A.CallTo(() => refundRepository.AddExternalReconciliation(A<MarketplaceExternalRefundReconciliation>._))
            .MustNotHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Move_NonTerminal_Refund_To_Reconciliation_When_Payout_Fails(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IStripeCheckoutSessionRepository checkoutRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IMarketplaceRefundTransitionService refundTransitionService,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var checkout = new StripeCheckoutSession
        {
            PayoutId = "po_1",
            TransferId = "tr_1",
            ChargeId = "",
            PaymentIntentId = "",
        };
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Processing,
            StripeTransferId = "tr_1",
            PostPayoutRefund = true,
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(checkoutRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => checkoutRepository.GetByPayoutIdAsync("po_1", cancellationToken)).Returns(checkout);
        A.CallTo(() => refundRepository.GetByStripePaymentContextAsync("tr_1", "", "", cancellationToken))
            .Returns([refund]);
        A.CallTo(() => refundTransitionService.TransitionAsync(
                A<MarketplaceRefund>.That.IsSameAs(refund),
                MarketplaceRefundStatusConstants.ReconciliationRequired,
                A<string>._,
                null,
                A<string?>._,
                cancellationToken))
            .Invokes((MarketplaceRefund item, string _, string? error, string? _, string? _, CancellationToken _) =>
            {
                item.Status = MarketplaceRefundStatusConstants.ReconciliationRequired;
                item.LastError = error;
            })
            .Returns(refund);

        await sut.HandleStateChangedAsync(
            new Payout
            {
                Id = "po_1",
                Status = "failed",
                FailureMessage = "bank rejected",
            },
            "payout.failed", cancellationToken);

        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.ReconciliationRequired);
        refund.LastError.ShouldNotBeNull();
        refund.LastError.ShouldContain("failed or was canceled");
        refund.PaymentRefundLastError.ShouldBe(refund.LastError);
        A.CallTo(() => refundTransitionService.TransitionAsync(
                refund,
                MarketplaceRefundStatusConstants.ReconciliationRequired,
                A<string>.That.Contains("failed or was canceled"),
                null,
                A<string?>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
