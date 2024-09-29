import type { locationPage_query$key } from '@/queries/__generated__/locationPage_query.graphql';
import Stack from '@mui/material/Stack';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import Typography from '@mui/material/Typography';
import { LocationAvatar } from '@repo/shared/components/avatars';
import { getCurrentCompleteUrl } from '@repo/shared/libs/utils';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import LocationAboutTab from './location-about-tab';
import LocationAnalyticsTab from './location-analytics-tab';
import LocationBookingsTab from './location-bookings-tab';
import LocationDesksTab from './location-desks-tab';
import LocationPeopleTab from './location-people-tab';
import LocationZonesTab from './location-zones-tab';

type Props = {
  rootDataRelay: locationPage_query$key;
  locationId: string;
  organizationId: string;
};

const Location = ({ rootDataRelay, locationId, organizationId }: Props) => {
  const rootData = useFragment<locationPage_query$key>(
    graphql`
      fragment locationPage_query on Query {
        location(id: $locationId) {
          name
          canViewAnalytics
        }
        ...locationBookingsTab_query
        ...locationAboutTab_query
        ...locationPeopleTab_query
        ...locationPeopleTab_query_organizationMembers
        ...locationZonesTab_query
        ...locationDesksTab_query
      }
    `,
    rootDataRelay,
  );

  const searchParams = useSearchParams();
  const tab = searchParams.get('tab');
  const router = useRouter();

  let initialTabIndex = 0;

  if (tab === 'bookings') {
    initialTabIndex = 0;
  } else if (tab === 'about') {
    initialTabIndex = 1;
  } else if (tab === 'people') {
    initialTabIndex = 2;
  } else if (tab === 'zones') {
    initialTabIndex = 3;
  } else if (tab === 'desks') {
    initialTabIndex = 4;
  } else if (tab === 'analytics') {
    initialTabIndex = 5;
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
    } else if (newValue === 3) {
      tab = 'zones';
    } else if (newValue === 4) {
      tab = 'desks';
    } else if (newValue === 5) {
      tab = 'analytics';
    }

    if (tab) {
      router.push(`${getCurrentCompleteUrl()}?tab=${tab}`);
    }
  };

  if (!rootData.location) {
    return null;
  }

  return (
    <Stack direction="column" spacing={1}>
      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
        <LocationAvatar name={{ name: rootData.location?.name }} photo={{ url: null }} sx={{ marginBottom: 1 }} />
        <Typography variant="h6">{rootData.location?.name}</Typography>
      </Stack>

      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Bookings" />
        <Tab label="About" />
        <Tab label="People" />
        <Tab label="Zones" />
        <Tab label="Desks" />
        {rootData.location.canViewAnalytics && <Tab label="Analytics" />}
      </Tabs>

      <>
        {tabIndex === 0 && <LocationBookingsTab rootDataRelay={rootData} organizationId={organizationId} locationId={locationId} />}
        {tabIndex === 1 && <LocationAboutTab rootDataRelay={rootData} organizationId={organizationId} />}
        {tabIndex === 2 && (
          <LocationPeopleTab rootDataLocationMembersRelay={rootData} rootDataOrganizationMembersRelay={rootData} organizationId={organizationId} />
        )}
        {tabIndex === 3 && <LocationZonesTab rootDataRelay={rootData} locationId={locationId} />}
        {tabIndex === 4 && <LocationDesksTab rootDataRelay={rootData} locationId={locationId} />}
        {tabIndex === 5 && rootData.location.canViewAnalytics && <LocationAnalyticsTab organizationId={organizationId} locationId={locationId} />}
      </>
    </Stack>
  );
};

export default memo(Location);
