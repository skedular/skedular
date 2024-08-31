'use client';

import { Organization } from '@/components/organization/organizationPage';
import { RootShell } from '@/components/rootShell';
import type { pageOrganization_rootQuery } from '@/queries/__generated__/pageOrganization_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import { useParams } from 'next/navigation';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageOrganization_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query pageOrganization_rootQuery(
    $organizationId: String!
    $locationId: String!
    $dateToGetAvailableDesks: DateTime!
    $deskIdsToIncludeToGetAvailableDesks: [String!]!
    $peopleNameSearchText: String!
    $bookingPeopleNameSearchText: String!
    $bookingSortingValues: [BookingOrderInput!]!
    $organizationPeopleSortingValues: [OrganizationMemberOrderInput!]
    $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $organizationLocationsSortingValues: [LocationOrderInput!]!
    $organizationTeamsSortingValues: [TeamOrderInput!]!
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaUntil: DateTime!
    $locationNameSearchText: String!
    $teamNameSearchText: String!
    $organizationAnalyticsFrom: DateTime!
    $organizationAnalyticsUntil: DateTime!
  ) {
    organizationCustomerRecordSynced
    ...rootShell_query
    ...organizationPage_query
  }
`;

const OrganizationPage = ({ queryReference, onReloadRequire, organizationId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganization_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(
    () => rootData?.organizationCustomerRecordSynced,
    [rootData?.organizationCustomerRecordSynced],
  );

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequire={onReloadRequire}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.organizationCustomerRecordSynced]}
    >
      <Organization rootDataRelay={rootData} organizationId={organizationId} />
    </RootShell>
  );
};

const MemoOrganizationPage = memo(OrganizationPage);

type PropsWithRelay = {};

const OrganizationPageWithRelay = ({}: PropsWithRelay) => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganization_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const { organizationId } = useParams();
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

  useEffect(() => {
    const from = startOfDay(null).toISOString();
    const until = startOfDay(null).add(1, 'month').toISOString();

    const organizationAnalyticsFrom = startOfDay(null).subtract(30, 'days').toISOString();
    const organizationAnalyticsUntil = startOfDay(null).toISOString();

    loadQuery(
      {
        organizationId: finalOrganizationId,
        locationId: '',
        deskIdsToIncludeToGetAvailableDesks: [],
        peopleNameSearchText: '',
        bookingPeopleNameSearchText: '',
        bookingSortingValues: [
          {
            direction: 'Ascending',
            field: 'from',
          },
        ],
        organizationPeopleSortingValues: [
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
        organizationLocationsSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        organizationTeamsSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        bookingsSearchCriteriaFrom: from,
        bookingsSearchCriteriaUntil: until,
        locationNameSearchText: '',
        teamNameSearchText: '',
        organizationAnalyticsFrom,
        organizationAnalyticsUntil,
        dateToGetAvailableDesks: from,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload, finalOrganizationId]);

  const handleReloadRequire = () => {
    setTriggerReload(triggerReload + 1);
  };

  if (queryReference == null) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationPage queryReference={queryReference} onReloadRequire={handleReloadRequire} organizationId={finalOrganizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationPageWithRelay);
