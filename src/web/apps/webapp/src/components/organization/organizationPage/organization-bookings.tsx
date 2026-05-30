import { RelayError, endOfWeek, startOfDay, startOfWeek, toRootError } from '@skedular/shared';
import { NewBookingButton } from '@/components/booking/addBooking';
import { Bookings } from '@/components/booking/bookings';
import { GridContainer, StackColumn } from '@skedular/ui';
import { WeekRangePicker } from '@/components/datePickers';
import { Loading } from '@/components/loading';
import { LocationSelector } from '@/components/location/locationSelector';
import { OrganizationUserSelector } from '@/components/organization/organizationUserSelector';

import { TeamSelector } from '@/components/team/teamSelector';

import type { organizationBookings_rootQuery } from '@/queries/__generated__/organizationBookings_rootQuery.graphql';
import Box from '@mui/system/Box';
import { Dayjs } from 'dayjs';
import { useSearchParams } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

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
    marketplaceBookingSubscriptionCancellationModes {
      type
      name
    }
    marketplaceBookingSubscriptions(first: 100, where: { organizationCustomDomain: $organizationCustomDomain }) {
      edges {
        node {
          id
          recurringBookings {
            id
          }
        }
      }
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

  if (!rootData.myTeams || !rootData.myLocations) {
    return null;
  }

  if (!organizationCustomDomain) {
    return null;
  }

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center' }}>
      <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto' }} spacing={2}>
        <Bookings
          rootDataRelay={rootData}
          rootDataBookingRelay={rootData}
          organizationCustomDomain={organizationCustomDomain}
          from={startWeek}
          to={endWeek}
          locationIds={locationIds}
          teamIds={teamIds}
          customerIds={customerIds}
          toolbar={
            <GridContainer spacing={1}>
              <OrganizationUserSelector rootDataOrganizationMembersRelay={rootData} onChange={handlCustomerChanged} defaultValue={customerId} />
              <LocationSelector rootDataRelay={rootData} onChange={handlLocationChanged} defaultValue={locationId} />
              <TeamSelector rootDataRelay={rootData} onChange={handlTeamChanged} defaultValue={teamId} />
              <WeekRangePicker defaultStartWeek={startWeek} onWeekChanged={handleWeehChanged} />
            </GridContainer>
          }
          actions={<NewBookingButton onReloadRequired={onReloadRequired} defaultDate={today} organizationCustomDomain={organizationCustomDomain} />}
        />
      </StackColumn>
    </Box>
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
