import { BookingCard } from '@/components/booking';
import { NewBookingButton } from '@/components/booking/addBooking';
import type { locationBookingsTab_bookings_query$key } from '@/queries/__generated__/locationBookingsTab_bookings_query.graphql';
import type {
  BookingOrderField,
  BookingOrderInput,
  locationBookingsTab_bookings_refetchableFragment,
} from '@/queries/__generated__/locationBookingsTab_bookings_refetchableFragment.graphql';
import type { locationBookingsTab_query$key } from '@/queries/__generated__/locationBookingsTab_query.graphql';
import type { locationBookingsTab_rootQuery } from '@/queries/__generated__/locationBookingsTab_rootQuery.graphql';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import Typography from '@mui/material/Typography';
import { DateRangePicker } from '@mui/x-date-pickers-pro/DateRangePicker';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { startOfDay, toShortDate } from '@repo/shared/libs/utils';
import dayjs, { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, useFragment, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<locationBookingsTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
  locationId: string;
};

const RootQuery = graphql`
  query locationBookingsTab_rootQuery(
    $organizationId: String!
    $organizationExists: Boolean!
    $locationId: String!
    $locationExists: Boolean!
    $dateToGetAvailableDesks: DateTime!
    $deskIdsToIncludeToGetAvailableDesks: [String!]!
    $bookingPeopleNameSearchText: String
    $bookingSortingValues: [BookingOrderInput!]!
    $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
  ) {
    ...locationBookingsTab_query
    ...locationBookingsTab_bookings_query
  }
`;

const LocationBookingsTab = ({ queryReference, onReloadRequired, organizationId, locationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<locationBookingsTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<locationBookingsTab_query$key>(
    graphql`
      fragment locationBookingsTab_query on Query {
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
  } = usePaginationFragment<locationBookingsTab_bookings_refetchableFragment, locationBookingsTab_bookings_query$key>(
    graphql`
      fragment locationBookingsTab_bookings_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "locationBookingsTab_bookings_refetchableFragment") {
        bookings(
          first: $count
          after: $cursor
          where: { locationIds: [$locationId], fromGTE: $bookingsSearchCriteriaFrom, fromLTE: $bookingsSearchCriteriaTo, includeMineOnly: false }
          orderBy: $bookingSortingValues
        ) @connection(key: "locationBookingsTab_bookings") {
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
  const [pageSize, setPageSize] = useState(50);
  const [pageContextOpen, setPageContextOpen] = useState(false);
  const [selectedFromDate, setSelectedFromDate] = useState<Dayjs | null>(startOfDay());
  const [selectedUntilDate, setSelectedUntilDate] = useState<Dayjs | null>(startOfDay().add(1, 'month'));

  const handleChangePage = (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    if (newPage > page) {
      loadNextPage();
    }

    setPage(newPage);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const pageSize = parseInt(event.target.value, 10);

    setPageSize(parseInt(event.target.value, 10));

    handleRefetch(pageSize, sortingOrder, selectedFromDate, selectedUntilDate);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: BookingOrderInput, from: Dayjs | null, until: Dayjs | null) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            bookingSortingValues: [order],
            bookingsSearchCriteriaFrom: from && from.isValid() ? from.toISOString() : null,
            bookingsSearchCriteriaTo: until && until.isValid() ? until.toISOString() : null,
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
      selectedFromDate,
      selectedUntilDate,
    );
  };

  const handleSelectedDateChange = (from: Dayjs | null, until: Dayjs | null) => {
    setSelectedFromDate(from);
    setSelectedUntilDate(until);

    handleRefetch(pageSize, sortingOrder, from, until);
  };

  const handlePageContextOpenStateChange = (event: React.SyntheticEvent, isExpanded: boolean) => {
    if (isExpanded) {
      setPageContextOpen(true);
    } else {
      setPageContextOpen(false);
    }
  };

  if (!rootData.me || !rootDataBookings.bookings) {
    return <></>;
  }

  return (
    <>
      <Stack direction="column" spacing={1}>
        <Stack direction="row" sx={{ width: 'auto' }}>
          <NewBookingButton
            onReloadRequired={onReloadRequired}
            organizationId={organizationId}
            locationId={locationId}
            connectionIds={connectionIds}
            hideLocationControl={true}
            hideOrganizationControl={true}
          />
        </Stack>

        <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen} sx={{ width: '100%' }}>
          <AccordionSummary expandIcon={<ExpandMoreIcon />}>
            {!pageContextOpen && <Typography>{`From ${toShortDate(selectedFromDate)} until ${toShortDate(selectedUntilDate)}`}</Typography>}
          </AccordionSummary>
          <AccordionDetails>
            <DateRangePicker
              localeText={{ start: 'From', end: 'To' }}
              defaultValue={[selectedFromDate, selectedUntilDate]}
              onChange={(dateRangeValue) => handleSelectedDateChange(dateRangeValue[0], dateRangeValue[1])}
            />
          </AccordionDetails>
        </Accordion>

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

const MemoLocationBookingsTab = memo(LocationBookingsTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId?: string;
  locationId: string;
};

const LocationBookingsTabWithRelay = ({ onReloadRequired, organizationId, locationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationBookingsTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const from = startOfDay().toISOString();
    const to = startOfDay().add(1, 'month').toISOString();

    loadQuery(
      {
        locationId,
        locationExists: !!locationId,
        deskIdsToIncludeToGetAvailableDesks: [],
        organizationId: organizationId ?? '',
        organizationExists: !!organizationId,
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
        bookingsSearchCriteriaFrom: from,
        bookingsSearchCriteriaTo: to,
        dateToGetAvailableDesks: from,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId, locationId]);

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
      <MemoLocationBookingsTab
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationId={organizationId}
        locationId={locationId}
      />
    </ErrorBoundary>
  );
};

export default memo(LocationBookingsTabWithRelay);
