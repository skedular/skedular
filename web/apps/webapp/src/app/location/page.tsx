'use client';

import { Locations } from '@/components/location/locations';
import { RootShell } from '@/components/rootShell';
import type { pageLocations_rootQuery } from '@/queries/__generated__/pageLocations_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageLocations_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
};

const RootQuery = graphql`
  query pageLocations_rootQuery($locationsSortingValues: [LocationOrderInput!]!, $locationNameSearchText: String!) {
    locationCustomerRecordSynced
    ...rootShell_query
    ...locations_query
  }
`;

const LocationsPage = ({ queryReference, onReloadRequire }: Props) => {
  const rootData = usePreloadedQuery<pageLocations_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(() => rootData?.locationCustomerRecordSynced, [rootData?.locationCustomerRecordSynced]);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequire={onReloadRequire}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.locationCustomerRecordSynced]}
    >
      <Locations rootDataRelay={rootData} />
    </RootShell>
  );
};

const MemoLocationsPage = memo(LocationsPage);

const LocationsPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageLocations_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);

  useEffect(() => {
    loadQuery(
      {
        locationsSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        locationNameSearchText: '',
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
      <MemoLocationsPage queryReference={queryReference} onReloadRequire={handleReloadRequire} />
    </ErrorBoundary>
  );
};

export default memo(LocationsPageWithRelay);
