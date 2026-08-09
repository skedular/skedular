import { PaletteModeContext, dateRangeToShortDateWithAdditionalDayInfo, getCustomerFullName, getRelayErrorMessage, toShortDate, useIntegratedPlatform } from '@skedular/shared';
import { CustomerAvatar } from '@/components/avatars';
import RecurringBookingDeleteConfirmationDialog from '@/components/booking/recurring-booking-delete-confirmation-dialog';
import OperatorCancelBookingButton from '@/components/booking/operator-cancel-booking-button';
import { CaptionIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';
import { CustomTags } from '@/components/customTag';
import { CalendarIcon, EllipseMenuIcon, JoinIcon, NotesIcon, PaymentStatusIcon, PdfIcon } from '@/components/icons';
import { getOrganizationBookingBaseLink, getOrganizationBookingModificationLink, getOrganizationSubscriptionBaseLink } from '@/components/links';
import MarketplaceRefundAdminPanel from '@/components/marketplaceRefund/marketplace-refund-admin-panel';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { Resources } from '@/components/resource';
import { Zones } from '@/components/zone';

import { coal } from '@skedular/ui';

import type { bookingCard_addPrivateBookingMutation } from '@/queries/__generated__/bookingCard_addPrivateBookingMutation.graphql';
import type { bookingCard_BookingDetails$key } from '@/queries/__generated__/bookingCard_BookingDetails.graphql';
import type { bookingCard_confirmBookingPaymentMutation } from '@/queries/__generated__/bookingCard_confirmBookingPaymentMutation.graphql';
import type { bookingCard_deleteMarketplaceBookingMutation } from '@/queries/__generated__/bookingCard_deleteMarketplaceBookingMutation.graphql';
import type { bookingCard_deleteMarketplaceBookingSubscriptionMutation } from '@/queries/__generated__/bookingCard_deleteMarketplaceBookingSubscriptionMutation.graphql';
import type { bookingCard_deletePrivateBookingMutation } from '@/queries/__generated__/bookingCard_deletePrivateBookingMutation.graphql';
import type { bookingCard_deletePrivateRecurringBookingMutation } from '@/queries/__generated__/bookingCard_deletePrivateRecurringBookingMutation.graphql';
import type { bookingCard_makeBookingPaymentNotRequiredMutation } from '@/queries/__generated__/bookingCard_makeBookingPaymentNotRequiredMutation.graphql';
import type { bookingCard_query$key } from '@/queries/__generated__/bookingCard_query.graphql';
import type { bookingCard_rejectBookingPaymentMutation } from '@/queries/__generated__/bookingCard_rejectBookingPaymentMutation.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Tooltip from '@mui/material/Tooltip';
import type { SxProps, Theme } from '@mui/system';
import Box from '@mui/system/Box';
import dayjs from 'dayjs';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  rootDataRelay: bookingCard_query$key;
  bookingDetailsRelay: bookingCard_BookingDetails$key;
  organizationCustomDomain: string;
  connectionIds: string[];
  canJoinBooking: boolean;
  recurringMarketplaceSubscriptionIds?: Record<string, string>;
};

type CustomTagDetails = {
  id: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

type ZoneDetails = {
  id: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

type PendingDeleteAction = 'booking' | 'occurrence' | 'series' | null;

const isConfirmedPaymentStatus = (paymentStatusType: string) => paymentStatusType === 'CONFIRMED' || paymentStatusType === 'PAID';
const isPendingPaymentStatus = (paymentStatusType: string) => paymentStatusType === 'PENDING';

const sectionSx: SxProps<Theme> = {
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  borderRadius: 3,
  p: 1.25,
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.02)' : 'transparent'),
};

const BookingCard = ({ rootDataRelay, bookingDetailsRelay, organizationCustomDomain, connectionIds, canJoinBooking, recurringMarketplaceSubscriptionIds = {} }: Props) => {
  const rootData = useFragment<bookingCard_query$key>(
    graphql`
      fragment bookingCard_query on Query {
        me {
          id
          name
          givenName
          middleName
          familyName
          photoUrl
        }
        organizationBookingPermissions(organizationCustomDomain: $organizationCustomDomain) {
          canModifyPaymentMethod
        }
        paymentStatuses {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const bookingDetails = useFragment(
    graphql`
      fragment bookingCard_BookingDetails on BookingDetails {
        id
        cancellationPolicyOverridden
        cancellationOverrideReason
        from
        until
        notes
        category {
          category
          name
        }
        channel {
          channel
          name
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
        }
        involvedLocations {
          uniqueId
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
      }
    `,
    bookingDetailsRelay,
  );

  const [commitDeletePrivateBooking] = useMutation<bookingCard_deletePrivateBookingMutation>(graphql`
    mutation bookingCard_deletePrivateBookingMutation($connectionIds: [ID!]!, $input: DeletePrivateBookingInput!) {
      deletePrivateBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitDeleteMarketplaceBooking] = useMutation<bookingCard_deleteMarketplaceBookingMutation>(graphql`
    mutation bookingCard_deleteMarketplaceBookingMutation($connectionIds: [ID!]!, $input: DeleteMarketplaceBookingInput!) {
      deleteMarketplaceBooking(input: $input) {
        cancellationError {
          code
          message
        }
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitDeletePrivateRecurringBooking] = useMutation<bookingCard_deletePrivateRecurringBookingMutation>(graphql`
    mutation bookingCard_deletePrivateRecurringBookingMutation($input: DeletePrivateRecurringBookingInput!) {
      deletePrivateRecurringBooking(input: $input) {
        recurringBooking {
          id
        }
      }
    }
  `);

  const [commitDeleteMarketplaceBookingSubscription] = useMutation<bookingCard_deleteMarketplaceBookingSubscriptionMutation>(graphql`
    mutation bookingCard_deleteMarketplaceBookingSubscriptionMutation($input: DeleteMarketplaceBookingSubscriptionInput!) {
      deleteMarketplaceBookingSubscription(input: $input) {
        cancellationError {
          code
          message
        }
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

  const [commitAddPrivateBooking] = useMutation<bookingCard_addPrivateBookingMutation>(graphql`
    mutation bookingCard_addPrivateBookingMutation($connectionIds: [ID!]!, $input: AddPrivateBookingInput!) @raw_response_type {
      addPrivateBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
          id
          from
          until
          notes
          channel {
            channel
          }
          category {
            category
            name
          }
          involvedCustomers {
            id
            name
            givenName
            middleName
            familyName
            photoUrl
          }
          involvedLocations {
            uniqueId
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
        }
      }
    }
  `);

  const [commitConfirmBookingPayment] = useMutation<bookingCard_confirmBookingPaymentMutation>(graphql`
    mutation bookingCard_confirmBookingPaymentMutation($input: ConfirmBookingPaymentInput!) @raw_response_type {
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

  const [commitRejectBookingPayment] = useMutation<bookingCard_rejectBookingPaymentMutation>(graphql`
    mutation bookingCard_rejectBookingPaymentMutation($input: RejectBookingPaymentInput!) @raw_response_type {
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

  const [commitMakeBookingPaymentNotRequired] = useMutation<bookingCard_makeBookingPaymentNotRequiredMutation>(graphql`
    mutation bookingCard_makeBookingPaymentNotRequiredMutation($input: MakeBookingPaymentNotRequiredInput!) @raw_response_type {
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

  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const [pendingDeleteAction, setPendingDeleteAction] = useState<PendingDeleteAction>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const shortDateFormatFrom = toShortDate(bookingDetails.from);
  const refund = bookingDetails.marketplaceBooking?.refund;
  const primaryCustomer = bookingDetails.involvedCustomers[0];
  const refundEntityLabel = `booking ${primaryCustomer ? getCustomerFullName(primaryCustomer) : 'customer'} on ${shortDateFormatFrom}`;
  const canManageRefund = rootData.organizationBookingPermissions.canModifyPaymentMethod && !!refund;
  const recurringBooking = bookingDetails.recurringBooking;
  const isMarketplaceRecurringBooking = !!recurringBooking?.marketplaceBooking;
  const canDeleteRecurringOccurrence = !!recurringBooking && !isMarketplaceRecurringBooking && bookingDetails.channel.channel === 'PRIVATE';
  const canEditRecurringSeries = canDeleteRecurringOccurrence;
  const recurringSeriesLabel = recurringBooking ? `${recurringBooking.frequency.name} recurring booking` : null;
  const recurringSeriesDateLabel = recurringBooking
    ? recurringBooking.endDate
      ? `${toShortDate(recurringBooking.startDate)} - ${toShortDate(recurringBooking.endDate)}`
      : `Starts ${toShortDate(recurringBooking.startDate)}`
    : null;
  const bookingDateRange = dateRangeToShortDateWithAdditionalDayInfo(dayjs(bookingDetails.from), dayjs(bookingDetails.until));
  const recurringSeriesActionLabel = recurringBooking ? 'Remove recurring series' : null;
  const recurringOccurrenceActionLabel = canDeleteRecurringOccurrence ? 'Remove this occurrence' : null;
  const recurringDeleteConfirmationMessage = recurringBooking
    ? `This booking is part of a recurring series. ${isMarketplaceRecurringBooking ? 'If you continue, the full recurring series will be cancelled' : 'If you continue, the full recurring series will be removed'}, not just this booking.`
    : null;
  const recurringOccurrenceDeleteConfirmationMessage = canDeleteRecurringOccurrence
    ? 'Only this booking will be removed. The rest of the recurring series will stay active.'
    : null;
  const bookingDeleteConfirmationMessage =
    bookingDetails.channel.channel === 'MARKETPLACE' ? 'This booking will be canceled and removed from the calendar.' : 'This booking will be removed from the calendar.';
  const deleteDialogTitle = pendingDeleteAction === 'series' ? (isMarketplaceRecurringBooking ? 'Cancel Recurring Series' : 'Remove Recurring Series') : 'Remove This Booking';
  const deleteDialogDescription =
    pendingDeleteAction === 'occurrence'
      ? recurringOccurrenceDeleteConfirmationMessage
      : pendingDeleteAction === 'series'
        ? recurringDeleteConfirmationMessage
        : bookingDeleteConfirmationMessage;
  const deleteDialogPrimaryLabel =
    pendingDeleteAction === 'series'
      ? isMarketplaceRecurringBooking
        ? 'Cancel series'
        : 'Remove series'
      : bookingDetails.channel.channel === 'MARKETPLACE'
        ? 'Cancel booking'
        : 'Remove booking';
  const visibleViewDetailsLabel = canEditRecurringSeries ? 'View occurrence details' : 'View details';
  const visibleRemoveBookingLabel =
    recurringOccurrenceActionLabel ?? (!recurringBooking ? (bookingDetails.channel.channel === 'MARKETPLACE' ? 'Cancel booking' : 'Remove booking') : null);
  const canModifyMarketplaceBooking =
    rootData.organizationBookingPermissions.canModifyPaymentMethod &&
    bookingDetails.channel.channel === 'MARKETPLACE' &&
    !!bookingDetails.marketplaceBooking &&
    (bookingDetails.marketplaceBooking.paymentStatus.type === 'CONFIRMED' || bookingDetails.marketplaceBooking.paymentStatus.type === 'NO_PAYMENT_REQUIRED') &&
    new Date(bookingDetails.from) > new Date();

  const moreActionsOption: MoreActionsMenuItemType[] = [];

  if (canEditRecurringSeries) {
    moreActionsOption.push(moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditRecurringBooking]);
  }

  if (canModifyMarketplaceBooking) {
    moreActionsOption.push({
      ...moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditBooking],
      label: 'Modify booking',
    });
  }

  if (recurringBooking && recurringSeriesActionLabel) {
    moreActionsOption.push({
      ...moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteRecurringBooking],
      label: recurringSeriesActionLabel,
    });
  }

  if (
    rootData.organizationBookingPermissions.canModifyPaymentMethod &&
    bookingDetails.marketplaceBooking &&
    bookingDetails.marketplaceBooking.isPaymentRequired &&
    bookingDetails.marketplaceBooking.paymentStatus.type === 'PENDING'
  ) {
    moreActionsOption.push(
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.ConfirmBookingPayment],
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.RejectBookingPayment],
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.MakeBookingPaymentNotRequired],
    );
  }

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };

  const handleViewBookingDetailsClick = () => {
    router.push(getOrganizationBookingBaseLink(integratedPlatform, organizationCustomDomain, bookingDetails.id));
  };

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditBooking:
        router.push(getOrganizationBookingModificationLink(integratedPlatform, organizationCustomDomain, bookingDetails.id));
        break;

      case MoreActionsMenuOptionType.EditRecurringBooking:
        router.push(getOrganizationBookingBaseLink(integratedPlatform, organizationCustomDomain, bookingDetails.id, { editMode: 'recurring' }));
        break;

      case MoreActionsMenuOptionType.DeleteBooking:
        handleRemoveBookingClick();
        break;

      case MoreActionsMenuOptionType.DeleteRecurringBooking:
        handleRemoveRecurringBookingClick();
        break;

      case MoreActionsMenuOptionType.ConfirmBookingPayment:
        handleConfirmPaymentClick();
        break;

      case MoreActionsMenuOptionType.RejectBookingPayment:
        handleRejectPaymentClick();
        break;

      case MoreActionsMenuOptionType.MakeBookingPaymentNotRequired:
        handleMakePaymentNotRequiredClick();
        break;
    }
  };

  const handleRemoveBookingClick = () => {
    if (recurringOccurrenceDeleteConfirmationMessage) {
      setPendingDeleteAction('occurrence');
      return;
    }

    setPendingDeleteAction('booking');
  };

  const handleCancelRecurringDeleteClick = () => {
    setPendingDeleteAction(null);
  };

  const handleConfirmRecurringDeleteClick = () => {
    const action = pendingDeleteAction;
    setPendingDeleteAction(null);

    if (action === 'booking' || action === 'occurrence') {
      removeBooking();
      return;
    }

    if (action === 'series') {
      removeRecurringBooking();
    }
  };

  const removeBooking = (cancellationOverrideReason?: string) => {
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    if (bookingDetails.channel.channel === 'PRIVATE') {
      commitDeletePrivateBooking({
        variables: { connectionIds, input: { clientMutationId: uuid(), id: bookingDetails.id } },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            themedToast(<NotificationContent content={`We couldn't remove booking ${bookingDetailsInfo}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

            return;
          }
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`We couldn't remove booking ${bookingDetailsInfo}. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions);
        },
      });
    } else {
      cancellationOverrideReason ??= window.prompt('Cancellation reason')?.trim() ?? undefined;
      if (!cancellationOverrideReason) {
        return;
      }
      commitDeleteMarketplaceBooking({
        variables: { connectionIds, input: { clientMutationId: uuid(), id: bookingDetails.id, cancellationOverrideReason } },
        onCompleted: (data, errors) => {
          const cancellationError = data.deleteMarketplaceBooking.cancellationError;
          if (cancellationError) {
            themedToast(<NotificationContent content={`We couldn't remove booking ${bookingDetailsInfo}. ${cancellationError.message}`} />, errorNotificationOptions);
            return;
          }
          if (errors && errors.length > 0) {
            themedToast(<NotificationContent content={`We couldn't remove booking ${bookingDetailsInfo}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

            return;
          }
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`We couldn't remove booking ${bookingDetailsInfo}. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions);
        },
      });
    }
  };

  const handleRemoveRecurringBookingClick = () => {
    if (!recurringBooking || !recurringSeriesLabel || !recurringDeleteConfirmationMessage) {
      return;
    }

    setPendingDeleteAction('series');
  };

  const removeRecurringBooking = () => {
    if (!recurringBooking || !recurringSeriesLabel) {
      return;
    }

    if (isMarketplaceRecurringBooking) {
      const subscriptionId = recurringMarketplaceSubscriptionIds[recurringBooking.id];

      if (!subscriptionId) {
        themedToast(<NotificationContent content="We couldn't find the recurring series for this booking." />, errorNotificationOptions);

        return;
      }

      const cancellationOverrideReason = window.prompt('Cancellation reason')?.trim();
      if (!cancellationOverrideReason) {
        return;
      }

      commitDeleteMarketplaceBookingSubscription({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: subscriptionId,
            cancellationMode: 'IMMEDIATE',
            cancellationOverrideReason,
          },
        },
        onCompleted: (data, errors) => {
          const cancellationError = data.deleteMarketplaceBookingSubscription.cancellationError;
          if (cancellationError) {
            themedToast(<NotificationContent content={`We couldn't cancel this recurring series. ${cancellationError.message}`} />, errorNotificationOptions);
            return;
          }
          if (errors && errors.length > 0) {
            themedToast(<NotificationContent content={`We couldn't cancel this recurring series. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

            return;
          }

          router.refresh();
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`We couldn't cancel this recurring series. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions);
        },
      });

      return;
    }

    commitDeletePrivateRecurringBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: recurringBooking.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't remove this recurring series. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        router.refresh();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove this recurring series. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions);
      },
    });
  };

  const handleJoinClick = () => {
    const id = uuid();

    commitAddPrivateBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          id,
          from: bookingDetails.from,
          until: bookingDetails.until,
          customerIds: [rootData.me.id],
          organizationIds: bookingDetails.involvedOrganizations.map(({ id }) => id),
          teamIds: [],
          resourceIds: [],
          category: bookingDetails.category.category,
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to make a booking '${shortDateFormatFrom}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to make a booking '${shortDateFormatFrom}'. Error: ${getRelayErrorMessage(error)}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        addPrivateBooking: {
          booking: {
            id,
            from: bookingDetails.from,
            until: bookingDetails.until,
            notes: null,
            channel: { channel: 'PRIVATE' },
            category: {
              category: bookingDetails.category.category,
              name: bookingDetails.category.name,
            },
            involvedCustomers: [
              {
                id: rootData.me.id,
                name: rootData.me.name,
                givenName: rootData.me.givenName,
                middleName: rootData.me.middleName,
                familyName: rootData.me.familyName,
                photoUrl: rootData.me.photoUrl,
              },
            ],
            involvedLocations: [],
            involvedTeams: [],
            bookingResources: [],
          },
        },
      },
    });
  };

  const handleConfirmPaymentClick = () => {
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }
    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    commitConfirmBookingPayment({
      variables: { input: { clientMutationId: uuid(), id: bookingDetails.id } },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`Failed to confirm payment for booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(errors)}.`} />,
            errorNotificationOptions,
          );
          return;
        }
      },
      onError: (error) => {
        themedToast(
          <NotificationContent content={`Failed to confirm payment for booking '${shortDateFormatFrom}'. Error: ${getRelayErrorMessage(error)}.`} />,
          errorNotificationOptions,
        );
      },
      optimisticResponse: {
        confirmBookingPayment: {
          booking: {
            id: bookingDetails.id,
            marketplaceBooking: {
              id: bookingDetails.marketplaceBooking?.id ?? uuid(),
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
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }
    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    commitRejectBookingPayment({
      variables: { input: { clientMutationId: uuid(), id: bookingDetails.id } },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`Failed to reject payment for booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(errors)}.`} />,
            errorNotificationOptions,
          );
          return;
        }
      },
      onError: (error) => {
        themedToast(
          <NotificationContent content={`Failed to reject payment for booking '${shortDateFormatFrom}'. Error: ${getRelayErrorMessage(error)}.`} />,
          errorNotificationOptions,
        );
      },
      optimisticResponse: {
        rejectBookingPayment: {
          booking: {
            id: bookingDetails.id,
            marketplaceBooking: {
              id: bookingDetails.marketplaceBooking?.id ?? uuid(),
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
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }
    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    commitMakeBookingPaymentNotRequired({
      variables: { input: { clientMutationId: uuid(), id: bookingDetails.id } },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`Failed to make payment for booking ${bookingDetailsInfo} not required. Error: ${getRelayErrorMessage(errors)}.`} />,
            errorNotificationOptions,
          );
          return;
        }
      },
      onError: (error) => {
        themedToast(
          <NotificationContent content={`Failed to make payment for booking '${shortDateFormatFrom}' not required. Error: ${getRelayErrorMessage(error)}.`} />,
          errorNotificationOptions,
        );
      },
      optimisticResponse: {
        makeBookingPaymentNotRequired: {
          booking: {
            id: bookingDetails.id,
            marketplaceBooking: {
              id: bookingDetails.marketplaceBooking?.id ?? uuid(),
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

  const customTags = bookingDetails.bookingResources
    .flatMap(({ resource }) => resource.customTags)
    .reduce((acc: CustomTagDetails[], customTag) => {
      if (!acc.some((item) => item.id === customTag.id)) {
        acc.push(customTag);
      }

      return acc;
    }, []);
  const zones = bookingDetails.bookingResources
    .flatMap(({ resource }) => resource.zones)
    .reduce((acc: ZoneDetails[], zone) => {
      if (!acc.some((item) => item.id === zone.id)) {
        acc.push(zone);
      }

      return acc;
    }, []);

  const locationName =
    bookingDetails.involvedLocations
      .map((location) => location.name)
      .filter(Boolean)
      .join(', ') || 'Location pending';
  return (
    <>
      <Card
        sx={{
          width: '100%',
          height: '100%',
          borderRadius: 4,
          border: 1,
          borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
          boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 10px 28px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
          backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.92)' : theme.palette.background.paper),
        }}
      >
        <CardContent sx={{ p: 2, height: '100%' }}>
          <StackColumn spacing={1.25} sx={{ height: '100%' }}>
            <StackRow sx={{ alignItems: 'flex-start', flexWrap: 'nowrap', gap: 1 }}>
              <StackColumn spacing={0.25} sx={{ minWidth: 0, flexGrow: 1 }}>
                <Tooltip title={locationName}>
                  <Link
                    component={NextLink}
                    href={getOrganizationBookingBaseLink(integratedPlatform, organizationCustomDomain, bookingDetails.id)}
                    underline="none"
                    color="inherit"
                    sx={{ display: 'block', minWidth: 0, maxWidth: '100%' }}
                  >
                    <LeadIconTypography label={locationName} noWrap sx={{ minWidth: 0, maxWidth: '100%' }} />
                  </Link>
                </Tooltip>
                <StackColumn spacing={0.1} sx={{ minWidth: 0 }}>
                  <SmallIconTypography startElement={<CalendarIcon />} label={bookingDateRange.primaryLine} noWrap />
                  {bookingDateRange.secondaryLine ? <SmallIconTypography label={bookingDateRange.secondaryLine} noWrap sx={{ pl: 3.5 }} /> : null}
                </StackColumn>
              </StackColumn>

              {canJoinBooking ? (
                <IconButton onClick={handleJoinClick} aria-label="Join booking" sx={{ color: paletteMode === 'dark' ? 'inherit' : coal, mt: -0.25 }}>
                  <JoinIcon />
                </IconButton>
              ) : null}

              {moreActionsOption.length > 0 ? (
                <IconButton onClick={handleMoreActionsMenuClick} aria-label="Open booking actions" sx={{ color: paletteMode === 'dark' ? 'inherit' : coal, mt: -0.25, mr: -0.5 }}>
                  <EllipseMenuIcon />
                </IconButton>
              ) : null}
            </StackRow>

            <StackRow sx={{ gap: 1, flexWrap: 'wrap', alignItems: 'center' }}>
              {recurringSeriesLabel ? (
                <Tooltip title={`${recurringSeriesLabel}. ${recurringSeriesDateLabel ?? ''}`.trim()}>
                  <Chip label="Recurring" size="small" variant="outlined" />
                </Tooltip>
              ) : null}
              {recurringBooking && recurringMarketplaceSubscriptionIds[recurringBooking.id] ? (
                <Link
                  component={NextLink}
                  href={getOrganizationSubscriptionBaseLink(integratedPlatform, organizationCustomDomain, recurringMarketplaceSubscriptionIds[recurringBooking.id])}
                  underline="hover"
                >
                  Subscription details
                </Link>
              ) : null}
              {bookingDetails.marketplaceBooking?.isPaymentRequired ? (
                <Chip
                  label={bookingDetails.marketplaceBooking.paymentStatus.name}
                  size="small"
                  icon={<PaymentStatusIcon />}
                  color={
                    isConfirmedPaymentStatus(bookingDetails.marketplaceBooking.paymentStatus.type)
                      ? 'success'
                      : isPendingPaymentStatus(bookingDetails.marketplaceBooking.paymentStatus.type)
                        ? 'warning'
                        : 'default'
                  }
                  variant={
                    isConfirmedPaymentStatus(bookingDetails.marketplaceBooking.paymentStatus.type) || isPendingPaymentStatus(bookingDetails.marketplaceBooking.paymentStatus.type)
                      ? 'filled'
                      : 'outlined'
                  }
                />
              ) : null}
              {bookingDetails.marketplaceBooking?.invoiceUrl ? (
                <Link component={NextLink} href={bookingDetails.marketplaceBooking.invoiceUrl} target="_blank" rel="noopener noreferrer" underline="none">
                  <Chip label="View Invoice" size="small" icon={<PdfIcon />} clickable />
                </Link>
              ) : null}
            </StackRow>

            <StackRow sx={{ alignItems: 'center', gap: 1, minWidth: 0 }}>
              <AvatarGroup max={5}>
                {bookingDetails.involvedCustomers.map((item) => (
                  <CustomerAvatar key={item.id} name={item} photo={{ url: item.photoUrl }} size="medium" showFullName />
                ))}
              </AvatarGroup>
              <Tooltip title={bookingDetails.involvedCustomers.map((item) => getCustomerFullName(item)).join(', ')}>
                <Box sx={{ minWidth: 0, flexGrow: 1 }}>
                  <SmallIconTypography label={bookingDetails.involvedCustomers.map((item) => getCustomerFullName(item)).join(', ')} noWrap sx={{ minWidth: 0 }} />
                </Box>
              </Tooltip>
            </StackRow>

            <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
              <Button variant="contained" size="small" onClick={handleViewBookingDetailsClick} sx={{ textTransform: 'none' }}>
                {visibleViewDetailsLabel}
              </Button>
              {visibleRemoveBookingLabel && bookingDetails.channel.channel === 'MARKETPLACE' && !recurringBooking ? (
                <OperatorCancelBookingButton bookingId={bookingDetails.id} label={visibleRemoveBookingLabel} onConfirm={removeBooking} />
              ) : visibleRemoveBookingLabel ? (
                <Button variant="outlined" color="error" size="small" onClick={handleRemoveBookingClick} sx={{ textTransform: 'none' }}>
                  {visibleRemoveBookingLabel}
                </Button>
              ) : null}
            </StackRow>

            {canManageRefund && refund ? (
              <Box sx={sectionSx}>
                <MarketplaceRefundAdminPanel entityLabel={refundEntityLabel} refund={refund} />
              </Box>
            ) : null}

            <Divider />

            <Box sx={sectionSx}>
              <StackColumn spacing={1}>
                {bookingDetails.cancellationPolicyOverridden ? (
                  <CaptionIconTypography
                    label={`Cancellation policy overridden${bookingDetails.cancellationOverrideReason ? `: ${bookingDetails.cancellationOverrideReason}` : ''}`}
                  />
                ) : null}
                <SubtitleIconTypography label="Booking details" />
                <Resources resources={bookingDetails.bookingResources.map((item) => ({ id: item.resource.id, name: item.resource.name, color: item.resource.color }))} hideNAText />
                <CustomTags customTags={customTags.map((customTag) => ({ id: customTag.id, name: customTag.name, color: customTag.color }))} hideNAText />
                <Zones zones={zones.map((zone) => ({ id: zone.id, name: zone.name, color: zone.color }))} hideNAText />
                {bookingDetails.notes ? <CaptionIconTypography startElement={<NotesIcon />} label={bookingDetails.notes} /> : null}
              </StackColumn>
            </Box>

            <Box sx={{ flexGrow: 1 }} />
          </StackColumn>
        </CardContent>
      </Card>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />

      {pendingDeleteAction && deleteDialogDescription ? (
        <RecurringBookingDeleteConfirmationDialog
          open={pendingDeleteAction !== null}
          title={deleteDialogTitle}
          description={deleteDialogDescription}
          confirmLabel={deleteDialogPrimaryLabel}
          onConfirm={handleConfirmRecurringDeleteClick}
          onCancel={handleCancelRecurringDeleteClick}
        />
      ) : null}
    </>
  );
};

export default memo(BookingCard);
