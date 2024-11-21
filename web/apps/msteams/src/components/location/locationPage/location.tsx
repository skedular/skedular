import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { UpdateBreadcrumpsContext } from '@repo/shared/libs/providers';
import graphql from 'babel-plugin-relay/macro';
import { Bookings } from 'components/booking/bookingsPage';
import { getLocationBaseLink, LocationLink } from 'components/location';
import { getOrganizationBaseLink } from 'components/organization';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { useSearchParams } from 'react-router-dom';
import type { location_rootQuery } from './__generated__/location_rootQuery.graphql';
import LocationAboutTab from './location-about-tab';
import LocationAnalyticsTab from './location-analytics-tab';
import LocationDesksTab from './location-desks-tab';
import LocationZonesTab from './location-zones-tab';

type Props = {
  queryReference: PreloadedQuery<location_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
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

const Location = ({ queryReference, onReloadRequired, locationId, organizationId }: Props) => {
  const rootData = usePreloadedQuery<location_rootQuery>(RootQuery, queryReference);
  const [searchParams, setSearchParams] = useSearchParams();
  const tab = searchParams.get('tab');
  const updateBreadcrumps = useContext(UpdateBreadcrumpsContext);
  let initialTabIndex = 0;

  useEffect(() => {
    let breadcrumbs = new Map<string, string>();

    if (rootData.organization) {
      breadcrumbs = breadcrumbs.set(getOrganizationBaseLink(rootData.organization.id), rootData.organization?.name!);
    }

    if (rootData.location && rootData.organization) {
      breadcrumbs = breadcrumbs.set(getLocationBaseLink(rootData.location.id, rootData.organization.id), rootData.location?.name!);
    }

    updateBreadcrumps(breadcrumbs);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rootData.organization, rootData.location]);

  let tabCount = 0;
  const bookingTabIndex = tabCount++;
  const aboutTabIndex = tabCount++;
  const zonesTabIndex = tabCount++;
  const desksTabIndex = tabCount++;
  const analyticsTabIndex = rootData.location?.canViewAnalytics ? tabCount++ : -1;

  if (tab === 'bookings') {
    initialTabIndex = bookingTabIndex;
  } else if (tab === 'about') {
    initialTabIndex = aboutTabIndex;
  } else if (tab === 'zones') {
    initialTabIndex = zonesTabIndex;
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
    } else if (newValue === zonesTabIndex) {
      tab = 'zones';
    } else if (newValue === desksTabIndex) {
      tab = 'desks';
    } else if (newValue === analyticsTabIndex) {
      tab = 'analytics';
    }

    if (tab) {
      setSearchParams({ tab });
    }
  };

  if (!rootData.location) {
    return null;
  }

  return (
    <>
      <LocationLink organizationId={organizationId} id={locationId} name={rootData.location?.name} excludeLink />

      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Bookings" />
        <Tab label="About" />
        <Tab label="Zones" />
        <Tab label="Desks" />
        {rootData.location.canViewAnalytics && <Tab label="Analytics" />}
      </Tabs>

      <>
        {tabIndex === bookingTabIndex && <Bookings onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} />}
        {tabIndex === aboutTabIndex && (
          <LocationAboutTab onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} />
        )}
        {tabIndex === zonesTabIndex && <LocationZonesTab onReloadRequired={onReloadRequired} locationId={locationId} />}
        {tabIndex === desksTabIndex && <LocationDesksTab onReloadRequired={onReloadRequired} locationId={locationId} />}
        {tabIndex === analyticsTabIndex && rootData.location.canViewAnalytics && (
          <LocationAnalyticsTab
            onReloadRequired={onReloadRequired}
            organizationId={organizationId}
            locationId={locationId}
            locationName={rootData.location.name}
          />
        )}
      </>
    </>
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
        organizationId,
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
