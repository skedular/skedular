import { NewBookingButton } from '@/components/booking/addBooking';
import { Bookings } from '@/components/booking/bookings';
import { LocationSelector } from '@/components/location/locationSelector';
import { TeamSelector } from '@/components/team/teamSelector';
import type { organizationBookings_rootQuery } from '@/queries/__generated__/organizationBookings_rootQuery.graphql';
import { GridContainer, PushToRight, StackColumn } from '@repo/shared/components/commons';
import { WeekRangePicker } from '@repo/shared/components/datePickers';
import { ListGridToggle } from '@repo/shared/components/listGridToggle';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { defaultPadding, maxScreenWidth } from '@repo/shared/libs/theme';
import { endOfWeek, startOfDay, startOfWeek } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { useSearchParams } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import OrganizationUserSelector from '../organizationUserSelector/organization-user-selector';

type Props = {
  queryReference: PreloadedQuery<organizationBookings_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  customerId?: string | null;
  locationId?: string | null;
  teamId?: string | null;
  defaultStartWeek: Dayjs;
};

const RootQuery = graphql`
  query organizationBookings_rootQuery(
    $organizationId: String!
    $nullableOrganizationId: String
    $locationIds: [String!]!
    $teamIds: [String!]!
    $customerIds: [String!]!
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
    $locationsSortingValues: [LocationOrderInput!]
    $peopleNameSearchText: String
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
  ) {
    organization(id: $organizationId) {
      id
      name
    }
    myLocations(organizationId: $nullableOrganizationId) {
      id
      name
      organization {
        uniqueId
        name
      }
    }
    myTeams(organizationId: $nullableOrganizationId) {
      id
      name
      organization {
        uniqueId
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

const ModernOrganization = ({ queryReference, onReloadRequired, organizationId, customerId, locationId, teamId, defaultStartWeek }: Props) => {
  const rootData = usePreloadedQuery<organizationBookings_rootQuery>(RootQuery, queryReference);
  const [today] = useState(startOfDay());
  const [startWeek, setStartWeek] = useState(defaultStartWeek);
  const [endWeek, setEndWeek] = useState(endOfWeek(defaultStartWeek).add(-1, 'milliseconds'));
  const [customerIds, setCustomerIds] = useState<string[]>(customerId ? [customerId] : []);
  const [locationIds, setLocationIds] = useState<string[]>(locationId ? [locationId] : []);
  const [teamIds, setTeamIds] = useState<string[]>(teamId ? [teamId] : []);
  const [viewMode, setViewMode] = useState<'list' | 'grid'>('list');

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
    return <></>;
  }

  if (!organizationId) {
    return <></>;
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
        <NewBookingButton onReloadRequired={onReloadRequired} defaultDate={today} organizationId={organizationId} />
      </GridContainer>
      <Bookings
        rootDataRelay={rootData}
        rootDataBookingRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationId={organizationId}
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

const MemoModernOrganization = memo(ModernOrganization);

type RelayProps = {
  organizationId: string;
  customerId?: string | null;
  locationId?: string | null;
  teamId?: string | null;
};

const ModernOrganizationWithRelay = ({ organizationId }: RelayProps) => {
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
        organizationId,
        nullableOrganizationId: organizationId,
        bookingsSearchCriteriaFrom,
        bookingsSearchCriteriaTo,
        locationIds: locationId ? [locationId] : [],
        teamIds: teamId ? [teamId] : [],
        customerIds: customerId ? [customerId] : [],
        locationsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        organizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload, startWeek, organizationId, locationId, teamId, customerId]);

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
      <MemoModernOrganization
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationId={organizationId}
        customerId={customerId}
        locationId={locationId}
        teamId={teamId}
        defaultStartWeek={startWeek}
      />
    </ErrorBoundary>
  );
};

export default memo(ModernOrganizationWithRelay);
