import { NewBookingButton } from '@/components/booking/addBooking';
import { Bookings } from '@/components/booking/bookings';
import { GridContainer, PushToRight, StackColumn } from '@/components/commons';
import { WeekRangePicker } from '@/components/datePickers';
import { ListGridToggle } from '@/components/listGridToggle';
import { Loading } from '@/components/loading';
import { LocationSelector } from '@/components/location/locationSelector';
import { OrganizationUserSelector } from '@/components/organization/organizationUserSelector';
import { RelayError, toRootError } from '@/components/relayError';
import { TeamSelector } from '@/components/team/teamSelector';
import { defaultPadding, maxScreenWidth } from '@/libs/theme';
import { endOfWeek, startOfDay, startOfWeek } from '@/libs/utils';
import type { organizationBookings_rootQuery } from '@/queries/__generated__/organizationBookings_rootQuery.graphql';
import { Dayjs } from 'dayjs';
import Box from '@mui/system/Box';
import type { SxProps, Theme } from '@mui/system';
import { useSearchParams } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

const filterSurfaceSx: SxProps<Theme> = {
  borderRadius: 4,
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
  boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : '0 1px 3px rgba(0, 0, 0, 0.24)'),
};

type Props = {
  queryReference: PreloadedQuery<organizationBookings_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  customerId?: string | null;
  locationId?: string | null;
  teamId?: string | null;
  defaultStartWeek: Dayjs;
};

const RootQuery = graphql`
  query organizationBookings_rootQuery(
    $organizationCustomDomain: String!
    $locationIds: [String!]!
    $teamIds: [String!]!
    $customerIds: [String!]!
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
    $locationsSortingValues: [LocationOrderInput!]
    $peopleNameSearchText: String
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
  ) {
    organization(customDomain: $organizationCustomDomain) {
      id
      name
    }
    myLocations(organizationCustomDomain: $organizationCustomDomain) {
      id
      name
      organization {
        id
        name
      }
    }
    myTeams(organizationCustomDomain: $organizationCustomDomain) {
      id
      name
      organization {
        id
        name
      }
    }
    ...organizationUserSelector_organizationMembers_query
    ...locationSelector_allLocations_query
    ...teamSelector_allTeams_query
    ...bookings_query
    ...bookings_bookings_query
  }
`;

const OrganizationBookings = ({ queryReference, onReloadRequired, organizationCustomDomain, customerId, locationId, teamId, defaultStartWeek }: Props) => {
  const rootData = usePreloadedQuery<organizationBookings_rootQuery>(RootQuery, queryReference);
  const [today] = useState(startOfDay());
  const [startWeek, setStartWeek] = useState(defaultStartWeek);
  const [endWeek, setEndWeek] = useState(endOfWeek(defaultStartWeek).add(-1, 'milliseconds'));
  const [customerIds, setCustomerIds] = useState<string[]>(customerId ? [customerId] : []);
  const [locationIds, setLocationIds] = useState<string[]>(locationId ? [locationId] : []);
  const [teamIds, setTeamIds] = useState<string[]>(teamId ? [teamId] : []);
  const [viewMode, setViewMode] = useState<'list' | 'grid'>('grid');

  const handleWeehChanged = (date: Dayjs) => {
    setStartWeek(date);
    setEndWeek(endOfWeek(date).add(-1, 'milliseconds'));
  };

  const handlCustomerChanged = (id?: string) => {
    setCustomerIds(id ? [id] : []);
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
    return null;
  }

  if (!organizationCustomDomain) {
    return null;
  }

  return (
    <StackColumn sx={{ maxWidth: maxScreenWidth, width: '100%' }} spacing={2}>
      <Box sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
        <Box sx={{ ...filterSurfaceSx, px: 2, py: 1.5 }}>
          <GridContainer spacing={1}>
            <OrganizationUserSelector rootDataOrganizationMembersRelay={rootData} onChange={handlCustomerChanged} defaultValue={customerId} />
            <LocationSelector rootDataRelay={rootData} onChange={handlLocationChanged} defaultValue={locationId} />
            <TeamSelector rootDataRelay={rootData} onChange={handlTeamChanged} defaultValue={teamId} />
            <WeekRangePicker defaultStartWeek={startWeek} onWeekChanged={handleWeehChanged} />
            <ListGridToggle defaultValue={viewMode} onChange={handlViewModeChanged} />
            <PushToRight />
            <NewBookingButton onReloadRequired={onReloadRequired} defaultDate={today} organizationCustomDomain={organizationCustomDomain} />
          </GridContainer>
        </Box>
      </Box>
      <Bookings
        rootDataRelay={rootData}
        rootDataBookingRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationCustomDomain={organizationCustomDomain}
        from={startWeek}
        to={endWeek}
        locationIds={locationIds}
        teamIds={teamIds}
        customerIds={customerIds}
        viewMode={viewMode}
      />
    </StackColumn>
  );
};

const MemoOrganizationBookings = memo(OrganizationBookings);

type RelayProps = {
  organizationCustomDomain: string;
  customerId?: string | null;
  locationId?: string | null;
  teamId?: string | null;
};

const ModernOrganizationWithRelay = ({ organizationCustomDomain }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationBookings_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();
  const [startWeek] = useState(startOfWeek());
  const searchParams = useSearchParams();
  const customerId = searchParams.get('customerId');
  const locationId = searchParams.get('locationId');
  const teamId = searchParams.get('teamId');

  useEffect(() => {
    const bookingsSearchCriteriaFrom = startWeek.toISOString();
    const bookingsSearchCriteriaTo = endOfWeek(startWeek).add(-1, 'milliseconds').toISOString();

    loadQuery(
      {
        organizationCustomDomain,
        bookingsSearchCriteriaFrom,
        bookingsSearchCriteriaTo,
        locationIds: locationId ? [locationId] : [],
        teamIds: teamId ? [teamId] : [],
        customerIds: customerId ? [customerId] : [],
        locationsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        organizationMembersSortingValues: [
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
  }, [loadQuery, triggerReload, startWeek, organizationCustomDomain, locationId, teamId, customerId]);

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
      <MemoOrganizationBookings
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationCustomDomain={organizationCustomDomain}
        customerId={customerId}
        locationId={locationId}
        teamId={teamId}
        defaultStartWeek={startWeek}
      />
    </ErrorBoundary>
  );
};

export default memo(ModernOrganizationWithRelay);
