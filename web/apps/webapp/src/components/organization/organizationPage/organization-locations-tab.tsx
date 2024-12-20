import { NewLocationButton } from '@/components/location/addLocation';
import { LocationBookingsCard } from '@/components/location/locationBookingCard';
import type {
  LocationOrderField,
  LocationOrderInput,
  organizationLocationsTab_location_refetchableFragment,
} from '@/queries/__generated__/organizationLocationsTab_location_refetchableFragment.graphql';
import type { organizationLocationsTab_locations_query$key } from '@/queries/__generated__/organizationLocationsTab_locations_query.graphql';
import type { organizationLocationsTab_query$key } from '@/queries/__generated__/organizationLocationsTab_query.graphql';
import type { organizationLocationsTab_rootQuery } from '@/queries/__generated__/organizationLocationsTab_rootQuery.graphql';
import Grid from '@mui/material/Grid2';
import TablePagination from '@mui/material/TablePagination';
import { GridContainer, StackRow, StackRowFullWidth } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, useFragment, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<organizationLocationsTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query organizationLocationsTab_rootQuery(
    $organizationId: String!
    $organizationLocationsSortingValues: [LocationOrderInput!]!
    $locationNameSearchText: String
  ) {
    ...organizationLocationsTab_query
    ...organizationLocationsTab_locations_query
  }
`;

const OrganizationLocationsTab = ({ queryReference }: Props) => {
  const rootDataRelay = usePreloadedQuery<organizationLocationsTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<organizationLocationsTab_query$key>(
    graphql`
      fragment organizationLocationsTab_query on Query {
        organization(id: $organizationId) {
          id
          canModify
        }
      }
    `,
    rootDataRelay,
  );
  const {
    data: rootDataLocations,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<organizationLocationsTab_location_refetchableFragment, organizationLocationsTab_locations_query$key>(
    graphql`
      fragment organizationLocationsTab_locations_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "organizationLocationsTab_location_refetchableFragment") {
        locations(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $locationNameSearchText }
          orderBy: $organizationLocationsSortingValues
        ) @connection(key: "organizationLocationsTab_locations") {
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
            organizationLocationsSortingValues: [order],
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

  const connectionIds = useMemo(() => (rootDataLocations.locations ? [rootDataLocations.locations.__id] : []), [rootDataLocations.locations]);
  const locationEdges = rootDataLocations.locations ? rootDataLocations.locations.edges : [];
  const slicedEdges = locationEdges.slice(
    page * pageSize,
    page * pageSize + pageSize > locationEdges.length ? locationEdges.length : page * pageSize + pageSize,
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

  const handleSearchTextChange = (str: string) => {
    setLocationNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  if (!rootData.organization) {
    return <></>;
  }

  return (
    <>
      {rootData.organization.canModify && <NewLocationButton organizationId={rootData.organization.id} />}

      <StackRowFullWidth>
        <Search size="small" placeholder="Find a location..." defaultValue={locationNameSearchText} onChange={handleSearchTextChange} />
        <StackRow>
          <TablePagination
            count={rootDataLocations.locations?.totalCount ? rootDataLocations.locations.totalCount : 0}
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
      </StackRowFullWidth>

      <GridContainer>
        {slicedEdges.map((edge) => {
          if (!edge.node.organization) {
            return <></>;
          }

          return (
            <Grid key={edge.node.id}>
              <LocationBookingsCard
                organizationId={edge.node.organization?.uniqueId}
                organizationName={edge.node.organization?.name}
                locationId={edge.node.id}
                locationName={edge.node.name}
                locationsConnectionIds={connectionIds}
              />
            </Grid>
          );
        })}
      </GridContainer>
    </>
  );
};

const MemoOrganizationLocationsTab = memo(OrganizationLocationsTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationLocationsTabWithRelay = ({ onReloadRequired, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationLocationsTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        organizationLocationsSortingValues: [
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
  }, [loadQuery, triggerReloadId, organizationId]);

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
      <MemoOrganizationLocationsTab queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationLocationsTabWithRelay);
