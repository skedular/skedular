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
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { TeamBookingsCard } from 'components/team/teamBookingCard';
import debounce from 'lodash.debounce';
import { memo, useCallback, useMemo, useState, useTransition } from 'react';
import { usePaginationFragment } from 'react-relay';
import type { TeamOrderField, TeamOrderInput, organizationTeams_PaginationQuery } from './__generated__/organizationTeams_PaginationQuery.graphql';
import type { organizationTeamsTab_query$key } from './__generated__/organizationTeamsTab_query.graphql';

type Props = {
  rootDataRelay: organizationTeamsTab_query$key;
};

const OrganizationTeamsTab = ({ rootDataRelay }: Props) => {
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<organizationTeams_PaginationQuery, organizationTeamsTab_query$key>(
    graphql`
      fragment organizationTeamsTab_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "organizationTeams_PaginationQuery") {
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
              }
            }
          }
        }
        organization(id: $organizationId) {
          id
          canModify
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

  const connectionIds = useMemo(() => [rootData.teams?.__id], [rootData.teams]);
  const teamEdges = rootData.teams.edges;
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

  const handlePageContextOpenStateChange = (event: React.SyntheticEvent, isExpanded: boolean) => {
    if (isExpanded) {
      setPageContextOpen(true);
    } else {
      setPageContextOpen(false);
    }
  };

  const handleSearchTextChange = (str: string) => {
    setTeamNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };
  const debounceSearchTextChange = debounce(handleSearchTextChange, keyboardDebounceTimeout);

  if (!rootData.organization) {
    return <></>;
  }

  return (
    <Stack direction="column" spacing={1}>
      {rootData.organization.canModify && (
        <Stack direction="row" sx={{ width: 'auto' }}>
          <Link href={`/organization/${rootData.organization.id}/team/add`}>
            <Button variant="contained" startIcon={<AddIcon />}>
              Add Team
            </Button>
          </Link>
        </Stack>
      )}

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
          count={rootData.teams?.totalCount ? rootData.teams.totalCount : 0}
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

export default memo(OrganizationTeamsTab);
