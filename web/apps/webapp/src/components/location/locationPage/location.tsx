import { Bookings } from '@/components/booking/bookingsPage';
import { LocationLink } from '@/components/location';
import type { location_rootQuery } from '@/queries/__generated__/location_rootQuery.graphql';
import Stack from '@mui/material/Stack';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { getCurrentCompleteUrl } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import LocationAboutTab from './location-about-tab';
import LocationAnalyticsTab from './location-analytics-tab';
import LocationDesksTab from './location-desks-tab';
import LocationPeopleTab from './location-people-tab';
import LocationZonesTab from './location-zones-tab';

type Props = {
  queryReference: PreloadedQuery<location_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  locationId: string;
};

const RootQuery = graphql`
  query location_rootQuery($locationId: String!) {
    location(id: $locationId) {
      name
      canViewAnalytics
      organization {
        uniqueId
      }
    }
  }
`;

const Location = ({ queryReference, onReloadRequired, organizationId, locationId }: Props) => {
  const rootData = usePreloadedQuery<location_rootQuery>(RootQuery, queryReference);
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
      <LocationLink organizationId={rootData.location.organization?.uniqueId} id={locationId} name={rootData.location?.name} excludeLink />

      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Bookings" />
        <Tab label="About" />
        <Tab label="People" />
        <Tab label="Zones" />
        <Tab label="Desks" />
        {rootData.location.canViewAnalytics && <Tab label="Analytics" />}
      </Tabs>

      <>
        {tabIndex === 0 && <Bookings onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} />}
        {tabIndex === 1 && <LocationAboutTab onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} />}
        {tabIndex === 2 && <LocationPeopleTab onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} />}
        {tabIndex === 3 && <LocationZonesTab onReloadRequired={onReloadRequired} locationId={locationId} />}
        {tabIndex === 4 && <LocationDesksTab onReloadRequired={onReloadRequired} locationId={locationId} />}
        {tabIndex === 5 && rootData.location.canViewAnalytics && (
          <LocationAnalyticsTab
            onReloadRequired={onReloadRequired}
            organizationId={organizationId}
            locationId={locationId}
            locationName={rootData.location.name}
          />
        )}
      </>
    </Stack>
  );
};

const MemoLocation = memo(Location);

type RelayProps = {
  organizationId: string;
  locationId: string;
};

const LocationWithRelay = ({ organizationId, locationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<location_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        locationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, locationId]);

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
      <MemoLocation queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} locationId={locationId} />
    </ErrorBoundary>
  );
};

export default memo(LocationWithRelay);
