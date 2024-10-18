import { BookingCard } from '@/components/booking';
import { NewBookingButton } from '@/components/booking/addBooking';
import type { bookings_bookings_query$key } from '@/queries/__generated__/bookings_bookings_query.graphql';
import type {
  BookingOrderField,
  BookingOrderInput,
  bookings_bookings_refetchableFragment,
} from '@/queries/__generated__/bookings_bookings_refetchableFragment.graphql';
import type { bookings_query$key } from '@/queries/__generated__/bookings_query.graphql';
import type { bookings_rootQuery } from '@/queries/__generated__/bookings_rootQuery.graphql';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { WeekPicker } from '@repo/shared/components/weekPicker';
import { endOfWeek, startOfDay, startOfWeek } from '@repo/shared/libs/utils';
import dayjs, { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, useFragment, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<bookings_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
  locationId?: string;
  teamId?: string;
};

const RootQuery = graphql`
  query bookings_rootQuery(
    $organizationId: String!
    $organizationExists: Boolean!
    $locationId: String!
    $locationExists: Boolean!
    $teamId: String!
    $dateToGetAvailableDesks: DateTime!
    $deskIdsToIncludeToGetAvailableDesks: [String!]!
    $bookingPeopleNameSearchText: String
    $bookingSortingValues: [BookingOrderInput!]!
    $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
  ) {
    ...bookings_query
    ...bookings_bookings_query
  }
`;

const Bookings = ({ queryReference, onReloadRequired, organizationId, locationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<bookings_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<bookings_query$key>(
    graphql`
      fragment bookings_query on Query {
        me {
          id
        }
        ...bookingCard_query
        ...newBookingDialog_query
      }
    `,
    rootDataRelay,
  );
  const {
    data: rootDataBookings,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<bookings_bookings_refetchableFragment, bookings_bookings_query$key>(
    graphql`
      fragment bookings_bookings_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "bookings_bookings_refetchableFragment") {
        bookings(
          first: $count
          after: $cursor
          where: {
            organizationIds: [$organizationId]
            locationIds: [$locationId]
            teamIds: [$teamId]
            fromGTE: $bookingsSearchCriteriaFrom
            fromLTE: $bookingsSearchCriteriaTo
            includeMineOnly: false
          }
          orderBy: $bookingSortingValues
        ) @connection(key: "bookings_bookings") {
          __id
          totalCount
          edges {
            node {
              id
              from
              to
              customer {
                uniqueId
              }
              ...bookingCard_BookingDetails
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<BookingOrderInput>({
    direction: 'Ascending',
    field: 'from',
  });
  const [page, setPage] = useState(0);
  const [startWeek, setStartWeek] = useState(startOfWeek);
  const [pageSize, setPageSize] = useState(50);

  const handleChangePage = (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    if (newPage > page) {
      loadNextPage();
    }

    setPage(newPage);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const pageSize = parseInt(event.target.value, 10);

    setPageSize(parseInt(event.target.value, 10));

    handleRefetch(pageSize, sortingOrder, startWeek);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: BookingOrderInput, date: Dayjs) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            bookingSortingValues: [order],
            bookingsSearchCriteriaFrom: date.toISOString(),
            bookingsSearchCriteriaTo: endOfWeek(date).add(-1, 'milliseconds').toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
            onComplete: () => {
              setPage(0);
            },
          },
        );
      });
    },
    [refetch],
  );

  const loadNextPage = useCallback(() => {
    if (isLoadingNext) {
      return;
    }

    loadNext(pageSize);
  }, [loadNext, isLoadingNext, pageSize]);

  const connectionIds = useMemo(() => (rootDataBookings.bookings ? [rootDataBookings.bookings.__id] : []), [rootDataBookings.bookings]);
  const bookings = useMemo(() => {
    if (!rootDataBookings.bookings) {
      return [];
    }

    const bookingEdges = rootDataBookings.bookings.edges;
    const slicedEdges = bookingEdges.slice(
      page * pageSize,
      page * pageSize + pageSize > bookingEdges.length ? bookingEdges.length : page * pageSize + pageSize,
    );

    return slicedEdges.map(({ node }) => node);
  }, [page, pageSize, rootDataBookings.bookings]);

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as BookingOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as BookingOrderField,
      },
      startWeek,
    );
  };

  const handleWeehChange = (date: Dayjs) => {
    setStartWeek(date);

    handleRefetch(pageSize, sortingOrder, date);
  };

  if (!rootData.me || !rootDataBookings.bookings) {
    return <></>;
  }

  return (
    <>
      <Stack direction="column" spacing={1}>
        <Stack direction="row" sx={{ width: 'auto', justifyContent: 'space-between' }}>
          <Stack direction="row">
            <WeekPicker startWeek={startWeek} onWeekChanged={handleWeehChange} />
          </Stack>
          <NewBookingButton
            onReloadRequired={onReloadRequired}
            organizationId={organizationId}
            locationId={locationId}
            connectionIds={connectionIds}
            hideLocationControl={true}
            hideOrganizationControl={true}
            defaultDate={startWeek}
          />
        </Stack>

        <Stack direction="row" sx={{ justifyContent: 'flex-end' }}>
          <TablePagination
            count={rootDataBookings.bookings.totalCount ? rootDataBookings.bookings.totalCount : 0}
            page={page}
            onPageChange={handleChangePage}
            rowsPerPage={pageSize}
            onRowsPerPageChange={handlePageSizeChange}
          />
          <Sorting
            options={[
              { id: 'from', label: 'Booking date' },
              { id: 'name', label: 'Name' },
              { id: 'givenName', label: 'Given Name' },
              { id: 'middleName', label: 'Middle Name' },
              { id: 'familyName', label: 'Family Name' },
              { id: 'organizationName', label: 'Organization' },
              { id: 'teamName', label: 'Team' },
            ]}
            defaultOption={sortingOrder.field}
            defaultSortingDirectionValue={sortingOrder.direction as unknown as Direction}
            onValueChange={handleSortingChanged}
          />
        </Stack>

        <Grid container spacing={1}>
          {bookings.map((booking) => {
            const canJoinBooking =
              booking.customer.uniqueId === rootData.me?.id
                ? false
                : !!!bookings
                    .filter((otherBooking) => otherBooking.customer.uniqueId === rootData.me?.id)
                    .find((myBooking) => {
                      const from = dayjs(booking.from);
                      const myFrom = dayjs(myBooking.from);

                      return from.year() === myFrom.year() && from.month() === myFrom.month() && from.date() === myFrom.date();
                    });

            return (
              <Grid key={booking.id}>
                <BookingCard
                  rootDataRelay={rootData}
                  bookingDetailsRelay={booking}
                  connectionIds={connectionIds}
                  hideOrganizationControl={true}
                  hideLocationControl={true}
                  canJoinBooking={canJoinBooking}
                />
              </Grid>
            );
          })}
        </Grid>
      </Stack>
    </>
  );
};

const MemoBookings = memo(Bookings);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId?: string;
  locationId?: string;
  teamId?: string;
};

const BookingsWithRelay = ({ onReloadRequired, organizationId, locationId, teamId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<bookings_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const from = startOfWeek();
    const to = endOfWeek(from).add(-1, 'milliseconds');

    loadQuery(
      {
        organizationId: organizationId ?? '',
        organizationExists: !!organizationId,
        locationId: locationId ?? '',
        locationExists: !!locationId,
        teamId: teamId ?? '',
        deskIdsToIncludeToGetAvailableDesks: [],
        bookingSortingValues: [
          {
            direction: 'Ascending',
            field: 'from',
          },
        ],
        bookingDetailsSelectorOrganizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        bookingsSearchCriteriaFrom: from.toISOString(),
        bookingsSearchCriteriaTo: to.toISOString(),
        dateToGetAvailableDesks: startOfDay().toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId, locationId, teamId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoBookings queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} locationId={locationId} />
    </ErrorBoundary>
  );
};

export default memo(BookingsWithRelay);
