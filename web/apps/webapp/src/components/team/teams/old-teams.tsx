import { NewTeamButton } from '@/components/team/addTeam';
import { TeamBookingsCard } from '@/components/team/teamBookingCard';
import type { oldTeams_query$key } from '@/queries/__generated__/oldTeams_query.graphql';
import type { TeamOrderField, TeamOrderInput, oldTeams_refetchableFragment } from '@/queries/__generated__/oldTeams_refetchableFragment.graphql';
import type { oldTeams_rootQuery } from '@/queries/__generated__/oldTeams_rootQuery.graphql';
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
import { PreloadedQuery, graphql, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<oldTeams_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query oldTeams_rootQuery($teamsSortingValues: [TeamOrderInput!]!, $teamNameSearchText: String) {
    ...oldTeams_query
  }
`;

const OldTeams = ({ queryReference }: Props) => {
  const rootDataRelay = usePreloadedQuery<oldTeams_rootQuery>(RootQuery, queryReference);
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<oldTeams_refetchableFragment, oldTeams_query$key>(
    graphql`
      fragment oldTeams_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "oldTeams_refetchableFragment") {
        teams(first: $count, after: $cursor, where: { nameContains: $teamNameSearchText }, orderBy: $teamsSortingValues)
          @connection(key: "oldTeams_teams") {
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
  const handleSearchTextChange = (str: string) => {
    setTeamNameSearchText(str);

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

    handleRefetch(pageSize, sortingOrder, teamNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: TeamOrderInput, teamNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            teamsSortingValues: [order],
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
  if (!rootData.teams) {
    return <></>;
  }

  const slicedEdges = rootData.teams.edges?.slice(
    page * pageSize,
    page * pageSize + pageSize > rootData.teams.edges.length ? rootData.teams.edges.length : page * pageSize + pageSize,
  );

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

  return (
    <>
      <NewTeamButton />

      <StackRowFullWidth>
        <Search size="small" placeholder="Find a team..." defaultValue={teamNameSearchText} onChange={handleSearchTextChange} />
        <StackRow>
          <TablePagination
            count={rootData.teams.totalCount ? rootData.teams.totalCount : 0}
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

      <GridContainer spacing={1}>
        {slicedEdges.map((edge) => (
          <Grid key={edge.node.id}>
            <TeamBookingsCard
              organizationId={edge.node.organization?.uniqueId}
              organizationName={edge.node.organization?.name}
              teamId={edge.node.id}
              teamName={edge.node.name}
              teamsConnectionIds={connectionIds}
            />
          </Grid>
        ))}
      </GridContainer>
    </>
  );
};

const MemoOldTeams = memo(OldTeams);

const OldTeamsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<oldTeams_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        teamsSortingValues: [
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
      <MemoOldTeams queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(OldTeamsWithRelay);
