'use client';

import { RootShell } from '@/components/rootShell';
import { Team } from '@/components/team/teamPage';
import type { pageTeam_rootQuery } from '@/queries/__generated__/pageTeam_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import { useParams } from 'next/navigation';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageTeam_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  teamId: string;
};

const RootQuery = graphql`
  query pageTeam_rootQuery(
    $organizationId: String!
    $organizationExists: Boolean!
    $locationId: String!
    $locationExists: Boolean!
    $teamId: String!
    $teamExists: Boolean!
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

const TeamPage = ({ queryReference, onReloadRequired, teamId }: Props) => {
  const rootData = usePreloadedQuery<pageTeam_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(() => rootData?.teamCustomerRecordSynced, [rootData?.teamCustomerRecordSynced]);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequired={onReloadRequired}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.teamCustomerRecordSynced]}
    >
      <Team rootDataRelay={rootData} teamId={teamId} organizationId="" />
    </RootShell>
  );
};

const MemoTeamPage = memo(TeamPage);

const TeamPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageTeam_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const { teamId } = useParams();
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
    const from = startOfDay().toISOString();
    const until = startOfDay().add(1, 'month').toISOString();

    loadQuery(
      {
        teamId: finalTeamId,
        teamExists: !!finalTeamId,
        locationId: '',
        locationExists: false,
        deskIdsToIncludeToGetAvailableDesks: [],
        organizationId: '',
        organizationExists: false,
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
  }, [loadQuery, triggerReload, finalTeamId]);

  const handleReloadRequired = () => {
    setTriggerReload(triggerReload + 1);
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoTeamPage queryReference={queryReference} onReloadRequired={handleReloadRequired} teamId={finalTeamId} />
    </ErrorBoundary>
  );
};

export default memo(TeamPageWithRelay);
