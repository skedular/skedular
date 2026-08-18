import { Loading } from '@/components/loading';
import { MarketplaceLocations } from '@/components/location/marketplaceLocations';
import CustomerEntitlementsStrip from '@/components/marketplaceEntitlement/customer-entitlements-strip';
import { NoOrganizationRootShell, UnauthenticatedRootShell } from '@/components/rootShell';
import logger from '@/libs/logging';
import { logAggregateMarketplaceDiscoveryStarted } from '@/libs/logging/aggregate-marketplace-telemetry';
import type { pageHome_rootQuery } from '@/queries/__generated__/pageHome_rootQuery.graphql';
import { RelayError, toRootError } from '@skedular/shared';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<pageHome_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageHome_rootQuery(
    $searchBoundaries: PolygonInput
    $locationsSortingValues: [LocationOrderInput!]
    $resourceTypeToFilterWith: OrganizationTagType
    $userSignedIn: Boolean!
  ) {
    ...marketplaceLocations_query
    ...marketplaceLocations_locations_query
    ...customerEntitlementsStrip_query @include(if: $userSignedIn) @alias
  }
`;

const RootPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageHome_rootQuery>(RootQuery, queryReference);
  const { user } = useAuth();

  if (user) {
    return (
      <NoOrganizationRootShell>
        <CustomerEntitlementsStrip queryReference={rootData.customerEntitlementsStrip_query!} />
        <MarketplaceLocations rootDataRelay={rootData} rootDataLocationsRelay={rootData} onReloadRequired={onReloadRequired} />
      </NoOrganizationRootShell>
    );
  }

  return (
    <UnauthenticatedRootShell>
      <MarketplaceLocations rootDataRelay={rootData} rootDataLocationsRelay={rootData} onReloadRequired={onReloadRequired} />
    </UnauthenticatedRootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageHome_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { user, loading } = useAuth();

  useEffect(() => {
    logAggregateMarketplaceDiscoveryStarted({ logger, isSignedIn: !loading && !!user, hasFilters: false });
    loadQuery(
      {
        locationsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        userSignedIn: !loading && !!user,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, loading, user]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
