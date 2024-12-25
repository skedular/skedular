import { DeskTypeCard, NewDeskTypeDialog } from '@/components/deskType';
import type { organizationDeskTypesTab_deskTypes_query$key } from '@/queries/__generated__/organizationDeskTypesTab_deskTypes_query.graphql';
import type {
  OrganizationTagOrderField,
  OrganizationTagOrderInput,
  organizationDeskTypesTab_deskTypes_refetchableFragment,
} from '@/queries/__generated__/organizationDeskTypesTab_deskTypes_refetchableFragment.graphql';
import type { organizationDeskTypesTab_query$key } from '@/queries/__generated__/organizationDeskTypesTab_query.graphql';
import type { organizationDeskTypesTab_rootQuery } from '@/queries/__generated__/organizationDeskTypesTab_rootQuery.graphql';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import TablePagination from '@mui/material/TablePagination';
import { GridContainer, PushToRight, StackRow } from '@repo/shared/components/commons';
import { AddIcon } from '@repo/shared/components/icons';
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
  queryReference: PreloadedQuery<organizationDeskTypesTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationDeskTypesTab_rootQuery(
    $organizationId: String!
    $deskTypeNameSearchText: String
    $deskTypeSortingValues: [OrganizationTagOrderInput!]!
  ) {
    ...organizationDeskTypesTab_query
    ...organizationDeskTypesTab_deskTypes_query
  }
`;

const OrganizationDeskTypesTab = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<organizationDeskTypesTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<organizationDeskTypesTab_query$key>(
    graphql`
      fragment organizationDeskTypesTab_query on Query {
        organization(id: $organizationId) {
          canModify
        }
        ...deskTypeCard_Query
      }
    `,
    rootDataRelay,
  );
  const {
    data: rootDataPaginatedOrganizationTags,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<organizationDeskTypesTab_deskTypes_refetchableFragment, organizationDeskTypesTab_deskTypes_query$key>(
    graphql`
      fragment organizationDeskTypesTab_deskTypes_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "organizationDeskTypesTab_deskTypes_refetchableFragment") {
        deskTypes(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $deskTypeNameSearchText }
          orderBy: $deskTypeSortingValues
        ) @connection(key: "organizationDeskTypesTab_deskTypes") {
          __id
          totalCount
          edges {
            node {
              id
              ...deskTypeCard_OrganizationTagDetails
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

    handleRefetch(pageSize, sortingOrder, deskTypeNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: OrganizationTagOrderInput, deskTypeNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            deskTypeSortingValues: [order],
            deskTypeNameSearchText,
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

  const [deskTypeNameSearchText, setDeskTypeNameSearchText] = useState<string>('');

  const handleSearchTextChange = (str: string) => {
    setDeskTypeNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  const connectionIds = useMemo(
    () => (rootDataPaginatedOrganizationTags.deskTypes ? [rootDataPaginatedOrganizationTags.deskTypes.__id] : []),
    [rootDataPaginatedOrganizationTags.deskTypes],
  );
  const [isAddDeskTypeDialogOpen, setIsAddDeskTypeDialogOpen] = useState(false);

  if (!rootData.organization || !rootDataPaginatedOrganizationTags.deskTypes) {
    return <></>;
  }

  const organizationTagEdges = rootDataPaginatedOrganizationTags.deskTypes.edges;
  const slicedEdges = organizationTagEdges.slice(
    page * pageSize,
    page * pageSize + pageSize > organizationTagEdges.length ? organizationTagEdges.length : page * pageSize + pageSize,
  );

  const handleAddDeskTypeClick = () => {
    setIsAddDeskTypeDialogOpen(true);
  };

  const handleAddDeskTypeDialogAddClick = () => {
    setIsAddDeskTypeDialogOpen(false);

    handleRefetch(pageSize, sortingOrder, deskTypeNameSearchText);
  };

  const handleAddDeskTypeDialogCancelClick = () => {
    setIsAddDeskTypeDialogOpen(false);
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
      deskTypeNameSearchText,
    );
  };

  return (
    <>
      {rootData.organization.canModify && (
        <Button variant="contained" size="small" startIcon={<AddIcon />} onClick={handleAddDeskTypeClick}>
          Add Desk Type
        </Button>
      )}

      <StackRow>
        <Search size="small" placeholder="Find a desk type..." defaultValue={deskTypeNameSearchText} onChange={handleSearchTextChange} />
        <PushToRight />
        <TablePagination
          component="div"
          count={rootDataPaginatedOrganizationTags.deskTypes.totalCount ? rootDataPaginatedOrganizationTags.deskTypes.totalCount : 0}
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
            <DeskTypeCard rootDataRelay={rootData} organizationTagDetailsRelay={edge.node} connectionIds={connectionIds} />
          </Grid>
        ))}
      </GridContainer>

      <NewDeskTypeDialog
        connectionIds={connectionIds}
        isDialogOpen={isAddDeskTypeDialogOpen}
        onAddClicked={handleAddDeskTypeDialogAddClick}
        onCancelClicked={handleAddDeskTypeDialogCancelClick}
        organizationId={organizationId}
      />
    </>
  );
};

const MemoOrganizationDeskTypesTab = memo(OrganizationDeskTypesTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationDeskTypesTabWithRelay = ({ onReloadRequired, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationDeskTypesTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        deskTypeSortingValues: [
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
      <MemoOrganizationDeskTypesTab queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationDeskTypesTabWithRelay);
