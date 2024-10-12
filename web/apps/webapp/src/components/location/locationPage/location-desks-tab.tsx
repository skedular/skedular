import { BulkNewDeskDialog, DeskCard, NewDeskDialog } from '@/components/desk';
import type { DeskOrderField, DeskOrderInput, desks_PaginationQuery } from '@/queries/__generated__/desks_PaginationQuery.graphql';
import type { locationDesksTab_query$key } from '@/queries/__generated__/locationDesksTab_query.graphql';
import type { locationDesksTab_rootQuery } from '@/queries/__generated__/locationDesksTab_rootQuery.graphql';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { StaticDatePicker } from '@mui/x-date-pickers/StaticDatePicker';
import { EmptyCalendarToolbar, SimpleCalendarSlotProps } from '@repo/shared/components/generics';
import { AddIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { TAG_TYPE_LOCATION_ZONE } from '@repo/shared/components/zone';
import { endOfDay, keyboardDebounceTimeout, startOfDay, toShortDate } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import debounce from 'lodash.debounce';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<locationDesksTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  locationId: string;
};

const RootQuery = graphql`
  query locationDesksTab_rootQuery(
    $locationId: String!
    $locationExists: Boolean!
    $zoneTagType: String!
    $fromToGetBookings: DateTime
    $toToGetBookings: DateTime
    $deskNameSearchText: String
    $deskSortingValues: [DeskOrderInput!]!
    $deskMultipleChoicesZonesSortingValues: [LocationTagOrderInput!]
  ) {
    ...locationDesksTab_query
  }
`;

const LocationDesksTab = ({ queryReference, onReloadRequired, locationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<locationDesksTab_rootQuery>(RootQuery, queryReference);
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<desks_PaginationQuery, locationDesksTab_query$key>(
    graphql`
      fragment locationDesksTab_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "desks_PaginationQuery") {
        location(id: $locationId) {
          canModify
        }
        paginatedLocationDesks(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $deskNameSearchText }
          orderBy: $deskSortingValues
        ) @connection(key: "locationDesksTab_paginatedLocationDesks") @include(if: $locationExists) {
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

  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<DeskOrderInput>({
    direction: 'Ascending',
    field: 'name',
  });

  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(startOfDay());

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
            locationExists: !!locationId,
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

  if (!rootData.location || !rootData.paginatedLocationDesks) {
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
        <Stack direction="row" sx={{ justifyContent: 'flex-start' }} spacing={1}>
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleAddDeskClick}>
            Add Desk
          </Button>
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleBulkAddDeskClick}>
            Bulk Add Desk
          </Button>
        </Stack>
      )}

      <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen} sx={{ width: '100%' }}>
        <AccordionSummary expandIcon={<ExpandMoreIcon />}>
          {!pageContextOpen && <Typography>{toShortDate(selectedDate)}</Typography>}
        </AccordionSummary>
        <AccordionDetails>
          <StaticDatePicker
            slots={{
              toolbar: EmptyCalendarToolbar,
            }}
            slotProps={SimpleCalendarSlotProps}
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

      <Stack direction="row" sx={{ justifyContent: 'flex-end' }}>
        <TablePagination
          count={rootData.paginatedLocationDesks.totalCount ? rootData.paginatedLocationDesks.totalCount : 0}
          page={page}
          onPageChange={handleChangePage}
          rowsPerPage={pageSize}
          onRowsPerPageChange={handlePageSizeChange}
        />
        <Sorting
          options={[{ id: 'name', label: 'Name' }]}
          defaultOption={sortingOrder.field}
          defaultSortingDirectionValue={sortingOrder.direction as unknown as Direction}
          onValueChange={handleSortingChanged}
        />
      </Stack>

      <Grid container spacing={1}>
        {slicedEdges.map((edge) => {
          const foundBooking = rootData.allBookings?.find((booking) => booking.desks.find(({ uniqueId }) => uniqueId === edge.node.id));

          return (
            <Grid key={edge.node.id}>
              <DeskCard
                rootDataRelay={rootData}
                deskMultipleChoicesZonesData={rootData}
                deskDetailsRelay={edge.node}
                connectionIds={connectionIds}
                customerDetails={foundBooking ? foundBooking.customer : null}
                locationId={locationId}
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

const MemoLocationDesksTab = memo(LocationDesksTab);

type RelayProps = {
  onReloadRequired: () => void;
  locationId: string;
};

const LocationDesksTabWithRelay = ({ onReloadRequired, locationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationDesksTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const from = startOfDay().toISOString();
    const to = endOfDay(from).toISOString();

    loadQuery(
      {
        locationId,
        locationExists: !!locationId,
        zoneTagType: TAG_TYPE_LOCATION_ZONE,
        fromToGetBookings: from,
        toToGetBookings: to,
        deskSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        deskMultipleChoicesZonesSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, locationId]);

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
      <MemoLocationDesksTab queryReference={queryReference} onReloadRequired={handleReloadRequired} locationId={locationId} />
    </ErrorBoundary>
  );
};

export default memo(LocationDesksTabWithRelay);
