import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { TeamAvatar } from '@repo/shared/components/avatars';
import { TeamIcon } from '@repo/shared/components/icons';
import graphql from 'babel-plugin-relay/macro';
import { memo, useMemo, useState } from 'react';
import { usePaginationFragment } from 'react-relay';
import type { teamSelector_allTeams_query$key } from './__generated__/teamSelector_allTeams_query.graphql';
import type { teamSelector_allTeams_refetchableFragment } from './__generated__/teamSelector_allTeams_refetchableFragment.graphql';

type Props = {
  rootDataRelay: teamSelector_allTeams_query$key;
  onTeamChanged: (teamId?: string) => void;
};

const allTeamsId = 'kkigMVsUXwi2YMSSrXv7i';

const TeamSelector = ({ rootDataRelay, onTeamChanged }: Props) => {
  const { data: rootData } = usePaginationFragment<teamSelector_allTeams_refetchableFragment, teamSelector_allTeams_query$key>(
    graphql`
      fragment teamSelector_allTeams_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "teamSelector_allTeams_refetchableFragment") {
        teams(first: $count, after: $cursor, where: { organizationId: $organizationId }) @connection(key: "teamSelector_teams") {
          __id
          totalCount
          edges {
            node {
              id
              name
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [selectedTeamId, setSelectedTeamId] = useState<string>(allTeamsId);
  const allTeams = useMemo(() => (rootData.teams?.edges ? rootData.teams.edges.map(({ node }) => node) : []), [rootData.teams]);

  const handleSelectedTeamChange = (event: SelectChangeEvent) => {
    const id = event.target.value as string;

    setSelectedTeamId(id);
    onTeamChanged(id === allTeamsId ? undefined : id);
  };

  return (
    <Select
      value={selectedTeamId}
      onChange={handleSelectedTeamChange}
      sx={{
        '& .MuiOutlinedInput-notchedOutline': {
          borderRadius: 4,
        },
        width: {
          xs: '100%',
          sm: 'min(100%, 300px)',
        },
      }}
      size="small"
      renderValue={(selectedId) => {
        const selectedTeam = allTeams.find((team) => team.id === selectedId);
        if (selectedTeam) {
          return (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <TeamIcon />
              <Typography variant="h6">Team</Typography>
              <Divider orientation="vertical" flexItem />
              <Typography variant="body1">{selectedTeam.name}</Typography>
            </Stack>
          );
        }

        return (
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <TeamIcon />
            <Typography variant="h6">Team</Typography>
            <Divider orientation="vertical" flexItem />
            <Typography variant="body1">All</Typography>
          </Stack>
        );
      }}
    >
      <MenuItem value={allTeamsId}>
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <Typography variant="h6">All</Typography>
        </Stack>
      </MenuItem>

      {allTeams.map((team) => (
        <MenuItem key={team.id} value={team.id}>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <TeamAvatar name={{ name: team.name }} size="small" />
            <Typography variant="h6">{team.name}</Typography>
          </Stack>
        </MenuItem>
      ))}
    </Select>
  );
};

export default memo(TeamSelector);
