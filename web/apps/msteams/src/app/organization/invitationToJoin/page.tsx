'use client';

import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageInvitationToJoin_rootQuery } from '@/queries/__generated__/pageInvitationToJoin_rootQuery.graphql';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageInvitationToJoin_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
};

const RootQuery = graphql`
  query pageInvitationToJoin_rootQuery {
    organizationCustomerRecordSynced
    ...rootShell_query
  }
`;

const InvitationToJoin = ({ queryReference, onReloadRequire }: Props) => {
  const rootData = usePreloadedQuery<pageInvitationToJoin_rootQuery>(RootQuery, queryReference);
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
      Invitations
    </RootShell>
  );
};

const MemoInvitationToJoin = memo(InvitationToJoin);

type PropsWithRelay = {};

const InvitationToJoinWithRelay = ({}: PropsWithRelay) => {
  const [queryReference, loadQuery] = useQueryLoader<pageInvitationToJoin_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);

  useEffect(() => {
    loadQuery(
      {},
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
      <MemoInvitationToJoin queryReference={queryReference} onReloadRequire={handleReloadRequire} />
    </ErrorBoundary>
  );
};

export default memo(InvitationToJoinWithRelay);
