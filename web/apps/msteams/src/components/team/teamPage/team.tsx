import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import Typography from '@mui/material/Typography';
import graphql from 'babel-plugin-relay/macro';
import { memo, useState } from 'react';
import { useFragment } from 'react-relay';
import { useLocation, useNavigate, useSearchParams } from 'react-router-dom';
import TeamAvatar from '../team-avatar';
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

  const [searchParams] = useSearchParams();
  const location = useLocation();
  const tab = searchParams.get('tab');
  const navigate = useNavigate();

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
      navigate(`${location.pathname}?tab=${tab}`);
    }
  };

  if (!rootData.team) {
    return null;
  }

  return (
    <Box sx={{ display: 'flex' }}>
      <Stack direction="column">
        <Stack direction="column">
          <TeamAvatar name={{ name: rootData.team?.name }} photo={{ url: null }} sx={{ marginBottom: 1 }} />
          <Typography gutterBottom variant="h6">
            {rootData.team?.name}
          </Typography>
        </Stack>
        <Tabs value={tabIndex} onChange={handleTabChange}>
          <Tab label="Bookings" />
          <Tab label="About" />
          <Tab label="People" />
        </Tabs>
        <Stack direction="column">
          {tabIndex === 0 && <TeamBookingsTab rootDataRelay={rootData} organizationId={organizationId} teamId={teamId} />}
          {tabIndex === 1 && <TeamAboutTab rootDataRelay={rootData} organizationId={organizationId} />}
          {tabIndex === 2 && <TeamPeopleTab rootDataRelay={rootData} organizationId={organizationId} />}
        </Stack>
      </Stack>
    </Box>
  );
};

export default memo(Team);
