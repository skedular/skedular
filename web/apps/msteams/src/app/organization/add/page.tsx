'use client';

import { Loading } from '@repo/shared/components/loading';
import { AddOrganization } from '@/components/organization/addOrganization';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageAddOrganization_rootQuery } from '@/queries/__generated__/pageAddOrganization_rootQuery.graphql';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageAddOrganization_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
};

const RootQuery = graphql`
  query pageAddOrganization_rootQuery {
    organizationCustomerRecordSynced
    ...rootShell_query
    ...addOrganization_query
  }
`;

const AddOrganizationPage = ({ queryReference, onReloadRequire }: Props) => {
  const rootData = usePreloadedQuery<pageAddOrganization_rootQuery>(RootQuery, queryReference);
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
      <AddOrganization rootDataRelay={rootData} />
    </RootShell>
  );
};

const MemoAddOrganizationPage = memo(AddOrganizationPage);

type PropsWithRelay = {};

const AddOrganizationPageWithRelay = ({}: PropsWithRelay) => {
  const [queryReference, loadQuery] = useQueryLoader<pageAddOrganization_rootQuery>(RootQuery);
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
      <MemoAddOrganizationPage queryReference={queryReference} onReloadRequire={handleReloadRequire} />
    </ErrorBoundary>
  );
};

export default memo(AddOrganizationPageWithRelay);
