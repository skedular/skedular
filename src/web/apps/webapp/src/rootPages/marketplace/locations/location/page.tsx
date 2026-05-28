import { Loading } from '@/components/loading';
import { MarketplaceLocation } from '@/components/location/marketplaceLocation';
import { RelayError, toRootError } from '@/components/relayError';
import { NoOrganizationRootShell, OrganizationStoreFrontRootShell, UnauthenticatedOrganizationStoreFrontRootShell, UnauthenticatedRootShell } from '@/components/rootShell';
import type { pageMarketplaceLocation_rootQuery } from '@/queries/__generated__/pageMarketplaceLocation_rootQuery.graphql';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<pageMarketplaceLocation_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageMarketplaceLocation_rootQuery($locationId: String!, $selectedFloorPlanId: String, $floorPlanSelected: Boolean!) {
    ...marketplaceLocation_query @arguments(locationId: $locationId, selectedFloorPlanId: $selectedFloorPlanId, floorPlanSelected: $floorPlanSelected)
  }
`;

const RootPage = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<pageMarketplaceLocation_rootQuery>(RootQuery, queryReference);
  const { user } = useAuth();
  const { isCustomDomain } = useKnownParams();

  if (user) {
    if (isCustomDomain) {
      return (
        <OrganizationStoreFrontRootShell>
          <MarketplaceLocation rootDataRelay={rootData} />
        </OrganizationStoreFrontRootShell>
      );
    } else {
      return (
        <NoOrganizationRootShell>
          <MarketplaceLocation rootDataRelay={rootData} />
        </NoOrganizationRootShell>
      );
    }
  }

  if (isCustomDomain) {
    return (
      <UnauthenticatedOrganizationStoreFrontRootShell>
        <MarketplaceLocation rootDataRelay={rootData} />
      </UnauthenticatedOrganizationStoreFrontRootShell>
    );
  } else {
    return (
      <UnauthenticatedRootShell>
        <MarketplaceLocation rootDataRelay={rootData} />
      </UnauthenticatedRootShell>
    );
  }
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageMarketplaceLocation_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { locationId } = useKnownParams();

  if (!locationId) {
    throw new Error('locationId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        locationId,
        selectedFloorPlanId: null,
        floorPlanSelected: false,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, locationId]);

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
