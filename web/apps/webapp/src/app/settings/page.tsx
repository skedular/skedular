'use client';

import { CustomerSettings } from '@/components/customer/settings';
import { RootShell } from '@/components/rootShell';
import type { pageSettings_rootQuery } from '@/queries/__generated__/pageSettings_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { memo, useCallback, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageSettings_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
};

const RootQuery = graphql`
  query pageSettings_rootQuery {
    ...rootShell_query
    ...customerSettingsPage_query
  }
`;

const Settings = ({ queryReference, onReloadRequire }: Props) => {
  const rootData = usePreloadedQuery<pageSettings_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(() => true, []);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequire={onReloadRequire}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[]}
    >
      <CustomerSettings rootDataRelay={rootData} />
    </RootShell>
  );
};

const MemoSettings = memo(Settings);

const SettingsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageSettings_rootQuery>(RootQuery);

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery]);

  const handleReloadRequire = () => {};

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoSettings queryReference={queryReference} onReloadRequire={handleReloadRequire} />
    </ErrorBoundary>
  );
};

export default memo(SettingsWithRelay);
