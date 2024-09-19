import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { RootShell } from 'components/rootShell';
import { AddTeam } from 'components/team/addTeam';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { useParams } from 'react-router-dom';
import type { addOrganizationTeam_rootQuery } from './__generated__/addOrganizationTeam_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<addOrganizationTeam_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query addOrganizationTeam_rootQuery(
    $organizationId: String!
    $bookingPeopleNameSearchText: String!
    $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
  ) {
    teamCustomerRecordSynced
    ...rootShell_query
    ...addTeam_query
  }
`;

const AddTeamPage = ({ queryReference, onReloadRequire, organizationId }: Props) => {
  const rootData = usePreloadedQuery<addOrganizationTeam_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(() => rootData?.teamCustomerRecordSynced, [rootData?.teamCustomerRecordSynced]);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequire={onReloadRequire}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.teamCustomerRecordSynced]}
    >
      <AddTeam rootDataRelay={rootData} organizationId={organizationId} />
    </RootShell>
  );
};

const MemoAddTeamPage = memo(AddTeamPage);

const AddTeamPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<addOrganizationTeam_rootQuery>(RootQuery);
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
    loadQuery(
      {
        organizationId: finalOrganizationId,
        bookingPeopleNameSearchText: '',
        organizationMemberSelectorOrganizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
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
      <MemoAddTeamPage queryReference={queryReference} onReloadRequire={handleReloadRequire} organizationId={finalOrganizationId} />
    </ErrorBoundary>
  );
};

export default memo(AddTeamPageWithRelay);
