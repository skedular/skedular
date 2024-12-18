import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import { SelectChangeEvent } from '@mui/material/Select';
import { TeamAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@repo/shared/components/commons';
import { TeamIcon } from '@repo/shared/components/icons';
import { DefaultSelect } from '@repo/shared/components/styled';
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

  const handleChanged = (event: SelectChangeEvent<unknown>) => {
    const id = event.target.value as string;

    setId(id);
    onChange(id === allId ? undefined : id);
  };

  return (
    <DefaultSelect
      value={id}
      onChange={handleChanged}
      size="small"
      renderValue={(selectedId) => {
        const selectedItem = allItems.find((item) => item.id === selectedId);
        if (selectedItem) {
          return (
            <StackRow sx={{ alignItems: 'center' }}>
              <LeadIconTypography label="Team" icon={<TeamIcon />} />
              <Divider orientation="vertical" flexItem />
              <PushToRight />
              <SmallIconTypography label={selectedItem.name} />
            </StackRow>
          );
        }

        return (
          <StackRow sx={{ alignItems: 'center' }}>
            <LeadIconTypography label="Team" icon={<TeamIcon />} />
            <Divider orientation="vertical" flexItem />
            <PushToRight />
            <SmallIconTypography label="All" />
          </StackRow>
        );
      }}
    >
      <MenuItem value={allId}>
        <BodyIconTypography label="All" />
      </MenuItem>

      {allItems.map((item) => (
        <MenuItem key={item.id} value={item.id}>
          <BodyIconTypography icon={<TeamAvatar name={{ name: item.name }} size="small" />} label={item.name} />
        </MenuItem>
      ))}
    </DefaultSelect>
  );
};

export default memo(TeamSelector);
