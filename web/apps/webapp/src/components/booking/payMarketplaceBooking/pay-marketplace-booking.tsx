import { CustomerAvatar } from '@/components/avatars';
import LocationAvatar from '@/components/avatars/location-avatar';
import TeamAvatar from '@/components/avatars/team-avatar';
import { AppBarWithStackColumn, BodyIconTypography, SmallIconTypography, StackRow } from '@/components/commons';
import FormFieldLabel from '@/components/commons/form-field-label';
import StackColumn from '@/components/commons/stack-column';
import { getOrganizationMarketplaceBaseLink } from '@/components/links';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { getCustomerFullName, getOpeningHoursFromDateTime, isMidnight, joinErrors, toOpeningHoursFromTime, toShortDate, toShortTime } from '@/libs/utils';
import type { payMarketplaceBooking_booking_query$key } from '@/queries/__generated__/payMarketplaceBooking_booking_query.graphql';
import type { payMarketplaceBooking_booking_refetchableFragment } from '@/queries/__generated__/payMarketplaceBooking_booking_refetchableFragment.graphql';
import type { payMarketplaceBooking_deleteBookingMutation } from '@/queries/__generated__/payMarketplaceBooking_deleteBookingMutation.graphql';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import dayjs, { Dayjs } from 'dayjs';
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
          bookingCheckoutSession {
            checkoutUrl
          }
          paymentExpiry
          lineItems {
            quantity
            productVersion {
              uniqueId
              name
              priceToDisplay
            }
          }
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

  const shortDateFormatFrom = toShortDate(rootData.booking?.from);
  const getTimeLeftToPayInSeconds = (paymentExpiry: string) => {
    const expirtTime = dayjs(paymentExpiry).utc();
    const currentTime = dayjs().utc();

    return expirtTime.isBefore(currentTime) ? null : new Date(expirtTime.diff(currentTime, 'second') * 1000).toISOString().slice(11, 19);
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

            <StackColumn sx={{ paddingRight: defaultPadding, paddingTop: defaultPadding }}>
              {booking.bookingCheckoutSession && (
                <StackRow>
                  <SmallIconTypography label={`Time left to pay: ${timeLeftToPayInSeconds ? timeLeftToPayInSeconds : 'Expired'}`} color="error.main" />
                  {timeLeftToPayInSeconds && (
                    <>
                      <Button LinkComponent={Link} variant="contained" href={booking.bookingCheckoutSession?.checkoutUrl}>
                        Pay
                      </Button>
                      <Button variant="contained" onClick={handleCancelBookingClick} sx={defaultButtonStyle}>
                        Cancel
                      </Button>
                    </>
                  )}
                </StackRow>
              )}
              {!booking.bookingCheckoutSession && (
                <StackRow>
                  <CircularProgress />
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
