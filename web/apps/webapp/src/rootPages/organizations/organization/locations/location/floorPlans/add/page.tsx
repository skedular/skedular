import { BodyIconTypography, StackColumn } from '@/components/commons';
import { AddFloorPlan } from '@/components/floorPlan/addFloorPlan';
import { Loading } from '@/components/loading';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationLocationFloorPlansAdd_rootQuery } from '@/queries/__generated__/pageOrganizationLocationFloorPlansAdd_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

const RootQuery = graphql`
  query pageOrganizationLocationFloorPlansAdd_rootQuery($locationId: String!) {
    location(id: $locationId) {
      name
    }
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationLocationFloorPlansAdd_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  locationId: string;
};

const RootPage = ({ queryReference, locationId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationLocationFloorPlansAdd_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  const handleAdded = () => {
    router.back();
  };

  const handleCancelled = () => {
    router.back();
  };

  const handleReloadRequired = () => {};

  if (!rootData.location) {
    return <></>;
  }

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="Location" />
          <BodyIconTypography label={rootData.location.name} />
          <BodyIconTypography label="Add new floor plan" />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <AddFloorPlan locationId={locationId} showDismiss={false} onAdded={handleAdded} onCancel={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationLocationFloorPlansAdd_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { locationId } = useParams();

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

  useEffect(() => {
    loadQuery(
      {
        locationId: finalLocationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, finalLocationId]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} locationId={finalLocationId} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
