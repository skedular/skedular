import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import Typography from '@mui/material/Typography';
import { DateRangePicker } from '@mui/x-date-pickers-pro/DateRangePicker';
import { AddIcon } from '@repo/shared/components/icons';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { startOfDay, toShortDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { BookingCard } from 'components/booking';
import { NewBookingDialog } from 'components/booking/addBooking';
import dayjs, { Dayjs } from 'dayjs';
import { memo, useCallback, useMemo, useState, useTransition } from 'react';
import { usePaginationFragment } from 'react-relay';
import type {
  BookingOrderField,
  BookingOrderInput,
  locationBookings_PaginationQuery,
} from './__generated__/locationBookings_PaginationQuery.graphql';
import type { locationBookingsTab_query$key } from './__generated__/locationBookingsTab_query.graphql';

type Props = {
  rootDataRelay: locationBookingsTab_query$key;
  organizationId: string;
  locationId: string;
};

const LocationBookingsTab = ({ rootDataRelay, organizationId, locationId }: Props) => {
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<locationBookings_PaginationQuery, locationBookingsTab_query$key>(
    graphql`
      fragment locationBookingsTab_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "locationBookings_PaginationQuery") {
        bookings(
          first: $count
          after: $cursor
          where: { locationIds: [$locationId], fromGTE: $bookingsSearchCriteriaFrom, fromLTE: $bookingsSearchCriteriaUntil, includeMineOnly: false }
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
        me {
          id
        }
        ...bookingCard_query
        ...newBookingDialog_query
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<BookingOrderInput>({
    direction: 'Ascending',
    field: 'from',
  });
  const [isAddBookingDialogOpen, setIsAddBookingDialogOpen] = useState(false);
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
            bookingsSearchCriteriaUntil: until && until.isValid() ? until.toISOString() : null,
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

  const connectionIds = useMemo(() => [rootData.bookings.__id], [rootData.bookings]);

  const bookings = useMemo(() => {
    const bookingEdges = rootData.bookings.edges;
    const slicedEdges = bookingEdges.slice(
      page * pageSize,
      page * pageSize + pageSize > bookingEdges.length ? bookingEdges.length : page * pageSize + pageSize,
    );

    return slicedEdges.map(({ node }) => node);
  }, [page, pageSize, rootData.bookings.edges]);

  const handleAddBookingClick = () => {
    setIsAddBookingDialogOpen(true);
  };

  const handleAddBookingDialogAddClick = () => {
    setIsAddBookingDialogOpen(false);

    handleRefetch(pageSize, sortingOrder, selectedFromDate, selectedUntilDate);
  };

  const handleAddBookingDialogCancelClick = () => {
    setIsAddBookingDialogOpen(false);
  };

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

  if (!rootData.me) {
    return <></>;
  }

  return (
    <>
      <Stack direction="column" spacing={1}>
        <Stack direction="row" sx={{ width: 'auto' }}>
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleAddBookingClick}>
            Add Booking
          </Button>
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
            count={rootData.bookings.totalCount ? rootData.bookings.totalCount : 0}
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

      <NewBookingDialog
        rootDataRelay={rootData}
        connectionIds={connectionIds}
        isDialogOpen={isAddBookingDialogOpen}
        onAddClicked={handleAddBookingDialogAddClick}
        onCancelClicked={handleAddBookingDialogCancelClick}
        organizationId={organizationId}
        locationId={locationId}
        defaultTeamId={null}
        hideOrganizationControl={true}
        hideLocationControl={true}
      />
    </>
  );
};

export default memo(LocationBookingsTab);
