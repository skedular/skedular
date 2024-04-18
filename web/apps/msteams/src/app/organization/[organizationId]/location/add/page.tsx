'use client';

import { Loading } from '@repo/shared/components/loading';
import { AddLocation } from '@/components/location/addLocation';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageAddOrganizationLocation_rootQuery } from '@/queries/__generated__/pageAddOrganizationLocation_rootQuery.graphql';
import { useParams } from 'next/navigation';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageAddOrganizationLocation_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query pageAddOrganizationLocation_rootQuery {
    locationCustomerRecordSynced
    ...rootShell_query
  }
`;

const AddLocationPage = ({ queryReference, onReloadRequire, organizationId }: Props) => {
  const rootData = usePreloadedQuery<pageAddOrganizationLocation_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(() => rootData?.locationCustomerRecordSynced, [rootData?.locationCustomerRecordSynced]);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequire={onReloadRequire}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.locationCustomerRecordSynced]}
    >
      <AddLocation organizationId={organizationId} />
    </RootShell>
  );
};

const MemoAddLocationPage = memo(AddLocationPage);

type PropsWithRelay = {};

const AddLocationPageWithRelay = ({}: PropsWithRelay) => {
  const { organizationId } = useParams();
  let finalOrganizationId = '';
  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
  } else {
    throw new Error('organizationId is required');
  }

  const [queryReference, loadQuery] = useQueryLoader<pageAddOrganizationLocation_rootQuery>(RootQuery);
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
      <MemoAddLocationPage queryReference={queryReference} onReloadRequire={handleReloadRequire} organizationId={finalOrganizationId} />
    </ErrorBoundary>
  );
};

export default memo(AddLocationPageWithRelay);
