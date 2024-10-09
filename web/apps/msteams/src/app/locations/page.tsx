import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { Locations } from 'components/location/locations';
import { RootShell } from 'components/rootShell';
import { memo, useCallback, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { useParams } from 'react-router-dom';
import type { pageLocations_rootQuery } from './__generated__/pageLocations_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<pageLocations_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query pageLocations_rootQuery($organizationId: String!, $locationsSortingValues: [LocationOrderInput!]!, $locationNameSearchText: String!) {
    locationCustomerRecordSynced
    ...rootShell_query
    ...locations_query
  }
`;

const LocationsPage = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootData = usePreloadedQuery<pageLocations_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(() => rootData?.locationCustomerRecordSynced, [rootData?.locationCustomerRecordSynced]);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequired={onReloadRequired}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.locationCustomerRecordSynced]}
    >
      <Locations rootDataRelay={rootData} organizationId={organizationId} />
    </RootShell>
  );
};

const MemoLocationsPage = memo(LocationsPage);

const LocationsPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageLocations_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();
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
      {
        organizationId: finalOrganizationId,
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
  }, [loadQuery, triggerReload, finalOrganizationId]);

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
      <MemoLocationsPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} />
    </ErrorBoundary>
  );
};

export default memo(LocationsPageWithRelay);
