'use client';

import { AddLocation } from '@/components/location/addLocation';
import { RootShell } from '@/components/rootShell';
import type { pageAddLocation_rootQuery } from '@/queries/__generated__/pageAddLocation_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageAddLocation_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
};

const RootQuery = graphql`
  query pageAddLocation_rootQuery {
    locationCustomerRecordSynced
    ...rootShell_query
  }
`;

const AddLocationPage = ({ queryReference, onReloadRequire }: Props) => {
  const rootData = usePreloadedQuery<pageAddLocation_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(() => rootData?.locationCustomerRecordSynced, [rootData?.locationCustomerRecordSynced]);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequire={onReloadRequire}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.locationCustomerRecordSynced]}
    >
      <AddLocation organizationId={null} />
    </RootShell>
  );
};

const MemoAddLocationPage = memo(AddLocationPage);

const AddLocationPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageAddLocation_rootQuery>(RootQuery);
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
      <MemoAddLocationPage queryReference={queryReference} onReloadRequire={handleReloadRequire} />
    </ErrorBoundary>
  );
};

export default memo(AddLocationPageWithRelay);
