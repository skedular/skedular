import { CustomerAvatar } from '@/components/avatars';
import { getOrganizationBookingModificationLink } from '@/components/links';
import InvoiceDownloadLinks from '@/components/booking/invoice-download-links';
import RecurringBookingDeleteConfirmationDialog from '@/components/booking/recurring-booking-delete-confirmation-dialog';
import MarketplaceRefundAdminPanel from '@/components/marketplaceRefund/marketplace-refund-admin-panel';
import { getResourceAssignmentPendingMessage } from '@/components/booking/editMarketplaceBooking/resource-assignment-status';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import type { editMarketplaceBooking_booking_query$key } from '@/queries/__generated__/editMarketplaceBooking_booking_query.graphql';
import type { editMarketplaceBooking_booking_refetchableFragment } from '@/queries/__generated__/editMarketplaceBooking_booking_refetchableFragment.graphql';
import type { editMarketplaceBooking_confirmBookingPaymentMutation } from '@/queries/__generated__/editMarketplaceBooking_confirmBookingPaymentMutation.graphql';
import type { editMarketplaceBooking_deleteMarketplaceBookingMutation } from '@/queries/__generated__/editMarketplaceBooking_deleteMarketplaceBookingMutation.graphql';
import type { editMarketplaceBooking_deleteMarketplaceBookingSubscriptionMutation } from '@/queries/__generated__/editMarketplaceBooking_deleteMarketplaceBookingSubscriptionMutation.graphql';
import type { editMarketplaceBooking_makeBookingPaymentNotRequiredMutation } from '@/queries/__generated__/editMarketplaceBooking_makeBookingPaymentNotRequiredMutation.graphql';
import type { editMarketplaceBooking_modifyMarketplaceBookingMutation } from '@/queries/__generated__/editMarketplaceBooking_modifyMarketplaceBookingMutation.graphql';
import type { editMarketplaceBooking_query$key } from '@/queries/__generated__/editMarketplaceBooking_query.graphql';
import type { editMarketplaceBooking_rejectBookingPaymentMutation } from '@/queries/__generated__/editMarketplaceBooking_rejectBookingPaymentMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import TextField from '@mui/material/TextField';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import {
  getCustomerFullName,
  getOpeningHoursFromDateTime,
  getRelayErrorMessage,
  isMidnight,
  PaletteModeContext,
  toOpeningHoursFromTime,
  toShortDate,
  toShortTime,
} from '@skedular/shared';
import {
  BodyIconTypography,
  DefaultDialogTitle,
  defaultPadding,
  MarketplaceBookingModificationForm,
  type MarketplaceBookingModificationFormValues,
  PageHeaderPanel,
  SettingsSectionCard,
  SmallIconTypography,
  StackColumn,
  StackRow,
  SubtitleIconTypography,
  TwoButtonsDialogActions,
} from '@skedular/ui';
import { Dayjs } from 'dayjs';
import { useRouter } from 'next/navigation';
import { memo, useCallback, useContext, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  rootDataRelay: editMarketplaceBooking_query$key;
  rootDataBookingRelay: editMarketplaceBooking_booking_query$key;
  onReloadRequired?: () => void;
  page?: boolean;
  organizationCustomDomain?: string;
};

const canShowMarketplacePaymentActions = (paymentStatusType: string | undefined, isPaymentRequired: boolean | undefined) => !!isPaymentRequired && paymentStatusType === 'PENDING';

const EditMarketplaceBooking = ({ rootDataRelay, rootDataBookingRelay, onReloadRequired, page = false, organizationCustomDomain }: Props) => {
  const rootData = useFragment<editMarketplaceBooking_query$key>(
    graphql`
      fragment editMarketplaceBooking_query on Query {
        organizationBookingPermissions(organizationCustomDomain: $organizationCustomDomain) {
          canModifyPaymentMethod
        }
        marketplaceBookingSubscriptions(first: 100, where: { organizationCustomDomain: $organizationCustomDomain }) {
          edges {
            node {
              id
              recurringBookings {
                id
              }
            }
          }
        }
        paymentStatuses {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const [rootDataBooking] = useRefetchableFragment<editMarketplaceBooking_booking_refetchableFragment, editMarketplaceBooking_booking_query$key>(
    graphql`
      fragment editMarketplaceBooking_booking_query on Query @refetchable(queryName: "editMarketplaceBooking_booking_refetchableFragment") {
        booking(id: $bookingId) {
          id
          entityFrameworkVersion
          cancellationOverrideReason
          from
          until
          notes
          hasRecurringInstanceOverrides
          category {
            category
          }
          involvedCustomers {
            id
            name
            givenName
            middleName
            familyName
            photoUrl
          }
          involvedOrganizations {
            id
            name
          }
          involvedLocations {
            name
          }
          involvedTeams {
            id
            name
          }
          bookingResources {
            resource {
              id
              name
              color
              customTags {
                id
                name
                color
              }
              zones {
                id
                name
                color
              }
            }
          }
          marketplaceBooking {
            id
            entitlementId
            isPaymentRequired
            paymentStatus {
              type
              name
            }
            invoiceUrl
            refund {
              id
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
              canProcessInXero
              xeroProcessingBlockedReason
            }
          }
          marketplaceBookingModifications {
            id
            occurredAt
            actorKind
            reason
            originalFrom
            resultFrom
            originalResourceNames
            resultResourceNames
          }
          recurringBooking {
            id
            startDate
            endDate
            frequency {
              name
            }
            marketplaceBooking {
              id
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
    `,
    rootDataBookingRelay,
  );

  const [commitDeleteMarketplaceBooking] = useMutation<editMarketplaceBooking_deleteMarketplaceBookingMutation>(graphql`
    mutation editMarketplaceBooking_deleteMarketplaceBookingMutation($input: DeleteMarketplaceBookingInput!) {
      deleteMarketplaceBooking(input: $input) {
        booking {
          id
        }
      }
    }
  `);

  const [commitDeleteMarketplaceBookingSubscription] = useMutation<editMarketplaceBooking_deleteMarketplaceBookingSubscriptionMutation>(graphql`
    mutation editMarketplaceBooking_deleteMarketplaceBookingSubscriptionMutation($input: DeleteMarketplaceBookingSubscriptionInput!) {
      deleteMarketplaceBookingSubscription(input: $input) {
        marketplaceBookingSubscription {
          id
          cancelAtPeriodEnd
          nextRenewalAt
          status {
            type
            name
          }
        }
      }
    }
  `);

  const [commitConfirmBookingPayment] = useMutation<editMarketplaceBooking_confirmBookingPaymentMutation>(graphql`
    mutation editMarketplaceBooking_confirmBookingPaymentMutation($input: ConfirmBookingPaymentInput!) @raw_response_type {
      confirmBookingPayment(input: $input) {
        booking {
          id
          marketplaceBooking {
            id
            paymentStatus {
              type
              name
            }
          }
        }
      }
    }
  `);

  const [commitRejectBookingPayment] = useMutation<editMarketplaceBooking_rejectBookingPaymentMutation>(graphql`
    mutation editMarketplaceBooking_rejectBookingPaymentMutation($input: RejectBookingPaymentInput!) @raw_response_type {
      rejectBookingPayment(input: $input) {
        booking {
          id
          marketplaceBooking {
            id
            paymentStatus {
              type
              name
            }
          }
        }
      }
    }
  `);

  const [commitMakeBookingPaymentNotRequired] = useMutation<editMarketplaceBooking_makeBookingPaymentNotRequiredMutation>(graphql`
    mutation editMarketplaceBooking_makeBookingPaymentNotRequiredMutation($input: MakeBookingPaymentNotRequiredInput!) @raw_response_type {
      makeBookingPaymentNotRequired(input: $input) {
        booking {
          id
          marketplaceBooking {
            id
            paymentStatus {
              type
              name
            }
          }
        }
      }
    }
  `);

  const [commitModifyMarketplaceBooking, isModifyMarketplaceBookingInFlight] = useMutation<editMarketplaceBooking_modifyMarketplaceBookingMutation>(graphql`
    mutation editMarketplaceBooking_modifyMarketplaceBookingMutation($input: ModifyMarketplaceBookingInput!) @raw_response_type {
      modifyMarketplaceBooking(input: $input) {
        booking {
          id
        }
        modification {
          id
        }
        eligibilityError {
          message
        }
        availabilityError {
          message
        }
        conflictError {
          message
        }
        accessError {
          message
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [pendingRecurringSeriesCancellation, setPendingRecurringSeriesCancellation] = useState(false);
  const [pendingCancellationReason, setPendingCancellationReason] = useState<'booking' | 'series' | null>(null);
  const [cancellationReason, setCancellationReason] = useState('');
  const [isModifyDialogOpen, setIsModifyDialogOpen] = useState(page);
  const [modificationFrom, setModificationFrom] = useState('');
  const [modificationUntil, setModificationUntil] = useState('');
  const [modificationReason, setModificationReason] = useState('');
  const allDay = useMemo<boolean>(
    () => isMidnight(rootDataBooking.booking?.from) && isMidnight(rootDataBooking.booking?.until),
    [rootDataBooking.booking?.from, rootDataBooking.booking?.until],
  );
  const timeRange = useMemo<DateRange<Dayjs>>(
    () => [toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootDataBooking.booking?.from)), toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootDataBooking.booking?.until))],
    [rootDataBooking.booking?.from, rootDataBooking.booking?.until],
  );
  const getBookingDetailsInfo = useCallback(() => {
    const booking = rootDataBooking.booking;
    if (!booking) {
      return 'this booking';
    }

    let bookingDetailsInfo = `for ${getCustomerFullName(booking.involvedCustomers[0])}`;
    if (booking.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${booking.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${toShortDate(booking.from)}`;
    return bookingDetailsInfo;
  }, [rootDataBooking.booking]);

  const handleOpenModifyDialog = () => {
    const booking = rootDataBooking.booking;
    if (!booking) {
      return;
    }

    if (organizationCustomDomain) {
      router.push(getOrganizationBookingModificationLink(undefined, organizationCustomDomain, booking.id));
      return;
    }

    setModificationFrom(booking.from.slice(0, 16));
    setModificationUntil(booking.until.slice(0, 16));
    setModificationReason('');
    setIsModifyDialogOpen(true);
  };

  const handleModifyMarketplaceBookingValues = (values?: MarketplaceBookingModificationFormValues) => {
    const booking = rootDataBooking.booking;
    const reason = (values?.reason ?? modificationReason.trim()) || null;
    const proposedFrom = values?.from ?? modificationFrom;
    const proposedUntil = values?.until ?? modificationUntil;
    if (!booking || !proposedFrom || !proposedUntil) {
      return;
    }

    const from = new Date(proposedFrom);
    const until = new Date(proposedUntil);
    if (Number.isNaN(from.valueOf()) || Number.isNaN(until.valueOf()) || until <= from) {
      themedToast(<NotificationContent content="Enter a valid end date and time after the start." />, errorNotificationOptions);
      return;
    }

    commitModifyMarketplaceBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          bookingId: booking.id,
          entitlementId: booking.marketplaceBooking?.entitlementId,
          expectedVersion: booking.entityFrameworkVersion,
          from: from.toISOString(),
          until: until.toISOString(),
          reason: reason || '',
          actorKind: 'ORGANIZATION_OPERATOR',
        },
      },
      onCompleted: (data, errors) => {
        const result = data?.modifyMarketplaceBooking;
        const modificationError = result?.eligibilityError ?? result?.availabilityError ?? result?.conflictError ?? result?.accessError;
        if (modificationError) {
          themedToast(<NotificationContent content={modificationError.message} />, errorNotificationOptions);
          return;
        }
        if (errors?.length) {
          themedToast(<NotificationContent content={`We couldn't update booking ${getBookingDetailsInfo()}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          return;
        }

        if (page) {
          router.back();
          return;
        }
        setIsModifyDialogOpen(false);
        themedToast(<NotificationContent content="Booking updated. The customer has been notified." />);
        onReloadRequired?.();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't update booking ${getBookingDetailsInfo()}. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions);
      },
    });
  };

  const handleRemoveBookingClick = () => {
    const booking = rootDataBooking.booking;
    if (!booking) {
      return;
    }

    setCancellationReason('');
    setPendingCancellationReason('booking');
  };

  const cancelBooking = (cancellationOverrideReason: string) => {
    const booking = rootDataBooking.booking;
    if (!booking) return;
    const bookingDetailsInfo = getBookingDetailsInfo();
    commitDeleteMarketplaceBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: booking.id,
          cancellationOverrideReason,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't remove booking ${bookingDetailsInfo}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        router.back();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove booking ${bookingDetailsInfo}. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions);
      },
    });
  };

  const handleRemoveRecurringSeriesClick = () => {
    setPendingRecurringSeriesCancellation(true);
  };

  const handleCancelRecurringSeriesCancellationClick = () => {
    setPendingRecurringSeriesCancellation(false);
  };

  const handleConfirmRecurringSeriesCancellationClick = (providedReason?: string) => {
    if (!providedReason) {
      setPendingRecurringSeriesCancellation(false);
      setCancellationReason('');
      setPendingCancellationReason('series');
      return;
    }
    const recurringBooking = rootDataBooking.booking?.recurringBooking;
    if (!recurringBooking) {
      return;
    }

    const subscription = rootData.marketplaceBookingSubscriptions.edges
      .map(({ node }) => node)
      .find((subscription) => subscription.recurringBookings.some((item) => item.id === recurringBooking.id));

    if (!subscription) {
      themedToast(<NotificationContent content="We couldn't find the recurring series for this booking." />, errorNotificationOptions);
      setPendingRecurringSeriesCancellation(false);

      return;
    }

    const booking = rootDataBooking.booking;
    if (!booking) {
      return;
    }
    const cancellationOverrideReason = providedReason;

    commitDeleteMarketplaceBookingSubscription({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: subscription.id,
          cancellationMode: 'IMMEDIATE',
          cancellationOverrideReason,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't cancel this recurring series. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        onReloadRequired?.();
        router.back();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't cancel this recurring series. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions);
      },
    });
    setPendingRecurringSeriesCancellation(false);
  };

  const confirmCancellationReason = () => {
    const reason = cancellationReason.trim();
    if (!reason || !pendingCancellationReason) return;
    const action = pendingCancellationReason;
    setPendingCancellationReason(null);
    setCancellationReason('');
    if (action === 'booking') cancelBooking(reason);
    else handleConfirmRecurringSeriesCancellationClick(reason);
  };

  const handleConfirmPaymentClick = () => {
    const booking = rootDataBooking.booking;
    if (!booking) {
      return;
    }

    const bookingDetailsInfo = getBookingDetailsInfo();
    commitConfirmBookingPayment({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: booking.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`Failed to confirm payment for booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(errors)}.`} />,
            errorNotificationOptions,
          );

          return;
        }

        onReloadRequired?.();
      },
      onError: (error) => {
        themedToast(
          <NotificationContent content={`Failed to confirm payment for booking '${toShortDate(booking.from)}'. Error: ${getRelayErrorMessage(error)}.`} />,
          errorNotificationOptions,
        );
      },
      optimisticResponse: {
        confirmBookingPayment: {
          booking: {
            id: booking.id,
            marketplaceBooking: {
              id: booking.marketplaceBooking?.id ?? uuid(),
              paymentStatus: {
                type: 'CONFIRMED',
                name: rootData.paymentStatuses.find((status) => status.type === 'CONFIRMED')!.name,
              },
            },
          },
        },
      },
    });
  };

  const handleRejectPaymentClick = () => {
    const booking = rootDataBooking.booking;
    if (!booking) {
      return;
    }

    const bookingDetailsInfo = getBookingDetailsInfo();
    commitRejectBookingPayment({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: booking.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`Failed to reject payment for booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(errors)}.`} />,
            errorNotificationOptions,
          );

          return;
        }

        onReloadRequired?.();
      },
      onError: (error) => {
        themedToast(
          <NotificationContent content={`Failed to reject payment for booking '${toShortDate(booking.from)}'. Error: ${getRelayErrorMessage(error)}.`} />,
          errorNotificationOptions,
        );
      },
      optimisticResponse: {
        rejectBookingPayment: {
          booking: {
            id: booking.id,
            marketplaceBooking: {
              id: booking.marketplaceBooking?.id ?? uuid(),
              paymentStatus: {
                type: 'REJECTED',
                name: rootData.paymentStatuses.find((status) => status.type === 'REJECTED')!.name,
              },
            },
          },
        },
      },
    });
  };

  const handleMakePaymentNotRequiredClick = () => {
    const booking = rootDataBooking.booking;
    if (!booking) {
      return;
    }

    const bookingDetailsInfo = getBookingDetailsInfo();
    commitMakeBookingPaymentNotRequired({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: booking.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`Failed to make payment for booking ${bookingDetailsInfo} not required. Error: ${getRelayErrorMessage(errors)}.`} />,
            errorNotificationOptions,
          );

          return;
        }

        onReloadRequired?.();
      },
      onError: (error) => {
        themedToast(
          <NotificationContent content={`Failed to make payment for booking '${toShortDate(booking.from)}' not required. Error: ${getRelayErrorMessage(error)}.`} />,
          errorNotificationOptions,
        );
      },
      optimisticResponse: {
        makeBookingPaymentNotRequired: {
          booking: {
            id: booking.id,
            marketplaceBooking: {
              id: booking.marketplaceBooking?.id ?? uuid(),
              paymentStatus: {
                type: 'NO_PAYMENT_REQUIRED',
                name: rootData.paymentStatuses.find((status) => status.type === 'NO_PAYMENT_REQUIRED')!.name,
              },
            },
          },
        },
      },
    });
  };

  if (!rootDataBooking.booking) {
    return null;
  }

  const booking = rootDataBooking.booking;
  if (page) {
    return (
      <MarketplaceBookingModificationForm
        initialFrom={booking.from}
        initialUntil={booking.until}
        currentResourceIds={[]}
        currentResources={[]}
        currentLocationId={null}
        locations={[]}
        resources={[]}
        canSelectResources={false}
        maximumResourceCount={0}
        isSubmitting={isModifyMarketplaceBookingInFlight}
        onSubmit={handleModifyMarketplaceBookingValues}
        onCancel={() => router.back()}
      />
    );
  }
  const pageTitle = booking.involvedCustomers.length > 0 ? `Edit Booking - ${getCustomerFullName(booking.involvedCustomers[0])}` : 'Edit Booking';
  const recurringBooking = booking.recurringBooking;
  const recurringSeriesLabel = recurringBooking ? `${recurringBooking.frequency.name} recurring booking` : null;
  const recurringSeriesDateLabel = recurringBooking
    ? recurringBooking.endDate
      ? `${toShortDate(recurringBooking.startDate)} - ${toShortDate(recurringBooking.endDate)}`
      : `Starts ${toShortDate(recurringBooking.startDate)}`
    : null;
  const canManagePayment =
    rootData.organizationBookingPermissions.canModifyPaymentMethod &&
    canShowMarketplacePaymentActions(booking.marketplaceBooking?.paymentStatus.type, booking.marketplaceBooking?.isPaymentRequired);
  const canManageRefund = rootData.organizationBookingPermissions.canModifyPaymentMethod && !!booking.marketplaceBooking?.refund;
  const canModifyBooking =
    !!booking.marketplaceBooking &&
    (booking.marketplaceBooking.paymentStatus.type === 'CONFIRMED' || booking.marketplaceBooking.paymentStatus.type === 'NO_PAYMENT_REQUIRED') &&
    new Date(booking.from) > new Date();
  const paymentStatusColor =
    booking.marketplaceBooking?.paymentStatus.type === 'CONFIRMED' ? 'success' : booking.marketplaceBooking?.paymentStatus.type === 'PENDING' ? 'warning' : 'default';
  const primaryCustomer = booking.involvedCustomers[0];
  const isResourceAssignmentPending = !!recurringBooking && booking.bookingResources.length === 0;

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pt: { xs: 2, md: 3 }, pb: defaultPadding }}>
      <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', backgroundColor: 'transparent', gap: 2 }}>
        <PageHeaderPanel
          eyebrow="Marketplace Booking"
          title={pageTitle}
          description="Update booking details and manage marketplace payment, invoice, cancellation, and refund actions from one place."
          actions={recurringSeriesLabel ? <Chip label="Recurring" variant="outlined" size="small" /> : undefined}
        />

        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', lg: 'minmax(0, 1fr) 360px' }, gap: 2, alignItems: 'start' }}>
          <StackColumn spacing={2} sx={{ minWidth: 0 }}>
            <SettingsSectionCard title="Customer Details" description="The customer on a marketplace booking is kept read-only from this admin view.">
              {primaryCustomer ? (
                <StackRow spacing={1.5} sx={{ alignItems: 'center', flexWrap: 'nowrap', minWidth: 0 }}>
                  <CustomerAvatar name={primaryCustomer} photo={{ url: primaryCustomer.photoUrl }} size="medium" />
                  <StackColumn spacing={0.25} sx={{ minWidth: 0 }}>
                    <SubtitleIconTypography label={getCustomerFullName(primaryCustomer)} sx={{ overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }} />
                    <SmallIconTypography label="Marketplace customer" sx={{ opacity: 0.72 }} />
                  </StackColumn>
                </StackRow>
              ) : (
                <BodyIconTypography label="No customer is assigned to this booking." />
              )}
            </SettingsSectionCard>

            <SettingsSectionCard title="Payment Actions" description="Payment and cancellation controls match the marketplace booking card.">
              <StackColumn spacing={1.25}>
                {canManagePayment ? (
                  <StackColumn spacing={1}>
                    <SubtitleIconTypography label="Payment" />
                    <StackRow spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                      <Button variant="contained" onClick={handleConfirmPaymentClick}>
                        Confirm Payment
                      </Button>
                      <Button variant="outlined" onClick={handleRejectPaymentClick}>
                        Reject Payment
                      </Button>
                      <Button variant="outlined" onClick={handleMakePaymentNotRequiredClick}>
                        Make Payment Not Required
                      </Button>
                    </StackRow>
                  </StackColumn>
                ) : (
                  <BodyIconTypography label="No payment action is currently available for this booking." sx={{ opacity: 0.78 }} />
                )}

                <StackColumn spacing={1}>
                  <SubtitleIconTypography label="Cancellation" />
                  {booking.cancellationOverrideReason ? <BodyIconTypography label={`Cancellation reason: ${booking.cancellationOverrideReason}`} sx={{ opacity: 0.78 }} /> : null}
                  {booking.cancellationOverrideReason ? null : recurringBooking?.marketplaceBooking && !isResourceAssignmentPending ? (
                    <Button color="error" variant="outlined" onClick={handleRemoveRecurringSeriesClick} sx={{ alignSelf: 'flex-start' }}>
                      Cancel Series
                    </Button>
                  ) : (
                    <Button color="error" variant="outlined" onClick={handleRemoveBookingClick} sx={{ alignSelf: 'flex-start' }}>
                      Cancel Booking
                    </Button>
                  )}
                </StackColumn>
              </StackColumn>
            </SettingsSectionCard>

            <SettingsSectionCard title="Modify Booking" description="Change this booking's date or time. Your reason is recorded and sent to the customer.">
              {canModifyBooking ? (
                <Button variant="contained" onClick={handleOpenModifyDialog} sx={{ alignSelf: 'flex-start' }}>
                  Modify Booking
                </Button>
              ) : (
                <BodyIconTypography label="This booking is not eligible to be modified." sx={{ opacity: 0.78 }} />
              )}
            </SettingsSectionCard>

            <SettingsSectionCard title="Change History" description="Recorded schedule changes for this booking." sx={{ order: 99 }}>
              {booking.marketplaceBookingModifications.length > 0 ? (
                booking.marketplaceBookingModifications.map((modification) => (
                  <StackColumn key={modification.id} spacing={0.25} sx={{ mb: 1.5 }}>
                    <BodyIconTypography label={`${toShortDate(modification.originalFrom)} → ${toShortDate(modification.resultFrom)}`} />
                    <SmallIconTypography
                      label={`${toShortDate(modification.occurredAt)} · ${modification.actorKind === 'ORGANIZATION_OPERATOR' ? 'Organization settingsistrator' : 'Customer'}`}
                      sx={{ opacity: 0.72 }}
                    />
                    {modification.reason ? <SmallIconTypography label={modification.reason} sx={{ opacity: 0.82 }} /> : null}
                    <SmallIconTypography
                      label={`Resources: ${modification.originalResourceNames.join(', ') || 'None'} → ${modification.resultResourceNames.join(', ') || 'None'}`}
                      sx={{ opacity: 0.82 }}
                    />
                  </StackColumn>
                ))
              ) : (
                <BodyIconTypography label="No booking changes have been recorded." sx={{ opacity: 0.78 }} />
              )}
            </SettingsSectionCard>
          </StackColumn>

          <StackColumn spacing={2} sx={{ minWidth: 0 }}>
            <SettingsSectionCard title="Booking Summary" description="Current marketplace booking status and billing links.">
              <StackColumn spacing={1.5}>
                <StackColumn spacing={0.4}>
                  <SmallIconTypography label="Date/Time" sx={{ opacity: 0.68, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                  <StackRow>
                    <SubtitleIconTypography label={`${toShortDate(booking.from)}, `} />
                    {allDay && <SubtitleIconTypography label="All day" />}
                    {!allDay && <SubtitleIconTypography label={`${toShortTime(timeRange[0])} - ${toShortTime(timeRange[1])}`} />}
                  </StackRow>
                </StackColumn>

                {recurringSeriesLabel ? (
                  <StackColumn spacing={0.4}>
                    <SmallIconTypography label="Recurring Series" sx={{ opacity: 0.68, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                    <SubtitleIconTypography label={recurringSeriesLabel} />
                    {recurringSeriesDateLabel ? <BodyIconTypography label={recurringSeriesDateLabel} sx={{ opacity: 0.78 }} /> : null}
                  </StackColumn>
                ) : null}

                {isResourceAssignmentPending ? (
                  <StackColumn spacing={0.4}>
                    <SmallIconTypography label="Resource Assignment" sx={{ opacity: 0.68, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                    <Chip label="Awaiting resource assignment" color="warning" size="small" sx={{ alignSelf: 'flex-start' }} />
                    <BodyIconTypography label={getResourceAssignmentPendingMessage(Boolean(booking.hasRecurringInstanceOverrides))} sx={{ opacity: 0.78 }} />
                  </StackColumn>
                ) : null}

                <StackColumn spacing={0.6}>
                  <SmallIconTypography label="Place Reserved" sx={{ opacity: 0.68, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                  <BodyIconTypography label="Entire location" />
                </StackColumn>

                <StackColumn spacing={0.4}>
                  {booking.marketplaceBooking?.entitlementId ? (
                    <BodyIconTypography
                      label="Credit-funded booking: this booking uses one customer entitlement credit. Date changes keep the credit consumed; cancellation restores or forfeits it according to the product policy."
                      sx={{ opacity: 0.78 }}
                    />
                  ) : null}
                  <SmallIconTypography label="Payment Status" sx={{ opacity: 0.68, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                  <StackRow>
                    {booking.marketplaceBooking?.paymentStatus.name ? <Chip label={booking.marketplaceBooking.paymentStatus.name} color={paymentStatusColor} size="small" /> : null}
                  </StackRow>
                </StackColumn>

                <StackColumn spacing={0.4}>
                  <SmallIconTypography label="Invoice" sx={{ opacity: 0.68, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                  <InvoiceDownloadLinks invoices={booking.arrearsInvoices ?? []} legacyInvoiceUrl={booking.marketplaceBooking?.invoiceUrl ?? null} />
                </StackColumn>
              </StackColumn>
            </SettingsSectionCard>

            {canManageRefund && booking.marketplaceBooking?.refund ? (
              <SettingsSectionCard title="Refund" description="Review and progress the marketplace refund for this booking.">
                <MarketplaceRefundAdminPanel entityLabel={getBookingDetailsInfo()} refund={booking.marketplaceBooking.refund} />
              </SettingsSectionCard>
            ) : null}
          </StackColumn>
        </Box>
      </StackColumn>

      <RecurringBookingDeleteConfirmationDialog
        open={pendingRecurringSeriesCancellation}
        title="Cancel Recurring Series"
        description="This booking is part of a marketplace recurring series. If you continue, the full recurring series will be cancelled, not just this booking."
        confirmLabel="Cancel series"
        onConfirm={handleConfirmRecurringSeriesCancellationClick}
        onCancel={handleCancelRecurringSeriesCancellationClick}
      />

      <Dialog open={pendingCancellationReason !== null} onClose={() => setPendingCancellationReason(null)} fullWidth maxWidth="sm">
        <DefaultDialogTitle title={pendingCancellationReason === 'series' ? 'Cancel recurring series' : 'Cancel booking'} />
        <DialogContent sx={{ marginTop: 2 }}>
          <StackColumn spacing={1.5}>
            <BodyIconTypography label="Review the cancellation before confirming this booking action." />
            <TextField
              label="Cancellation reason"
              value={cancellationReason}
              onChange={(event) => setCancellationReason(event.target.value)}
              required
              multiline
              minRows={2}
              helperText="Required when the cancellation policy would block this cancellation."
              fullWidth
              autoFocus
            />
            <TwoButtonsDialogActions
              primaryLabel={pendingCancellationReason === 'series' ? 'Cancel series' : 'Cancel booking'}
              primaryDisabled={!cancellationReason.trim()}
              secondaryLabel="Keep booking"
              onPrimaryClicked={confirmCancellationReason}
              onSecondaryClicked={() => setPendingCancellationReason(null)}
            />
          </StackColumn>
        </DialogContent>
      </Dialog>

      {isModifyDialogOpen ? (
        <Box sx={{ width: '100%', maxWidth: 1200, mx: 'auto', px: { xs: 1, sm: 2, md: 3 }, pb: 4 }}>
          <SettingsSectionCard title="Schedule" description="Pick the date and time for this booking.">
            <StackColumn spacing={2} sx={{ pt: 1 }}>
              <TextField
                label="Start"
                type="datetime-local"
                value={modificationFrom}
                onChange={(event) => setModificationFrom(event.target.value)}
                slotProps={{ inputLabel: { shrink: true } }}
                required
              />
              <TextField
                label="End"
                type="datetime-local"
                value={modificationUntil}
                onChange={(event) => setModificationUntil(event.target.value)}
                slotProps={{ inputLabel: { shrink: true } }}
                required
              />
              <TextField
                label="Reason for change"
                value={modificationReason}
                onChange={(event) => setModificationReason(event.target.value)}
                helperText="This reason is recorded in the booking history and included in the customer notification."
                multiline
                minRows={3}
                required
              />
              <BodyIconTypography label="Host bookings reserve the whole place; individual resources cannot be changed here." sx={{ opacity: 0.78 }} />
            </StackColumn>
            <StackRow spacing={1} sx={{ justifyContent: 'flex-end' }}>
              <Button onClick={() => setIsModifyDialogOpen(false)}>Cancel</Button>
              <Button variant="contained" onClick={() => handleModifyMarketplaceBookingValues()} disabled={!modificationReason.trim() || isModifyMarketplaceBookingInFlight}>
                Save Changes
              </Button>
            </StackRow>
          </SettingsSectionCard>
        </Box>
      ) : null}
    </Box>
  );
};

export default memo(EditMarketplaceBooking);
