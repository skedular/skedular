'use client';

import { Loading } from '@repo/shared/components/loading';
import { Organizations } from '@/components/organization/organizations';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizations_rootQuery } from '@/queries/__generated__/pageOrganizations_rootQuery.graphql';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageOrganizations_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
};

const RootQuery = graphql`
  query pageOrganizations_rootQuery($organizationsSortingValues: [OrganizationOrderInput!]!, $organizationNameSearchText: String!) {
    organizationCustomerRecordSynced
    ...rootShell_query
    ...organizations_query
  }
`;

const OrganizationsPage = ({ queryReference, onReloadRequire }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizations_rootQuery>(RootQuery, queryReference);
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
      <Organizations rootDataRelay={rootData} />
    </RootShell>
  );
};

const MemoOrganizationsPage = memo(OrganizationsPage);

type PropsWithRelay = {};

const OrganizationsPageWithRelay = ({}: PropsWithRelay) => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizations_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);

  useEffect(() => {
    loadQuery(
      {
        organizationsSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        organizationNameSearchText: '',
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
      <MemoOrganizationsPage queryReference={queryReference} onReloadRequire={handleReloadRequire} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationsPageWithRelay);
