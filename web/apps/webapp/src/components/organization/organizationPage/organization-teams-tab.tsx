import { NewTeamButton } from '@/components/team/addTeam';
import { TeamBookingsCard } from '@/components/team/teamBookingCard';
import type { organizationTeamsTab_rootQuery } from '@/queries/__generated__/organizationTeamsTab_rootQuery.graphql';
import type { organizationTeamsTab_teams_query$key } from '@/queries/__generated__/organizationTeamsTab_teams_query.graphql';
import type {
  TeamOrderField,
  TeamOrderInput,
  organizationTeamsTab_teams_refetchableFragment,
} from '@/queries/__generated__/organizationTeamsTab_teams_refetchableFragment.graphql';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
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
  queryReference: PreloadedQuery<organizationTeamsTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationTeamsTab_rootQuery($organizationId: String!, $organizationTeamsSortingValues: [TeamOrderInput!]!, $teamNameSearchText: String) {
    ...organizationTeamsTab_teams_query
  }
`;

const OrganizationTeamsTab = ({ queryReference, organizationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<organizationTeamsTab_rootQuery>(RootQuery, queryReference);
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<organizationTeamsTab_teams_refetchableFragment, organizationTeamsTab_teams_query$key>(
    graphql`
      fragment organizationTeamsTab_teams_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "organizationTeamsTab_teams_refetchableFragment") {
        teams(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $teamNameSearchText }
          orderBy: $organizationTeamsSortingValues
        ) @connection(key: "organizationTeamsTab_teams") {
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
  const [sortingOrder, setSortingOrder] = useState<TeamOrderInput>({
    direction: 'Ascending',
    field: 'Name',
  });
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);
  const [teamNameSearchText, setTeamNameSearchText] = useState<string>('');

  const handleChangePage = (_: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    if (newPage > page) {
      loadNextPage();
    }

    setPage(newPage);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const pageSize = parseInt(event.target.value, 10);

    setPageSize(parseInt(event.target.value, 10));

    handleRefetch(pageSize, sortingOrder, teamNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: TeamOrderInput, teamNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            organizationTeamsSortingValues: [order],
            teamNameSearchText,
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

  const connectionIds = useMemo(() => (rootData.teams ? [rootData.teams.__id] : []), [rootData.teams]);
  const teamEdges = rootData.teams ? rootData.teams.edges : [];
  const slicedEdges = teamEdges.slice(page * pageSize, page * pageSize + pageSize > teamEdges.length ? teamEdges.length : page * pageSize + pageSize);

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as TeamOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as TeamOrderField,
      },
      teamNameSearchText,
    );
  };

  const handleSearchTextChange = (str: string) => {
    setTeamNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  return (
    <>
      <NewTeamButton organizationId={organizationId} />

      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap' }}>
        <Search size="small" placeholder="Find a team..." defaultValue={teamNameSearchText} onChange={handleSearchTextChange} />
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <TablePagination
            count={rootData.teams?.totalCount ? rootData.teams.totalCount : 0}
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
          if (!edge.node.organization) {
            return <></>;
          }

          return (
            <Grid key={edge.node.id}>
              <TeamBookingsCard
                organizationId={edge.node.organization?.uniqueId}
                organizationName={edge.node.organization?.name}
                teamId={edge.node.id}
                teamName={edge.node.name}
                teamsConnectionIds={connectionIds}
              />
            </Grid>
          );
        })}
      </Grid>
    </>
  );
};

const MemoOrganizationTeamsTab = memo(OrganizationTeamsTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationTeamsTabWithRelay = ({ onReloadRequired, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationTeamsTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        organizationTeamsSortingValues: [
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
      <MemoOrganizationTeamsTab queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationTeamsTabWithRelay);
