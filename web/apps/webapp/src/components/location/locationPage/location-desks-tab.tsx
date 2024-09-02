import { BulkNewDeskDialog, DeskCard, NewDeskDialog } from '@/components/desk';
import type { DeskOrderField, DeskOrderInput, desksPaginationQuery } from '@/queries/__generated__/desksPaginationQuery.graphql';
import type { locationDesksTab_query$key } from '@/queries/__generated__/locationDesksTab_query.graphql';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import TablePagination from '@mui/material/TablePagination';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { StaticDatePicker } from '@mui/x-date-pickers/StaticDatePicker';
import { EmptyCalendarToolbar, SimpleCalendarSlotProps } from '@repo/shared/components/generics';
import { AddIcon } from '@repo/shared/components/icons';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { endOfDay, keyboardDebounceTimeout, startOfDay, toShortDate } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import debounce from 'lodash.debounce';
import { memo, startTransition, useCallback, useMemo, useState } from 'react';
import { graphql, usePaginationFragment } from 'react-relay';

type Props = {
  rootDataRelay: locationDesksTab_query$key;
  locationId: string;
};

const LocationDesksTab = ({ rootDataRelay, locationId }: Props) => {
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<desksPaginationQuery, locationDesksTab_query$key>(
    graphql`
      fragment locationDesksTab_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "desksPaginationQuery") {
        location(id: $locationId) {
          canModify
        }
        paginatedLocationDesks(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $deskNameSearchText }
          orderBy: $deskSortingValues
        ) @connection(key: "locationDesksTab_paginatedLocationDesks") {
          __id
          totalCount
          edges {
            node {
              id
              ...deskCard_DeskDetails
            }
          }
        }
        ...deskCard_query
        ...deskMultipleChoicesZones_query
        allBookings(where: { locationIds: [$locationId], fromGTE: $fromToGetBookings, toLTE: $toToGetBookings }) {
          id
          customer {
            uniqueId
            name
            givenName
            middleName
            familyName
            photoUrl
          }
          desks {
            uniqueId
          }
        }
        ...newDeskDialog_query
        ...bulkNewDeskDialog_query
      }
    `,
    rootDataRelay,
  );

  const [sortingOrder, setSortingOrder] = useState<DeskOrderInput>({
    direction: 'Ascending',
    field: 'name',
  });

  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(startOfDay(null));

  const handleChangePage = (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    if (newPage > page) {
      loadNextPage();
    }

    setPage(newPage);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const pageSize = parseInt(event.target.value, 10);

    setPageSize(parseInt(event.target.value, 10));

    handleRefetch(pageSize, sortingOrder, selectedDate, deskNameSearchText);
  };

  const handleSelectedDateChange = (date: Dayjs | null) => {
    setSelectedDate(date);

    handleRefetch(pageSize, sortingOrder, date, deskNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: DeskOrderInput, date: Dayjs | null, deskNameSearchText: string) => {
      let fromToGetBookings: string | null = null;
      let toToGetBookings: string | null = null;

      if (date) {
        fromToGetBookings = startOfDay(date).toISOString();
        toToGetBookings = endOfDay(date).toISOString();
      }

      startTransition(() => {
        refetch(
          {
            count: pageSize,
            deskSortingValues: [order],
            locationId,
            fromToGetBookings,
            toToGetBookings,
            deskNameSearchText,
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
    [refetch, locationId],
  );

  const loadNextPage = useCallback(() => {
    if (isLoadingNext) {
      return;
    }

    loadNext(pageSize);
  }, [loadNext, isLoadingNext, pageSize]);

  const [deskNameSearchText, setDeskNameSearchText] = useState<string>('');
  const handleSearchTextChange = (str: string) => {
    setDeskNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, selectedDate, str);
  };

  const debounceSearchTextChange = debounce(handleSearchTextChange, keyboardDebounceTimeout);
  const connectionIds = useMemo(
    () => (rootData.paginatedLocationDesks ? [rootData.paginatedLocationDesks.__id] : []),
    [rootData.paginatedLocationDesks],
  );
  const [isAddDeskDialogOpen, setIsAddDeskDialogOpen] = useState(false);
  const [isBulkAddDeskDialogOpen, setIsBulkAddDeskDialogOpen] = useState(false);
  const [pageContextOpen, setPageContextOpen] = useState(false);

  if (!rootData.location) {
    return <></>;
  }

  const desks = rootData.paginatedLocationDesks.edges;
  const slicedEdges = desks.slice(page * pageSize, page * pageSize + pageSize > desks.length ? desks.length : page * pageSize + pageSize);

  const handlePageContextOpenStateChange = (event: React.SyntheticEvent, isExpanded: boolean) => {
    if (isExpanded) {
      setPageContextOpen(true);
    } else {
      setPageContextOpen(false);
    }
  };

  const handleAddDeskClick = () => {
    setIsAddDeskDialogOpen(true);
  };

  const handleAddDeskDialogAddClick = () => {
    setIsAddDeskDialogOpen(false);

    handleRefetch(pageSize, sortingOrder, selectedDate, deskNameSearchText);
  };

  const handleAddDeskDialogCancelClick = () => {
    setIsAddDeskDialogOpen(false);
  };

  const handleBulkAddDeskClick = () => {
    setIsBulkAddDeskDialogOpen(true);
  };

  const handleBulkAddDeskDialogAddClick = () => {
    setIsBulkAddDeskDialogOpen(false);

    handleRefetch(pageSize, sortingOrder, selectedDate, deskNameSearchText);
  };

  const handleBulkAddDeskDialogCancelClick = () => {
    setIsBulkAddDeskDialogOpen(false);
  };

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as DeskOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as DeskOrderField,
      },
      selectedDate,
      deskNameSearchText,
    );
  };

  return (
    <>
      {rootData.location.canModify && (
        <Grid container sx={{ justifyContent: 'flex-start', marginTop: 1 }}>
          <Grid sx={{ marginRight: 1 }}>
            <Button variant="contained" startIcon={<AddIcon />} onClick={handleAddDeskClick}>
              Add Desk
            </Button>
          </Grid>
          <Grid sx={{ marginRight: 1 }}>
            <Button variant="contained" startIcon={<AddIcon />} onClick={handleBulkAddDeskClick}>
              Bulk Add Desk
            </Button>
          </Grid>
        </Grid>
      )}

      <Grid sx={{ marginTop: 1 }}>
        <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen}>
          <AccordionSummary expandIcon={<ExpandMoreIcon />}>
            {!pageContextOpen && <Typography>{toShortDate(selectedDate)}</Typography>}
          </AccordionSummary>
          <AccordionDetails>
            <StaticDatePicker
              slots={{
                toolbar: EmptyCalendarToolbar,
              }}
              // @ts-expect-error
              slotProps={SimpleCalendarSlotProps}
              // @ts-expect-error
              defaultValue={selectedDate}
              onChange={handleSelectedDateChange}
            />
            <TextField
              defaultValue={deskNameSearchText}
              helperText="Enter desk or zone name to narrow down the desks list"
              onChange={(event) => debounceSearchTextChange(event?.target.value)}
            />
          </AccordionDetails>
        </Accordion>
      </Grid>

      <Grid container sx={{ justifyContent: 'flex-end' }}>
        <Grid>
          <TablePagination
            count={rootData.paginatedLocationDesks.totalCount ? rootData.paginatedLocationDesks.totalCount : 0}
            page={page}
            onPageChange={handleChangePage}
            rowsPerPage={pageSize}
            onRowsPerPageChange={handlePageSizeChange}
          />
        </Grid>
        <Grid>
          <Sorting
            options={[{ id: 'name', label: 'Name' }]}
            // @ts-expect-error
            defaultOption={sortingOrder.field}
            defaultSortingDirectionValue={sortingOrder.direction as unknown as Direction}
            onValueChange={handleSortingChanged}
          />
        </Grid>
      </Grid>
      <Grid container spacing={{ xs: 2, md: 3 }}>
        {slicedEdges.map((edge) => {
          const foundBooking = rootData.allBookings.find((booking) => booking.desks.find(({ uniqueId }) => uniqueId === edge.node.id));

          return (
            <Grid key={edge.node.id}>
              <DeskCard
                rootDataRelay={rootData}
                deskMultipleChoicesZonesData={rootData}
                deskDetailsRelay={edge.node}
                connectionIds={connectionIds}
                customerDetails={foundBooking ? foundBooking.customer : null}
              />
            </Grid>
          );
        })}
      </Grid>
      <NewDeskDialog
        rootDataRelay={rootData}
        connectionIds={connectionIds}
        isDialogOpen={isAddDeskDialogOpen}
        onAddClicked={handleAddDeskDialogAddClick}
        onCancelClicked={handleAddDeskDialogCancelClick}
        locationId={locationId}
      />
      <BulkNewDeskDialog
        rootDataRelay={rootData}
        connectionIds={connectionIds}
        isDialogOpen={isBulkAddDeskDialogOpen}
        onAddClicked={handleBulkAddDeskDialogAddClick}
        onCancelClicked={handleBulkAddDeskDialogCancelClick}
        locationId={locationId}
      />
    </>
  );
};

export default memo(LocationDesksTab);
