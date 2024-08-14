import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { RootShell } from 'components/rootShell';
import { Teams } from 'components/team/teams';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { teams_rootQuery } from './__generated__/teams_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<teams_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
};

const RootQuery = graphql`
  query teams_rootQuery($teamsSortingValues: [TeamOrderInput!]!, $teamNameSearchText: String!) {
    teamCustomerRecordSynced
    ...rootShell_query
    ...teams_query
  }
`;

const TeamsPage = ({ queryReference, onReloadRequire }: Props) => {
  const rootData = usePreloadedQuery<teams_rootQuery>(RootQuery, queryReference);
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
  const [queryReference, loadQuery] = useQueryLoader<teams_rootQuery>(RootQuery);
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

  if (queryReference == null) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoTeamsPage queryReference={queryReference} onReloadRequire={handleReloadRequire} />
    </ErrorBoundary>
  );
};

export default memo(TeamsPageWithRelay);
