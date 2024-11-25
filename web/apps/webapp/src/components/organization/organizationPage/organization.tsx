import { NewBookingButton } from '@/components/booking/addBooking';
import { MyBookings } from '@/components/booking/myBookings';
import { GettingStarted } from '@/components/gettingStarted';
import { LocationSelector } from '@/components/location/locationSelector';
import { TeamSelector } from '@/components/team/teamSelector';
import type { organization_rootQuery } from '@/queries/__generated__/organization_rootQuery.graphql';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import { WeekRangePicker } from '@repo/shared/components/datePickers';
import { ListGridToggle } from '@repo/shared/components/listGridToggle';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { defaultPadding } from '@repo/shared/libs/theme';
import { endOfWeek, startOfDay, startOfWeek } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<organization_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  defaultStartWeek: Dayjs;
};

const RootQuery = graphql`
  query organization_rootQuery(
    $organizationId: String!
    $locationIds: [String!]!
    $teamIds: [String!]!
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
  ) {
    organization(id: $organizationId) {
      id
      name
    }
    myLocations(organizationId: $organizationId) {
      id
      name
      organization {
        uniqueId
        name
      }
    }
    myTeams(organizationId: $organizationId) {
      id
      name
      organization {
        uniqueId
        name
      }
    }
    ...locationSelector_allLocations_query
    ...teamSelector_allTeams_query
    ...gettingStarted_query
    ...myBookings_query
    ...myBookings_bookings_query
  }
`;

const Dashboard = ({ queryReference, onReloadRequired, organizationId, defaultStartWeek }: Props) => {
  const rootData = usePreloadedQuery<organization_rootQuery>(RootQuery, queryReference);
  const [today] = useState(startOfDay());
  const [startWeek, setStartWeek] = useState(defaultStartWeek);
  const [endWeek, setEndWeek] = useState(endOfWeek(defaultStartWeek).add(-1, 'milliseconds'));
  const [locationIds, setLocationIds] = useState<string[]>([]);
  const [teamIds, setTeamIds] = useState<string[]>([]);
  const [viewMode, setViewMode] = useState<'list' | 'grid'>('grid');

  const handleWeehChanged = (date: Dayjs) => {
    setStartWeek(date);
    setEndWeek(endOfWeek(date).add(-1, 'milliseconds'));
  };

  const handlLocationChanged = (id?: string) => {
    setLocationIds(id ? [id] : []);
  };

  const handlTeamChanged = (id?: string) => {
    setTeamIds(id ? [id] : []);
  };

  const handlViewModeChanged = (newViewMode: 'list' | 'grid') => {
    setViewMode(newViewMode);
  };

  if (!rootData.myTeams || !rootData.myLocations) {
    return <></>;
  }

  if (!organizationId) {
    return <></>;
  }

  return (
    <Stack direction="column" spacing={1}>
      <Stack
        direction="row"
        spacing={1}
        sx={{
          alignItems: 'center',
          flexWrap: 'wrap',
          paddingLeft: defaultPadding,
          paddingRight: defaultPadding,
          paddingBottom: defaultPadding,
          paddingTop: defaultPadding,
        }}
      >
        <LocationSelector rootDataRelay={rootData} onChange={handlLocationChanged} />
        <TeamSelector rootDataRelay={rootData} onChange={handlTeamChanged} />
        <WeekRangePicker defaultStartWeek={startWeek} onWeekChanged={handleWeehChanged} disablePastWeeksSelection />
        <ListGridToggle defaultValue={viewMode} onChange={handlViewModeChanged} />
        <Box sx={{ flexGrow: 1 }} /> {/* This will push NewBookingButton to the right */}
        <NewBookingButton
          hideLocationControl={false}
          hideOrganizationControl={true}
          onReloadRequired={onReloadRequired}
          defaultDate={today}
          organizationId={organizationId}
        />
      </Stack>
      <GettingStarted rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationId={organizationId} />
      <MyBookings
        rootDataRelay={rootData}
        rootDataBookingRelay={rootData}
        onReloadRequired={onReloadRequired}
        from={startWeek}
        to={endWeek}
        locationIds={locationIds}
        teamIds={teamIds}
        viewMode={viewMode}
      />
    </Stack>
  );
};

const MemoDashboard = memo(Dashboard);

type RelayProps = {
  organizationId: string;
};

const DashboardWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organization_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();
  const [startWeek] = useState(startOfWeek());

  useEffect(() => {
    const bookingsSearchCriteriaFrom = startWeek.toISOString();
    const bookingsSearchCriteriaTo = endOfWeek(startWeek).add(-1, 'milliseconds').toISOString();

    loadQuery(
      {
        organizationId,
        bookingsSearchCriteriaFrom,
        bookingsSearchCriteriaTo,
        locationIds: [],
        teamIds: [],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload, startWeek, organizationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReload(triggerReload + 1);
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoDashboard
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationId={organizationId}
        defaultStartWeek={startWeek}
      />
    </ErrorBoundary>
  );
};

export default memo(DashboardWithRelay);
