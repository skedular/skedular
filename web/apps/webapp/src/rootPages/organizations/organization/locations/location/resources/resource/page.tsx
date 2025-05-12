import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { EditResource } from '@/components/resource/editResource';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationLocationResource_rootQuery } from '@/queries/__generated__/pageOrganizationLocationResource_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { nanoid } from 'nanoid';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

const RootQuery = graphql`
  query pageOrganizationLocationResource_rootQuery(
    $organizationId: String!
    $locationId: String!
    $resourceId: String!
    $multipleChoicesCustomTagsSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesZonesSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]
  ) {
    resource(id: $resourceId) {
      name
    }
    ...editResource_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationLocationResource_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  resourceId: string;
};

const RootPage = ({ queryReference, onReloadRequired, organizationId, resourceId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationLocationResource_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  if (!rootData.resource) {
    return <></>;
  }

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="Resource Settings" />
          <BodyIconTypography label={rootData.resource.name} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <EditResource rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationId={organizationId} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationLocationResource_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();
  const { organizationId, locationId, resourceId } = useParams();
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

  let finalResourceId = '';

  if (typeof resourceId === 'string') {
    finalResourceId = resourceId;
  } else if (Array.isArray(resourceId)) {
    if (typeof resourceId[0] === 'undefined') {
      throw new Error('resourceId is required');
    }

    finalResourceId = resourceId[0];
  } else {
    throw new Error('resourceId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationId: finalOrganizationId,
        locationId: finalLocationId,
        resourceId: finalResourceId,
        multipleChoicesCustomTagsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        multipleChoicesZonesSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        multipleChoicesProductTagsSortingValues: [
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
  }, [loadQuery, triggerReloadId, finalOrganizationId, finalLocationId, finalResourceId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} resourceId={finalResourceId} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
