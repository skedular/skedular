import { RelayError, endOfWeek, startOfDay, startOfWeek, toRootError } from '@skedular/shared';
import { NewBookingButton } from '@/components/booking/addBooking';
import { Bookings } from '@/components/booking/bookings';
import { GettingStarted } from '@/components/gettingStarted';
import { GridContainer, StackColumn } from '@skedular/ui';
import { WeekRangePicker } from '@/components/datePickers';
import { Loading } from '@/components/loading';
import { LocationSelector } from '@/components/location/locationSelector';
import { OrganizationUserSelector } from '@/components/organization/organizationUserSelector';

import type { organizationBookings_rootQuery } from '@/queries/__generated__/organizationBookings_rootQuery.graphql';
import Box from '@mui/system/Box';
import { Dayjs } from 'dayjs';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<organizationBookings_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  customerId?: string | null;
  locationId?: string | null;
  defaultStartWeek: Dayjs;
};

const RootQuery = graphql`
  query organizationBookings_rootQuery(
    $organizationCustomDomain: String!
    $locationIds: [String!]!
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
    ...organizationUserSelector_organizationMembers_query
    ...locationSelector_allLocations_query
    ...gettingStarted_query
    ...bookings_query
    ...bookings_bookings_query
  }
`;

const OrganizationBookings = ({ queryReference, onReloadRequired, organizationCustomDomain, customerId, locationId, defaultStartWeek }: Props) => {
  const rootData = usePreloadedQuery<organizationBookings_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const [today] = useState(startOfDay());
  const startWeek = defaultStartWeek;
  const endWeek = endOfWeek(defaultStartWeek).add(-1, 'milliseconds');
  const customerIds = customerId ? [customerId] : [];
  const locationIds = locationId ? [locationId] : [];

  const updateFilterUrl = (updates: { customerId?: string; locationId?: string; weekStart?: string }) => {
    const params = new URLSearchParams(window.location.search);

    Object.entries(updates).forEach(([key, value]) => {
      if (value) {
        params.set(key, value);
      } else {
        params.delete(key);
      }
    });

    router.push(`?${params.toString()}`);
  };

  const handleWeehChanged = (date: Dayjs) => {
    updateFilterUrl({ weekStart: date.format('YYYY-MM-DD') });
  };

  const handlCustomerChanged = (id?: string) => {
    updateFilterUrl({ customerId: id });
  };

  const handlLocationChanged = (id?: string) => {
    updateFilterUrl({ locationId: id });
  };

  if (!rootData.myLocations) {
    return null;
  }

  if (!organizationCustomDomain) {
    return null;
  }

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center' }}>
      <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pt: { xs: 1, sm: 1, md: 2 } }} spacing={2}>
        <GettingStarted rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} />
        <Bookings
          rootDataRelay={rootData}
          rootDataBookingRelay={rootData}
          organizationCustomDomain={organizationCustomDomain}
          from={startWeek}
          to={endWeek}
          locationIds={locationIds}
          customerIds={customerIds}
          toolbar={
            <GridContainer spacing={1}>
              <OrganizationUserSelector key={`user-${customerId ?? 'all'}`} rootDataOrganizationMembersRelay={rootData} onChange={handlCustomerChanged} defaultValue={customerId} />
              <LocationSelector key={`location-${locationId ?? 'all'}`} rootDataRelay={rootData} onChange={handlLocationChanged} defaultValue={locationId} />
              <WeekRangePicker key={startWeek.format('YYYY-MM-DD')} defaultStartWeek={startWeek} onWeekChanged={handleWeehChanged} />
            </GridContainer>
          }
          hasTopInset={false}
          actions={
            <>
              <NewBookingButton onReloadRequired={onReloadRequired} defaultDate={today} organizationCustomDomain={organizationCustomDomain} />
            </>
          }
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
};

const ModernOrganizationWithRelay = ({ organizationCustomDomain }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationBookings_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();
  const defaultWeek = useMemo(() => startOfWeek(), []);
  const searchParams = useSearchParams();
  const customerId = searchParams.get('customerId');
  const locationId = searchParams.get('locationId');
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
  }, [loadQuery, triggerReload, parsedWeekStart, organizationCustomDomain, locationId, customerId]);

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
        defaultStartWeek={parsedWeekStart}
      />
    </ErrorBoundary>
  );
};

export default memo(ModernOrganizationWithRelay);
