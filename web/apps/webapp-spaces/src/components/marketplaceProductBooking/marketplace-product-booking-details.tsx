import {
  BodyIconTypography,
  CaptionIconTypography,
  DefaultDialogTitle,
  LeadIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
  SubtitleIconTypography,
  TwoButtonsDialogActions,
} from '@skedular/ui';
import { ArrowLeftIcon } from '@/components/icons';
import { getMarketplaceLocationLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import { useIntegratedPlatrform, useKnownParams } from '@skedular/shared';
import { getCustomerFullName, getRelayErrorMessage, isStoredFullDayRange, toStoredBookingTimeRange } from '@skedular/shared';
import type { marketplaceProductBookingDetails_rootQuery } from '@/queries/__generated__/marketplaceProductBookingDetails_rootQuery.graphql';
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
import Link from '@mui/material/Link';
import dayjs from 'dayjs';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, ReactNode, useEffect, useMemo, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useSubscription } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import MarketplaceProductBookingDetailsHero from './marketplace-product-booking-details-hero';
import MarketplaceProductBookingPaymentPanel from './marketplace-product-booking-payment-panel';
import MarketplaceRefundStatusCard from './marketplace-refund-status-card';

const RootQuery = graphql`
  query marketplaceProductBookingDetails_rootQuery($bookingId: String!) {
    booking(id: $bookingId) {
      id
      from
      until
      deletedByCustomer {
        id
      }
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
      marketplaceBooking {
        id
        quantity
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
      deletedByCustomer {
        id
      }
      marketplaceBooking {
        id
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
      booking {
        id
        deletedByCustomer {
          id
        }
      }
    }
  }
`;

const MarketplaceProductBookingDetails = ({ queryReference }: { queryReference: PreloadedQuery<marketplaceProductBookingDetails_rootQuery, Record<string, unknown>> }) => {
  const rootData = usePreloadedQuery<marketplaceProductBookingDetails_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const booking = rootData.booking;
  const marketplaceBooking = booking?.marketplaceBooking;
  const [hasCancelledLocally, setHasCancelledLocally] = useState(false);
  const [pendingCancellationConfirmation, setPendingCancellationConfirmation] = useState(false);
  const [commitDeleteMarketplaceBooking, isDeleteMarketplaceBookingInFlight] = useMutation(DeleteMarketplaceBookingMutation);
  const isCancelled = hasCancelledLocally || !!booking?.deletedByCustomer?.id;
  const hasConfirmedPayment = marketplaceBooking?.paymentStatus.type === 'CONFIRMED';
  const canRequestCancellation = useMemo(() => {
    if (!booking || !marketplaceBooking || isCancelled) {
      return false;
    }

    return dayjs.utc(booking.from).isAfter(dayjs.utc());
  }, [booking, isCancelled, marketplaceBooking]);
  const productTitle = marketplaceBooking?.productVersion?.listingMetadata.title ?? 'this booking';
  const handleRequestCancellationClick = () => {
    setPendingCancellationConfirmation(true);
  };
  const handleCancelCancellationClick = () => {
    setPendingCancellationConfirmation(false);
  };
  const handleConfirmCancellationClick = () => {
    if (!booking) {
      return;
    }

    let bookingDetailsInfo = productTitle;
    if (booking.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at ${booking.involvedLocations[0]!.name}`;
    }

    const toastId = toast(<NotificationContent content={`Cancelling ${bookingDetailsInfo}...`} />, infoNotificationOptions);

    commitDeleteMarketplaceBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: booking.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to cancel ${bookingDetailsInfo}. ${toMarketplaceBookingCancellationErrorMessage(getRelayErrorMessage(errors))}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: (
            <NotificationContent
              content={
                hasConfirmedPayment
                  ? `${bookingDetailsInfo} cancelled. Any eligible refund will be reviewed separately.`
                  : `${bookingDetailsInfo} cancelled. No refund is expected because payment was not confirmed.`
              }
            />
          ),
        });
        setHasCancelledLocally(true);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to cancel ${bookingDetailsInfo}. ${toMarketplaceBookingCancellationErrorMessage(getRelayErrorMessage(error))}`} />,
        });
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
            <Card sx={{ borderRadius: 4, border: 1, borderColor: (theme) => theme.palette.divider }}>
              <CardContent sx={{ p: { xs: 2.5, md: 3 } }}>
                <CaptionIconTypography label="Booking details" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
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

                {marketplaceBooking.refund ? (
                  <MarketplaceRefundStatusCard
                    entityLabel="booking"
                    hasInvoice={Boolean(marketplaceBooking.invoiceUrl) || (booking.arrearsInvoices?.length ?? 0) > 0}
                    isCancelled={isCancelled}
                    isPaymentRequired={marketplaceBooking.isPaymentRequired}
                    paymentStatusType={marketplaceBooking.paymentStatus.type}
                    refund={marketplaceBooking.refund}
                  />
                ) : null}

                {canRequestCancellation ? (
                  <Card sx={{ mt: 3, borderRadius: 3, border: 1, borderColor: (theme) => theme.palette.divider, boxShadow: 'none' }}>
                    <CardContent sx={{ p: 2.5 }}>
                      <CaptionIconTypography label="Booking actions" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.68 }} />
                      <SubtitleIconTypography label="Cancel booking" sx={{ mt: 1 }} />
                      <BodyIconTypography
                        label={
                          hasConfirmedPayment
                            ? 'If this booking is still within its cancellation window, you can cancel it here. If payment has already been recorded, any eligible refund is reviewed separately after the cancellation is accepted.'
                            : 'If this booking is still within its cancellation window, you can cancel it here. If payment was never confirmed, cancellation stops the booking without creating a refund.'
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

                <StackColumn spacing={2} sx={{ mt: 3 }}>
                  <DetailsRow label="Booking date" value={toStoredBookingDate(booking.from)} />
                  {!isStoredFullDayRange(booking.from, booking.until) ? <DetailsRow label="Booking time" value={toStoredBookingTimeRange(booking.from, booking.until)} /> : null}
                  {marketplaceBooking.productVersion?.type.type !== 'EVENT' ? <DetailsRow label="Quantity" value={`${marketplaceBooking.quantity}`} /> : null}
                  <DetailsRow label="Booked for" value={booking.involvedCustomers.map((item) => getCustomerFullName(item)).join(', ') || 'Not available'} />
                  <DetailsRow
                    label="Location"
                    value={
                      booking.involvedLocations.length > 0 ? (
                        <StackRow sx={{ rowGap: 1 }}>
                          {booking.involvedLocations.map((location) => (
                            <Link
                              key={location.uniqueId}
                              component={NextLink}
                              href={getMarketplaceLocationLink(integratedPlatrform, location.uniqueId)}
                              underline="none"
                              color="inherit"
                              sx={{
                                display: 'inline-flex',
                                width: 'fit-content',
                                px: 1.25,
                                py: 0.75,
                                borderRadius: 2,
                                bgcolor: (theme) => theme.palette.action.hover,
                                border: 1,
                                borderColor: (theme) => theme.palette.divider,
                                transition: 'background-color 120ms ease, border-color 120ms ease, transform 120ms ease',
                                '&:hover': {
                                  bgcolor: (theme) => theme.palette.action.selected,
                                  borderColor: (theme) => theme.palette.primary.main,
                                  transform: 'translateY(-1px)',
                                },
                              }}
                            >
                              <SubtitleIconTypography label={location.name} />
                            </Link>
                          ))}
                        </StackRow>
                      ) : (
                        'Not available'
                      )
                    }
                  />
                  <DetailsRow label="Resources" value={booking.bookingResources.map((item) => item.resource.name).join(', ') || 'Assigned later'} />
                </StackColumn>
              </CardContent>
            </Card>

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
          </Box>
        )}
      </Container>

      <Dialog open={pendingCancellationConfirmation} onClose={handleCancelCancellationClick}>
        <DefaultDialogTitle title="Cancel Booking" />
        <DialogContent sx={{ mt: 2 }}>
          <DialogContentText>{`Cancel ${productTitle} now? If this booking is still within its cancellation window, it will be cancelled immediately.`}</DialogContentText>
          <DialogContentText sx={{ mt: 1.5 }}>
            {hasConfirmedPayment
              ? 'If payment has already been recorded, any refund still depends on the cancellation policy and may be processed after the cancellation is confirmed.'
              : 'If payment was never confirmed, this cancellation will not create a refund.'}
          </DialogContentText>
          <TwoButtonsDialogActions
            primaryDisabled={isDeleteMarketplaceBookingInFlight}
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
    <SmallIconTypography label={label} sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
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
