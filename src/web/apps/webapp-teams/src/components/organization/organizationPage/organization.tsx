import { RelayError, endOfWeek, startOfDay, startOfWeek, toRootError } from '@skedular/shared';
import { NewBookingButton } from '@/components/booking/addBooking';
import { MyBookings } from '@/components/booking/myBookings';
import { WeekRangePicker } from '@/components/datePickers';
import { GettingStarted } from '@/components/gettingStarted';
import { Loading } from '@/components/loading';
import { LocationSelector } from '@/components/location/locationSelector';

import { TeamSelector } from '@/components/team/teamSelector';
import type { organization_rootQuery } from '@/queries/__generated__/organization_rootQuery.graphql';
import Box from '@mui/system/Box';

import { GridContainer, StackColumn } from '@skedular/ui';
import { Dayjs } from 'dayjs';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<organization_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  defaultStartWeek: Dayjs;
};

const RootQuery = graphql`
  query organization_rootQuery(
    $organizationCustomDomain: String!
    $locationIds: [String!]!
    $teamIds: [String!]!
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    organization(customDomain: $organizationCustomDomain) {
      canModify
    }
    ...locationSelector_allLocations_query
    ...teamSelector_allTeams_query
    ...gettingStarted_query
    ...myBookings_query
    ...myBookings_bookings_query
  }
`;

const Organization = ({ queryReference, onReloadRequired, organizationCustomDomain, defaultStartWeek }: Props) => {
  const rootData = usePreloadedQuery<organization_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const searchParams = useSearchParams();
  const locationId = searchParams.get('locationId');
  const teamId = searchParams.get('teamId');
  const [today] = useState(startOfDay());
  const startWeek = defaultStartWeek;
  const endWeek = endOfWeek(defaultStartWeek).add(-1, 'milliseconds');
  const locationIds = locationId ? [locationId] : [];
  const teamIds = teamId ? [teamId] : [];

  const updateFilterUrl = (updates: { locationId?: string; teamId?: string; weekStart?: string }) => {
    const params = new URLSearchParams(window.location.search);

    Object.entries(updates).forEach(([key, value]) => {
      if (value) params.set(key, value);
      else params.delete(key);
    });

    router.push(`?${params.toString()}`);
  };

  const handleWeehChanged = (date: Dayjs) => {
    updateFilterUrl({ weekStart: date.format('YYYY-MM-DD') });
  };

  const handlLocationChanged = (id?: string) => {
    updateFilterUrl({ locationId: id });
  };

  const handlTeamChanged = (id?: string) => {
    updateFilterUrl({ teamId: id });
  };

  if (!organizationCustomDomain) {
    return null;
  }

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center' }}>
      <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pt: { xs: 1, sm: 1, md: 2 } }} spacing={2}>
        <GettingStarted rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} />

        <MyBookings
          rootDataRelay={rootData}
          rootDataBookingRelay={rootData}
          organizationCustomDomain={organizationCustomDomain}
          from={startWeek}
          to={endWeek}
          locationIds={locationIds}
          teamIds={teamIds}
          toolbar={
            <GridContainer spacing={1}>
              <LocationSelector key={`location-${locationId ?? 'all'}`} rootDataRelay={rootData} onChange={handlLocationChanged} defaultValue={locationId} />
              <TeamSelector key={`team-${teamId ?? 'all'}`} rootDataRelay={rootData} onChange={handlTeamChanged} defaultValue={teamId} />
              <WeekRangePicker key={startWeek.format('YYYY-MM-DD')} defaultStartWeek={startWeek} onWeekChanged={handleWeehChanged} />
            </GridContainer>
          }
          hasTopInset={false}
          actions={<NewBookingButton onReloadRequired={onReloadRequired} defaultDate={today} organizationCustomDomain={organizationCustomDomain} />}
        />
      </StackColumn>
    </Box>
  );
};

const MemoOrganization = memo(Organization);

type RelayProps = {
  organizationCustomDomain: string;
};

const OrganizationWithRelay = ({ organizationCustomDomain }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organization_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();
  const defaultWeek = useMemo(() => startOfWeek(), []);
  const searchParams = useSearchParams();
  const locationId = searchParams.get('locationId');
  const teamId = searchParams.get('teamId');
  const weekStart = searchParams.get('weekStart');
  const parsedWeekStart = useMemo(() => (weekStart ? startOfWeek(weekStart) : defaultWeek), [defaultWeek, weekStart]);

  useEffect(() => {
    const bookingsSearchCriteriaFrom = parsedWeekStart.toISOString();
    const bookingsSearchCriteriaTo = endOfWeek(parsedWeekStart).add(-1, 'milliseconds').toISOString();

    loadQuery(
      {
        organizationCustomDomain,
        bookingsSearchCriteriaFrom,
        bookingsSearchCriteriaTo,
        locationIds: locationId ? [locationId] : [],
        teamIds: teamId ? [teamId] : [],
        locationsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload, parsedWeekStart, organizationCustomDomain, locationId, teamId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReload(triggerReload + 1);
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoOrganization
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationCustomDomain={organizationCustomDomain}
        defaultStartWeek={parsedWeekStart}
      />
    </ErrorBoundary>
  );
};

export default memo(OrganizationWithRelay);
