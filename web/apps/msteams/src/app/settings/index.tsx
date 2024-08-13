'use client';

import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { CustomerSettings } from 'components/customer/settings';
import { RootShell } from 'components/rootShell';
import { memo, useCallback, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { settings_rootQuery } from './__generated__/settings_rootQuery.graphql';
type Props = {
  queryReference: PreloadedQuery<settings_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
};

const RootQuery = graphql`
  query settings_rootQuery {
    ...rootShell_query
    ...customerSettingsPage_query
  }
`;

const Settings = ({ queryReference, onReloadRequire }: Props) => {
  const rootData = usePreloadedQuery<settings_rootQuery>(RootQuery, queryReference);
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
  const [queryReference, loadQuery] = useQueryLoader<settings_rootQuery>(RootQuery);

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery]);

  const handleReloadRequire = () => {};

  if (queryReference == null) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoSettings queryReference={queryReference} onReloadRequire={handleReloadRequire} />
    </ErrorBoundary>
  );
};

export default memo(SettingsWithRelay);
