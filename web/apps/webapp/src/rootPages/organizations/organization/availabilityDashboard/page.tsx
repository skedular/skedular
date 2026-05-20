'use client';

import AvailabilityDashboard from '@/components/availabilityDashboard/AvailabilityDashboard';
import type { AvailabilityFilters } from '@/components/availabilityDashboard/AvailabilityFilterBar';
import { Loading } from '@/components/loading';
import { RootShell } from '@/components/rootShell';
import type { pageAvailabilityDashboardQuery, ResourceAvailabilityClassification } from '@/queries/__generated__/pageAvailabilityDashboardQuery.graphql';
import { memo, Suspense, useCallback, useEffect, useState, useTransition } from 'react';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import useKnownParams from '@/hooks/use-known-params';

// Hardcoded ordering — no sort controls in the UI.
const HARDCODED_ORDER_BY = [{ direction: 'ASCENDING' as const, field: 'RESOURCE_NAME' as const }];

const getTodayString = () => {
  const now = new Date();
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-${String(now.getDate()).padStart(2, '0')}`;
};

const RootQuery = graphql`
  query pageAvailabilityDashboardQuery(
    $organizationCustomDomain: String!
    $filter: ResourceAvailabilityFilterInput!
    $orderBy: [ResourceAvailabilityOrderByInput!]!
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    resourceDayViews(filter: $filter, orderBy: $orderBy) {
      ...AvailabilityDashboard_data
    }
    resourceAvailabilityStatuses {
      ...AvailabilityFilterBar_statuses
    }
    ...AvailabilityFilterBar_locations
  }
`;

type ContentProps = {
  queryReference: PreloadedQuery<pageAvailabilityDashboardQuery>;
  filters: AvailabilityFilters;
  onFiltersChange: (filters: AvailabilityFilters) => void;
  onRefresh: () => void;
  isPending: boolean;
  organizationCustomDomain: string;
};

const AvailabilityDashboardContent = ({ queryReference, filters, onFiltersChange, onRefresh, isPending, organizationCustomDomain }: ContentProps) => {
  const data = usePreloadedQuery<pageAvailabilityDashboardQuery>(RootQuery, queryReference);

  return (
    <AvailabilityDashboard
      dataRef={data.resourceDayViews}
      statusesRef={data.resourceAvailabilityStatuses}
      locationsRef={data}
      filters={filters}
      onFiltersChange={onFiltersChange}
      onRefresh={onRefresh}
      isPending={isPending}
      organizationCustomDomain={organizationCustomDomain}
    />
  );
};

type ShellProps = { organizationCustomDomain: string };

const AvailabilityDashboardPageShell = ({ organizationCustomDomain }: ShellProps) => {
  const [queryReference, loadQuery] = useQueryLoader<pageAvailabilityDashboardQuery>(RootQuery);
  const [isPending, startTransition] = useTransition();

  const [filters, setFilters] = useState<AvailabilityFilters>({
    date: getTodayString(),
    locationIds: [],
    statuses: [],
  });

  const buildQueryVars = useCallback(
    (f: AvailabilityFilters) => ({
      organizationCustomDomain,
      filter: {
        date: f.date,
        organizationCustomDomain,
        locationIds: f.locationIds ?? [],
        statuses: (f.statuses ?? []) as ResourceAvailabilityClassification[],
      },
      orderBy: HARDCODED_ORDER_BY,
      locationsSortingValues: [{ field: 'NAME' as const, direction: 'ASCENDING' as const }],
    }),
    [organizationCustomDomain],
  );

  // Initial load — suspends via Suspense boundary.
  useEffect(() => {
    loadQuery(buildQueryVars(filters), { fetchPolicy: 'store-and-network' });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleFiltersChange = useCallback(
    (newFilters: AvailabilityFilters) => {
      setFilters(newFilters);
      // startTransition keeps the old content visible while fetching — no re-mount.
      startTransition(() => {
        loadQuery(buildQueryVars(newFilters), { fetchPolicy: 'network-only' });
      });
    },
    [buildQueryVars, loadQuery, startTransition],
  );

  const handleRefresh = useCallback(() => {
    startTransition(() => {
      loadQuery(buildQueryVars(filters), { fetchPolicy: 'network-only' });
    });
  }, [buildQueryVars, filters, loadQuery, startTransition]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <AvailabilityDashboardContent
      queryReference={queryReference}
      filters={filters}
      onFiltersChange={handleFiltersChange}
      onRefresh={handleRefresh}
      isPending={isPending}
      organizationCustomDomain={organizationCustomDomain}
    />
  );
};

const RootPage = () => {
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  return (
    <RootShell>
      <Suspense fallback={<Loading />}>
        <AvailabilityDashboardPageShell organizationCustomDomain={organizationCustomDomain} />
      </Suspense>
    </RootShell>
  );
};

export default memo(RootPage);
