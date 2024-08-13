import Badge from '@mui/material/Badge';
import { PickersDay, PickersDayProps } from '@mui/x-date-pickers/PickersDay';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { convertCalendarDayToStartOfDay, endOfDay, joinErrors, toShortDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import type { Dayjs } from 'dayjs';
import dayjs from 'dayjs';
import { useSnackbar } from 'notistack';
import { useMutation } from 'react-relay';
import { v4 as uuidv4 } from 'uuid';
import type { smallMonthlyViewCalendarDay_addBookingMutation } from './__generated__/smallMonthlyViewCalendarDay_addBookingMutation.graphql';
import type { smallMonthlyViewCalendarDay_deleteBookingMutation } from './__generated__/smallMonthlyViewCalendarDay_deleteBookingMutation.graphql';
import type { smallMonthlyViewCalendar_query$data } from './__generated__/smallMonthlyViewCalendar_query.graphql';

type Props = {
  rootData: smallMonthlyViewCalendar_query$data;
  connectionIds: string[];
  organizationId?: string | null;
};

const SmallMonthlyViewCalendarDay = ({ rootData, connectionIds, organizationId }: Props) => {
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

  const { enqueueSnackbar } = useSnackbar();

  const renderDay = (props: PickersDayProps<Dayjs>): JSX.Element => {
    if (!rootData.monthlyBookings.__id) {
      return <></>;
    }

    const matchingBookingFound = rootData.monthlyBookings.edges
      .map((edge) => edge.node)
      ?.find((booking) => {
        const from = dayjs(booking.from);

        return from.year() === props.day.year() && from.month() === props.day.month() && from.date() === props.day.date();
      });

    const pickersDay = (
      <PickersDay
        {...props}
        selected={false}
        onClick={() => {
          const id = matchingBookingFound ? matchingBookingFound.id : uuidv4();
          const startOfDay = convertCalendarDayToStartOfDay(props.day);
          const from = startOfDay.toISOString();
          const to = endOfDay(startOfDay).toISOString();
          const fromToPrint = toShortDate(startOfDay);

          if (matchingBookingFound) {
            commitDeleteBooking({
              variables: {
                connectionIds,
                input: {
                  clientMutationId: uuidv4(),
                  id,
                },
              },
              onCompleted: (_, errors) => {
                if (errors && errors.length > 0) {
                  enqueueSnackbar(`Failed to delete booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
                    variant: 'error',
                    anchorOrigin,
                  });
                }
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
                  clientMutationId: uuidv4(),
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
                  enqueueSnackbar(`Failed to add booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
                    variant: 'error',
                    anchorOrigin,
                  });
                }
              },
              onError: (error) => {
                enqueueSnackbar(`Failed to add booking '${fromToPrint}'. Error: ${error.message}`, {
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
      />
    );

    return matchingBookingFound ? (
      <Badge variant="dot" color="primary">
        {pickersDay}
      </Badge>
    ) : (
      pickersDay
    );
  };

  return renderDay;
};

export default SmallMonthlyViewCalendarDay;
