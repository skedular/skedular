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
import graphql from 'babel-plugin-relay/macro';
import { CustomTagCard, NewCustomTagDialog } from 'components/customTag';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, useFragment, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { organizationCustomTagsTab_customTags_query$key } from './__generated__/organizationCustomTagsTab_customTags_query.graphql';
import type {
  OrganizationTagOrderField,
  OrganizationTagOrderInput,
  organizationCustomTagsTab_customTags_refetchableFragment,
} from './__generated__/organizationCustomTagsTab_customTags_refetchableFragment.graphql';
import type { organizationCustomTagsTab_query$key } from './__generated__/organizationCustomTagsTab_query.graphql';
import type { organizationCustomTagsTab_rootQuery } from './__generated__/organizationCustomTagsTab_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<organizationCustomTagsTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationCustomTagsTab_rootQuery(
    $organizationId: String!
    $customTagNameSearchText: String
    $customTagSortingValues: [OrganizationTagOrderInput!]
  ) {
    ...organizationCustomTagsTab_query
    ...organizationCustomTagsTab_customTags_query
  }
`;

const OrganizationCustomTagsTab = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<organizationCustomTagsTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<organizationCustomTagsTab_query$key>(
    graphql`
      fragment organizationCustomTagsTab_query on Query {
        organization(id: $organizationId) {
          canModify
        }
        ...customTagCard_Query
      }
    `,
    rootDataRelay,
  );
  const {
    data: rootDataPaginatedOrganizationTags,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<organizationCustomTagsTab_customTags_refetchableFragment, organizationCustomTagsTab_customTags_query$key>(
    graphql`
      fragment organizationCustomTagsTab_customTags_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "organizationCustomTagsTab_customTags_refetchableFragment") {
        customTags(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $customTagNameSearchText }
          orderBy: $customTagSortingValues
        ) @connection(key: "organizationCustomTagsTab_customTags") {
          __id
          totalCount
          edges {
            node {
              id
              ...customTagCard_OrganizationTagDetails
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

    handleRefetch(pageSize, sortingOrder, customTagNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: OrganizationTagOrderInput, customTagNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            customTagSortingValues: [order],
            customTagNameSearchText,
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

  const [customTagNameSearchText, setCustomTagNameSearchText] = useState<string>('');

  const handleSearchTextChange = (str: string) => {
    setCustomTagNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  const connectionIds = useMemo(
    () => (rootDataPaginatedOrganizationTags.customTags ? [rootDataPaginatedOrganizationTags.customTags.__id] : []),
    [rootDataPaginatedOrganizationTags.customTags],
  );
  const [isAddCustomTagDialogOpen, setIsAddCustomTagDialogOpen] = useState(false);

  if (!rootData.organization || !rootDataPaginatedOrganizationTags.customTags) {
    return <></>;
  }

  const organizationTagEdges = rootDataPaginatedOrganizationTags.customTags.edges;
  const slicedEdges = organizationTagEdges.slice(
    page * pageSize,
    page * pageSize + pageSize > organizationTagEdges.length ? organizationTagEdges.length : page * pageSize + pageSize,
  );

  const handleAddCustomTagClick = () => {
    setIsAddCustomTagDialogOpen(true);
  };

  const handleAddCustomTagDialogAddClick = () => {
    setIsAddCustomTagDialogOpen(false);

    handleRefetch(pageSize, sortingOrder, customTagNameSearchText);
  };

  const handleAddCustomTagDialogCancelClick = () => {
    setIsAddCustomTagDialogOpen(false);
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
      customTagNameSearchText,
    );
  };

  return (
    <>
      {rootData.organization.canModify && (
        <Button variant="contained" size="small" startIcon={<AddIcon />} onClick={handleAddCustomTagClick}>
          Add Tag
        </Button>
      )}

      <StackRow>
        <Search size="small" placeholder="Find a tag..." defaultValue={customTagNameSearchText} onChange={handleSearchTextChange} />
        <PushToRight />
        <TablePagination
          component="div"
          count={rootDataPaginatedOrganizationTags.customTags.totalCount ? rootDataPaginatedOrganizationTags.customTags.totalCount : 0}
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
            <CustomTagCard rootDataRelay={rootData} organizationTagDetailsRelay={edge.node} connectionIds={connectionIds} />
          </Grid>
        ))}
      </GridContainer>

      <NewCustomTagDialog
        connectionIds={connectionIds}
        isDialogOpen={isAddCustomTagDialogOpen}
        onAddClicked={handleAddCustomTagDialogAddClick}
        onCancel={handleAddCustomTagDialogCancelClick}
        organizationId={organizationId}
      />
    </>
  );
};

const MemoOrganizationCustomTagsTab = memo(OrganizationCustomTagsTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationCustomTagsTabWithRelay = ({ onReloadRequired, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationCustomTagsTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        customTagSortingValues: [
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
      <MemoOrganizationCustomTagsTab queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationCustomTagsTabWithRelay);
