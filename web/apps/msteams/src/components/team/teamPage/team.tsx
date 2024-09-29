import Stack from '@mui/material/Stack';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import Typography from '@mui/material/Typography';
import { TeamAvatar } from '@repo/shared/components/avatars';
import graphql from 'babel-plugin-relay/macro';
import { memo, useState } from 'react';
import { useFragment } from 'react-relay';
import { useSearchParams } from 'react-router-dom';
import type { teamPage_query$key } from './__generated__/teamPage_query.graphql';
import TeamAboutTab from './team-about-tab';
import TeamBookingsTab from './team-bookings-tab';
import TeamPeopleTab from './team-people-tab';

type Props = {
  rootDataRelay: teamPage_query$key;
  teamId: string;
  organizationId: string;
};

const Team = ({ rootDataRelay, teamId, organizationId }: Props) => {
  const rootData = useFragment<teamPage_query$key>(
    graphql`
      fragment teamPage_query on Query {
        team(id: $teamId) {
          name
        }
        ...teamBookingsTab_query
        ...teamAboutTab_query
        ...teamPeopleTab_query
      }
    `,
    rootDataRelay,
  );

  const [searchParams, setSearchParams] = useSearchParams();
  const tab = searchParams.get('tab');

  let initialTabIndex = 0;

  if (tab === 'bookings') {
    initialTabIndex = 0;
  } else if (tab === 'about') {
    initialTabIndex = 1;
  } else if (tab === 'people') {
    initialTabIndex = 2;
  }

  const [tabIndex, setTabIndex] = useState(initialTabIndex);

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabIndex(newValue);

    let tab = '';

    if (newValue === 0) {
      tab = 'bookings';
    } else if (newValue === 1) {
      tab = 'about';
    } else if (newValue === 2) {
      tab = 'people';
    }

    if (tab) {
      setSearchParams({ tab });
    }
  };

  if (!rootData.team) {
    return null;
  }

  return (
    <Stack direction="column" spacing={1}>
      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
        <TeamAvatar name={{ name: rootData.team?.name }} photo={{ url: null }} sx={{ marginBottom: 1 }} />
        <Typography variant="h6">{rootData.team?.name}</Typography>
      </Stack>

      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Bookings" />
        <Tab label="About" />
        <Tab label="People" />
      </Tabs>

      <>
        {tabIndex === 0 && <TeamBookingsTab rootDataRelay={rootData} organizationId={organizationId} teamId={teamId} />}
        {tabIndex === 1 && <TeamAboutTab rootDataRelay={rootData} organizationId={organizationId} />}
        {tabIndex === 2 && <TeamPeopleTab rootDataRelay={rootData} organizationId={organizationId} />}
      </>
    </Stack>
  );
};

export default memo(Team);
