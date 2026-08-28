import { BodyIconTypography } from '@skedular/ui';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@skedular/shared';
import type { marketplaceProductDetail_rootQuery } from '@/queries/__generated__/marketplaceProductDetail_rootQuery.graphql';
import type { marketplaceProductDetailBreadcrumb_query$key } from '@/queries/__generated__/marketplaceProductDetailBreadcrumb_query.graphql';
import Breadcrumbs from '@mui/material/Breadcrumbs';
import Container from '@mui/material/Container';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import MarketplaceProductDetailBookingCard from './marketplace-product-detail-booking-card';
import MarketplaceProductDetailOverview from './marketplace-product-detail-overview';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<marketplaceProductDetail_rootQuery, Record<string, unknown>>;
};

const RootQuery = graphql`
  query marketplaceProductDetail_rootQuery($productId: String!) {
    ...marketplaceProductDetailBreadcrumb_query @arguments(productId: $productId)
    ...marketplaceProductDetailOverview_query @arguments(productId: $productId)
    ...marketplaceProductDetailBookingCard_query @arguments(productId: $productId)
  }
`;

const MarketplaceProductDetail = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<marketplaceProductDetail_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const breadcrumbData = useFragment<marketplaceProductDetailBreadcrumb_query$key>(
    graphql`
      fragment marketplaceProductDetailBreadcrumb_query on Query @argumentDefinitions(productId: { type: "String!" }) {
        product(id: $productId) {
          listingMetadata {
            title
          }
        }
      }
    `,
    rootData,
  );
  const productTitle = breadcrumbData.product?.listingMetadata.title ?? 'Product';

  return (
    <Box sx={{ bgcolor: (theme) => theme.palette.background.default, minHeight: '100vh', pb: 8 }}>
      <Container maxWidth="xl" sx={{ pt: { xs: 3, md: 4 } }}>
        <Breadcrumbs separator="/" sx={{ mb: 3 }}>
          <Link component="button" onClick={() => router.back()} underline="hover" color="text.secondary" sx={{ fontSize: '0.9rem' }}>
            Marketplace
          </Link>
          <BodyIconTypography label={productTitle} color="text.primary" />
        </Breadcrumbs>

        <Box
          sx={{
            display: 'grid',
            gap: { xs: 3, lg: 4.5 },
            gridTemplateColumns: { xs: '1fr', lg: 'minmax(0, 1.45fr) minmax(360px, 0.95fr)' },
            alignItems: 'start',
            minWidth: 0,
          }}
        >
          <MarketplaceProductDetailOverview rootDataRelay={rootData} />
          <MarketplaceProductDetailBookingCard rootDataRelay={rootData} />
        </Box>
      </Container>
    </Box>
  );
};

const MemoMarketplaceProductDetail = memo(MarketplaceProductDetail);

const MarketplaceProductDetailWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<marketplaceProductDetail_rootQuery>(RootQuery);
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
      <MemoMarketplaceProductDetail queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(MarketplaceProductDetailWithRelay);
