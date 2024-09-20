import { BookingCard } from '@/components/booking';
import type { bookingFeedsPaginationQuery } from '@/queries/__generated__/bookingFeedsPaginationQuery.graphql';
import type { bookingFeeds_query$key } from '@/queries/__generated__/bookingFeeds_query.graphql';
import Grid from '@mui/material/Grid2';
import { memo, useCallback, useMemo } from 'react';
import { graphql, usePaginationFragment } from 'react-relay';

type Props = {
  rootDataRelay: bookingFeeds_query$key;
};

const BookingFeeds = ({ rootDataRelay }: Props) => {
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
  } = usePaginationFragment<bookingFeedsPaginationQuery, bookingFeeds_query$key>(
    graphql`
      fragment bookingFeeds_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "bookingFeedsPaginationQuery") {
        bookings(first: $count, after: $cursor, where: { includeMineOnly: true }, orderBy: $bookingSortingValues)
          @connection(key: "BookingFeeds_bookings") {
          __id
          edges {
            node {
              id
              ...bookingCard_BookingDetails
            }
          }
        }
        ...bookingCard_query
      }
    `,
    rootDataRelay,
  );

  const loadMore = useCallback(() => {
    if (isLoadingNext) {
      return;
    }

    loadNext(10);
  }, [loadNext, isLoadingNext]);

  const connectionIds = useMemo(() => [rootData.bookings?.__id], [rootData.bookings]);

  const bookings = useMemo(() => rootData.bookings, [rootData.bookings]);

  return (
    <Grid container spacing={1}>
      {bookings.edges.map((edge) => (
        <Grid key={edge.node.id}>
          <BookingCard
            rootDataRelay={rootData}
            bookingDetailsRelay={edge.node}
            connectionIds={connectionIds}
            hideOrganizationControl={false}
            hideLocationControl={false}
            canJoinBooking={false}
          />
        </Grid>
      ))}
    </Grid>
  );
};

export default memo(BookingFeeds);
