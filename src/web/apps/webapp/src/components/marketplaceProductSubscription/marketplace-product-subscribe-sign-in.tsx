import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@skedular/shared';
import type { marketplaceProductSubscribeSignIn_rootQuery } from '@/queries/__generated__/marketplaceProductSubscribeSignIn_rootQuery.graphql';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, useQueryLoader, usePreloadedQuery, PreloadedQuery } from 'react-relay';
import MarketplaceProductSubscribeAuthGate from './marketplace-product-subscribe-auth-gate';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<marketplaceProductSubscribeSignIn_rootQuery, Record<string, unknown>>;
};

const RootQuery = graphql`
  query marketplaceProductSubscribeSignIn_rootQuery($productId: String!) {
    ...marketplaceProductSubscribeAuthGate_query @arguments(productId: $productId)
  }
`;

const MarketplaceProductSubscribeSignIn = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<marketplaceProductSubscribeSignIn_rootQuery>(RootQuery, queryReference);

  return <MarketplaceProductSubscribeAuthGate rootDataRelay={rootData} />;
};

const MemoMarketplaceProductSubscribeSignIn = memo(MarketplaceProductSubscribeSignIn);

const MarketplaceProductSubscribeSignInWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<marketplaceProductSubscribeSignIn_rootQuery>(RootQuery);
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
      <MemoMarketplaceProductSubscribeSignIn queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(MarketplaceProductSubscribeSignInWithRelay);
