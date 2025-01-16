'use client';

import { EditBooking } from '@/components/booking/editBooking';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationBooking_rootQuery } from '@/queries/__generated__/pageOrganizationBooking_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { BodyIconTypography, StackColumn } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay, toShortDateWithAdditionalDayInfo } from '@repo/shared/libs/utils';
import dayjs from 'dayjs';
import { nanoid } from 'nanoid';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

const RootQuery = graphql`
  query pageOrganizationBooking_rootQuery(
    $organizationId: String!
    $bookingId: String!
    $peopleNameSearchText: String
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $locationId: String!
    $locationExists: Boolean!
    $dateToGetAvailableDesks: DateTime!
    $deskIdsToIncludeToGetAvailableDesks: [String!]!
    $customerId: String!
    $customerExists: Boolean!
    $teamsSortingValues: [TeamOrderInput!]
  ) {
    booking(id: $bookingId) {
      from
    }
    ...editBooking_query
    ...editBooking_organizationMembers_query
    ...editBooking_customerTeams_query
    ...editBooking_availableLocationDesks_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationBooking_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  bookingId: string;
};

const LocationPage = ({ queryReference, onReloadRequired, organizationId, bookingId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationBooking_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

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
      <EditBooking
        rootDataRelay={rootData}
        rootDataTeamsRelay={rootData}
        rootDataOrganizationMembersRelay={rootData}
        rootDataAvailableLocationDesksRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationId={organizationId}
      />
    </RootShell>
  );
};

const MemoLocationPage = memo(LocationPage);

const LocationPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationBooking_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
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
    const date = startOfDay().toISOString();

    loadQuery(
      {
        organizationId: finalOrganizationId,
        bookingId: finalBookingId,
        organizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        locationId: '',
        locationExists: false,
        dateToGetAvailableDesks: date,
        deskIdsToIncludeToGetAvailableDesks: [],
        customerId: '',
        customerExists: false,
        teamsSortingValues: [
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
  }, [loadQuery, triggerReloadId, finalOrganizationId, finalBookingId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoLocationPage
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationId={finalOrganizationId}
        bookingId={finalBookingId}
      />
    </ErrorBoundary>
  );
};

export default memo(LocationPageWithRelay);
