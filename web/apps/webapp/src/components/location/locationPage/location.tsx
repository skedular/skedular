import { Bookings } from '@/components/booking/bookingsPage';
import { LocationLink } from '@/components/location';
import { OrganizationDeskTypes } from '@/components/organization/organizationPage';
import type { location_rootQuery } from '@/queries/__generated__/location_rootQuery.graphql';
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
import LocationMembersTab from './location-members-tab';
import LocationZonesTab from './location-zones-tab';

type Props = {
  queryReference: PreloadedQuery<location_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
  locationId: string;
};

const RootQuery = graphql`
  query location_rootQuery($organizationId: String!, $organizationExists: Boolean!, $locationId: String!) {
    organization(id: $organizationId) @include(if: $organizationExists) {
      id
      name
    }
    location(id: $locationId) {
      id
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

  let tabCount = 0;
  const bookingTabIndex = tabCount++;
  const aboutTabIndex = tabCount++;
  const membersTabIndex = organizationId ? -1 : tabCount++;
  const zonesTabIndex = tabCount++;
  const deskTypesTabIndex = organizationId ? tabCount++ : -1;
  const desksTabIndex = tabCount++;
  const analyticsTabIndex = rootData.location?.canViewAnalytics ? tabCount++ : -1;

  if (tab === 'bookings') {
    initialTabIndex = bookingTabIndex;
  } else if (tab === 'about') {
    initialTabIndex = aboutTabIndex;
  } else if (tab === 'members') {
    initialTabIndex = membersTabIndex;
  } else if (tab === 'zones') {
    initialTabIndex = zonesTabIndex;
  } else if (tab === 'deskTypes') {
    initialTabIndex = deskTypesTabIndex;
  } else if (tab === 'desks') {
    initialTabIndex = desksTabIndex;
  } else if (tab === 'analytics') {
    initialTabIndex = analyticsTabIndex;
  }

  const [tabIndex, setTabIndex] = useState(initialTabIndex);

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    setTabIndex(newValue);

    let tab = '';

    if (newValue === bookingTabIndex) {
      tab = 'bookings';
    } else if (newValue === aboutTabIndex) {
      tab = 'about';
    } else if (newValue === membersTabIndex) {
      tab = 'members';
    } else if (newValue === zonesTabIndex) {
      tab = 'zones';
    } else if (newValue === deskTypesTabIndex) {
      tab = 'deskTypes';
    } else if (newValue === desksTabIndex) {
      tab = 'desks';
    } else if (newValue === analyticsTabIndex) {
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
    <>
      <LocationLink organizationId={rootData.location.organization?.uniqueId} id={locationId} name={rootData.location?.name} excludeLink />

      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Bookings" />
        <Tab label="About" />
        {!organizationId && <Tab label="Members" />}
        <Tab label="Zones" />
        {organizationId && <Tab label="Desk Types" />}
        <Tab label="Desks" />
        {rootData.location.canViewAnalytics && <Tab label="Analytics" />}
      </Tabs>

      {tabIndex === bookingTabIndex && <Bookings onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} />}
      {tabIndex === aboutTabIndex && <LocationAboutTab onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} />}
      {tabIndex === membersTabIndex && !organizationId && <LocationMembersTab onReloadRequired={onReloadRequired} locationId={locationId} />}
      {tabIndex === zonesTabIndex && <LocationZonesTab onReloadRequired={onReloadRequired} locationId={locationId} />}
      {tabIndex === deskTypesTabIndex && organizationId && (
        <OrganizationDeskTypes onReloadRequired={onReloadRequired} organizationId={organizationId} />
      )}
      {tabIndex === desksTabIndex && <LocationDesksTab onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} />}
      {tabIndex === analyticsTabIndex && rootData.location.canViewAnalytics && (
        <LocationAnalyticsTab
          onReloadRequired={onReloadRequired}
          organizationId={organizationId}
          locationId={locationId}
          locationName={rootData.location.name}
        />
      )}
    </>
  );
};

const MemoLocation = memo(Location);

type RelayProps = {
  organizationId?: string;
  locationId: string;
};

const LocationWithRelay = ({ organizationId, locationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<location_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId: organizationId ?? '',
        organizationExists: !!organizationId,
        locationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId, locationId]);

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
