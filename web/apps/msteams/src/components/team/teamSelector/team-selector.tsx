import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { TeamAvatar } from '@repo/shared/components/avatars';
import { TeamIcon } from '@repo/shared/components/icons';
import graphql from 'babel-plugin-relay/macro';
import { memo, useMemo, useState } from 'react';
import { useFragment } from 'react-relay';
import type { teamSelector_allTeams_query$key } from './__generated__/teamSelector_allTeams_query.graphql';

type Props = {
  rootDataRelay: teamSelector_allTeams_query$key;
  onChange: (id?: string) => void;
};

const allId = 'kkigMVsUXwi2YMSSrXv7i';

const TeamSelector = ({ rootDataRelay, onChange }: Props) => {
  const rootData = useFragment<teamSelector_allTeams_query$key>(
    graphql`
      fragment teamSelector_allTeams_query on Query {
        teams(where: { organizationId: $organizationId }) {
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

  const [id, setId] = useState<string>(allId);
  const allItems = useMemo(() => (rootData.teams?.edges ? rootData.teams.edges.map(({ node }) => node) : []), [rootData.teams]);

  const handleChanged = (event: SelectChangeEvent) => {
    const id = event.target.value as string;

    setId(id);
    onChange(id === allId ? undefined : id);
  };

  return (
    <Select
      value={id}
      onChange={handleChanged}
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
        const selectedItem = allItems.find((item) => item.id === selectedId);
        if (selectedItem) {
          return (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <TeamIcon />
              <Typography variant="h6">Team</Typography>
              <Divider orientation="vertical" flexItem />
              <Typography variant="body1">{selectedItem.name}</Typography>
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
      <MenuItem value={allId}>
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <Typography variant="h6">All</Typography>
        </Stack>
      </MenuItem>

      {allItems.map((team) => (
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
