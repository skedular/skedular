import type { myBookings_bookings_query$key } from '@/queries/__generated__/myBookings_bookings_query.graphql';
import type { myBookings_bookings_refetchableFragment } from '@/queries/__generated__/myBookings_bookings_refetchableFragment.graphql';
import type { myBookings_query$key } from '@/queries/__generated__/myBookings_query.graphql';
import Box from '@mui/system/Box';
import dayjs, { Dayjs } from 'dayjs';
import { memo, startTransition, useCallback, useEffect, useMemo } from 'react';
import { graphql, useFragment, useRefetchableFragment } from 'react-relay';
import MyBookingCard from './my-booking-card';
import MyBookingsPageShell from './my-bookings-page-shell';

type Props = {
  rootDataRelay: myBookings_query$key;
  rootDataBookingRelay: myBookings_bookings_query$key;
  organizationCustomDomain: string;
  from: Dayjs;
  to: Dayjs;
  locationIds: string[];
  teamIds: string[];
};

type CustomerDetails = {
  id: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

type MarketplaceSubscriptionLookup = Record<string, string>;

const MyBookings = ({ rootDataRelay, rootDataBookingRelay, organizationCustomDomain, from, to, locationIds, teamIds }: Props) => {
  const rootData = useFragment<myBookings_query$key>(
    graphql`
      fragment myBookings_query on Query {
        me {
          id
        }
        marketplaceBookingSubscriptionCancellationModes {
          type
          name
        }
        marketplaceBookingSubscriptions(first: 100, where: { includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain }) {
          edges {
            node {
              id
              recurringBookings {
                id
              }
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [rootDataRefetchable, refetch] = useRefetchableFragment<myBookings_bookings_refetchableFragment, myBookings_bookings_query$key>(
    graphql`
      fragment myBookings_bookings_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "myBookings_bookings_refetchableFragment") {
        bookings(
          first: $count
          after: $cursor
          where: {
            organizationCustomDomain: $organizationCustomDomain
            locationIds: $locationIds
            teamIds: $teamIds
            fromGte: $bookingsSearchCriteriaFrom
            fromLte: $bookingsSearchCriteriaTo
          }
          orderBy: [{ field: FROM, direction: ASCENDING }]
        ) @connection(key: "myBookings_bookings") {
          __id
          totalCount
          edges {
            node {
              id
              from
              until
              notes
              channel {
                channel
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
              ...myBookingCard_BookingDetails
            }
          }
        }
      }
    `,
    rootDataBookingRelay,
  );

  const bookings = useMemo(() => rootDataRefetchable.bookings.edges.map((edge) => edge.node), [rootDataRefetchable.bookings]);
  const connectionIds = useMemo(() => [rootDataRefetchable.bookings.__id], [rootDataRefetchable.bookings]);
  const myBookings = useMemo(() => bookings.filter((booking) => booking.involvedCustomers.some((item) => item.id === rootData.me?.id)), [bookings, rootData.me?.id]);
  const recurringMarketplaceSubscriptionIds = useMemo(() => {
    return rootData.marketplaceBookingSubscriptions.edges.reduce((lookup, edge) => {
      const subscription = edge.node;

      if (!subscription) {
        return lookup;
      }

      subscription.recurringBookings.forEach((recurringBooking) => {
        lookup[recurringBooking.id] = subscription.id;
      });

      return lookup;
    }, {} as MarketplaceSubscriptionLookup);
  }, [rootData.marketplaceBookingSubscriptions.edges]);

  const convertDateToKey = (date: Dayjs) => dayjs(date).format('YYYY-MM-DD');

  const groupedBookingsByFromDate = useMemo(() => {
    return bookings.reduce(
      (acc, booking) => {
        const key = convertDateToKey(booking.from);

        if (!acc[key]) {
          acc[key] = [];
        }

        acc[key].push(booking);

        return acc;
      },
      {} as Record<string, typeof bookings>,
    );
  }, [bookings]);

  const handleRefetch = useCallback(
    (from: Dayjs, to: Dayjs, locationIds: string[], teamIds: string[]) => {
      startTransition(() => {
        refetch(
          {
            bookingsSearchCriteriaFrom: from.toISOString(),
            bookingsSearchCriteriaTo: to.toISOString(),
            locationIds,
            teamIds,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  useEffect(() => handleRefetch(from, to, locationIds, teamIds), [handleRefetch, from, to, locationIds, teamIds]);

  if (!rootDataRefetchable.bookings) {
    return null;
  }

  return (
    <MyBookingsPageShell isEmpty={myBookings.length === 0} emptyMessage="No bookings match the selected week and filters.">
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
        {myBookings.map((myBooking) => {
          const key = convertDateToKey(myBooking.from);
          const otherTeammates = (
            groupedBookingsByFromDate[key]
              ?.filter(
                (booking) =>
                  booking.id !== myBooking.id &&
                  booking.involvedCustomers.some((item) => item.id !== rootData.me?.id) &&
                  booking.involvedLocations.some((item) => myBooking.involvedLocations.some((item2) => item.uniqueId === item2.uniqueId)),
              )
              .flatMap((booking) => booking.involvedCustomers) ?? []
          ).reduce((acc: CustomerDetails[], teammate) => {
            if (!acc.some((item) => item.id === teammate.id) && teammate.id !== rootData.me?.id) {
              acc.push(teammate);
            }

            return acc;
          }, []);

          return (
            <MyBookingCard
              key={myBooking.id}
              bookingDetailsRelay={myBooking}
              organizationCustomDomain={organizationCustomDomain}
              connectionIds={connectionIds}
              otherTeammates={otherTeammates}
              recurringMarketplaceSubscriptionIds={recurringMarketplaceSubscriptionIds}
            />
          );
        })}
      </Box>
    </MyBookingsPageShell>
  );
};

export default memo(MyBookings);
