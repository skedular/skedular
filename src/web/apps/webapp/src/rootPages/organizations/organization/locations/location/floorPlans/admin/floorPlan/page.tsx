import { EditFloorPlan } from '@/components/floorPlan/editFloorPlan';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationLocationFloorPlanAdmin_rootQuery } from '@/queries/__generated__/pageOrganizationLocationFloorPlanAdmin_rootQuery.graphql';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import useKnownParams from '@/hooks/use-known-params';

const RootQuery = graphql`
  query pageOrganizationLocationFloorPlanAdmin_rootQuery($locationId: String!, $floorPlanId: String!, $resourcesSortingValues: [ResourceOrderInput!]) {
    floorPlan(id: $floorPlanId) {
      name
    }
    ...editFloorPlan_query
    ...editFloorPlan_resources_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationLocationFloorPlanAdmin_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationLocationFloorPlanAdmin_rootQuery>(RootQuery, queryReference);

  if (!rootData.floorPlan) {
    return null;
  }

  return (
    <RootShell>
      <EditFloorPlan rootDataRelay={rootData} rootDataResourcesRelay={rootData} onReloadRequired={onReloadRequired} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationLocationFloorPlanAdmin_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { locationId, floorPlanId } = useKnownParams();

  if (!locationId) {
    throw new Error('locationId is required');
  }

  if (!floorPlanId) {
    throw new Error('floorPlanId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        locationId,
        floorPlanId,
        resourcesSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, locationId, floorPlanId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
