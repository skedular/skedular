import { TeamAvatar } from '@/components/avatars';
import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@/components/commons';
import { TeamIcon } from '@/components/icons';
import { DefaultSelect } from '@/components/styled';
import type { teamSelector_allTeams_query$key } from '@/queries/__generated__/teamSelector_allTeams_query.graphql';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import { SelectChangeEvent } from '@mui/material/Select';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: teamSelector_allTeams_query$key;
  onChange: (id?: string) => void;
  defaultValue?: string | null;
};

const allId = 'kkigMVsUXwi2YMSSrXv7i';

const TeamSelector = ({ rootDataRelay, onChange, defaultValue }: Props) => {
  const rootData = useFragment<teamSelector_allTeams_query$key>(
    graphql`
      fragment teamSelector_allTeams_query on Query {
        teams(where: { organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName }) {
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

  const allItems = useMemo(() => (rootData.teams?.edges ? rootData.teams.edges.map(({ node }) => node) : []), [rootData.teams]);
  const [id, setId] = useState<string>(defaultValue ?? allId);

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
              <LeadIconTypography label="Team" startElement={<TeamIcon excludeTooltip />} />
              <Divider orientation="vertical" flexItem />
              <PushToRight />
              <SmallIconTypography label={selectedItem.name} />
            </StackRow>
          );
        }

        return (
          <StackRow sx={{ alignItems: 'center' }}>
            <LeadIconTypography label="Team" startElement={<TeamIcon excludeTooltip />} />
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
          <BodyIconTypography startElement={<TeamAvatar name={{ name: item.name }} size="small" />} label={item.name} />
        </MenuItem>
      ))}
    </DefaultSelect>
  );
};

export default memo(TeamSelector);
