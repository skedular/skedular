import { ArrowLeftIcon } from '@/components/icons';
import { getMarketplaceBookingModificationLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { getCustomerFullName, getRelayErrorMessage, isStoredFullDayRange, RelayError, toRootError, toStoredBookingTimeRange, useIntegratedPlatform } from '@skedular/shared';
import {
  BodyIconTypography,
  CaptionIconTypography,
  DefaultDialogTitle,
  LeadIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
  SettingsSectionCard,
  SubtitleIconTypography,
  TwoButtonsDialogActions,
} from '@skedular/ui';

import useKnownParams from '@/hooks/use-known-params';
import logger from '@/libs/logging';
import { logCustomerSelfServiceActionRejected, logCustomerSelfServiceActionStarted } from '@/libs/logging/aggregate-marketplace-telemetry';
import type { marketplaceProductBookingDetails_rootQuery } from '@/queries/__generated__/marketplaceProductBookingDetails_rootQuery.graphql';
import type { marketplaceProductBookingDetails_deleteMarketplaceBookingMutation } from '@/queries/__generated__/marketplaceProductBookingDetails_deleteMarketplaceBookingMutation.graphql';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Container from '@mui/material/Container';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import TextField from '@mui/material/TextField';
import dayjs from 'dayjs';
import { useRouter } from 'next/navigation';
import { memo, ReactNode, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useSubscription } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import MarketplaceProductBookingDetailsHero from './marketplace-product-booking-details-hero';
import MarketplaceProductBookingPaymentPanel from './marketplace-product-booking-payment-panel';
import MarketplaceRefundStatusCard from './marketplace-refund-status-card';
import { getFailureCleanupMessage } from './marketplace-booking-failure-eligibility';
import { canRequestMarketplaceBookingModification } from './marketplace-self-service-eligibility';
import { RefundHistoryTimeline } from '@/components/refund/RefundHistoryTimeline';

const RootQuery = graphql`
  query marketplaceProductBookingDetails_rootQuery($bookingId: String!) {
    ...modifyMarketplaceBookingDialog_query @arguments(bookingId: $bookingId)
    booking(id: $bookingId) {
      id
      entityFrameworkVersion
      from
      until
      deletedByCustomer {
        id
      }
      cancellationAvailability {
        canCancel
        requiresReason
        isPolicyOverride
        unavailableReason
        isCreditFunded
        creditOutcome
      }
      cancellationPolicyOverridden
      cancellationOverrideReason
      involvedCustomers {
        id
        name
        givenName
        middleName
        familyName
      }
      involvedLocations {
        uniqueId
        name
      }
      bookingResources {
        resource {
          id
          name
        }
      }
      recurringBooking {
        marketplaceBooking {
          paymentStatus {
            type
          }
        }
      }
      marketplaceBooking {
        id
        quantity
        failure {
          id
          category {
            type
            name
          }
          finalizedAt
          customerAction {
            type
            name
          }
          resourceReleaseStatus {
            type
            name
          }
          accountingCleanupStatus {
            type
            name
          }
          resolutionDeadlineAt
          resolutionDecision
          allocatedRefundAmount
        }
        refund {
          currency {
            type
            name
          }
          status {
            type
            name
          }
          requestedAt
          lastProcessedAt
          refundAmount
          refundPercentage
          currencyToDisplay
          reason
          lastError
          externalRefundNumber
          requestedByCustomerName
          events {
            id
            eventType {
              type
              name
            }
            occurredAt
            refundAmount
            currencyToDisplay
            reason
            lastError
            externalRefundNumber
            actorName
            previousStatus
            newStatus
          }
        }
        invoiceUrl
        invoiceNumber
        isPaymentRequired
        paymentExpiry
        productVersion {
          type {
            type
            name
          }
          listingMetadata {
            title
            subTitle
            about
            includedFeatures
          }
          featureImages {
            original {
              url
            }
          }
        }
        bookingCheckoutSession {
          checkoutUrl
        }
        paymentMethod {
          type
          name
        }
        paymentStatus {
          type
          name
        }
      }
      marketplaceBookingModifications {
        id
        occurredAt
        actorKind
        reason
        originalFrom
        originalUntil
        resultFrom
        resultUntil
        originalResourceNames
        resultResourceNames
      }
      arrearsInvoices {
        invoiceNumber
        invoiceUrl
        billingPeriodStartInclusive
        billingPeriodEndExclusive
      }
    }
  }
`;

const BookingSubscription = graphql`
  subscription marketplaceProductBookingDetails_booking_Subscription($bookingId: String!) {
    booking(id: $bookingId) {
      entityFrameworkVersion
      from
      until
      deletedByCustomer {
        id
      }
      cancellationAvailability {
        canCancel
        requiresReason
        isPolicyOverride
        unavailableReason
        isCreditFunded
        creditOutcome
      }
      cancellationPolicyOverridden
      cancellationOverrideReason
      marketplaceBooking {
        id
        failure {
          id
          category {
            type
            name
          }
          finalizedAt
          customerAction {
            type
            name
          }
          resourceReleaseStatus {
            type
            name
          }
          accountingCleanupStatus {
            type
            name
          }
        }
        refund {
          currency {
            type
            name
          }
          status {
            type
            name
          }
          requestedAt
          lastProcessedAt
          refundAmount
          refundPercentage
          currencyToDisplay
          reason
          lastError
          externalRefundNumber
          requestedByCustomerName
          events {
            id
            eventType {
              type
              name
            }
            occurredAt
            refundAmount
            currencyToDisplay
            reason
            lastError
            externalRefundNumber
            actorName
            previousStatus
            newStatus
          }
        }
        invoiceUrl
        invoiceNumber
        isPaymentRequired
        paymentExpiry
        bookingCheckoutSession {
          checkoutUrl
        }
        paymentStatus {
          type
          name
        }
      }
      marketplaceBookingModifications {
        id
        occurredAt
        actorKind
        reason
        originalFrom
        originalUntil
        resultFrom
        resultUntil
        originalResourceNames
        resultResourceNames
      }
      arrearsInvoices {
        invoiceNumber
        invoiceUrl
        billingPeriodStartInclusive
        billingPeriodEndExclusive
      }
    }
  }
`;

const DeleteMarketplaceBookingMutation = graphql`
  mutation marketplaceProductBookingDetails_deleteMarketplaceBookingMutation($input: DeleteMarketplaceBookingInput!) {
    deleteMarketplaceBooking(input: $input) {
      cancellationError {
        code
        message
      }
      booking {
        id
        deletedByCustomer {
          id
        }
      }
    }
  }
`;
const AcceptPartialMutation = graphql`
  mutation marketplaceProductBookingDetails_acceptPartialMutation($input: ResolvePartialMarketplaceBookingInput!) {
    acceptPartialMarketplaceBooking(input: $input) {
      id
      resolutionDecision
    }
  }
`;
const DeclinePartialMutation = graphql`
  mutation marketplaceProductBookingDetails_declinePartialMutation($input: ResolvePartialMarketplaceBookingInput!) {
    declinePartialMarketplaceBooking(input: $input) {
      id
      resolutionDecision
    }
  }
`;

const getMarketplaceBookingModificationActorLabel = (actorKind: string) => (actorKind === 'ORGANIZATION_OPERATOR' ? 'Organization administrator' : 'Customer');

const MarketplaceProductBookingDetails = ({ queryReference }: { queryReference: PreloadedQuery<marketplaceProductBookingDetails_rootQuery, Record<string, unknown>> }) => {
  const rootData = usePreloadedQuery<marketplaceProductBookingDetails_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const { integratedPlatform } = useIntegratedPlatform();
  const booking = rootData.booking;
  const marketplaceBooking = booking?.marketplaceBooking;
  const [hasCancelledLocally, setHasCancelledLocally] = useState(false);
  const [pendingCancellationConfirmation, setPendingCancellationConfirmation] = useState(false);
  const [cancellationOverrideReason, setCancellationOverrideReason] = useState('');
  const [commitDeleteMarketplaceBooking, isDeleteMarketplaceBookingInFlight] =
    useMutation<marketplaceProductBookingDetails_deleteMarketplaceBookingMutation>(DeleteMarketplaceBookingMutation);
  const [commitAcceptPartial] = useMutation(AcceptPartialMutation);
  const [commitDeclinePartial] = useMutation(DeclinePartialMutation);
  const isCancelled = hasCancelledLocally || !!booking?.deletedByCustomer?.id;
  const hasConfirmedPayment = marketplaceBooking?.paymentStatus.type === 'CONFIRMED';
  const canRequestModification = canRequestMarketplaceBookingModification({
    bookingStartsAt: booking?.from,
    isCancelled,
    paymentStatusType: marketplaceBooking?.paymentStatus.type ?? booking?.recurringBooking?.marketplaceBooking?.paymentStatus.type,
    now: new Date(),
  });
  const cancellationAvailability = booking?.cancellationAvailability;
  const canRequestCancellation = !isCancelled && cancellationAvailability?.canCancel === true;
  const productTitle = marketplaceBooking?.productVersion?.listingMetadata.title ?? 'this booking';
  const handleRequestCancellationClick = () => {
    logCustomerSelfServiceActionStarted({
      logger,
      actionType: 'cancel_booking',
      purchaseId: booking?.id ?? 'unknown',
      purchaseType: 'booking',
    });
    setPendingCancellationConfirmation(true);
  };
  const handleCancelCancellationClick = () => {
    setPendingCancellationConfirmation(false);
  };
  const handleConfirmCancellationClick = () => {
    if (!booking) {
      return;
    }

    if (cancellationAvailability?.requiresReason && !cancellationOverrideReason.trim()) {
      return;
    }

    let bookingDetailsInfo = productTitle;
    if (booking.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at ${booking.involvedLocations[0]!.name}`;
    }

    commitDeleteMarketplaceBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: booking.id,
          cancellationOverrideReason: cancellationAvailability?.requiresReason ? cancellationOverrideReason.trim() : null,
        },
      },
      onCompleted: (data, errors) => {
        const cancellationError = data?.deleteMarketplaceBooking?.cancellationError;
        if (cancellationError) {
          toast(
            <NotificationContent content={`Failed to cancel ${bookingDetailsInfo}. ${toMarketplaceBookingCancellationErrorMessage(cancellationError.message)}`} />,
            errorNotificationOptions,
          );
          return;
        }
        if (errors && errors.length > 0) {
          logCustomerSelfServiceActionRejected({
            logger,
            actionType: 'cancel_booking',
            purchaseType: 'booking',
            reasonCode: getRelayErrorMessage(errors),
          });
          toast(
            <NotificationContent content={`Failed to cancel ${bookingDetailsInfo}. ${toMarketplaceBookingCancellationErrorMessage(getRelayErrorMessage(errors))}`} />,
            errorNotificationOptions,
          );

          return;
        }

        setHasCancelledLocally(true);
      },
      onError: (error) => {
        logCustomerSelfServiceActionRejected({
          logger,
          actionType: 'cancel_booking',
          purchaseType: 'booking',
          reasonCode: getRelayErrorMessage(error),
        });
        toast(
          <NotificationContent content={`Failed to cancel ${bookingDetailsInfo}. ${toMarketplaceBookingCancellationErrorMessage(getRelayErrorMessage(error))}`} />,
          errorNotificationOptions,
        );
      },
    });

    setPendingCancellationConfirmation(false);
  };

  useSubscription({
    variables: { bookingId: booking?.id ?? '' },
    subscription: BookingSubscription,
  });

  return (
    <Box
      sx={{
        minHeight: '100vh',
        pb: 8,
        background:
          'radial-gradient(circle at top left, rgba(23, 93, 175, 0.14), transparent 28%), radial-gradient(circle at top right, rgba(255, 159, 67, 0.12), transparent 22%)',
      }}
    >
      <Container maxWidth="xl" sx={{ pt: { xs: 3, md: 4 } }}>
        <Button variant="text" onClick={() => router.back()} sx={{ textTransform: 'none', px: 0, mb: 2 }}>
          <StackRow spacing={0.5} sx={{ flexWrap: 'nowrap' }}>
            <ArrowLeftIcon fontSize="small" />
            <BodyIconTypography label="Back" />
          </StackRow>
        </Button>

        {marketplaceBooking?.productVersion ? (
          <MarketplaceProductBookingDetailsHero
            about={marketplaceBooking.productVersion.listingMetadata.about}
            imageUrl={marketplaceBooking.productVersion.featureImages[0]?.original?.url}
            includedFeatures={marketplaceBooking.productVersion.listingMetadata.includedFeatures}
            subTitle={marketplaceBooking.productVersion.listingMetadata.subTitle}
            title={marketplaceBooking.productVersion.listingMetadata.title}
          />
        ) : null}

        {!booking || !marketplaceBooking ? (
          <Alert severity="info" sx={{ mt: 2, borderRadius: 3 }}>
            We couldn&apos;t find this booking anymore.
          </Alert>
        ) : (
          <Box
            sx={{
              mt: 1,
              display: 'grid',
              gap: { xs: 3, lg: 4 },
              gridTemplateColumns: { xs: '1fr', lg: 'minmax(0, 1.15fr) 380px' },
              alignItems: 'start',
            }}
          >
            <Card
              sx={{
                borderRadius: 4,
                border: 1,
                borderColor: (theme) => theme.palette.divider,
              }}
            >
              <CardContent sx={{ p: { xs: 2.5, md: 3 } }}>
                <CaptionIconTypography
                  label="Booking details"
                  sx={{
                    letterSpacing: '0.08em',
                    textTransform: 'uppercase',
                    opacity: 0.66,
                  }}
                />
                <LeadIconTypography label="Review your booking and payment status" sx={{ mt: 1 }} />
                <BodyIconTypography
                  label={
                    isCancelled
                      ? 'This booking has been cancelled. You can keep this page as your confirmation and review the original booking details here.'
                      : 'This page stays in sync while checkout is being prepared, and it remains the place to return to after payment.'
                  }
                  sx={{ mt: 1, opacity: 0.82 }}
                />

                <StackRow sx={{ mt: 2, rowGap: 1 }}>
                  <Chip
                    label={isCancelled ? 'Cancelled' : marketplaceBooking.paymentStatus.name}
                    color={isCancelled || marketplaceBooking.paymentStatus.type === 'CONFIRMED' ? 'success' : 'default'}
                  />
                  <Chip label={marketplaceBooking.paymentMethod.name} variant="outlined" />
                </StackRow>

                {isCancelled ? (
                  <Alert severity="success" sx={{ mt: 3, borderRadius: 3 }}>
                    {hasConfirmedPayment
                      ? 'This booking has been cancelled. Refund processing, if applicable, continues separately after cancellation.'
                      : 'This booking has been cancelled. No refund is expected because payment was not confirmed.'}
                  </Alert>
                ) : null}

                {booking.cancellationPolicyOverridden ? (
                  <Alert severity="info" sx={{ mt: 3, borderRadius: 3 }}>
                    Cancellation policy overridden
                    {booking.cancellationOverrideReason ? `: ${booking.cancellationOverrideReason}` : ''}
                  </Alert>
                ) : null}

                {marketplaceBooking.failure ? (
                  <Alert severity="warning" sx={{ mt: 3, borderRadius: 3 }}>
                    <SubtitleIconTypography label={marketplaceBooking.failure.category.name} />
                    <BodyIconTypography label={getMarketplaceBookingFailureMessage(marketplaceBooking.failure.category.type)} sx={{ mt: 0.5 }} />
                    <BodyIconTypography label={getFailureCleanupMessage(marketplaceBooking.failure)} sx={{ mt: 0.5 }} />
                    {marketplaceBooking.failure.customerAction.type === 'Rebook' ? (
                      <Button onClick={() => router.back()} sx={{ mt: 1, textTransform: 'none' }} variant="outlined">
                        Start a new booking
                      </Button>
                    ) : null}
                    {marketplaceBooking.failure.resolutionDeadlineAt && !marketplaceBooking.failure.resolutionDecision ? (
                      <StackRow sx={{ mt: 1 }}>
                        <Button
                          variant="contained"
                          onClick={() =>
                            commitAcceptPartial({
                              variables: {
                                input: { id: marketplaceBooking.failure!.id },
                              },
                            })
                          }
                        >
                          Keep available bookings
                        </Button>
                        <Button
                          variant="outlined"
                          onClick={() =>
                            commitDeclinePartial({
                              variables: {
                                input: { id: marketplaceBooking.failure!.id },
                              },
                            })
                          }
                        >
                          Cancel all and refund
                        </Button>
                      </StackRow>
                    ) : null}
                  </Alert>
                ) : null}

                {marketplaceBooking.refund ? (
                  <>
                    <MarketplaceRefundStatusCard
                      entityLabel="booking"
                      hasInvoice={Boolean(marketplaceBooking.invoiceUrl) || (booking.arrearsInvoices?.length ?? 0) > 0}
                      isCancelled={isCancelled}
                      isPaymentRequired={marketplaceBooking.isPaymentRequired}
                      paymentStatusType={marketplaceBooking.paymentStatus.type}
                      refund={marketplaceBooking.refund}
                    />
                    {marketplaceBooking.refund.events.length > 0 ? (
                      <Card
                        sx={{
                          mt: 3,
                          borderRadius: 3,
                          border: 1,
                          borderColor: (theme) => theme.palette.divider,
                          boxShadow: 'none',
                        }}
                      >
                        <CardContent sx={{ p: 2.5 }}>
                          <SubtitleIconTypography label="Refund history" />
                          <RefundHistoryTimeline
                            events={marketplaceBooking.refund.events.map((event) => ({
                              id: event.id,
                              eventType: event.eventType.name,
                              occurredAt: event.occurredAt,
                              actorName: event.actorName,
                              previousStatus: event.previousStatus,
                              newStatus: event.newStatus,
                            }))}
                          />
                        </CardContent>
                      </Card>
                    ) : null}
                  </>
                ) : null}

                {canRequestModification ? (
                  <Card
                    sx={{
                      mt: 3,
                      borderRadius: 3,
                      border: 1,
                      borderColor: (theme) => theme.palette.divider,
                      boxShadow: 'none',
                    }}
                  >
                    <CardContent sx={{ p: 2.5 }}>
                      <CaptionIconTypography
                        label="Booking actions"
                        sx={{
                          letterSpacing: '0.08em',
                          textTransform: 'uppercase',
                          opacity: 0.68,
                        }}
                      />
                      <SubtitleIconTypography label="Change date and time" sx={{ mt: 1 }} />
                      <BodyIconTypography
                        label="Check a new date and time for this confirmed booking. Your product, price, and payment will not change."
                        sx={{ mt: 1, opacity: 0.82 }}
                      />
                      <StackRow sx={{ mt: 2 }}>
                        <Button variant="outlined" onClick={() => router.push(getMarketplaceBookingModificationLink(integratedPlatform, true, '', booking.id))}>
                          Change booking
                        </Button>
                      </StackRow>
                    </CardContent>
                  </Card>
                ) : null}

                {canRequestCancellation ? (
                  <Card
                    sx={{
                      mt: 3,
                      borderRadius: 3,
                      border: 1,
                      borderColor: (theme) => theme.palette.divider,
                      boxShadow: 'none',
                    }}
                  >
                    <CardContent sx={{ p: 2.5 }}>
                      <CaptionIconTypography
                        label="Booking actions"
                        sx={{
                          letterSpacing: '0.08em',
                          textTransform: 'uppercase',
                          opacity: 0.68,
                        }}
                      />
                      <SubtitleIconTypography label="Cancel booking" sx={{ mt: 1 }} />
                      <BodyIconTypography
                        label={
                          cancellationAvailability?.isPolicyOverride
                            ? 'You have permission to override this product’s cancellation policy. Please provide a reason before cancelling.'
                            : cancellationAvailability?.isCreditFunded && cancellationAvailability.creditOutcome
                              ? cancellationAvailability.creditOutcome
                              : hasConfirmedPayment
                                ? 'You can cancel this booking here. If payment has already been recorded, any eligible refund is reviewed separately after cancellation.'
                                : 'You can cancel this booking here. If payment was never confirmed, cancellation stops the booking without creating a refund.'
                        }
                        sx={{ mt: 1, opacity: 0.82 }}
                      />
                      <StackRow sx={{ mt: 2 }}>
                        <Button color="error" variant="outlined" onClick={handleRequestCancellationClick} disabled={isDeleteMarketplaceBookingInFlight}>
                          Cancel booking
                        </Button>
                      </StackRow>
                    </CardContent>
                  </Card>
                ) : null}

                {!isCancelled && cancellationAvailability && !cancellationAvailability.canCancel ? (
                  <Alert severity="info" sx={{ mt: 3, borderRadius: 3 }}>
                    {cancellationAvailability.unavailableReason}
                  </Alert>
                ) : null}
              </CardContent>
            </Card>

            <StackColumn spacing={2} sx={{ gridColumn: { xs: '1', lg: '2' }, gridRow: { xs: 'auto', lg: 1 } }}>
              {!isCancelled ? (
                <MarketplaceProductBookingPaymentPanel
                  checkoutUrl={marketplaceBooking.bookingCheckoutSession?.checkoutUrl ?? null}
                  entityLabel="booking"
                  invoices={booking.arrearsInvoices ?? []}
                  invoiceUrl={marketplaceBooking.invoiceUrl ?? null}
                  isPaymentRequired={marketplaceBooking.isPaymentRequired}
                  paymentExpiry={marketplaceBooking.paymentExpiry}
                  paymentMethodType={marketplaceBooking.paymentMethod.type}
                  paymentStatusLabel={marketplaceBooking.paymentStatus.name}
                  paymentStatusType={marketplaceBooking.paymentStatus.type}
                />
              ) : null}
              <SettingsSectionCard title="Booking Summary" description="Your current booking details.">
                <StackColumn spacing={2}>
                  <DetailsRow label="Booking date" value={toStoredBookingDate(booking.from)} />
                  {!isStoredFullDayRange(booking.from, booking.until) ? <DetailsRow label="Booking time" value={toStoredBookingTimeRange(booking.from, booking.until)} /> : null}
                  {marketplaceBooking.productVersion?.type.type !== 'EVENT' ? <DetailsRow label="Quantity" value={`${marketplaceBooking.quantity}`} /> : null}
                  <DetailsRow label="Booked for" value={booking.involvedCustomers.map((item) => getCustomerFullName(item)).join(', ') || 'Not available'} />
                  <DetailsRow label="Location" value={booking.involvedLocations.map((location) => location.name).join(', ') || 'Not available'} />
                  <DetailsRow label="Resources" value={booking.bookingResources.map((item) => item.resource.name).join(', ') || 'Assigned later'} />
                </StackColumn>
              </SettingsSectionCard>
            </StackColumn>

            {booking.marketplaceBookingModifications.length > 0 ? (
              <SettingsSectionCard title="Change History" description="Recorded schedule and resource changes for this booking." sx={{ mt: 0, gridColumn: { xs: '1', lg: '1' } }}>
                {booking.marketplaceBookingModifications.map((modification) => (
                  <StackColumn key={modification.id} spacing={0.25} sx={{ mb: 1.5 }}>
                    <BodyIconTypography label={`${toStoredBookingDate(modification.originalFrom)} → ${toStoredBookingDate(modification.resultFrom)}`} />
                    <SmallIconTypography
                      label={`${dayjs(modification.occurredAt).format('D MMM YYYY, h:mm A')} · ${getMarketplaceBookingModificationActorLabel(modification.actorKind)}`}
                      sx={{ opacity: 0.72 }}
                    />
                    <SmallIconTypography label={`Reason: ${modification.reason?.trim() || 'Not provided'}`} sx={{ opacity: 0.82 }} />
                    <SmallIconTypography
                      label={`Resources: ${modification.originalResourceNames.join(', ') || 'None'} → ${modification.resultResourceNames.join(', ') || 'None'}`}
                      sx={{ opacity: 0.82 }}
                    />
                  </StackColumn>
                ))}
              </SettingsSectionCard>
            ) : null}
          </Box>
        )}
      </Container>

      <Dialog open={pendingCancellationConfirmation} onClose={handleCancelCancellationClick}>
        <DefaultDialogTitle title="Cancel Booking" />
        <DialogContent sx={{ mt: 2 }}>
          <DialogContentText>{`Cancel ${productTitle} now? If this booking is still within its cancellation window, it will be cancelled immediately.`}</DialogContentText>
          <DialogContentText sx={{ mt: 1.5 }}>
            {cancellationAvailability?.isCreditFunded && cancellationAvailability.creditOutcome
              ? cancellationAvailability.creditOutcome
              : hasConfirmedPayment
                ? 'If payment has already been recorded, any refund still depends on the cancellation policy and may be processed after the cancellation is confirmed.'
                : 'If payment was never confirmed, this cancellation will not create a refund.'}
          </DialogContentText>
          {cancellationAvailability?.requiresReason ? (
            <TextField
              autoFocus
              fullWidth
              required
              label="Cancellation reason"
              value={cancellationOverrideReason}
              onChange={(event) => setCancellationOverrideReason(event.target.value)}
              helperText="This reason is recorded because you are overriding the published cancellation policy."
              multiline
              minRows={3}
              sx={{ mt: 2 }}
            />
          ) : null}
          <TwoButtonsDialogActions
            primaryDisabled={isDeleteMarketplaceBookingInFlight || (cancellationAvailability?.requiresReason === true && !cancellationOverrideReason.trim())}
            onPrimaryClicked={handleConfirmCancellationClick}
            onSecondaryClicked={handleCancelCancellationClick}
            primaryLabel="Cancel booking"
            secondaryLabel="Keep booking"
          />
        </DialogContent>
      </Dialog>
    </Box>
  );
};

const DetailsRow = ({ label, value }: { label: string; value: ReactNode }) => (
  <StackColumn spacing={0.35}>
    <SmallIconTypography
      label={label}
      sx={{
        opacity: 0.62,
        textTransform: 'uppercase',
        letterSpacing: '0.06em',
      }}
    />
    {typeof value === 'string' ? <SubtitleIconTypography label={value} /> : value}
  </StackColumn>
);

const toStoredBookingDate = (date?: string | null) => {
  // Marketplace scheduler timestamps are stored as timezone-free wall-clock values in UTC.
  return date ? dayjs.utc(date).format('dddd, Do MMM YYYY') : '';
};

const toMarketplaceBookingCancellationErrorMessage = (message: string) => {
  const normalizedMessage = message.toLowerCase();
  if (normalizedMessage.includes('marketplacebookingcancellationnotallowed') || (normalizedMessage.includes('cancellation') && normalizedMessage.includes('not allowed'))) {
    return 'This booking can no longer be cancelled because it is outside the allowed cancellation window.';
  }

  return message;
};

const getMarketplaceBookingFailureMessage = (category: string) => {
  switch (category) {
    case 'AvailabilityConflict':
      return 'The requested time is no longer available. Please start a new booking to check the latest availability.';
    case 'PaymentExpired':
      return 'Your payment window expired, so the reserved capacity was released. Please start a new booking to check availability again.';
    default:
      return 'We could not complete your payment, so the reserved capacity was released. Please start a new booking to check availability again.';
  }
};

const MemoMarketplaceProductBookingDetails = memo(MarketplaceProductBookingDetails);

const MarketplaceProductBookingDetailsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<marketplaceProductBookingDetails_rootQuery>(RootQuery);
  const { bookingId } = useKnownParams();

  if (!bookingId) {
    throw new Error('bookingId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        bookingId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [bookingId, loadQuery]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoMarketplaceProductBookingDetails queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(MarketplaceProductBookingDetailsWithRelay);
