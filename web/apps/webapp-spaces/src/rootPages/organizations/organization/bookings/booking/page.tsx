import { EditMarketplaceBooking } from '@/components/booking/editMarketplaceBooking';
import { EditPrivateBooking } from '@/components/booking/editPrivateBooking';
import { EditPrivateRecurringBooking } from '@/components/booking/editPrivateRecurringBooking';
import { PayMarketplaceBooking } from '@/components/booking/payMarketplaceBooking';
import { BodyIconTypography, StackColumn } from '@skedular/ui';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@skedular/shared';
import { startOfDay, toShortDateWithAdditionalDayInfo } from '@skedular/shared';
import type { pageOrganizationBooking_rootQuery } from '@/queries/__generated__/pageOrganizationBooking_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import dayjs from 'dayjs';
import { useRouter, useSearchParams } from 'next/navigation';
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
    ...editMarketplaceBooking_organizationMembers_query
    ...payMarketplaceBooking_booking_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationBooking_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const RootPage = ({ queryReference, onReloadRequired, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationBooking_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const searchParams = useSearchParams();
  const shouldPay = useMemo(() => {
    if (!rootData.booking?.marketplaceBooking) {
      return false;
    }

    const marketplaceBooking = rootData.booking.marketplaceBooking;
    return marketplaceBooking.isPaymentRequired && (!marketplaceBooking.paymentStatus || marketplaceBooking.paymentStatus.type === 'PENDING');
  }, [rootData.booking]);

  const handleBackClick = () => {
    router.back();
  };

  if (!rootData.booking) {
    return null;
  }

  const editMode = searchParams.get('editMode');
  const showRecurringPrivateBookingEditor = rootData.booking.channel.channel === 'PRIVATE' && !!rootData.booking.recurringBooking && editMode === 'recurring';

  const date = dayjs(rootData.booking.from);

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="Booking" />
          <BodyIconTypography label={toShortDateWithAdditionalDayInfo(date)} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      {rootData.booking.channel.channel === 'MARKETPLACE' && shouldPay && (
        <PayMarketplaceBooking rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} />
      )}
      {rootData.booking.channel.channel === 'MARKETPLACE' && !shouldPay && (
        <EditMarketplaceBooking rootDataRelay={rootData} rootDataBookingRelay={rootData} rootDataOrganizationMembersRelay={rootData} onReloadRequired={onReloadRequired} />
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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
