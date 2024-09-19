'use client';

import { RootShell } from '@/components/rootShell';
import { Teams } from '@/components/team/teams';
import type { pageTeams_rootQuery } from '@/queries/__generated__/pageTeams_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageTeams_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
};

const RootQuery = graphql`
  query pageTeams_rootQuery($teamsSortingValues: [TeamOrderInput!]!, $teamNameSearchText: String!) {
    teamCustomerRecordSynced
    ...rootShell_query
    ...teams_query
  }
`;

const TeamsPage = ({ queryReference, onReloadRequire }: Props) => {
  const rootData = usePreloadedQuery<pageTeams_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(() => rootData?.teamCustomerRecordSynced, [rootData?.teamCustomerRecordSynced]);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequire={onReloadRequire}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.teamCustomerRecordSynced]}
    >
      <Teams rootDataRelay={rootData} />
    </RootShell>
  );
};

const MemoTeamsPage = memo(TeamsPage);

const TeamsPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageTeams_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);

  useEffect(() => {
    loadQuery(
      {
        teamsSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        teamNameSearchText: '',
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload]);

  const handleReloadRequire = () => {
    setTriggerReload(triggerReload + 1);
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoTeamsPage queryReference={queryReference} onReloadRequire={handleReloadRequire} />
    </ErrorBoundary>
  );
};

export default memo(TeamsPageWithRelay);
