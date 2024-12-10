import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationMemberCard } from 'components/organization';
import { InvitePeopleToJoinOrganizationButton } from 'components/organization/invitePeopleToJoinOrganization';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, useFragment, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { organizationMembersTab_organizationMembers_query$key } from './__generated__/organizationMembersTab_organizationMembers_query.graphql';
import type {
  OrganizationMemberOrderField,
  OrganizationMemberOrderInput,
  organizationMembersTab_organizationMembers_refetchableFragment,
} from './__generated__/organizationMembersTab_organizationMembers_refetchableFragment.graphql';
import type { organizationMembersTab_query$key } from './__generated__/organizationMembersTab_query.graphql';
import type { organizationMembersTab_rootQuery } from './__generated__/organizationMembersTab_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<organizationMembersTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationMembersTab_rootQuery(
    $organizationId: String!
    $peopleNameSearchText: String
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
  ) {
    ...organizationMembersTab_query
    ...organizationMembersTab_organizationMembers_query
  }
`;

const OrganizationMembersTab = ({ queryReference, organizationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<organizationMembersTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<organizationMembersTab_query$key>(
    graphql`
      fragment organizationMembersTab_query on Query {
        organization(id: $organizationId) {
          id
          name
          canInvitePeople
        }
        ...organizationSingleChoiceMembershipType_query
      }
    `,
    rootDataRelay,
  );

  const {
    data: rootDataPaginatedOrganizationMembers,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<organizationMembersTab_organizationMembers_refetchableFragment, organizationMembersTab_organizationMembers_query$key>(
    graphql`
      fragment organizationMembersTab_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "organizationMembersTab_organizationMembers_refetchableFragment") {
        organizationMembers(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $peopleNameSearchText }
          orderBy: $organizationMembersSortingValues
        ) @connection(key: "organizationMembersTab_organizationMembers") {
          __id
          totalCount
          edges {
            node {
              id
              ...organizationMemberCard_OrganizationMemberDetails
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<OrganizationMemberOrderInput>({
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

    handleRefetch(pageSize, sortingOrder, peopleNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: OrganizationMemberOrderInput, peopleNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            organizationMembersSortingValues: [order],
            peopleNameSearchText,
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

  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');

  const handleSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  const connectionIds = useMemo(
    () => (rootDataPaginatedOrganizationMembers.organizationMembers ? [rootDataPaginatedOrganizationMembers.organizationMembers.__id] : []),
    [rootDataPaginatedOrganizationMembers.organizationMembers],
  );

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as OrganizationMemberOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as OrganizationMemberOrderField,
      },
      peopleNameSearchText,
    );
  };

  if (!rootData.organization || !rootDataPaginatedOrganizationMembers.organizationMembers) {
    return <></>;
  }

  const organizationMemberEdges = rootDataPaginatedOrganizationMembers.organizationMembers.edges;
  const slicedEdges = organizationMemberEdges.slice(
    page * pageSize,
    page * pageSize + pageSize > organizationMemberEdges.length ? organizationMemberEdges.length : page * pageSize + pageSize,
  );

  return (
    <>
      {rootData.organization.canInvitePeople && <InvitePeopleToJoinOrganizationButton organizationId={organizationId} />}

      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap' }}>
        <Search size="small" placeholder="Search for members" defaultValue={peopleNameSearchText} onChange={handleSearchTextChange} />
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <TablePagination
            count={
              rootDataPaginatedOrganizationMembers.organizationMembers.totalCount
                ? rootDataPaginatedOrganizationMembers.organizationMembers.totalCount
                : 0
            }
            page={page}
            onPageChange={handleChangePage}
            rowsPerPage={pageSize}
            onRowsPerPageChange={handlePageSizeChange}
          />
          <Sorting
            options={[
              { id: 'Name', label: 'Name' },
              { id: 'GivenName', label: 'Given name' },
              { id: 'MiddleName', label: 'Middle name' },
              { id: 'FamilyName', label: 'Family Name' },
              { id: 'MembershipType', label: 'Membership type' },
            ]}
            defaultOption={sortingOrder.field}
            defaultSortingDirectionValue={sortingOrder.direction as unknown as Direction}
            onValueChange={handleSortingChanged}
          />
        </Stack>
      </Stack>

      <Grid container spacing={1}>
        {slicedEdges.map((edge) => (
          <Grid key={edge.node.id}>
            <OrganizationMemberCard data={rootData} organizationMemberDetailsRelay={edge.node} connectionIds={connectionIds} />
          </Grid>
        ))}
      </Grid>
    </>
  );
};

const MemoOrganizationMembersTab = memo(OrganizationMembersTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationMembersTabWithRelay = ({ onReloadRequired, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationMembersTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        organizationMembersSortingValues: [
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
      <MemoOrganizationMembersTab queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationMembersTabWithRelay);
