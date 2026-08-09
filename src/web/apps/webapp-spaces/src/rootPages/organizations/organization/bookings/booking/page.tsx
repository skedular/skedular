import { EditMarketplaceBooking } from '@/components/booking/editMarketplaceBooking';
import { EditPrivateBooking } from '@/components/booking/editPrivateBooking';
import { EditPrivateRecurringBooking } from '@/components/booking/editPrivateRecurringBooking';
import { PayMarketplaceBooking } from '@/components/booking/payMarketplaceBooking';
import { Loading } from '@/components/loading';
import { RelayError, startOfDay, toRootError, useKnownParams } from '@skedular/shared';

import { RootShell } from '@/components/rootShell';
import type { pageOrganizationBooking_rootQuery } from '@/queries/__generated__/pageOrganizationBooking_rootQuery.graphql';

import { useSearchParams } from 'next/navigation';
import { memo, useEffect, useMemo, useState, useTransition } from 'react';
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
      marketplaceBooking {
        isPaymentRequired
        paymentStatus {
          type
        }
      }
    }
    ...editPrivateBooking_query
    ...editPrivateRecurringBooking_query
    ...editPrivateBooking_organizationMembers_query
    ...editPrivateBooking_availableResources_query
    ...editPrivateRecurringBooking_organizationMembers_query
    ...editPrivateRecurringBooking_availableResources_query
    ...editMarketplaceBooking_query
    ...editMarketplaceBooking_booking_query
    ...payMarketplaceBooking_booking_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationBooking_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  forceModify?: boolean;
};

const RootPage = ({ queryReference, onReloadRequired, organizationCustomDomain, forceModify = false }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationBooking_rootQuery>(RootQuery, queryReference);
  const searchParams = useSearchParams();
  const shouldPay = useMemo(() => {
    if (!rootData.booking?.marketplaceBooking) {
      return false;
    }

    const marketplaceBooking = rootData.booking.marketplaceBooking;
    return marketplaceBooking.isPaymentRequired && (!marketplaceBooking.paymentStatus || marketplaceBooking.paymentStatus.type === 'PENDING');
  }, [rootData.booking]);

  if (!rootData.booking) {
    return null;
  }

  const editMode = searchParams.get('editMode');
  const showRecurringPrivateBookingEditor = rootData.booking.channel.channel === 'PRIVATE' && !!rootData.booking.recurringBooking && editMode === 'recurring';

  return (
    <RootShell>
      {rootData.booking.channel.channel === 'MARKETPLACE' && shouldPay && (
        <PayMarketplaceBooking rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} />
      )}
      {rootData.booking.channel.channel === 'MARKETPLACE' && !shouldPay && (
        <EditMarketplaceBooking
          rootDataRelay={rootData}
          rootDataBookingRelay={rootData}
          onReloadRequired={onReloadRequired}
          page={forceModify}
          organizationCustomDomain={organizationCustomDomain}
        />
      )}
      {rootData.booking.channel.channel === 'PRIVATE' &&
        (showRecurringPrivateBookingEditor ? (
          <EditPrivateRecurringBooking
            rootDataRelay={rootData}
            rootDataOrganizationMembersRelay={rootData}
            rootDataAvailableResourcesRelay={rootData}
            onReloadRequired={onReloadRequired}
          />
        ) : (
          <EditPrivateBooking rootDataRelay={rootData} rootDataOrganizationMembersRelay={rootData} rootDataAvailableResourcesRelay={rootData} onReloadRequired={onReloadRequired} />
        ))}
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = ({ forceModify = false }: { forceModify?: boolean }) => {
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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} forceModify={forceModify} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
