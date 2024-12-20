import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import TablePagination from '@mui/material/TablePagination';
import { GridContainer, StackRow, StackRowFullWidth } from '@repo/shared/components/commons';
import { AddIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import graphql from 'babel-plugin-relay/macro';
import { NewZoneDialog, ZoneCard } from 'components/zone';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, useFragment, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { organizationZonesTab_query$key } from './__generated__/organizationZonesTab_query.graphql';
import type { organizationZonesTab_rootQuery } from './__generated__/organizationZonesTab_rootQuery.graphql';
import type { organizationZonesTab_zones_query$key } from './__generated__/organizationZonesTab_zones_query.graphql';
import type {
  OrganizationTagOrderField,
  OrganizationTagOrderInput,
  organizationZonesTab_zones_refetchableFragment,
} from './__generated__/organizationZonesTab_zones_refetchableFragment.graphql';

type Props = {
  queryReference: PreloadedQuery<organizationZonesTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationZonesTab_rootQuery($organizationId: String!, $zoneNameSearchText: String, $zoneSortingValues: [OrganizationTagOrderInput!]!) {
    ...organizationZonesTab_query
    ...organizationZonesTab_zones_query
  }
`;

const OrganizationZonesTab = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<organizationZonesTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<organizationZonesTab_query$key>(
    graphql`
      fragment organizationZonesTab_query on Query {
        organization(id: $organizationId) {
          canModify
        }
        ...zoneCard_Query
      }
    `,
    rootDataRelay,
  );
  const {
    data: rootDataPaginatedOrganizationTags,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<organizationZonesTab_zones_refetchableFragment, organizationZonesTab_zones_query$key>(
    graphql`
      fragment organizationZonesTab_zones_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "organizationZonesTab_zones_refetchableFragment") {
        zones(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $zoneNameSearchText }
          orderBy: $zoneSortingValues
        ) @connection(key: "organizationZonesTab_zones") {
          __id
          totalCount
          edges {
            node {
              id
              ...zoneCard_OrganizationTagDetails
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<OrganizationTagOrderInput>({
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

    handleRefetch(pageSize, sortingOrder, zoneNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: OrganizationTagOrderInput, zoneNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            zoneSortingValues: [order],
            zoneNameSearchText,
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

  const [zoneNameSearchText, setZoneNameSearchText] = useState<string>('');

  const handleSearchTextChange = (str: string) => {
    setZoneNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  const connectionIds = useMemo(
    () => (rootDataPaginatedOrganizationTags.zones ? [rootDataPaginatedOrganizationTags.zones.__id] : []),
    [rootDataPaginatedOrganizationTags.zones],
  );
  const [isAddZoneDialogOpen, setIsAddZoneDialogOpen] = useState(false);

  if (!rootData.organization || !rootDataPaginatedOrganizationTags.zones) {
    return <></>;
  }

  const organizationTagEdges = rootDataPaginatedOrganizationTags.zones.edges;
  const slicedEdges = organizationTagEdges.slice(
    page * pageSize,
    page * pageSize + pageSize > organizationTagEdges.length ? organizationTagEdges.length : page * pageSize + pageSize,
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
      field: value as unknown as OrganizationTagOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as OrganizationTagOrderField,
      },
      zoneNameSearchText,
    );
  };

  return (
    <>
      {rootData.organization.canModify && (
        <Button variant="contained" size="small" startIcon={<AddIcon />} onClick={handleAddZoneClick}>
          Add Zone
        </Button>
      )}

      <StackRowFullWidth>
        <Search size="small" placeholder="Find a desk type..." defaultValue={zoneNameSearchText} onChange={handleSearchTextChange} />
        <StackRow>
          <TablePagination
            count={rootDataPaginatedOrganizationTags.zones.totalCount ? rootDataPaginatedOrganizationTags.zones.totalCount : 0}
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
        {slicedEdges.map((edge) => (
          <Grid key={edge.node.id}>
            <ZoneCard rootDataRelay={rootData} organizationTagDetailsRelay={edge.node} connectionIds={connectionIds} />
          </Grid>
        ))}
      </GridContainer>

      <NewZoneDialog
        connectionIds={connectionIds}
        isDialogOpen={isAddZoneDialogOpen}
        onAddClicked={handleAddZoneDialogAddClick}
        onCancelClicked={handleAddZoneDialogCancelClick}
        organizationId={organizationId}
      />
    </>
  );
};

const MemoOrganizationZonesTab = memo(OrganizationZonesTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationZonesTabWithRelay = ({ onReloadRequired, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationZonesTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        zoneSortingValues: [
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
      <MemoOrganizationZonesTab queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationZonesTabWithRelay);
