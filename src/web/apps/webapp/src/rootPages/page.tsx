import { Loading } from '@/components/loading';
import { MarketplaceLocations } from '@/components/location/marketplaceLocations';
import CustomerEntitlementsStrip from '@/components/marketplaceEntitlement/customer-entitlements-strip';
import { NoOrganizationRootShell, UnauthenticatedRootShell } from '@/components/rootShell';
import logger from '@/libs/logging';
import { logAggregateMarketplaceDiscoveryStarted } from '@/libs/logging/aggregate-marketplace-telemetry';
import type { pageHome_rootQuery } from '@/queries/__generated__/pageHome_rootQuery.graphql';
import type { pageHome_favouriteLocationsQuery } from '@/queries/__generated__/pageHome_favouriteLocationsQuery.graphql';
import { RelayError, toRootError } from '@skedular/shared';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<pageHome_rootQuery, Record<string, unknown>>;
  favouriteLocationsQueryReference: PreloadedQuery<pageHome_favouriteLocationsQuery, Record<string, unknown>> | null | undefined;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageHome_rootQuery($searchBoundaries: PolygonInput, $locationsSortingValues: [LocationOrderInput!], $resourceTypeToFilterWith: OrganizationTagType) {
    ...marketplaceLocations_locations_query
  }
`;

const FavouriteLocationsQuery = graphql`
  query pageHome_favouriteLocationsQuery {
    me {
      favouriteLocations {
        id
      }
    }
  }
`;

const RootPage = ({ queryReference, favouriteLocationsQueryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageHome_rootQuery>(RootQuery, queryReference);
  const { user } = useAuth();

  if (user) {
    return (
      <NoOrganizationRootShell>
        <CustomerEntitlementsStrip />
        {favouriteLocationsQueryReference ? (
          <AuthenticatedMarketplaceLocations rootData={rootData} favouriteLocationsQueryReference={favouriteLocationsQueryReference} onReloadRequired={onReloadRequired} />
        ) : (
          <Loading />
        )}
      </NoOrganizationRootShell>
    );
  }

  return (
    <UnauthenticatedRootShell>
      <MarketplaceLocations rootDataLocationsRelay={rootData} onReloadRequired={onReloadRequired} />
    </UnauthenticatedRootShell>
  );
};

const AuthenticatedMarketplaceLocations = ({
  rootData,
  favouriteLocationsQueryReference,
  onReloadRequired,
}: {
  rootData: pageHome_rootQuery['response'];
  favouriteLocationsQueryReference: PreloadedQuery<pageHome_favouriteLocationsQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
}) => {
  const favouriteLocationsData = usePreloadedQuery<pageHome_favouriteLocationsQuery>(FavouriteLocationsQuery, favouriteLocationsQueryReference);
  const favouriteLocationIds = new Set(favouriteLocationsData.me?.favouriteLocations.map((location) => location.id) ?? []);
  return <MarketplaceLocations rootDataLocationsRelay={rootData} favouriteLocationIds={favouriteLocationIds} onReloadRequired={onReloadRequired} />;
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageHome_rootQuery>(RootQuery);
  const [favouriteLocationsQueryReference, loadFavouriteLocationsQuery, disposeFavouriteLocationsQuery] = useQueryLoader<pageHome_favouriteLocationsQuery>(FavouriteLocationsQuery);
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
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
    if (user) {
      loadFavouriteLocationsQuery({}, { fetchPolicy: 'store-and-network' });
    } else {
      disposeFavouriteLocationsQuery();
    }
  }, [disposeFavouriteLocationsQuery, loadFavouriteLocationsQuery, loadQuery, triggerReloadId, loading, user]);

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
      {!user || favouriteLocationsQueryReference ? (
        <MemoRootPage queryReference={queryReference} favouriteLocationsQueryReference={favouriteLocationsQueryReference} onReloadRequired={handleReloadRequired} />
      ) : (
        <Loading />
      )}
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
