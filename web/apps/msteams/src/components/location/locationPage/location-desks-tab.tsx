import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import { DayPicker } from '@repo/shared/components/datePickers';
import { AddIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { LOCATION_TAG_TYPE_LOCATION_ZONE } from '@repo/shared/components/zone';
import { endOfDay, startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { BulkNewDeskDialog, DeskCard, NewDeskDialog } from 'components/desk';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, useFragment, usePaginationFragment, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import type { locationDesksTab_allBookings_query$key } from './__generated__/locationDesksTab_allBookings_query.graphql';
import type { locationDesksTab_allBookings_refetchableFragment } from './__generated__/locationDesksTab_allBookings_refetchableFragment.graphql';
import type { locationDesksTab_locationDesks_query$key } from './__generated__/locationDesksTab_locationDesks_query.graphql';
import type {
  DeskOrderField,
  DeskOrderInput,
  locationDesksTab_locationDesks_refetchableFragment,
} from './__generated__/locationDesksTab_locationDesks_refetchableFragment.graphql';
import type { locationDesksTab_query$key } from './__generated__/locationDesksTab_query.graphql';
import type { locationDesksTab_rootQuery } from './__generated__/locationDesksTab_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<locationDesksTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  locationId: string;
};

const RootQuery = graphql`
  query locationDesksTab_rootQuery(
    $organizationId: String!
    $locationId: String!
    $zoneTagType: String!
    $fromToGetBookings: DateTime
    $toToGetBookings: DateTime
    $deskNameSearchText: String
    $deskSortingValues: [DeskOrderInput!]!
    $deskMultipleChoicesZonesSortingValues: [LocationTagOrderInput!]
    $deskMultipleChoicesDeskTypesSortingValues: [OrganizationTagOrderInput!]
  ) {
    ...locationDesksTab_query
    ...locationDesksTab_locationDesks_query
    ...locationDesksTab_allBookings_query
  }
`;

const LocationDesksTab = ({ queryReference, onReloadRequired, locationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<locationDesksTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<locationDesksTab_query$key>(
    graphql`
      fragment locationDesksTab_query on Query {
        location(id: $locationId) {
          canModify
        }
        ...deskCard_query
        ...deskMultipleChoicesZones_query
        ...deskMultipleChoicesDeskTypes_query
        ...newDeskDialog_query
        ...bulkNewDeskDialog_query
      }
    `,
    rootDataRelay,
  );
  const {
    data: rootDataRefetchPaginatedLocationDesks,
    loadNext: loadNextRefetchPaginatedLocationDesks,
    isLoadingNext: isLoadingNextrefetchPaginatedLocationDesks,
    refetch: refetchPaginatedLocationDesks,
  } = usePaginationFragment<locationDesksTab_locationDesks_refetchableFragment, locationDesksTab_locationDesks_query$key>(
    graphql`
      fragment locationDesksTab_locationDesks_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "locationDesksTab_locationDesks_refetchableFragment") {
        locationDesks(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $deskNameSearchText }
          orderBy: $deskSortingValues
        ) @connection(key: "locationDesksTab_locationDesks") {
          __id
          totalCount
          edges {
            node {
              id
              ...deskCard_DeskDetails
            }
          }
        }
      }
    `,
    rootDataRelay,
  );
  const [rootDatarefetchAllBookings, refetchAllBookings] = useRefetchableFragment<
    locationDesksTab_allBookings_refetchableFragment,
    locationDesksTab_allBookings_query$key
  >(
    graphql`
      fragment locationDesksTab_allBookings_query on Query @refetchable(queryName: "locationDesksTab_allBookings_refetchableFragment") {
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
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<DeskOrderInput>({
    direction: 'Ascending',
    field: 'Name',
  });

  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);

  const handleChangePage = (_: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    if (newPage > page) {
      loadNextPage();
    }

    setPage(newPage);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const pageSize = parseInt(event.target.value, 10);

    setPageSize(parseInt(event.target.value, 10));

    handleRefetchPaginatedLocationDesks(pageSize, sortingOrder, deskNameSearchText);
  };

  const handleSelectedDateChange = (date: Dayjs) => {
    handleRefetchAllBookings(date);
  };

  const handleRefetchPaginatedLocationDesks = useCallback(
    (pageSize: number, order: DeskOrderInput, deskNameSearchText: string) => {
      startTransition(() => {
        refetchPaginatedLocationDesks(
          {
            count: pageSize,
            deskSortingValues: [order],
            locationId,
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
    [refetchPaginatedLocationDesks, locationId],
  );

  const handleRefetchAllBookings = useCallback(
    (date: Dayjs | null) => {
      let fromToGetBookings: string | null = null;
      let toToGetBookings: string | null = null;

      if (date) {
        fromToGetBookings = startOfDay(date).toISOString();
        toToGetBookings = endOfDay(date).toISOString();
      }

      startTransition(() => {
        refetchAllBookings(
          {
            locationId,
            fromToGetBookings,
            toToGetBookings,
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
    [refetchAllBookings, locationId],
  );

  const loadNextPage = useCallback(() => {
    if (isLoadingNextrefetchPaginatedLocationDesks) {
      return;
    }

    loadNextRefetchPaginatedLocationDesks(pageSize);
  }, [loadNextRefetchPaginatedLocationDesks, isLoadingNextrefetchPaginatedLocationDesks, pageSize]);

  const [deskNameSearchText, setDeskNameSearchText] = useState<string>('');
  const handleSearchTextChange = (str: string) => {
    setDeskNameSearchText(str);

    handleRefetchPaginatedLocationDesks(pageSize, sortingOrder, str);
  };

  const connectionIds = useMemo(
    () => (rootDataRefetchPaginatedLocationDesks.locationDesks ? [rootDataRefetchPaginatedLocationDesks.locationDesks.__id] : []),
    [rootDataRefetchPaginatedLocationDesks.locationDesks],
  );
  const [isAddDeskDialogOpen, setIsAddDeskDialogOpen] = useState(false);
  const [isBulkAddDeskDialogOpen, setIsBulkAddDeskDialogOpen] = useState(false);

  if (!rootData.location || !rootDataRefetchPaginatedLocationDesks.locationDesks) {
    return <></>;
  }

  const desks = rootDataRefetchPaginatedLocationDesks.locationDesks.edges;
  const slicedEdges = desks.slice(page * pageSize, page * pageSize + pageSize > desks.length ? desks.length : page * pageSize + pageSize);

  const handleAddDeskClick = () => {
    setIsAddDeskDialogOpen(true);
  };

  const handleAddDeskDialogAddClick = () => {
    setIsAddDeskDialogOpen(false);

    handleRefetchPaginatedLocationDesks(pageSize, sortingOrder, deskNameSearchText);
  };

  const handleAddDeskDialogCancelClick = () => {
    setIsAddDeskDialogOpen(false);
  };

  const handleBulkAddDeskClick = () => {
    setIsBulkAddDeskDialogOpen(true);
  };

  const handleBulkAddDeskDialogAddClick = () => {
    setIsBulkAddDeskDialogOpen(false);

    handleRefetchPaginatedLocationDesks(pageSize, sortingOrder, deskNameSearchText);
  };

  const handleBulkAddDeskDialogCancelClick = () => {
    setIsBulkAddDeskDialogOpen(false);
  };

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as DeskOrderField,
    });

    handleRefetchPaginatedLocationDesks(
      pageSize,
      {
        direction,
        field: value as unknown as DeskOrderField,
      },
      deskNameSearchText,
    );
  };

  return (
    <>
      {rootData.location.canModify && (
        <Stack direction="row" sx={{ justifyContent: 'flex-start' }} spacing={1}>
          <Button variant="contained" size="small" startIcon={<AddIcon />} onClick={handleAddDeskClick}>
            Add Desk
          </Button>
          <Button variant="contained" size="small" startIcon={<AddIcon />} onClick={handleBulkAddDeskClick}>
            Bulk Add Desk
          </Button>
        </Stack>
      )}

      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap' }}>
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <Search size="small" placeholder="Find a desk..." defaultValue={deskNameSearchText} onChange={handleSearchTextChange} />
          <DayPicker onDateChanged={handleSelectedDateChange} />
        </Stack>
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <TablePagination
            count={
              rootDataRefetchPaginatedLocationDesks.locationDesks.totalCount ? rootDataRefetchPaginatedLocationDesks.locationDesks.totalCount : 0
            }
            page={page}
            onPageChange={handleChangePage}
            rowsPerPage={pageSize}
            onRowsPerPageChange={handlePageSizeChange}
          />
          <Sorting
            options={[{ id: 'Name', label: 'Name' }]}
            defaultOption={sortingOrder.field}
            defaultSortingDirectionValue={sortingOrder.direction as unknown as Direction}
            onValueChange={handleSortingChanged}
          />
        </Stack>
      </Stack>

      <Grid container spacing={1}>
        {slicedEdges.map((edge) => {
          const foundBooking = rootDatarefetchAllBookings.allBookings?.find((booking) =>
            booking.desks.find(({ uniqueId }) => uniqueId === edge.node.id),
          );

          return (
            <Grid key={edge.node.id}>
              <DeskCard
                rootDataRelay={rootData}
                deskMultipleChoicesZonesData={rootData}
                deskMultipleChoicesDeskTypesData={rootData}
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
  organizationId: string;
  locationId: string;
};

const LocationDesksTabWithRelay = ({ onReloadRequired, organizationId, locationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationDesksTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const from = startOfDay().toISOString();
    const to = endOfDay(from).toISOString();

    loadQuery(
      {
        organizationId,
        locationId,
        zoneTagType: LOCATION_TAG_TYPE_LOCATION_ZONE,
        fromToGetBookings: from,
        toToGetBookings: to,
        deskSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        deskMultipleChoicesZonesSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        deskMultipleChoicesDeskTypesSortingValues: [
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
      <MemoLocationDesksTab queryReference={queryReference} onReloadRequired={handleReloadRequired} locationId={locationId} />
    </ErrorBoundary>
  );
};

export default memo(LocationDesksTabWithRelay);
