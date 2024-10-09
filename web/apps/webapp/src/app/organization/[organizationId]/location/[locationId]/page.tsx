'use client';

import { Location } from '@/components/location/locationPage';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationLocation_rootQuery } from '@/queries/__generated__/pageOrganizationLocation_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { useParams } from 'next/navigation';
import { memo, useCallback, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationLocation_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageOrganizationLocation_rootQuery {
    locationCustomerRecordSynced
    ...rootShell_query
  }
`;

const LocationPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationLocation_rootQuery>(RootQuery, queryReference);
  const { organizationId, locationId } = useParams();
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

  let finalLocationId = '';

  if (typeof locationId === 'string') {
    finalLocationId = locationId;
  } else if (Array.isArray(locationId)) {
    if (typeof locationId[0] === 'undefined') {
      throw new Error('locationId is required');
    }

    finalLocationId = locationId[0];
  } else {
    throw new Error('locationId is required');
  }
  const areAdditionalCustomerRecordsSync = useCallback(() => rootData?.locationCustomerRecordSynced, [rootData?.locationCustomerRecordSynced]);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequired={onReloadRequired}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.locationCustomerRecordSynced]}
    >
      <Location organizationId={finalOrganizationId} locationId={finalLocationId} />
    </RootShell>
  );
};

const MemoLocationPage = memo(LocationPage);

const LocationPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationLocation_rootQuery>(RootQuery);
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
      <MemoLocationPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(LocationPageWithRelay);
