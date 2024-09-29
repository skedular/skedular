import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { Organization } from 'components/organization/organizationPage';
import { RootShell } from 'components/rootShell';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { useParams } from 'react-router-dom';
import type { organization_rootQuery } from './__generated__/organization_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<organization_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organization_rootQuery(
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
  ) {
    organizationCustomerRecordSynced
    ...rootShell_query
    ...organizationPage_query
  }
`;

const OrganizationPage = ({ queryReference, onReloadRequire, organizationId }: Props) => {
  const rootData = usePreloadedQuery<organization_rootQuery>(RootQuery, queryReference);
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

const OrganizationPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<organization_rootQuery>(RootQuery);
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

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationPage queryReference={queryReference} onReloadRequire={handleReloadRequire} organizationId={finalOrganizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationPageWithRelay);
