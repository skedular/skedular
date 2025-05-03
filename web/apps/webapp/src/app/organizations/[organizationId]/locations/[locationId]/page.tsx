'use client';

import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import { OrganizationLocation } from '@/components/organization/organizationLocation';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationLocation_rootQuery } from '@/queries/__generated__/pageOrganizationLocation_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { nanoid } from 'nanoid';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

const RootQuery = graphql`
  query pageOrganizationLocation_rootQuery(
    $organizationId: String!
    $locationId: String!
    $resourceNameSearchText: String
    $resourceZoneIds: [String!]
    $resourceCustomTagIds: [String!]
    $zonesSortingValues: [OrganizationTagOrderInput!]
    $customTagsSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesLocationTagsSortingValues: [OrganizationTagOrderInput!]
  ) {
    location(id: $locationId) {
      name
    }
    ...organizationLocation_query
    ...organizationLocation_resources_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationLocation_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  locationId: string;
};

const RootPage = ({ queryReference, onReloadRequired, organizationId, locationId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationLocation_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="Location Settings" />
          <BodyIconTypography label={rootData.location?.name} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <OrganizationLocation
        rootDataRelay={rootData}
        rootDataResourcesRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationId={organizationId}
        locationId={locationId}
      />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationLocation_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();
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

  useEffect(() => {
    loadQuery(
      {
        organizationId: finalOrganizationId,
        locationId: finalLocationId,
        zonesSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        customTagsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        multipleChoicesLocationTagsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, finalOrganizationId, finalLocationId]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} locationId={finalLocationId} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
