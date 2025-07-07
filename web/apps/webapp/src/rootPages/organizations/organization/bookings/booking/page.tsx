import { EditMarketplaceBooking } from '@/components/booking/editMarketplaceBooking';
import { EditPrivateBooking } from '@/components/booking/editPrivateBooking';
import { PayMarketplaceBooking } from '@/components/booking/payMarketplaceBooking';
import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { startOfDay, toShortDateWithAdditionalDayInfo } from '@/libs/utils';
import type { pageOrganizationBooking_rootQuery } from '@/queries/__generated__/pageOrganizationBooking_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import dayjs from 'dayjs';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

const RootQuery = graphql`
  query pageOrganizationBooking_rootQuery(
    $organizationId: String!
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
      bookedOnMarketplace
      isPaymentRequired
      paymentStatus {
        type
      }
    }
    ...editPrivateBooking_query
    ...editPrivateBooking_organizationMembers_query
    ...editPrivateBooking_customerTeams_query
    ...editPrivateBooking_availableResources_query
    ...editMarketplaceBooking_query
    ...editMarketplaceBooking_booking_query
    ...editMarketplaceBooking_organizationMembers_query
    ...editMarketplaceBooking_customerTeams_query
    ...payMarketplaceBooking_booking_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationBooking_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  bookingId: string;
};

const RootPage = ({ queryReference, onReloadRequired, organizationId, bookingId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationBooking_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const shouldPay = useMemo(() => {
    if (!rootData.booking) {
      return false;
    }

    if (rootData.booking.isPaymentRequired && !rootData.booking.paymentStatus) {
      return true;
    }

    return rootData.booking.isPaymentRequired && rootData.booking.paymentStatus && rootData.booking.paymentStatus.type === 'PENDING';
  }, [rootData.booking]);

  const handleBackClick = () => {
    router.back();
  };

  if (!rootData.booking) {
    return <></>;
  }

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
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      {rootData.booking.bookedOnMarketplace && shouldPay && <PayMarketplaceBooking rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationId={organizationId} />}
      {rootData.booking.bookedOnMarketplace && !shouldPay && (
        <EditMarketplaceBooking
          rootDataRelay={rootData}
          rootDataBookingRelay={rootData}
          rootDataTeamsRelay={rootData}
          rootDataOrganizationMembersRelay={rootData}
          onReloadRequired={onReloadRequired}
          organizationId={organizationId}
        />
      )}
      {!rootData.booking.bookedOnMarketplace && (
        <EditPrivateBooking
          rootDataRelay={rootData}
          rootDataTeamsRelay={rootData}
          rootDataOrganizationMembersRelay={rootData}
          rootDataAvailableResourcesRelay={rootData}
          onReloadRequired={onReloadRequired}
          organizationId={organizationId}
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
  const { organizationId, bookingId } = useParams();
  let finalOrganizationId = '';

  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
  } else {
    throw new Error('organizationId is required');
  }

  let finalBookingId = '';

  if (typeof bookingId === 'string') {
    finalBookingId = bookingId;
  } else if (Array.isArray(bookingId)) {
    if (typeof bookingId[0] === 'undefined') {
      throw new Error('bookingId is required');
    }

    finalBookingId = bookingId[0];
  } else {
    throw new Error('bookingId is required');
  }

  useEffect(() => {
    const date = startOfDay();
    const startDate = date.toISOString();
    const endDate = date.add(1, 'day').toISOString();

    loadQuery(
      {
        organizationId: finalOrganizationId,
        bookingId: finalBookingId,
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
  }, [loadQuery, triggerReloadId, finalOrganizationId, finalBookingId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} bookingId={finalBookingId} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
