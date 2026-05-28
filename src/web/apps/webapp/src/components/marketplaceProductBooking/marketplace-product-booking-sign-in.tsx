import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import MarketplaceProductSubscribeAuthGate from '@/components/marketplaceProductSubscription/marketplace-product-subscribe-auth-gate';
import type { marketplaceProductBookingSignIn_rootQuery } from '@/queries/__generated__/marketplaceProductBookingSignIn_rootQuery.graphql';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<marketplaceProductBookingSignIn_rootQuery, Record<string, unknown>>;
};

const RootQuery = graphql`
  query marketplaceProductBookingSignIn_rootQuery($productId: String!) {
    ...marketplaceProductSubscribeAuthGate_query @arguments(productId: $productId)
  }
`;

const MarketplaceProductBookingSignIn = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<marketplaceProductBookingSignIn_rootQuery>(RootQuery, queryReference);

  return (
    <MarketplaceProductSubscribeAuthGate
      rootDataRelay={rootData}
      mode="booking"
      bodyLabel="You’ll need an account to book workspaces and manage your booking details later."
      trustLabel="Return to this exact booking after auth"
    />
  );
};

const MemoMarketplaceProductBookingSignIn = memo(MarketplaceProductBookingSignIn);

const MarketplaceProductBookingSignInWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<marketplaceProductBookingSignIn_rootQuery>(RootQuery);
  const { productId } = useKnownParams();

  if (!productId) {
    throw new Error('productId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        productId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, productId]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoMarketplaceProductBookingSignIn queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(MarketplaceProductBookingSignInWithRelay);
