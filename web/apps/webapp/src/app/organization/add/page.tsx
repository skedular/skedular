'use client';

import { AddOrganization } from '@/components/organization/addOrganization';
import { RootShell } from '@/components/rootShell';
import type { pageAddOrganization_rootQuery } from '@/queries/__generated__/pageAddOrganization_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { memo, useCallback, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageAddOrganization_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageAddOrganization_rootQuery {
    organizationCustomerRecordSynced
    ...rootShell_query
    ...addOrganization_query
  }
`;

const AddOrganizationPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageAddOrganization_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(
    () => rootData?.organizationCustomerRecordSynced,
    [rootData?.organizationCustomerRecordSynced],
  );

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequired={onReloadRequired}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.organizationCustomerRecordSynced]}
    >
      <AddOrganization rootDataRelay={rootData} />
    </RootShell>
  );
};

const MemoAddOrganizationPage = memo(AddOrganizationPage);

const AddOrganizationPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageAddOrganization_rootQuery>(RootQuery);
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
      <MemoAddOrganizationPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(AddOrganizationPageWithRelay);
