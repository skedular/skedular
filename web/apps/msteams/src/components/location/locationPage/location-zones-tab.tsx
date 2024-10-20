import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import { AddIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { TAG_TYPE_LOCATION_ZONE } from '@repo/shared/components/zone';
import graphql from 'babel-plugin-relay/macro';
import { NewZoneDialog, ZoneCard } from 'components/zone';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, useFragment, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { locationZonesTab_paginatedLocationTags_query$key } from './__generated__/locationZonesTab_paginatedLocationTags_query.graphql';
import type {
  LocationTagOrderField,
  LocationTagOrderInput,
  locationZonesTab_paginatedLocationTags_refetchableFragment,
} from './__generated__/locationZonesTab_paginatedLocationTags_refetchableFragment.graphql';
import type { locationZonesTab_query$key } from './__generated__/locationZonesTab_query.graphql';
import type { locationZonesTab_rootQuery } from './__generated__/locationZonesTab_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<locationZonesTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  locationId: string;
};

const RootQuery = graphql`
  query locationZonesTab_rootQuery(
    $locationId: String!
    $locationExists: Boolean!
    $zoneTagType: String!
    $zoneNameSearchText: String
    $zoneSortingValues: [LocationTagOrderInput!]!
  ) {
    ...locationZonesTab_query
    ...locationZonesTab_paginatedLocationTags_query
  }
`;

const LocationZonesTab = ({ queryReference, onReloadRequired, locationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<locationZonesTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<locationZonesTab_query$key>(
    graphql`
      fragment locationZonesTab_query on Query {
        location(id: $locationId) {
          canModify
        }
        ...zoneCard_Query
      }
    `,
    rootDataRelay,
  );
  const {
    data: rootDataPaginatedLocationTags,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<locationZonesTab_paginatedLocationTags_refetchableFragment, locationZonesTab_paginatedLocationTags_query$key>(
    graphql`
      fragment locationZonesTab_paginatedLocationTags_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "locationZonesTab_paginatedLocationTags_refetchableFragment") {
        paginatedLocationTags(
          first: $count
          after: $cursor
          where: { locationId: $locationId, tagType: $zoneTagType, nameContains: $zoneNameSearchText }
          orderBy: $zoneSortingValues
        ) @connection(key: "locationZonesTab_paginatedLocationTags") @include(if: $locationExists) {
          __id
          totalCount
          edges {
            node {
              id
              ...zoneCard_LocationTagDetails
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<LocationTagOrderInput>({
    direction: 'Ascending',
    field: 'name',
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

    handleRefetch(pageSize, sortingOrder, zoneNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: LocationTagOrderInput, zoneNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            zoneSortingValues: [order],
            zoneNameSearchText,
            locationExists: !!locationId,
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

  const [zoneNameSearchText, setZoneNameSearchText] = useState<string>('');

  const handleSearchTextChange = (str: string) => {
    setZoneNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  const connectionIds = useMemo(
    () => (rootDataPaginatedLocationTags.paginatedLocationTags ? [rootDataPaginatedLocationTags.paginatedLocationTags.__id] : []),
    [rootDataPaginatedLocationTags.paginatedLocationTags],
  );
  const [isAddZoneDialogOpen, setIsAddZoneDialogOpen] = useState(false);

  if (!rootData.location || !rootDataPaginatedLocationTags.paginatedLocationTags) {
    return <></>;
  }

  const locationTagEdges = rootDataPaginatedLocationTags.paginatedLocationTags.edges;
  const slicedEdges = locationTagEdges.slice(
    page * pageSize,
    page * pageSize + pageSize > locationTagEdges.length ? locationTagEdges.length : page * pageSize + pageSize,
  );

  const handleAddZoneClick = () => {
    setIsAddZoneDialogOpen(true);
  };

  const handleAddZoneDialogAddClick = () => {
    setIsAddZoneDialogOpen(false);

    handleRefetch(pageSize, sortingOrder, zoneNameSearchText);
  };

  const handleAddZoneDialogCancelClick = () => {
    setIsAddZoneDialogOpen(false);
  };

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as LocationTagOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as LocationTagOrderField,
      },
      zoneNameSearchText,
    );
  };

  return (
    <>
      {rootData.location.canModify && (
        <Stack direction="row" sx={{ justifyContent: 'flex-start' }} spacing={1}>
          <Button variant="contained" size="small" startIcon={<AddIcon />} onClick={handleAddZoneClick}>
            Add Zone
          </Button>
        </Stack>
      )}

      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap' }}>
        <Search size="small" placeholder="Find a zone..." defaultValue={zoneNameSearchText} onChange={handleSearchTextChange} />
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <TablePagination
            count={
              rootDataPaginatedLocationTags.paginatedLocationTags.totalCount ? rootDataPaginatedLocationTags.paginatedLocationTags.totalCount : 0
            }
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
      </Stack>

      <Grid container spacing={1}>
        {slicedEdges.map((edge) => (
          <Grid key={edge.node.id}>
            <ZoneCard rootDataRelay={rootData} locationTagDetailsRelay={edge.node} connectionIds={connectionIds} />
          </Grid>
        ))}
      </Grid>

      <NewZoneDialog
        connectionIds={connectionIds}
        isDialogOpen={isAddZoneDialogOpen}
        onAddClicked={handleAddZoneDialogAddClick}
        onCancelClicked={handleAddZoneDialogCancelClick}
        locationId={locationId}
      />
    </>
  );
};

const MemoLocationZonesTab = memo(LocationZonesTab);

type RelayProps = {
  onReloadRequired: () => void;
  locationId: string;
};

const LocationZonesTabWithRelay = ({ onReloadRequired, locationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationZonesTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        locationId,
        locationExists: !!locationId,
        zoneTagType: TAG_TYPE_LOCATION_ZONE,
        zoneSortingValues: [
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
      <MemoLocationZonesTab queryReference={queryReference} onReloadRequired={handleReloadRequired} locationId={locationId} />
    </ErrorBoundary>
  );
};

export default memo(LocationZonesTabWithRelay);
