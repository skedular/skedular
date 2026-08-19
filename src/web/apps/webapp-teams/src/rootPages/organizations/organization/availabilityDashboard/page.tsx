'use client';

import AvailabilityDashboard from '@/components/availabilityDashboard/AvailabilityDashboard';
import type { AvailabilityFilters } from '@/components/availabilityDashboard/AvailabilityFilterBar';
import { Loading } from '@/components/loading';
import { RootShell } from '@/components/rootShell';
import type { pageAvailabilityDashboardQuery, ResourceAvailabilityClassification } from '@/queries/__generated__/pageAvailabilityDashboardQuery.graphql';
import { useKnownParams } from '@skedular/shared';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, Suspense, useCallback, useEffect, useMemo, useTransition } from 'react';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

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
  const router = useRouter();
  const searchParams = useSearchParams();

  const filters = useMemo<AvailabilityFilters>(
    () => ({
      date: searchParams.get('date') ?? getTodayString(),
      locationIds: searchParams.get('locationIds')?.split(',').filter(Boolean) ?? [],
      statuses: searchParams.get('statuses')?.split(',').filter(Boolean) ?? [],
    }),
    [searchParams],
  );

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

  // URL parameters are the source of truth so refresh and browser navigation restore the dashboard.
  useEffect(() => {
    loadQuery(buildQueryVars(filters), { fetchPolicy: 'store-and-network' });
  }, [buildQueryVars, filters, loadQuery]);

  const handleFiltersChange = useCallback(
    (newFilters: AvailabilityFilters) => {
      const params = new URLSearchParams(window.location.search);
      params.set('date', newFilters.date);
      if (newFilters.locationIds?.length) params.set('locationIds', newFilters.locationIds.join(','));
      else params.delete('locationIds');
      if (newFilters.statuses?.length) params.set('statuses', newFilters.statuses.join(','));
      else params.delete('statuses');
      router.push(`?${params.toString()}`);
    },
    [router],
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
