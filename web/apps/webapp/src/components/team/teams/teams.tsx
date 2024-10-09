import { getTeamAddLink } from '@/components/team';
import { TeamBookingsCard } from '@/components/team/teamBookingCard';
import type { TeamOrderField, TeamOrderInput, teams_PaginationQuery } from '@/queries/__generated__/teams_PaginationQuery.graphql';
import type { teams_query$key } from '@/queries/__generated__/teams_query.graphql';
import type { teams_rootQuery } from '@/queries/__generated__/teams_rootQuery.graphql';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import TextField from '@mui/material/TextField';
import { AddIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import debounce from 'lodash.debounce';
import NextLink from 'next/link';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<teams_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query teams_rootQuery($teamsSortingValues: [TeamOrderInput!]!, $teamNameSearchText: String) {
    ...teams_query
  }
`;

const Teams = ({ queryReference }: Props) => {
  const rootDataRelay = usePreloadedQuery<teams_rootQuery>(RootQuery, queryReference);
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<teams_PaginationQuery, teams_query$key>(
    graphql`
      fragment teams_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "teams_PaginationQuery") {
        teams(first: $count, after: $cursor, where: { nameContains: $teamNameSearchText }, orderBy: $teamsSortingValues)
          @connection(key: "teams_teams") {
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
    field: 'name',
  });
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);
  const [pageContextOpen, setPageContextOpen] = useState(false);
  const [teamNameSearchText, setTeamNameSearchText] = useState<string>('');
  const handleSearchTextChange = (str: string) => {
    setTeamNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  const debounceSearchTextChange = debounce(handleSearchTextChange, keyboardDebounceTimeout);

  const handlePageContextOpenStateChange = (event: React.SyntheticEvent, isExpanded: boolean) => {
    if (isExpanded) {
      setPageContextOpen(true);
    } else {
      setPageContextOpen(false);
    }
  };

  const handleChangePage = (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
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
    <Stack direction="column" spacing={1}>
      <Link component={NextLink} href={getTeamAddLink()}>
        <Button variant="contained" startIcon={<AddIcon />}>
          Add Team
        </Button>
      </Link>

      <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen} sx={{ width: '100%' }}>
        <AccordionSummary expandIcon={<ExpandMoreIcon />} />
        <AccordionDetails>
          <TextField
            defaultValue={teamNameSearchText}
            helperText="Enter team name to narrow down the teams list"
            onChange={(event) => debounceSearchTextChange(event?.target.value)}
          />
        </AccordionDetails>
      </Accordion>

      <Stack direction="row" sx={{ justifyContent: 'flex-end' }}>
        <TablePagination
          count={rootData.teams.totalCount ? rootData.teams.totalCount : 0}
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

      <Grid container spacing={1}>
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
      </Grid>
    </Stack>
  );
};

const MemoTeams = memo(Teams);

const TeamsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<teams_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        teamsSortingValues: [
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
  }, [loadQuery, triggerReload]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReload(triggerReload + 1);
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoTeams queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(TeamsWithRelay);
