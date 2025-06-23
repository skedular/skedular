import { BodyIconTypography, StackColumn } from '@/components/commons';
import { FloorPlans } from '@/components/floorPlan/floorPlans';
import { Loading } from '@/components/loading';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { endOfDay, startOfDay } from '@/libs/utils';
import type { pageFloorPlans_rootQuery } from '@/queries/__generated__/pageFloorPlans_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { nanoid } from 'nanoid';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

const RootQuery = graphql`
  query pageFloorPlans_rootQuery(
    $organizationId: String!
    $locationId: String!
    $floorPlanId: String!
    $floorPlanExists: Boolean!
    $zonesSortingValues: [OrganizationTagOrderInput!]
    $customTagsSortingValues: [OrganizationTagOrderInput!]
    $floorPlansSortingValues: [FloorPlanOrderInput!]
    $resourcesSortingValues: [ResourceOrderInput!]
    $peopleNameSearchText: String
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
  ) {
    location(id: $locationId) {
      name
    }
    ...floorPlans_query
    ...floorPlans_floorPlan_query
    ...floorPlans_bookings_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageFloorPlans_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  locationId: string;
};

const RootPage = ({ queryReference, onReloadRequired, organizationId, locationId }: Props) => {
  const rootData = usePreloadedQuery<pageFloorPlans_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

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
          <BodyIconTypography label="Floor Plans" />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <FloorPlans
        rootDataRelay={rootData}
        rootDataFloorPlanRelay={rootData}
        rootDataBookingsRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationId={organizationId}
        locationId={locationId}
      />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageFloorPlans_rootQuery>(RootQuery);
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
    const today = startOfDay();
    const bookingsSearchCriteriaFrom = today.toISOString();
    const bookingsSearchCriteriaTo = endOfDay(today).toISOString();

    loadQuery(
      {
        organizationId: finalOrganizationId,
        locationId: finalLocationId,
        floorPlanId: '',
        floorPlanExists: false,
        bookingsSearchCriteriaFrom,
        bookingsSearchCriteriaTo,
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
        floorPlansSortingValues: [
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
        organizationMembersSortingValues: [
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
