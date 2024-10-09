'use client';

import { RootShell } from '@/components/rootShell';
import { Team } from '@/components/team/teamPage';
import type { pageTeam_rootQuery } from '@/queries/__generated__/pageTeam_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { useParams } from 'next/navigation';
import { memo, useCallback, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageTeam_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageTeam_rootQuery {
    teamCustomerRecordSynced
    ...rootShell_query
  }
`;

const TeamPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageTeam_rootQuery>(RootQuery, queryReference);
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
  const areAdditionalCustomerRecordsSync = useCallback(() => rootData?.teamCustomerRecordSynced, [rootData?.teamCustomerRecordSynced]);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequired={onReloadRequired}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.teamCustomerRecordSynced]}
    >
      <Team organizationId="" teamId={finalTeamId} />
    </RootShell>
  );
};

const MemoTeamPage = memo(TeamPage);

const TeamPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageTeam_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReload(triggerReload + 1);
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoTeamPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(TeamPageWithRelay);
