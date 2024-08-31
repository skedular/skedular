import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { AddLocation } from 'components/location/addLocation';
import { RootShell } from 'components/rootShell';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { useParams } from 'react-router-dom';
import type { addOrganizationLocation_rootQuery } from './__generated__/addOrganizationLocation_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<addOrganizationLocation_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query addOrganizationLocation_rootQuery {
    locationCustomerRecordSynced
    ...rootShell_query
  }
`;

const AddLocationPage = ({ queryReference, onReloadRequire, organizationId }: Props) => {
  const rootData = usePreloadedQuery<addOrganizationLocation_rootQuery>(RootQuery, queryReference);
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

const AddLocationPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<addOrganizationLocation_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
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
