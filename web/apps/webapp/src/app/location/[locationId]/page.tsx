'use client';

import { Location } from '@/components/location/locationPage';
import { RootShell } from '@/components/rootShell';
import type { pageLocation_rootQuery } from '@/queries/__generated__/pageLocation_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { TAG_TYPE_LOCATION_ZONE } from '@repo/shared/components/zone';
import { endOfDay, startOfDay } from '@repo/shared/libs/utils';
import { useParams } from 'next/navigation';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageLocation_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  locationId: string;
};

const RootQuery = graphql`
  query pageLocation_rootQuery(
    $organizationId: String!
    $locationId: String!
    $locationExists: Boolean!
    $zoneTagType: String!
    $dateToGetAvailableDesks: DateTime!
    $deskIdsToIncludeToGetAvailableDesks: [String!]!
    $fromToGetBookings: DateTime
    $toToGetBookings: DateTime
    $peopleNameSearchText: String!
    $zoneNameSearchText: String!
    $deskNameSearchText: String!
    $bookingPeopleNameSearchText: String!
    $bookingSortingValues: [BookingOrderInput!]!
    $locationPeopleSortingValues: [LocationMemberOrderInput!]
    $locationOrganizationPeopleSortingValues: [CustomerOrderInput!]
    $zoneSortingValues: [LocationTagOrderInput!]!
    $deskSortingValues: [DeskOrderInput!]!
    $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $deskMultipleChoicesZonesSortingValues: [LocationTagOrderInput!]
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaUntil: DateTime!
  ) {
    locationCustomerRecordSynced
    ...rootShell_query
    ...locationPage_query
  }
`;

const LocationPage = ({ queryReference, onReloadRequired, locationId }: Props) => {
  const rootData = usePreloadedQuery<pageLocation_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(() => rootData?.locationCustomerRecordSynced, [rootData?.locationCustomerRecordSynced]);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequired={onReloadRequired}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.locationCustomerRecordSynced]}
    >
      <Location rootDataRelay={rootData} locationId={locationId} organizationId="" />
    </RootShell>
  );
};

const MemoLocationPage = memo(LocationPage);

const LocationPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageLocation_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const { locationId } = useParams();
  let finalLocationId = '';
  if (typeof locationId === 'string') {
    finalLocationId = locationId;
  } else if (Array.isArray(locationId)) {
    if (typeof locationId[0] === 'undefined') {
      throw new Error('locationId is required');
    }

    finalLocationId = locationId[0];
  } else {
    throw new Error('locationId is required');
  }

  useEffect(() => {
    const from = startOfDay().toISOString();
    const to = endOfDay(from).toISOString();
    const until = startOfDay().add(1, 'month').toISOString();

    loadQuery(
      {
        locationId: finalLocationId,
        locationExists: !!finalLocationId,
        zoneTagType: TAG_TYPE_LOCATION_ZONE,
        deskIdsToIncludeToGetAvailableDesks: [],
        fromToGetBookings: from,
        toToGetBookings: to,
        organizationId: '',
        peopleNameSearchText: '',
        zoneNameSearchText: '',
        deskNameSearchText: '',
        bookingPeopleNameSearchText: '',
        bookingSortingValues: [
          {
            direction: 'Ascending',
            field: 'from',
          },
        ],
        locationPeopleSortingValues: [
          {
            direction: 'Descending',
            field: 'name',
          },
        ],
        locationOrganizationPeopleSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        zoneSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        deskSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        bookingDetailsSelectorOrganizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        deskMultipleChoicesZonesSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        bookingsSearchCriteriaFrom: from,
        bookingsSearchCriteriaUntil: until,
        dateToGetAvailableDesks: from,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload, finalLocationId]);

  const handleReloadRequired = () => {
    setTriggerReload(triggerReload + 1);
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoLocationPage queryReference={queryReference} onReloadRequired={handleReloadRequired} locationId={finalLocationId} />
    </ErrorBoundary>
  );
};

export default memo(LocationPageWithRelay);
