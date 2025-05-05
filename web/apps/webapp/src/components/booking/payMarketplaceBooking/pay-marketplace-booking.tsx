import CustomerAvatar from '@/components/avatars/customer-avatar';
import LocationAvatar from '@/components/avatars/location-avatar';
import TeamAvatar from '@/components/avatars/team-avatar';
import { AppBarWithStackColumn, BodyIconTypography } from '@/components/commons';
import FormFieldLabel from '@/components/commons/form-field-label';
import StackColumn from '@/components/commons/stack-column';
import StackRow from '@/components/commons/stack-row';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { getCustomerFullName, getOpeningHoursFromDateTime, isMidnight, toOpeningHoursFromTime, toShortDate, toShortTime } from '@/libs/utils';
import type { payMarketplaceBooking_booking_query$key } from '@/queries/__generated__/payMarketplaceBooking_booking_query.graphql';
import type { payMarketplaceBooking_booking_refetchableFragment } from '@/queries/__generated__/payMarketplaceBooking_booking_refetchableFragment.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Link from '@mui/material/Link';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import { Dayjs } from 'dayjs';
import { useRouter } from 'next/navigation';
import { memo, useState } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataRelay: payMarketplaceBooking_booking_query$key;
  onReloadRequired?: () => void;
};

const PayMarketplaceBooking = ({ rootDataRelay }: Props) => {
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
          bookingCheckoutSession {
            checkoutUrl
            paymentStatus
            amountTotalToDisplay
          }
          bookingCheckoutSessionExpiry
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

  const router = useRouter();
  const [allDay] = useState<boolean>(isMidnight(rootData.booking?.from) && isMidnight(rootData.booking?.until));
  const [timeRange] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootData.booking?.from)),
    toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootData.booking?.until)),
  ]);

  const handleCloseClick = () => {
    router.back();
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

            {booking.bookingCheckoutSession && (
              <FormFieldLabel label="Total Amount">
                <BodyIconTypography label={`${booking.bookingCheckoutSession.amountTotalToDisplay}`} />
              </FormFieldLabel>
            )}

            <StackColumn sx={{ paddingRight: defaultPadding, paddingTop: defaultPadding }}>
              <StackRow>
                <Button LinkComponent={Link} variant="contained" sx={defaultButtonStyle} href={booking.bookingCheckoutSession?.checkoutUrl}>
                  Pay
                </Button>
              </StackRow>
            </StackColumn>
          </StackColumn>
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

export default memo(PayMarketplaceBooking);
