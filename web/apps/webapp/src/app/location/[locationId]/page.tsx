'use client';

import { Loading } from '@repo/shared/components/loading';
import { Location } from '@/components/location/locationPage';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { RootShell } from '@/components/rootShell';
import { TAG_TYPE_LOCATION_ZONE } from '@/components/zone';
import type { pageLocation_rootQuery } from '@/queries/__generated__/pageLocation_rootQuery.graphql';
import { endOfDay, startOfDay } from '@repo/shared/libs/utils';
import { useParams } from 'next/navigation';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageLocation_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
  locationId: string;
};

const RootQuery = graphql`
  query pageLocation_rootQuery(
    $organizationId: String!
    $locationId: String!
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
    $locationAnalyticsFrom: DateTime!
    $locationAnalyticsUntil: DateTime!
  ) {
    locationCustomerRecordSynced
    ...rootShell_query
    ...locationPage_query
  }
`;

const LocationPage = ({ queryReference, onReloadRequire, locationId }: Props) => {
  const rootData = usePreloadedQuery<pageLocation_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(() => rootData?.locationCustomerRecordSynced, [rootData?.locationCustomerRecordSynced]);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequire={onReloadRequire}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.locationCustomerRecordSynced]}
    >
      <Location rootDataRelay={rootData} locationId={locationId} organizationId="" />
    </RootShell>
  );
};

const MemoLocationPage = memo(LocationPage);

type PropsWithRelay = {};

const LocationPageWithRelay = ({}: PropsWithRelay) => {
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

  const [queryReference, loadQuery] = useQueryLoader<pageLocation_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);

  useEffect(() => {
    const from = startOfDay(null).toISOString();
    const to = endOfDay(null).toISOString();
    const until = startOfDay(null).add(1, 'month').toISOString();
    const locationAnalyticsFrom = startOfDay(null).subtract(30, 'days').toISOString();
    const locationAnalyticsUntil = startOfDay(null).toISOString();

    loadQuery(
      {
        locationId: finalLocationId,
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
        locationAnalyticsFrom,
        locationAnalyticsUntil,
        dateToGetAvailableDesks: from,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload, finalLocationId]);

  const handleReloadRequire = () => {
    setTriggerReload(triggerReload + 1);
  };

  if (queryReference == null) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoLocationPage queryReference={queryReference} onReloadRequire={handleReloadRequire} locationId={finalLocationId} />
    </ErrorBoundary>
  );
};

export default memo(LocationPageWithRelay);
