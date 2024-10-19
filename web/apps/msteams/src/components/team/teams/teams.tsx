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
import graphql from 'babel-plugin-relay/macro';
import { getTeamAddLink } from 'components/team';
import { TeamBookingsCard } from 'components/team/teamBookingCard';
import debounce from 'lodash.debounce';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { teams_query$key } from './__generated__/teams_query.graphql';
import type { TeamOrderField, TeamOrderInput, teams_refetchableFragment } from './__generated__/teams_refetchableFragment.graphql';
import type { teams_rootQuery } from './__generated__/teams_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<teams_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query teams_rootQuery($organizationId: String!, $teamsSortingValues: [TeamOrderInput!]!, $teamNameSearchText: String) {
    ...teams_query
  }
`;

const Teams = ({ queryReference, organizationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<teams_rootQuery>(RootQuery, queryReference);
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<teams_refetchableFragment, teams_query$key>(
    graphql`
      fragment teams_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "teams_refetchableFragment") {
        teams(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $teamNameSearchText }
          orderBy: $teamsSortingValues
        ) @connection(key: "teams_teams") {
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
            organizationId,
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
    [refetch, organizationId],
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
      <Link href={getTeamAddLink(organizationId)}>
        <Button variant="contained" size="small" startIcon={<AddIcon />}>
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
    </Stack>
  );
};

const MemoTeams = memo(Teams);

type RelayProps = {
  organizationId: string;
};

const TeamsWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<teams_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
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
  }, [loadQuery, triggerReloadId, organizationId]);

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
      <MemoTeams queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(TeamsWithRelay);
