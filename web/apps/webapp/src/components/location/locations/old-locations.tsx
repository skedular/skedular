import { NewLocationButton } from '@/components/location/addLocation';
import { LocationBookingsCard } from '@/components/location/locationBookingCard';
import type { oldLocations_query$key } from '@/queries/__generated__/oldLocations_query.graphql';
import type {
  LocationOrderField,
  LocationOrderInput,
  oldLocations_refetchableFragment,
} from '@/queries/__generated__/oldLocations_refetchableFragment.graphql';
import type { oldLocations_rootQuery } from '@/queries/__generated__/oldLocations_rootQuery.graphql';
import Grid from '@mui/material/Grid2';
import TablePagination from '@mui/material/TablePagination';
import { GridContainer, PushToRight, StackRow } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<oldLocations_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query oldLocations_rootQuery($locationsSortingValues: [LocationOrderInput!]!, $locationNameSearchText: String) {
    ...oldLocations_query
  }
`;

const OldLocations = ({ queryReference }: Props) => {
  const rootDataRelay = usePreloadedQuery<oldLocations_rootQuery>(RootQuery, queryReference);
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<oldLocations_refetchableFragment, oldLocations_query$key>(
    graphql`
      fragment oldLocations_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "oldLocations_refetchableFragment") {
        locations(first: $count, after: $cursor, where: { nameContains: $locationNameSearchText }, orderBy: $locationsSortingValues)
          @connection(key: "oldLocations_locations") {
          __id
          totalCount
          edges {
            node {
              id
              name
              organization {
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

  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<LocationOrderInput>({
    direction: 'Ascending',
    field: 'Name',
  });
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);
  const [locationNameSearchText, setLocationNameSearchText] = useState<string>('');
  const handleSearchTextChange = (str: string) => {
    setLocationNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  const handleChangePage = (_: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    if (newPage > page) {
      loadNextPage();
    }

    setPage(newPage);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const pageSize = parseInt(event.target.value, 10);

    setPageSize(parseInt(event.target.value, 10));

    handleRefetch(pageSize, sortingOrder, locationNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: LocationOrderInput, locationNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            locationsSortingValues: [order],
            locationNameSearchText,
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

  const connectionIds = useMemo(() => (rootData.locations ? [rootData.locations.__id] : []), [rootData.locations]);
  if (!rootData.locations) {
    return <></>;
  }

  const slicedEdges = rootData.locations.edges?.slice(
    page * pageSize,
    page * pageSize + pageSize > rootData.locations.edges.length ? rootData.locations.edges.length : page * pageSize + pageSize,
  );

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as LocationOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as LocationOrderField,
      },
      locationNameSearchText,
    );
  };

  return (
    <>
      <NewLocationButton />
      <StackRow>
        <Search size="small" placeholder="Find a location..." defaultValue={locationNameSearchText} onChange={handleSearchTextChange} />
        <PushToRight />
        <TablePagination
          component="div"
          count={rootData.locations.totalCount ? rootData.locations.totalCount : 0}
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
      </StackRow>

      <GridContainer>
        {slicedEdges.map((edge) => (
          <Grid key={edge.node.id}>
            <LocationBookingsCard
              organizationId={edge.node.organization?.uniqueId}
              organizationName={edge.node.organization?.name}
              locationId={edge.node.id}
              locationName={edge.node.name}
              locationsConnectionIds={connectionIds}
            />
          </Grid>
        ))}
      </GridContainer>
    </>
  );
};

const MemoOldLocations = memo(OldLocations);

const OldLocationsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<oldLocations_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
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
  }, [loadQuery, triggerReloadId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOldLocations queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(OldLocationsWithRelay);
