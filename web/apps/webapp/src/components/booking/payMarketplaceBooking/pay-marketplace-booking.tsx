import { CustomerAvatar } from '@/components/avatars';
import LocationAvatar from '@/components/avatars/location-avatar';
import TeamAvatar from '@/components/avatars/team-avatar';
import { AppBarWithStackColumn, BodyIconTypography, SmallIconTypography, StackRow } from '@/components/commons';
import FormFieldLabel from '@/components/commons/form-field-label';
import StackColumn from '@/components/commons/stack-column';
import { PdfIcon } from '@/components/icons';
import { getOrganizationMarketplaceBaseLink } from '@/components/links';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { getCustomerFullName, getOpeningHoursFromDateTime, isMidnight, joinErrors, toOpeningHoursFromTime, toShortDate, toShortTime } from '@/libs/utils';
import type { payMarketplaceBooking_booking_query$key } from '@/queries/__generated__/payMarketplaceBooking_booking_query.graphql';
import type { payMarketplaceBooking_booking_refetchableFragment } from '@/queries/__generated__/payMarketplaceBooking_booking_refetchableFragment.graphql';
import type { payMarketplaceBooking_confirmBookingPaymentMutation } from '@/queries/__generated__/payMarketplaceBooking_confirmBookingPaymentMutation.graphql';
import type { payMarketplaceBooking_deleteBookingMutation } from '@/queries/__generated__/payMarketplaceBooking_deleteBookingMutation.graphql';
import type { payMarketplaceBooking_makeBookingPaymentNotRequiredMutation } from '@/queries/__generated__/payMarketplaceBooking_makeBookingPaymentNotRequiredMutation.graphql';
import type { payMarketplaceBooking_rejectBookingPaymentMutation } from '@/queries/__generated__/payMarketplaceBooking_rejectBookingPaymentMutation.graphql';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import dayjs, { Dayjs } from 'dayjs';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useState, useTransition } from 'react';
import { graphql, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  rootDataRelay: payMarketplaceBooking_booking_query$key;
  onReloadRequired?: () => void;
  organizationId: string;
};

const PayMarketplaceBooking = ({ rootDataRelay, organizationId }: Props) => {
  const [rootData, refetch] = useRefetchableFragment<payMarketplaceBooking_booking_refetchableFragment, payMarketplaceBooking_booking_query$key>(
    graphql`
      fragment payMarketplaceBooking_booking_query on Query @refetchable(queryName: "payMarketplaceBooking_booking_refetchableFragment") {
        booking(id: $bookingId) {
          id
          from
          until
          notes
          type {
            type
          }
          involvedCustomers {
            uniqueId
            name
            givenName
            middleName
            familyName
            photoUrl
          }
          involvedOrganizations {
            uniqueId
            name
          }
          involvedLocations {
            uniqueId
            name
          }
          involvedTeams {
            uniqueId
            name
          }
          resources {
            uniqueId
            name
            color
            customTags {
              uniqueId
              name
              color
            }
            zones {
              uniqueId
              name
              color
            }
          }
          totalAmountToDisplay
          paymentMethod {
            type
          }
          bookingCheckoutSession {
            checkoutUrl
          }
          paymentExpiry
          invoiceUrl
          lineItems {
            quantity
            productVersion {
              uniqueId
              name
              priceToDisplay
            }
          }
          isPaymentRequired
          paymentStatus {
            type
            name
          }
        }
        organizationBookingPermissions(organizationId: $organizationId) {
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

  const [commitDeleteBooking] = useMutation<payMarketplaceBooking_deleteBookingMutation>(graphql`
    mutation payMarketplaceBooking_deleteBookingMutation($input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
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
          paymentStatus {
            type
            name
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
          paymentStatus {
            type
            name
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
          paymentStatus {
            type
            name
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
  const [, startTransition] = useTransition();
  const [allDay] = useState<boolean>(isMidnight(rootData.booking?.from) && isMidnight(rootData.booking?.until));
  const [timeRange] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootData.booking?.from)),
    toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootData.booking?.until)),
  ]);
  const [timeLeftToPayInSeconds, setTimeLeftToPayInSeconds] = useState(() => (rootData.booking ? getTimeLeftToPayInSeconds(rootData.booking.paymentExpiry) : null));

  const handleRefetch = useCallback(() => {
    startTransition(() => {
      refetch(
        {},
        {
          fetchPolicy: 'store-and-network',
        },
      );
    });
  }, [refetch]);

  useEffect(() => {
    const interval = setInterval(() => {
      if (!rootData.booking?.bookingCheckoutSession) {
        handleRefetch();
      } else {
        clearInterval(interval);
      }
    }, 1000);

    return () => clearInterval(interval);
  }, [handleRefetch, rootData.booking?.bookingCheckoutSession]);

  useEffect(() => {
    const interval = setInterval(() => {
      setTimeLeftToPayInSeconds(rootData.booking ? getTimeLeftToPayInSeconds(rootData.booking.paymentExpiry) : null);
    }, 1000);

    return () => clearInterval(interval);
  }, [rootData.booking]);

  const handleCloseClick = () => {
    router.push(getOrganizationMarketplaceBaseLink(integratedPlatrform, organizationId));
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

    commitDeleteBooking({
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
            render: <NotificationContent content={`Failed to cancel booking ${bookingDetailsInfo}. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Booking ${bookingDetailsInfo} cancelled.`} />,
        });

        router.push(getOrganizationMarketplaceBaseLink(integratedPlatrform, organizationId));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to cancel booking ${bookingDetailsInfo}.`} />,
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
            render: <NotificationContent content={`Failed to confirm payment for booking ${bookingDetailsInfo}. Error: ${joinErrors(errors)}.`} />,
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
          render: <NotificationContent content={`Failed to confirm payment for booking '${shortDateFormatFrom}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        confirmBookingPayment: {
          booking: {
            id: bookingDetails.id,
            paymentStatus: {
              type: 'CONFIRMED',
              name: rootData.paymentStatuses.find((status) => status.type === 'CONFIRMED')!.name,
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
            render: <NotificationContent content={`Failed to reject payment for booking ${bookingDetailsInfo}. Error: ${joinErrors(errors)}.`} />,
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
          render: <NotificationContent content={`Failed to reject payment for booking '${shortDateFormatFrom}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        rejectBookingPayment: {
          booking: {
            id: bookingDetails.id,
            paymentStatus: {
              type: 'REJECTED',
              name: rootData.paymentStatuses.find((status) => status.type === 'REJECTED')!.name,
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
            render: <NotificationContent content={`Failed to make payment for booking ${bookingDetailsInfo} not required. Error: ${joinErrors(errors)}.`} />,
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
          render: <NotificationContent content={`Failed to make payment for booking '${shortDateFormatFrom}' not required. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        makeBookingPaymentNotRequired: {
          booking: {
            id: bookingDetails.id,
            paymentStatus: {
              type: 'NO_PAYMENT_REQUIRED',
              name: rootData.paymentStatuses.find((status) => status.type === 'NO_PAYMENT_REQUIRED')!.name,
            },
          },
        },
      },
    });
  };

  if (!rootData.booking) {
    return <></>;
  }

  const booking = rootData.booking;

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Pay Booking">
          <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
            <FormFieldLabel label="Date/Time">
              <StackRow>
                <BodyIconTypography label={`${toShortDate(booking.from)}, `} />
                {allDay && <BodyIconTypography label="All day" />}
                {!allDay && <BodyIconTypography label={`${toShortTime(timeRange[0])} - ${toShortTime(timeRange[1])}`} />}
              </StackRow>
            </FormFieldLabel>

            {booking.involvedCustomers.length > 0 && (
              <FormFieldLabel label="Users">
                <StackRow>
                  {booking.involvedCustomers.map((customer) => (
                    <BodyIconTypography
                      key={customer.uniqueId}
                      label={getCustomerFullName(customer)}
                      startElement={<CustomerAvatar name={customer} photo={{ url: booking.involvedCustomers[0].photoUrl }} size="small" />}
                    />
                  ))}
                </StackRow>
              </FormFieldLabel>
            )}

            {booking.involvedTeams.length > 0 && (
              <FormFieldLabel label="Teams">
                <StackRow>
                  {booking.involvedTeams.map((team) => (
                    <BodyIconTypography key={team.uniqueId} label={team.name} startElement={<TeamAvatar name={team} size="small" />} />
                  ))}
                </StackRow>
              </FormFieldLabel>
            )}

            {booking.involvedLocations.length > 0 && (
              <FormFieldLabel label="Locations">
                <StackRow>
                  {booking.involvedLocations.map((location) => (
                    <BodyIconTypography key={location.uniqueId} label={location.name} startElement={<LocationAvatar name={location} size="small" />} />
                  ))}
                </StackRow>
              </FormFieldLabel>
            )}

            {booking.resources.length > 0 && (
              <FormFieldLabel label="Resources">
                <BodyIconTypography
                  label={booking.resources
                    .reduce((acc, val) => `${acc}, ${val.name}`, '')
                    .trim()
                    .replace(/^,+|,+$/g, '')}
                />
              </FormFieldLabel>
            )}

            <FormFieldLabel label="Total Amount">
              <BodyIconTypography label={`${booking.totalAmountToDisplay}`} />
            </FormFieldLabel>

            {booking.invoiceUrl && (
              <FormFieldLabel label="">
                <Link component={NextLink} href={booking.invoiceUrl} target="_blank" rel="noopener noreferrer">
                  <BodyIconTypography label="Download Invoice" startElement={<PdfIcon />} />
                </Link>
              </FormFieldLabel>
            )}

            <StackColumn sx={{ paddingRight: defaultPadding, paddingTop: defaultPadding }}>
              <StackRow>
                <SmallIconTypography label={`Time left to pay: ${timeLeftToPayInSeconds ? timeLeftToPayInSeconds : 'Expired'}`} color="error.main" />
              </StackRow>

              <StackRow>
                {booking.bookingCheckoutSession?.checkoutUrl && (
                  <Button LinkComponent={Link} variant="contained" href={booking.bookingCheckoutSession.checkoutUrl}>
                    Pay
                  </Button>
                )}
                <Button variant="contained" onClick={handleCancelBookingClick} sx={defaultButtonStyle} endIcon={<CircularProgress size={20} />}>
                  Cancel
                </Button>
              </StackRow>

              {rootData.organizationBookingPermissions.canModifyPaymentMethod &&
                booking.isPaymentRequired &&
                booking.paymentStatus.type !== 'REJECTED' &&
                booking.paymentStatus.type !== 'EXPIRED' &&
                booking.paymentStatus.type !== 'RECORD_NEVER_CREATED' && (
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
