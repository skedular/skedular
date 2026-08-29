import { CustomerAvatar } from '@/components/avatars';
import InvoiceDownloadLinks from '@/components/booking/invoice-download-links';
import { getOrganizationProductsBaseLink } from '@/components/links';
import { PurchaseDetailPage, type PurchaseDetailAction } from '@/components/purchaseDetail/purchase-detail-page';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import type { payMarketplaceBooking_booking_query$key } from '@/queries/__generated__/payMarketplaceBooking_booking_query.graphql';
import type { payMarketplaceBooking_booking_Subscription } from '@/queries/__generated__/payMarketplaceBooking_booking_Subscription.graphql';
import type { payMarketplaceBooking_confirmBookingPaymentMutation } from '@/queries/__generated__/payMarketplaceBooking_confirmBookingPaymentMutation.graphql';
import type { payMarketplaceBooking_deleteMarketplaceBookingMutation } from '@/queries/__generated__/payMarketplaceBooking_deleteMarketplaceBookingMutation.graphql';
import type { payMarketplaceBooking_makeBookingPaymentNotRequiredMutation } from '@/queries/__generated__/payMarketplaceBooking_makeBookingPaymentNotRequiredMutation.graphql';
import type { payMarketplaceBooking_rejectBookingPaymentMutation } from '@/queries/__generated__/payMarketplaceBooking_rejectBookingPaymentMutation.graphql';
import { getCustomerFullName, getRelayErrorMessage, PaletteModeContext, toShortDate, useIntegratedPlatform } from '@skedular/shared';
import { useRouter } from 'next/navigation';
import { memo, useContext, useMemo } from 'react';
import { graphql, useFragment, useMutation, useSubscription } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  rootDataRelay: payMarketplaceBooking_booking_query$key;
  onReloadRequired?: () => void;
  organizationCustomDomain: string;
};

const PayMarketplaceBooking = ({ rootDataRelay, organizationCustomDomain }: Props) => {
  const rootData = useFragment<payMarketplaceBooking_booking_query$key>(
    graphql`
      fragment payMarketplaceBooking_booking_query on Query {
        booking(id: $bookingId) {
          id
          from
          until
          notes
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
            totalAmountExcludeTaxToDisplay
            taxAmountToDisplay
            totalAmountToDisplay
            paymentMethod {
              type
            }
            bookingCheckoutSession {
              checkoutUrl
            }
            paymentExpiry
            invoiceUrl
            quantity
            productPricing {
              listingMetadata {
                title
              }
              price
            }
            isPaymentRequired
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

  useSubscription<payMarketplaceBooking_booking_Subscription>(
    useMemo(
      () => ({
        variables: { bookingId: rootData.booking?.id ?? '' },
        subscription: graphql`
          subscription payMarketplaceBooking_booking_Subscription($bookingId: String!) {
            booking(id: $bookingId) {
              marketplaceBooking {
                paymentExpiry
                invoiceUrl
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
        `,
      }),
      [rootData.booking],
    ),
  );

  const [commitDeleteMarketplaceBooking] = useMutation<payMarketplaceBooking_deleteMarketplaceBookingMutation>(graphql`
    mutation payMarketplaceBooking_deleteMarketplaceBookingMutation($input: DeleteMarketplaceBookingInput!) {
      deleteMarketplaceBooking(input: $input) {
        cancellationError {
          code
          message
        }
        booking {
          id
        }
      }
    }
  `);

  const [commitConfirmBookingPayment] = useMutation<payMarketplaceBooking_confirmBookingPaymentMutation>(graphql`
    mutation payMarketplaceBooking_confirmBookingPaymentMutation($input: ConfirmBookingPaymentInput!) @raw_response_type {
      confirmBookingPayment(input: $input) {
        booking {
          id
          marketplaceBooking {
            paymentStatus {
              type
              name
            }
          }
        }
      }
    }
  `);

  const [commitRejectBookingPayment] = useMutation<payMarketplaceBooking_rejectBookingPaymentMutation>(graphql`
    mutation payMarketplaceBooking_rejectBookingPaymentMutation($input: RejectBookingPaymentInput!) @raw_response_type {
      rejectBookingPayment(input: $input) {
        booking {
          id
          marketplaceBooking {
            paymentStatus {
              type
              name
            }
          }
        }
      }
    }
  `);

  const [commitMakeBookingPaymentNotRequired] = useMutation<payMarketplaceBooking_makeBookingPaymentNotRequiredMutation>(graphql`
    mutation payMarketplaceBooking_makeBookingPaymentNotRequiredMutation($input: MakeBookingPaymentNotRequiredInput!) @raw_response_type {
      makeBookingPaymentNotRequired(input: $input) {
        booking {
          id
          marketplaceBooking {
            paymentStatus {
              type
              name
            }
          }
        }
      }
    }
  `);

  const shortDateFormatFrom = toShortDate(rootData.booking?.from);
  const { integratedPlatform } = useIntegratedPlatform();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const router = useRouter();

  const handleCancelBookingClick = () => {
    const bookingDetails = rootData.booking;
    if (!bookingDetails) {
      return;
    }

    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    commitDeleteMarketplaceBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: bookingDetails.id,
        },
      },
      onCompleted: (data, errors) => {
        const cancellationError = data?.deleteMarketplaceBooking?.cancellationError;
        if (cancellationError) {
          themedToast(<NotificationContent content={cancellationError.message} />, errorNotificationOptions);
          return;
        }
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to cancel booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);

          return;
        }

        router.push(getOrganizationProductsBaseLink(integratedPlatform, organizationCustomDomain));
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to cancel booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(error)}.`} />, errorNotificationOptions);
      },
    });
  };

  const handleConfirmPaymentClick = () => {
    const bookingDetails = rootData.booking;
    if (!bookingDetails) {
      return;
    }

    let bookingDetailsInfo = `for ${getCustomerFullName(booking.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    commitConfirmBookingPayment({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: bookingDetails.id,
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
              id: uuid(),
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
    const bookingDetails = rootData.booking;
    if (!bookingDetails) {
      return;
    }

    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    commitRejectBookingPayment({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: bookingDetails.id,
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
              id: uuid(),
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
    const bookingDetails = rootData.booking;
    if (!bookingDetails) {
      return;
    }

    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    commitMakeBookingPaymentNotRequired({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: bookingDetails.id,
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
              id: uuid(),

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

  if (!rootData.booking) {
    return null;
  }

  const booking = rootData.booking;

  const payment = <InvoiceDownloadLinks invoices={booking.arrearsInvoices ?? []} legacyInvoiceUrl={booking.marketplaceBooking?.invoiceUrl ?? null} size="body" />;
  const actions: PurchaseDetailAction[] = [{ label: 'Cancel booking', tone: 'destructive', onClick: handleCancelBookingClick }];
  if (
    rootData.organizationBookingPermissions.canModifyPaymentMethod &&
    booking.marketplaceBooking?.isPaymentRequired &&
    booking.marketplaceBooking.paymentStatus.type === 'PENDING'
  )
    actions.push(
      { label: 'Confirm payment', onClick: handleConfirmPaymentClick },
      { label: 'Reject payment', tone: 'destructive', onClick: handleRejectPaymentClick },
      { label: 'Payment not required', onClick: handleMakePaymentNotRequiredClick },
    );
  return (
    <PurchaseDetailPage
      title="Purchase details"
      purchaseType="One-time booking"
      customer={booking.involvedCustomers.map(getCustomerFullName).join(', ') || 'Customer unavailable'}
      customerAvatar={<CustomerAvatar name={booking.involvedCustomers[0]} photo={{ url: booking.involvedCustomers[0]?.photoUrl }} size="small" />}
      status={booking.marketplaceBooking?.paymentStatus.name ?? 'Unknown'}
      headline={booking.marketplaceBooking?.productPricing?.listingMetadata?.title ?? 'Booking'}
      summary={[
        { label: 'Date', value: toShortDate(booking.from) },
        { label: 'Amount', value: booking.marketplaceBooking?.totalAmountToDisplay ?? 'Amount unavailable' },
        { label: 'Method', value: booking.marketplaceBooking?.paymentMethod?.type ?? 'Not set' },
        { label: 'Resource', value: booking.bookingResources.map(({ resource }) => resource.name).join(', ') || 'Not assigned' },
      ]}
      payment={payment}
      actions={actions}
      history={[{ title: 'Booking created', meta: toShortDate(booking.from) }]}
    />
  );
};

export default memo(PayMarketplaceBooking);
