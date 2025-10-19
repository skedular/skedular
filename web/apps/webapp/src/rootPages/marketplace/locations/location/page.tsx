import { Loading } from '@/components/loading';
import { MarketplaceLocation } from '@/components/location/marketplaceLocation';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { NoOrganizationRootShell, UnauthenticatedRootShell } from '@/components/rootShell';
import type { pageMarketplaceLocation_rootQuery } from '@/queries/__generated__/pageMarketplaceLocation_rootQuery.graphql';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { useParams } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<pageMarketplaceLocation_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageMarketplaceLocation_rootQuery($locationId: String!) {
    ...marketplaceLocation_query
  }
`;

const RootPage = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<pageMarketplaceLocation_rootQuery>(RootQuery, queryReference);
  const [isUserSignedIn, setIsUserSignedIn] = useState(false);
  const { user, loading } = useAuth();

  useEffect(() => {
    setIsUserSignedIn(!!user);
  }, [user]);

  if (loading) {
    return <></>;
  }

  if (isUserSignedIn) {
    return (
      <NoOrganizationRootShell collapsed={true}>
        <MarketplaceLocation rootDataRelay={rootData} />
      </NoOrganizationRootShell>
    );
  }

  return (
    <UnauthenticatedRootShell>
      <MarketplaceLocation rootDataRelay={rootData} />
    </UnauthenticatedRootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageMarketplaceLocation_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { locationId } = useParams();

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
        locationId: finalLocationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, finalLocationId]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
