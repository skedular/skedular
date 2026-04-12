import type { bookings_bookings_query$key } from '@/queries/__generated__/bookings_bookings_query.graphql';
import type { bookings_bookings_refetchableFragment } from '@/queries/__generated__/bookings_bookings_refetchableFragment.graphql';
import type { bookings_query$key } from '@/queries/__generated__/bookings_query.graphql';
import Box from '@mui/system/Box';
import dayjs, { Dayjs } from 'dayjs';
import { memo, startTransition, useCallback, useEffect, useMemo } from 'react';
import { graphql, useFragment, useRefetchableFragment } from 'react-relay';
import BookingCard from './booking-card';
import OrganizationBookingsPageShell from './organization-bookings-page-shell';

type Props = {
  rootDataRelay: bookings_query$key;
  rootDataBookingRelay: bookings_bookings_query$key;
  organizationCustomDomain: string;
  from: Dayjs;
  to: Dayjs;
  locationIds: string[];
  teamIds: string[];
  customerIds: string[];
};

const Bookings = ({ rootDataRelay, rootDataBookingRelay, organizationCustomDomain, from, to, locationIds, teamIds, customerIds }: Props) => {
  const rootData = useFragment<bookings_query$key>(
    graphql`
      fragment bookings_query on Query {
        me {
          id
        }
        ...bookingCard_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataRefetchable, refetch] = useRefetchableFragment<bookings_bookings_refetchableFragment, bookings_bookings_query$key>(
    graphql`
      fragment bookings_bookings_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "bookings_bookings_refetchableFragment") {
        bookings(
          first: $count
          after: $cursor
          where: {
            organizationCustomDomain: $organizationCustomDomain
            locationIds: $locationIds
            teamIds: $teamIds
            customerIds: $customerIds
            fromGte: $bookingsSearchCriteriaFrom
            fromLte: $bookingsSearchCriteriaTo
          }
          orderBy: [{ field: FROM, direction: ASCENDING }]
        ) @connection(key: "bookings_bookings") {
          __id
          totalCount
          edges {
            node {
              id
              from
              until
              involvedCustomers {
                id
              }
              ...bookingCard_BookingDetails
            }
          }
        }
      }
    `,
    rootDataBookingRelay,
  );

  const bookings = useMemo(() => rootDataRefetchable.bookings.edges.map((edge) => edge.node), [rootDataRefetchable.bookings]);
  const connectionIds = useMemo(() => [rootDataRefetchable.bookings.__id], [rootDataRefetchable.bookings]);

  const handleRefetch = useCallback(
    (from: Dayjs, to: Dayjs, locationIds: string[], teamIds: string[], customerIds: string[]) => {
      startTransition(() => {
        refetch(
          {
            bookingsSearchCriteriaFrom: from.toISOString(),
            bookingsSearchCriteriaTo: to.toISOString(),
            locationIds,
            teamIds,
            customerIds,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  useEffect(() => handleRefetch(from, to, locationIds, teamIds, customerIds), [handleRefetch, from, to, locationIds, teamIds, customerIds]);

  if (!rootDataRefetchable.bookings) {
    return null;
  }

  return (
    <OrganizationBookingsPageShell isEmpty={bookings.length === 0} emptyMessage="No bookings match the selected week and filters.">
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: {
            xs: '1fr',
            sm: 'repeat(auto-fit, minmax(320px, 360px))',
          },
          gap: 2,
          alignItems: 'stretch',
          justifyContent: 'start',
        }}
      >
        {bookings.map((booking) => {
          const canJoinBooking = booking.involvedCustomers.some((item) => item.id === rootData.me?.id)
            ? false
            : !bookings
                .filter((otherBooking) => otherBooking.involvedCustomers.some((item) => item.id === rootData.me?.id))
                .find((myBooking) => {
                  const from = dayjs(booking.from);
                  const myFrom = dayjs(myBooking.from);

                  return from.year() === myFrom.year() && from.month() === myFrom.month() && from.date() === myFrom.date();
                });

          return (
            <Box key={booking.id} sx={{ height: '100%' }}>
              <BookingCard
                rootDataRelay={rootData}
                bookingDetailsRelay={booking}
                organizationCustomDomain={organizationCustomDomain}
                connectionIds={connectionIds}
                canJoinBooking={canJoinBooking}
              />
            </Box>
          );
        })}
      </Box>
    </OrganizationBookingsPageShell>
  );
};

export default memo(Bookings);
