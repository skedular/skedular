import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import { OrganizationLocation } from '@/components/organization/organizationLocation';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@/libs/providers';
import type { pageOrganizationLocation_rootQuery } from '@/queries/__generated__/pageOrganizationLocation_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

const RootQuery = graphql`
  query pageOrganizationLocation_rootQuery(
    $organizationCustomDomain: String!
    $locationId: String!
    $resourceNameSearchText: String
    $resourceZoneIds: [String!]
    $resourceCustomTagIds: [String!]
    $zonesSortingValues: [OrganizationTagOrderInput!]
    $customTagsSortingValues: [OrganizationTagOrderInput!]
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
  organizationCustomDomain: string;
  locationId: string;
};

const RootPage = ({ queryReference, onReloadRequired, organizationCustomDomain, locationId }: Props) => {
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
        organizationCustomDomain={organizationCustomDomain}
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
  const { organizationCustomDomain, locationId } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  if (!locationId) {
    throw new Error('locationId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        locationId,
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
  }, [loadQuery, triggerReloadId, organizationCustomDomain, locationId]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} locationId={locationId} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
