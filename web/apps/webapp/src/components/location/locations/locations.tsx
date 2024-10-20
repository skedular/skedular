import { getLocationAddLink } from '@/components/location';
import { LocationBookingsCard } from '@/components/location/locationBookingCard';
import type { locations_query$key } from '@/queries/__generated__/locations_query.graphql';
import type {
  LocationOrderField,
  LocationOrderInput,
  locations_refetchableFragment,
} from '@/queries/__generated__/locations_refetchableFragment.graphql';
import type { locations_rootQuery } from '@/queries/__generated__/locations_rootQuery.graphql';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import { AddIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import debounce from 'lodash.debounce';
import { nanoid } from 'nanoid';
import NextLink from 'next/link';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<locations_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query locations_rootQuery($locationsSortingValues: [LocationOrderInput!]!, $locationNameSearchText: String) {
    ...locations_query
  }
`;

const Locations = ({ queryReference }: Props) => {
  const rootDataRelay = usePreloadedQuery<locations_rootQuery>(RootQuery, queryReference);
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<locations_refetchableFragment, locations_query$key>(
    graphql`
      fragment locations_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "locations_refetchableFragment") {
        locations(first: $count, after: $cursor, where: { nameContains: $locationNameSearchText }, orderBy: $locationsSortingValues)
          @connection(key: "locations_locations") {
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
    field: 'name',
  });
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);
  const [locationNameSearchText, setLocationNameSearchText] = useState<string>('');
  const handleSearchTextChange = (str: string) => {
    setLocationNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  const debounceSearchTextChange = debounce(handleSearchTextChange, keyboardDebounceTimeout);
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
    <Stack direction="column" spacing={1}>
      <Link component={NextLink} href={getLocationAddLink()}>
        <Button variant="contained" size="small" startIcon={<AddIcon />}>
          Add Location
        </Button>
      </Link>

      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap' }}>
        <Search size="small" placeholder="Find a location..." defaultValue={locationNameSearchText} onChange={debounceSearchTextChange} />
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <TablePagination
            count={rootData.locations.totalCount ? rootData.locations.totalCount : 0}
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
            <LocationBookingsCard
              organizationId={edge.node.organization?.uniqueId}
              organizationName={edge.node.organization?.name}
              locationId={edge.node.id}
              locationName={edge.node.name}
              locationsConnectionIds={connectionIds}
            />
          </Grid>
        ))}
      </Grid>
    </Stack>
  );
};

const MemoLocations = memo(Locations);

const LocationsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<locations_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        locationsSortingValues: [
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
      <MemoLocations queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(LocationsWithRelay);
