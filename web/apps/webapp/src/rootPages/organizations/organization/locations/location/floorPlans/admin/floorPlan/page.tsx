import { BodyIconTypography, StackColumn } from '@/components/commons';
import { EditFloorPlan } from '@/components/floorPlan/editFloorPlan';
import { Loading } from '@/components/loading';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationLocationFloorPlanAdmin_rootQuery } from '@/queries/__generated__/pageOrganizationLocationFloorPlanAdmin_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

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
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  if (!rootData.floorPlan) {
    return <></>;
  }

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="Floor Plan" />
          <BodyIconTypography label={rootData.floorPlan.name} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <EditFloorPlan rootDataRelay={rootData} rootDataResourcesRelay={rootData} onReloadRequired={onReloadRequired} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationLocationFloorPlanAdmin_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { locationId, floorPlanId } = useParams();

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

  let finalFloorPlanId = '';

  if (typeof floorPlanId === 'string') {
    finalFloorPlanId = floorPlanId;
  } else if (Array.isArray(floorPlanId)) {
    if (typeof floorPlanId[0] === 'undefined') {
      throw new Error('floorPlanId is required');
    }

    finalFloorPlanId = floorPlanId[0];
  } else {
    throw new Error('floorPlanId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        locationId: finalLocationId,
        floorPlanId: finalFloorPlanId,
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
  }, [loadQuery, triggerReloadId, finalLocationId, finalFloorPlanId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
