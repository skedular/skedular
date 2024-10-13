import type { smallMonthlyViewCalendar_bookings_query$data } from '@/queries/__generated__/smallMonthlyViewCalendar_bookings_query.graphql';
import type { smallMonthlyViewCalendar_query$data } from '@/queries/__generated__/smallMonthlyViewCalendar_query.graphql';
import type { smallMonthlyViewCalendarDay_addBookingMutation } from '@/queries/__generated__/smallMonthlyViewCalendarDay_addBookingMutation.graphql';
import type { smallMonthlyViewCalendarDay_deleteBookingMutation } from '@/queries/__generated__/smallMonthlyViewCalendarDay_deleteBookingMutation.graphql';
import Badge from '@mui/material/Badge';
import { PickersDay, PickersDayProps } from '@mui/x-date-pickers/PickersDay';
import { UpdateGlobalReloadIdContext } from '@repo/shared/libs/providers';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { convertCalendarDayToStartOfDay, endOfDay, joinErrors, toShortDate } from '@repo/shared/libs/utils';
import type { Dayjs } from 'dayjs';
import dayjs from 'dayjs';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { useContext } from 'react';
import { graphql, useMutation } from 'react-relay';

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

  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const { enqueueSnackbar } = useSnackbar();
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
                  enqueueSnackbar(`Failed to delete booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
                    variant: 'error',
                    anchorOrigin,
                  });

                  return;
                }

                UpdateGlobalReloadId();
              },
              onError: (error) => {
                enqueueSnackbar(`Failed to delete booking '${fromToPrint}'. Error: ${error.message}`, {
                  variant: 'error',
                  anchorOrigin,
                });
              },
            });
          } else {
            if (!rootData.me) {
              return;
            }

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
              onCompleted: (_, errors) => {
                if (errors && errors.length > 0) {
                  enqueueSnackbar(`Failed to make a booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
                    variant: 'error',
                    anchorOrigin,
                  });

                  return;
                }

                UpdateGlobalReloadId();
              },
              onError: (error) => {
                enqueueSnackbar(`Failed to make a booking '${fromToPrint}'. Error: ${error.message}`, {
                  variant: 'error',
                  anchorOrigin,
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
