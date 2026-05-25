import { EditPrivateBooking } from '@/components/booking/editPrivateBooking';
import { EditPrivateRecurringBooking } from '@/components/booking/editPrivateRecurringBooking';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@skedular/shared';
import { startOfDay } from '@skedular/shared';
import type { pageOrganizationBooking_rootQuery } from '@/queries/__generated__/pageOrganizationBooking_rootQuery.graphql';
import dayjs from 'dayjs';
import { useSearchParams } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

const RootQuery = graphql`
  query pageOrganizationBooking_rootQuery(
    $organizationCustomDomain: String!
    $bookingId: String!
    $peopleNameSearchText: String
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $locationId: String!
    $dateFromToGetAvailableResources: DateTime!
    $dateUntilToGetAvailableResources: DateTime!
    $customerId: String!
    $customerExists: Boolean!
    $teamsSortingValues: [TeamOrderInput!]
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    booking(id: $bookingId) {
      from
      channel {
        channel
      }
      recurringBooking {
        id
      }
    }
    ...editPrivateBooking_query
    ...editPrivateRecurringBooking_query
    ...editPrivateBooking_organizationMembers_query
    ...editPrivateBooking_customerTeams_query
    ...editPrivateBooking_availableResources_query
    ...editPrivateRecurringBooking_organizationMembers_query
    ...editPrivateRecurringBooking_customerTeams_query
    ...editPrivateRecurringBooking_availableResources_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationBooking_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationBooking_rootQuery>(RootQuery, queryReference);
  const searchParams = useSearchParams();

  if (!rootData.booking) {
    return null;
  }

  if (rootData.booking.channel.channel !== 'PRIVATE') {
    return null;
  }

  const editMode = searchParams.get('editMode');
  const showRecurringPrivateBookingEditor = !!rootData.booking.recurringBooking && editMode === 'recurring';

  const date = dayjs(rootData.booking.from);

  return (
    <RootShell>
      {showRecurringPrivateBookingEditor ? (
        <EditPrivateRecurringBooking
          rootDataRelay={rootData}
          rootDataTeamsRelay={rootData}
          rootDataOrganizationMembersRelay={rootData}
          rootDataAvailableResourcesRelay={rootData}
          onReloadRequired={onReloadRequired}
        />
      ) : (
        <EditPrivateBooking
          rootDataRelay={rootData}
          rootDataTeamsRelay={rootData}
          rootDataOrganizationMembersRelay={rootData}
          rootDataAvailableResourcesRelay={rootData}
          onReloadRequired={onReloadRequired}
        />
      )}
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationBooking_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationCustomDomain, bookingId } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  if (!bookingId) {
    throw new Error('bookingId is required');
  }

  useEffect(() => {
    const date = startOfDay();
    const startDate = date.toISOString();
    const endDate = date.add(1, 'day').toISOString();

    loadQuery(
      {
        organizationCustomDomain,
        bookingId,
        organizationMembersSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        locationId: '',
        dateFromToGetAvailableResources: startDate,
        dateUntilToGetAvailableResources: endDate,
        customerId: '',
        customerExists: false,
        teamsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
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
  }, [loadQuery, triggerReloadId, organizationCustomDomain, bookingId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
