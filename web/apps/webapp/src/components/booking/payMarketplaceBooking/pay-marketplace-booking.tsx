import { CustomerAvatar } from '@/components/avatars';
import LocationAvatar from '@/components/avatars/location-avatar';
import TeamAvatar from '@/components/avatars/team-avatar';
import InvoiceDownloadLinks from '@/components/booking/invoice-download-links';
import { getOrganizationProductsBaseLink } from '@/components/links';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { payMarketplaceBooking_booking_query$key } from '@/queries/__generated__/payMarketplaceBooking_booking_query.graphql';
import type { payMarketplaceBooking_booking_Subscription } from '@/queries/__generated__/payMarketplaceBooking_booking_Subscription.graphql';
import type { payMarketplaceBooking_confirmBookingPaymentMutation } from '@/queries/__generated__/payMarketplaceBooking_confirmBookingPaymentMutation.graphql';
import type { payMarketplaceBooking_deleteMarketplaceBookingMutation } from '@/queries/__generated__/payMarketplaceBooking_deleteMarketplaceBookingMutation.graphql';
import type { payMarketplaceBooking_makeBookingPaymentNotRequiredMutation } from '@/queries/__generated__/payMarketplaceBooking_makeBookingPaymentNotRequiredMutation.graphql';
import type { payMarketplaceBooking_rejectBookingPaymentMutation } from '@/queries/__generated__/payMarketplaceBooking_rejectBookingPaymentMutation.graphql';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
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
  useIntegratedPlatrform,
} from '@skedular/shared';
import { AppBarWithStackColumn, BodyIconTypography, defaultButtonStyle, defaultPadding, FormFieldLabel, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import dayjs, { Dayjs } from 'dayjs';
import { useRouter } from 'next/navigation';
import { memo, useContext, useEffect, useMemo, useState } from 'react';
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
  const getTimeLeftToPayInSeconds = (paymentExpiry: string) => {
    const expirtTime = dayjs(paymentExpiry).utc();
    const currentTime = dayjs().utc();

    if (expirtTime.isBefore(currentTime)) {
      return null;
    }

    const totalSeconds = expirtTime.diff(currentTime, 'second');
    if (totalSeconds > 24 * 60 * 60) {
      // More than 24 hours
      const totalDays = expirtTime.diff(currentTime, 'day');

      return `${totalDays} day(s) and ${new Date(totalSeconds * 1000).toISOString().slice(11, 19)}`;
    } else {
      return new Date(totalSeconds * 1000).toISOString().slice(11, 19);
    }
  };

  const { integratedPlatrform } = useIntegratedPlatrform();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const router = useRouter();
  const [allDay] = useState<boolean>(isMidnight(rootData.booking?.from) && isMidnight(rootData.booking?.until));
  const [timeRange] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootData.booking?.from)),
    toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootData.booking?.until)),
  ]);
  const [timeLeftToPayInSeconds, setTimeLeftToPayInSeconds] = useState(() =>
    rootData.booking?.marketplaceBooking ? getTimeLeftToPayInSeconds(rootData.booking.marketplaceBooking.paymentExpiry) : null,
  );

  useEffect(() => {
    const interval = setInterval(() => {
      setTimeLeftToPayInSeconds(rootData.booking?.marketplaceBooking ? getTimeLeftToPayInSeconds(rootData.booking.marketplaceBooking.paymentExpiry) : null);
    }, 1000);

    return () => clearInterval(interval);
  }, [rootData.booking]);

  const handleCloseClick = () => {
    router.push(getOrganizationProductsBaseLink(integratedPlatrform, organizationCustomDomain));
  };

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

    const toastId = themedToast(<NotificationContent content={`Cancelling booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    commitDeleteMarketplaceBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: bookingDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to cancel booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Booking ${bookingDetailsInfo} cancelled.`} />,
        });

        router.push(getOrganizationProductsBaseLink(integratedPlatrform, organizationCustomDomain));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to cancel booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(error)}.`} />,
        });
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

    const toastId = themedToast(<NotificationContent content={`Confirming payment for booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    commitConfirmBookingPayment({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: bookingDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to confirm payment for booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Booking ${bookingDetailsInfo} payment confirmed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to confirm payment for booking '${shortDateFormatFrom}'. Error: ${getRelayErrorMessage(error)}.`} />,
        });
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

    const toastId = themedToast(<NotificationContent content={`Rejecting payment for booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    commitRejectBookingPayment({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: bookingDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to reject payment for booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Booking ${bookingDetailsInfo} payment rejected.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to reject payment for booking '${shortDateFormatFrom}'. Error: ${getRelayErrorMessage(error)}.`} />,
        });
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

    const toastId = themedToast(<NotificationContent content={`Making payment for booking '${bookingDetailsInfo}' not required...`} />, infoNotificationOptions);

    commitMakeBookingPaymentNotRequired({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: bookingDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to make payment for booking ${bookingDetailsInfo} not required. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Booking ${bookingDetailsInfo} payment made not required.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to make payment for booking '${shortDateFormatFrom}' not required. Error: ${getRelayErrorMessage(error)}.`} />,
        });
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

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Pay Booking">
          <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
            <FormFieldLabel label="Date/Time" stackLabelOnTop>
              <StackRow>
                <BodyIconTypography label={`${toShortDate(booking.from)}, `} />
                {allDay && <BodyIconTypography label="All day" />}
                {!allDay && <BodyIconTypography label={`${toShortTime(timeRange[0])} - ${toShortTime(timeRange[1])}`} />}
              </StackRow>
            </FormFieldLabel>

            {booking.involvedCustomers.length > 0 && (
              <FormFieldLabel label="Users" stackLabelOnTop>
                <StackRow>
                  {booking.involvedCustomers.map((customer) => (
                    <BodyIconTypography
                      key={customer.id}
                      label={getCustomerFullName(customer)}
                      startElement={<CustomerAvatar name={customer} photo={{ url: booking.involvedCustomers[0].photoUrl }} size="small" />}
                    />
                  ))}
                </StackRow>
              </FormFieldLabel>
            )}

            {booking.involvedTeams.length > 0 && (
              <FormFieldLabel label="Teams" stackLabelOnTop>
                <StackRow>
                  {booking.involvedTeams.map((team) => (
                    <BodyIconTypography key={team.id} label={team.name} startElement={<TeamAvatar name={team} size="small" />} />
                  ))}
                </StackRow>
              </FormFieldLabel>
            )}

            {booking.involvedLocations.length > 0 && (
              <FormFieldLabel label="Locations" stackLabelOnTop>
                <StackRow>
                  {booking.involvedLocations.map((location) => (
                    <BodyIconTypography key={location.uniqueId} label={location.name} startElement={<LocationAvatar name={location} size="small" />} />
                  ))}
                </StackRow>
              </FormFieldLabel>
            )}

            {booking.bookingResources.length > 0 && (
              <FormFieldLabel label="Resources" stackLabelOnTop>
                <BodyIconTypography
                  label={booking.bookingResources
                    .reduce((acc, val) => `${acc}, ${val.resource.name}`, '')
                    .trim()
                    .replace(/^,+|,+$/g, '')}
                />
              </FormFieldLabel>
            )}

            <FormFieldLabel label="Total Exclude GST/VAT" stackLabelOnTop>
              <BodyIconTypography label={`${booking.marketplaceBooking?.totalAmountExcludeTaxToDisplay}`} />
            </FormFieldLabel>

            <FormFieldLabel label="Total GST/VAT" stackLabelOnTop>
              <BodyIconTypography label={`${booking.marketplaceBooking?.taxAmountToDisplay}`} />
            </FormFieldLabel>

            <FormFieldLabel label="Total Amount" stackLabelOnTop>
              <BodyIconTypography label={`${booking.marketplaceBooking?.totalAmountToDisplay}`} />
            </FormFieldLabel>

            <FormFieldLabel label="" stackLabelOnTop>
              <InvoiceDownloadLinks invoices={booking.arrearsInvoices ?? []} legacyInvoiceUrl={booking.marketplaceBooking?.invoiceUrl ?? null} size="body" />
            </FormFieldLabel>

            <StackColumn sx={{ paddingRight: defaultPadding, paddingTop: defaultPadding }}>
              <StackRow>
                <SmallIconTypography label={`Time left to pay: ${timeLeftToPayInSeconds ? timeLeftToPayInSeconds : 'Expired'}`} color="error.main" />
              </StackRow>

              <StackRow>
                {booking.marketplaceBooking?.bookingCheckoutSession?.checkoutUrl && (
                  <Button LinkComponent={Link} variant="contained" href={booking.marketplaceBooking.bookingCheckoutSession.checkoutUrl}>
                    Pay
                  </Button>
                )}
                <Button variant="contained" onClick={handleCancelBookingClick} sx={defaultButtonStyle} endIcon={<CircularProgress size={20} />}>
                  Cancel
                </Button>
              </StackRow>

              {rootData.organizationBookingPermissions.canModifyPaymentMethod &&
                booking.marketplaceBooking &&
                booking.marketplaceBooking.isPaymentRequired &&
                booking.marketplaceBooking.paymentStatus.type !== 'REJECTED' &&
                booking.marketplaceBooking.paymentStatus.type !== 'EXPIRED' &&
                booking.marketplaceBooking.paymentStatus.type !== 'RECORD_NEVER_CREATED' && (
                  <StackRow>
                    <Button variant="contained" onClick={handleConfirmPaymentClick}>
                      Confirm Payment
                    </Button>
                    <Button variant="contained" onClick={handleRejectPaymentClick} sx={defaultButtonStyle}>
                      Reject Payment
                    </Button>
                    <Button variant="contained" onClick={handleMakePaymentNotRequiredClick} sx={defaultButtonStyle}>
                      Make Payment Not Required
                    </Button>
                  </StackRow>
                )}
            </StackColumn>
          </StackColumn>
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

export default memo(PayMarketplaceBooking);
