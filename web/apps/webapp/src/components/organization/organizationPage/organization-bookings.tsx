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
import { useSearchParams } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<organizationBookings_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
  customerId?: string | null;
  locationId?: string | null;
  teamId?: string | null;
  defaultStartWeek: Dayjs;
};

const RootQuery = graphql`
  query organizationBookings_rootQuery(
    $organizationUniqueAlphanumericName: String!
    $locationIds: [String!]!
    $teamIds: [String!]!
    $customerIds: [String!]!
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
    $locationsSortingValues: [LocationOrderInput!]
    $peopleNameSearchText: String
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
  ) {
    organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
      id
      name
    }
    myLocations(organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName) {
      id
      name
      organization {
        id
        name
      }
    }
    myTeams(organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName) {
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

const OrganizationBookings = ({ queryReference, onReloadRequired, organizationUniqueAlphanumericName, customerId, locationId, teamId, defaultStartWeek }: Props) => {
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

  if (!organizationUniqueAlphanumericName) {
    return null;
  }

  return (
    <StackColumn sx={{ maxWidth: maxScreenWidth }}>
      <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
        <OrganizationUserSelector rootDataOrganizationMembersRelay={rootData} onChange={handlCustomerChanged} defaultValue={customerId} />
        <LocationSelector rootDataRelay={rootData} onChange={handlLocationChanged} defaultValue={locationId} />
        <TeamSelector rootDataRelay={rootData} onChange={handlTeamChanged} defaultValue={teamId} />
        <WeekRangePicker defaultStartWeek={startWeek} onWeekChanged={handleWeehChanged} />
        <ListGridToggle defaultValue={viewMode} onChange={handlViewModeChanged} />
        <PushToRight />
        <NewBookingButton onReloadRequired={onReloadRequired} defaultDate={today} organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />
      </GridContainer>
      <Bookings
        rootDataRelay={rootData}
        rootDataBookingRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
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
  organizationUniqueAlphanumericName: string;
  customerId?: string | null;
  locationId?: string | null;
  teamId?: string | null;
};

const ModernOrganizationWithRelay = ({ organizationUniqueAlphanumericName }: RelayProps) => {
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
        organizationUniqueAlphanumericName,
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
  }, [loadQuery, triggerReload, startWeek, organizationUniqueAlphanumericName, locationId, teamId, customerId]);

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
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
        customerId={customerId}
        locationId={locationId}
        teamId={teamId}
        defaultStartWeek={startWeek}
      />
    </ErrorBoundary>
  );
};

export default memo(ModernOrganizationWithRelay);
