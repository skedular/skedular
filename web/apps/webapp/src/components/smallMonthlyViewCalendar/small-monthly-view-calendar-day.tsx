import type { smallMonthlyViewCalendar_bookings_query$data } from '@/queries/__generated__/smallMonthlyViewCalendar_bookings_query.graphql';
import type { smallMonthlyViewCalendar_query$data } from '@/queries/__generated__/smallMonthlyViewCalendar_query.graphql';
import type { smallMonthlyViewCalendarDay_addBookingMutation } from '@/queries/__generated__/smallMonthlyViewCalendarDay_addBookingMutation.graphql';
import type { smallMonthlyViewCalendarDay_deleteBookingMutation } from '@/queries/__generated__/smallMonthlyViewCalendarDay_deleteBookingMutation.graphql';
import Badge from '@mui/material/Badge';
import { PickersDay, PickersDayProps } from '@mui/x-date-pickers/PickersDay';
import {
  NotificationContent,
  errorNotificationOptions,
  infoNotificationOptions,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { LOCATION_TAG_TYPE_LOCATION_ZONE } from '@repo/shared/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@repo/shared/libs/providers';
import { convertCalendarDayToStartOfDay, endOfDay, getCustomerFullName, joinErrors, toShortDate } from '@repo/shared/libs/utils';
import type { Dayjs } from 'dayjs';
import dayjs from 'dayjs';
import { nanoid } from 'nanoid';
import type { JSX } from 'react';
import { useContext } from 'react';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';

type Props = {
  rootData: smallMonthlyViewCalendar_query$data;
  rootDataBookings: smallMonthlyViewCalendar_bookings_query$data;
  connectionIds: string[];
  organizationId?: string;
};

const SmallMonthlyViewCalendarDay = ({ rootData, rootDataBookings, connectionIds, organizationId }: Props) => {
  const [commitAddBooking] = useMutation<smallMonthlyViewCalendarDay_addBookingMutation>(graphql`
    mutation smallMonthlyViewCalendarDay_addBookingMutation($connectionIds: [ID!]!, $input: AddBookingInput!) @raw_response_type {
      addBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
          id
          from
          to
          notes
          customer {
            uniqueId
            name
            givenName
            middleName
            familyName
            photoUrl
          }
          organization {
            uniqueId
            name
          }
          location {
            uniqueId
            name
          }
          team {
            uniqueId
            name
          }
          desks {
            uniqueId
            name
            locationTags {
              uniqueId
              name
              tagType
            }
          }
        }
      }
    }
  `);

  const [commitDeleteBooking] = useMutation<smallMonthlyViewCalendarDay_deleteBookingMutation>(graphql`
    mutation smallMonthlyViewCalendarDay_deleteBookingMutation($connectionIds: [ID!]!, $input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const renderDay = (props: PickersDayProps<Dayjs>): JSX.Element => {
    if (!rootDataBookings.bookings || !rootDataBookings.bookings.__id) {
      return <></>;
    }

    const matchingBookingFound = rootDataBookings.bookings.edges
      .map((edge) => edge.node)
      ?.find((booking) => {
        const from = dayjs(booking.from);

        return from.year() === props.day.year() && from.month() === props.day.month() && from.date() === props.day.date();
      });

    return (
      <PickersDay
        {...props}
        selected={false}
        onClick={() => {
          const id = matchingBookingFound ? matchingBookingFound.id : nanoid();
          const startOfDay = convertCalendarDayToStartOfDay(props.day);
          const from = startOfDay.toISOString();
          const to = endOfDay(startOfDay).toISOString();
          const fromToPrint = toShortDate(startOfDay);

          if (matchingBookingFound) {
            let bookingDetailsInfo = `for ${getCustomerFullName(matchingBookingFound.customer)}`;
            if (matchingBookingFound.location) {
              bookingDetailsInfo += ` at the "${matchingBookingFound.location!.name}"`;
            }

            bookingDetailsInfo += ` on ${toShortDate(matchingBookingFound.from)}`;

            const toastId = themedToast(<NotificationContent content={`Removing booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

            commitDeleteBooking({
              variables: {
                connectionIds,
                input: {
                  clientMutationId: nanoid(),
                  id,
                },
              },
              onCompleted: (_, errors) => {
                if (errors && errors.length > 0) {
                  toast.update(toastId, {
                    ...errorNotificationOptions,
                    render: <NotificationContent content={`Failed to remove booking '${fromToPrint}'. Error: ${joinErrors(errors)}.`} />,
                  });

                  return;
                }

                toast.update(toastId, {
                  ...successNotificationOptions,
                  render: <NotificationContent content={`Booking ${bookingDetailsInfo} removed.`} />,
                });

                UpdateGlobalReloadId();
              },
              onError: (error) => {
                toast.update(toastId, {
                  ...errorNotificationOptions,
                  render: <NotificationContent content={`Failed to remove booking '${fromToPrint}'. Error: ${error.message}.`} />,
                });
              },
            });
          } else {
            if (!rootData.me) {
              return;
            }

            const toastId = themedToast(<NotificationContent content={`Making a booking on '${fromToPrint}'...`} />, infoNotificationOptions);

            commitAddBooking({
              variables: {
                connectionIds,
                input: {
                  clientMutationId: nanoid(),
                  id,
                  customerId: rootData.me.id,
                  from,
                  to,
                  organizationId,
                  deskIds: [],
                },
              },
              onCompleted: (response, errors) => {
                if (errors && errors.length > 0) {
                  toast.update(toastId, {
                    ...errorNotificationOptions,
                    render: <NotificationContent content={`Failed to make a booking '${fromToPrint}'. Error: ${joinErrors(errors)}.`} />,
                  });

                  return;
                }

                const booking = response.addBooking?.booking!;
                let message = `Booking made for ${getCustomerFullName(booking.customer)} to work`;

                if (booking.location) {
                  message += ` from the "${booking.location!.name}"`;
                }

                if (booking.desks.length > 0) {
                  message += ` at desk "${booking.desks.map(({ name }) => name).join(', ')}"`;

                  const zones = booking.desks
                    .flatMap(({ locationTags }) => locationTags)
                    .filter(({ tagType }) => tagType === LOCATION_TAG_TYPE_LOCATION_ZONE);
                  if (zones.length > 0) {
                    const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.uniqueId, zone), new Map()).values());

                    message += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
                  }
                }

                message += ` on ${toShortDate(booking.from)}.`;

                toast.update(toastId, {
                  ...successNotificationOptions,
                  render: <NotificationContent content={message} />,
                });

                UpdateGlobalReloadId();
              },
              onError: (error) => {
                toast.update(toastId, {
                  ...errorNotificationOptions,
                  render: <NotificationContent content={`Failed to make a booking '${fromToPrint}'. Error: ${error.message}.`} />,
                });
              },
              optimisticResponse: {
                addBooking: {
                  booking: {
                    id,
                    from,
                    to,
                    customer: {
                      uniqueId: rootData.me.id,
                      photoUrl: rootData.me.photoUrl,
                      name: rootData.me.name,
                      givenName: rootData.me.givenName,
                      middleName: rootData.me.middleName,
                      familyName: rootData.me.familyName,
                    },
                    notes: '',
                    organization: null,
                    location: null,
                    team: null,
                    desks: [],
                  },
                },
              },
            });
          }
        }}
      >
        {matchingBookingFound && (
          <Badge variant="dot" color="primary">
            {props.day.date()}
          </Badge>
        )}
        {!matchingBookingFound && <> {props.day.date()}</>}
      </PickersDay>
    );
  };

  return renderDay;
};

export default SmallMonthlyViewCalendarDay;
