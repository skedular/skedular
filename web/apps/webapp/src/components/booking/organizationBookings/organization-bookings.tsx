import type { organizationBookings_bookings_query$key } from '@/queries/__generated__/organizationBookings_bookings_query.graphql';
import type { organizationBookings_bookings_refetchableFragment } from '@/queries/__generated__/organizationBookings_bookings_refetchableFragment.graphql';
import Divider from '@mui/material/Divider';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { memo, useMemo } from 'react';
import { graphql, usePaginationFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationBookings_bookings_query$key;
  onReloadRequired: () => void;
  organizationId: string;
  topMargin: number;
};

const OrganizationBookings = ({ rootDataRelay, onReloadRequired, organizationId }: Props) => {
  const { data: rootData, refetch } = usePaginationFragment<
    organizationBookings_bookings_refetchableFragment,
    organizationBookings_bookings_query$key
  >(
    graphql`
      fragment organizationBookings_bookings_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationBookings_bookings_refetchableFragment") {
        bookings(
          first: $count
          after: $cursor
          where: { organizationIds: [$organizationId], fromGTE: $bookingsSearchCriteriaFrom, fromLTE: $bookingsSearchCriteriaTo }
          orderBy: [{ field: From, direction: Ascending }]
        ) @connection(key: "organizationBookings_bookings") {
          __id
          totalCount
          edges {
            node {
              id
              from
              to
              location {
                uniqueId
                name
              }
              team {
                uniqueId
                name
              }
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const bookings = useMemo(() => {
    if (!rootData.bookings) {
      return [];
    }

    return rootData.bookings.edges.map((edge) => edge.node);
  }, [rootData.bookings]);

  const groupedBookings = useMemo(() => {
    return bookings.reduce(
      (acc, booking) => {
        if (!booking.location) {
          return acc;
        }

        const locationId = booking.location.uniqueId;

        if (!acc[locationId]) {
          acc[locationId] = [];
        }

        acc[locationId].push(booking);

        return acc;
      },
      {} as Record<string, typeof bookings>,
    );
  }, [bookings]);

  if (!rootData.bookings) {
    return <></>;
  }

  return (
    <Stack direction="column">
      <Typography variant="h5">My Bookings</Typography>
      <Divider />
    </Stack>
  );
};

export default memo(OrganizationBookings);
