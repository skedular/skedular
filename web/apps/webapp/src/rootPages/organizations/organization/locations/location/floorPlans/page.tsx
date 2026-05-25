import { FloorPlans } from '@/components/floorPlan/floorPlans';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageFloorPlans_rootQuery } from '@/queries/__generated__/pageFloorPlans_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { endOfDay, startOfDay } from '@skedular/shared';
import { BodyIconTypography, StackColumn } from '@skedular/ui';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import useKnownParams from '@/hooks/use-known-params';

const RootQuery = graphql`
  query pageFloorPlans_rootQuery(
    $organizationCustomDomain: String!
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
  organizationCustomDomain: string;
  locationId: string;
};

const RootPage = ({ queryReference, onReloadRequired, organizationCustomDomain, locationId }: Props) => {
  const rootData = usePreloadedQuery<pageFloorPlans_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  if (!rootData.location) {
    return null;
  }

  const availabilityDashboardUrl = `/organizations/${organizationCustomDomain}/availability-dashboard`;

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
          {'< back'}
        </Button>
        <Button
          component={Link}
          href={availabilityDashboardUrl}
          variant="text"
          aria-label="View availability dashboard for this location"
          sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}
        >
          Availability Dashboard
        </Button>
      </Box>
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
    <RootShell hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <FloorPlans
        rootDataRelay={rootData}
        rootDataFloorPlanRelay={rootData}
        rootDataBookingsRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationCustomDomain={organizationCustomDomain}
        locationId={locationId}
      />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageFloorPlans_rootQuery>(RootQuery);
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
    const today = startOfDay();
    const bookingsSearchCriteriaFrom = today.toISOString();
    const bookingsSearchCriteriaTo = endOfDay(today).toISOString();

    loadQuery(
      {
        organizationCustomDomain,
        locationId,
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
