import Grid from '@mui/material/Grid2';
import TablePagination from '@mui/material/TablePagination';
import { GridContainer, PushToRight, StackRow } from '@repo/shared/components/commons';
import { WeekPicker } from '@repo/shared/components/datePickers';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { endOfWeek, startOfDay, startOfWeek } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OldBookingCard } from 'components/booking';
import { NewBookingButton } from 'components/booking/addBooking';
import dayjs, { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, useFragment, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { oldBookings_bookings_query$key } from './__generated__/oldBookings_bookings_query.graphql';
import type {
  BookingOrderField,
  BookingOrderInput,
  oldBookings_bookings_refetchableFragment,
} from './__generated__/oldBookings_bookings_refetchableFragment.graphql';
import type { oldBookings_query$key } from './__generated__/oldBookings_query.graphql';
import type { oldBookings_rootQuery } from './__generated__/oldBookings_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<oldBookings_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  teamId?: string;
};

const RootQuery = graphql`
  query oldBookings_rootQuery(
    $organizationId: String!
    $nullableOrganizationId: String
    $locationId: String!
    $locationExists: Boolean!
    $teamId: String!
    $teamExists: Boolean!
    $dateToGetAvailableDesks: DateTime!
    $deskIdsToIncludeToGetAvailableDesks: [String!]!
    $bookingPeopleNameSearchText: String
    $bookingSortingValues: [BookingOrderInput!]
    $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
    $peopleNameSearchText: String
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    ...oldBookings_query
    ...oldBookings_bookings_query
  }
`;

const OldBookings = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<oldBookings_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<oldBookings_query$key>(
    graphql`
      fragment oldBookings_query on Query {
        me {
          id
        }
        organization(id: $organizationId) {
          id
          name
        }
        location(id: $locationId) @include(if: $locationExists) {
          id
          name
        }
        team(id: $teamId) @include(if: $teamExists) {
          id
          name
        }
        ...oldBookingCard_query
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
  } = usePaginationFragment<oldBookings_bookings_refetchableFragment, oldBookings_bookings_query$key>(
    graphql`
      fragment oldBookings_bookings_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "oldBookings_bookings_refetchableFragment") {
        bookings(
          first: $count
          after: $cursor
          where: {
            organizationIds: [$organizationId]
            locationIds: [$locationId]
            teamIds: [$teamId]
            fromGTE: $bookingsSearchCriteriaFrom
            fromLTE: $bookingsSearchCriteriaTo
            nameContains: $peopleNameSearchText
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
              ...oldBookingCard_BookingDetails
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
    field: 'From',
  });
  const [page, setPage] = useState(0);
  const [startWeek, setStartWeek] = useState(startOfWeek());
  const [pageSize, setPageSize] = useState(50);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');

  const handleChangePage = (_: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    if (newPage > page) {
      loadNextPage();
    }

    setPage(newPage);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const pageSize = parseInt(event.target.value, 10);

    setPageSize(parseInt(event.target.value, 10));

    handleRefetch(pageSize, sortingOrder, startWeek, peopleNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: BookingOrderInput, date: Dayjs, peopleNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            bookingSortingValues: [order],
            bookingsSearchCriteriaFrom: date.toISOString(),
            bookingsSearchCriteriaTo: endOfWeek(date).add(-1, 'milliseconds').toISOString(),
            peopleNameSearchText,
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
      peopleNameSearchText,
    );
  };

  const handleWeehChange = (date: Dayjs) => {
    setStartWeek(date);

    handleRefetch(pageSize, sortingOrder, date, peopleNameSearchText);
  };

  const handleSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, startWeek, str);
  };

  if (!rootData.me || !rootDataBookings.bookings) {
    return <></>;
  }

  return (
    <>
      <StackRow>
        <NewBookingButton onReloadRequired={onReloadRequired} organizationId={organizationId} connectionIds={connectionIds} defaultDate={startWeek} />
        <WeekPicker defaultStartWeek={startWeek} onWeekChanged={handleWeehChange} />
        <Search size="small" placeholder="Search for members" defaultValue={peopleNameSearchText} onChange={handleSearchTextChange} />
        <PushToRight />
        <TablePagination
          component="div"
          count={rootDataBookings.bookings.totalCount ? rootDataBookings.bookings.totalCount : 0}
          page={page}
          onPageChange={handleChangePage}
          rowsPerPage={pageSize}
          onRowsPerPageChange={handlePageSizeChange}
        />
        <Sorting
          options={[
            { id: 'From', label: 'Booking date' },
            { id: 'Name', label: 'Name' },
            { id: 'GivenName', label: 'Given Name' },
            { id: 'MiddleName', label: 'Middle Name' },
            { id: 'FamilyName', label: 'Family Name' },
            { id: 'OrganizationName', label: 'Organization' },
            { id: 'TeamName', label: 'Team' },
          ]}
          defaultOption={sortingOrder.field}
          defaultSortingDirectionValue={sortingOrder.direction as unknown as Direction}
          onValueChange={handleSortingChanged}
        />
      </StackRow>

      <GridContainer>
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
              <OldBookingCard
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
      </GridContainer>
    </>
  );
};

const MemoBookings = memo(OldBookings);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
  locationId?: string;
  teamId?: string;
};

const BookingsWithRelay = ({ onReloadRequired, organizationId, locationId, teamId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<oldBookings_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const from = startOfWeek();
    const to = endOfWeek(from).add(-1, 'milliseconds');

    loadQuery(
      {
        organizationId,
        nullableOrganizationId: organizationId,
        locationId: locationId ?? '',
        locationExists: !!locationId,
        teamId: teamId ?? '',
        teamExists: !!teamId,
        deskIdsToIncludeToGetAvailableDesks: [],
        bookingSortingValues: [
          {
            direction: 'Ascending',
            field: 'From',
          },
        ],
        bookingDetailsSelectorOrganizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        bookingsSearchCriteriaFrom: from.toISOString(),
        bookingsSearchCriteriaTo: to.toISOString(),
        dateToGetAvailableDesks: startOfDay().toISOString(),
        locationsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
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
      <MemoBookings queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(BookingsWithRelay);
