'use client';

import { RootShell } from '@/components/rootShell';
import { Team } from '@/components/team/teamPage';
import type { pageOrganizationTeam_rootQuery } from '@/queries/__generated__/pageOrganizationTeam_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import { useParams } from 'next/navigation';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationTeam_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
  teamId: string;
  organizationId: string;
};

const RootQuery = graphql`
  query pageOrganizationTeam_rootQuery(
    $organizationId: String!
    $locationId: String!
    $teamId: String!
    $dateToGetAvailableDesks: DateTime!
    $deskIdsToIncludeToGetAvailableDesks: [String!]!
    $bookingPeopleNameSearchText: String!
    $bookingSortingValues: [BookingOrderInput!]!
    $teamPeopleSortingValues: [TeamMemberOrderInput!]
    $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaUntil: DateTime!
    $peopleNameSearchText: String!
  ) {
    teamCustomerRecordSynced
    ...rootShell_query
    ...teamPage_query
  }
`;

const TeamPage = ({ queryReference, onReloadRequire, teamId, organizationId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationTeam_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(() => rootData?.teamCustomerRecordSynced, [rootData?.teamCustomerRecordSynced]);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequire={onReloadRequire}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.teamCustomerRecordSynced]}
    >
      <Team rootDataRelay={rootData} teamId={teamId} organizationId={organizationId} />
    </RootShell>
  );
};

const MemoTeamPage = memo(TeamPage);

type PropsWithRelay = {};

const TeamPageWithRelay = ({}: PropsWithRelay) => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationTeam_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const { organizationId, teamId } = useParams();
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

  let finalTeamId = '';
  if (typeof teamId === 'string') {
    finalTeamId = teamId;
  } else if (Array.isArray(teamId)) {
    if (typeof teamId[0] === 'undefined') {
      throw new Error('teamId is required');
    }

    finalTeamId = teamId[0];
  } else {
    throw new Error('teamId is required');
  }

  useEffect(() => {
    const from = startOfDay(null).toISOString();
    const until = startOfDay(null).add(1, 'month').toISOString();

    loadQuery(
      {
        teamId: finalTeamId,
        locationId: '',
        deskIdsToIncludeToGetAvailableDesks: [],
        organizationId: finalOrganizationId,
        bookingPeopleNameSearchText: '',
        bookingSortingValues: [
          {
            direction: 'Ascending',
            field: 'from',
          },
        ],
        teamPeopleSortingValues: [
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
        organizationMemberSelectorOrganizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        bookingsSearchCriteriaFrom: from,
        bookingsSearchCriteriaUntil: until,
        peopleNameSearchText: '',
        dateToGetAvailableDesks: from,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload, finalOrganizationId, finalTeamId]);

  const handleReloadRequire = () => {
    setTriggerReload(triggerReload + 1);
  };

  if (queryReference == null) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoTeamPage queryReference={queryReference} onReloadRequire={handleReloadRequire} teamId={finalTeamId} organizationId={finalOrganizationId} />
    </ErrorBoundary>
  );
};

export default memo(TeamPageWithRelay);
