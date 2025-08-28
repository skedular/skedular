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
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

const RootQuery = graphql`
  query pageOrganizationLocation_rootQuery(
    $organizationUniqueAlphanumericName: String!
    $locationId: String!
    $resourceNameSearchText: String
    $resourceZoneIds: [String!]
    $resourceCustomTagIds: [String!]
    $zonesSortingValues: [OrganizationTagOrderInput!]
    $customTagsSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesLocationTagsSortingValues: [OrganizationTagOrderInput!]
    $resourcesSortingValues: [ResourceOrderInput!]
    $floorPlansSortingValues: [FloorPlanOrderInput!]
  ) {
    location(id: $locationId) {
      name
    }
    ...organizationLocation_query
    ...organizationLocation_resources_query
    ...organizationLocation_floorPlans_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationLocation_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
  locationId: string;
};

const RootPage = ({ queryReference, onReloadRequired, organizationUniqueAlphanumericName, locationId }: Props) => {
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
        rootDataFloorPlansRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
        locationId={locationId}
      />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationLocation_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationUniqueAlphanumericName, locationId } = useParams();
  let finalOrganizationUniqueAlphanumericName = '';

  if (typeof organizationUniqueAlphanumericName === 'string') {
    finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName;
  } else if (Array.isArray(organizationUniqueAlphanumericName)) {
    if (typeof organizationUniqueAlphanumericName[0] === 'undefined') {
      throw new Error('organizationUniqueAlphanumericName is required');
    }

    finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName[0];
  } else {
    throw new Error('organizationUniqueAlphanumericName is required');
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
        organizationUniqueAlphanumericName: finalOrganizationUniqueAlphanumericName,
        locationId: finalLocationId,
        zonesSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        customTagsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        multipleChoicesLocationTagsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        resourcesSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        floorPlansSortingValues: [
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
  }, [loadQuery, triggerReloadId, finalOrganizationUniqueAlphanumericName, finalLocationId]);

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
      <MemoRootPage
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationUniqueAlphanumericName={finalOrganizationUniqueAlphanumericName}
        locationId={finalLocationId}
      />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
