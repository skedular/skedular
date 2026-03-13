import { BodyIconTypography, StackRow } from '@/components/commons';
import { ArrowLeftIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { useKnownParams } from '@/libs/providers';
import type { marketplaceProductDetail_rootQuery } from '@/queries/__generated__/marketplaceProductDetail_rootQuery.graphql';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import MarketplaceProductDetailBookingCard from './marketplace-product-detail-booking-card';
import MarketplaceProductDetailOverview from './marketplace-product-detail-overview';

type Props = {
  queryReference: PreloadedQuery<marketplaceProductDetail_rootQuery, Record<string, unknown>>;
};

const RootQuery = graphql`
  query marketplaceProductDetail_rootQuery($productId: String!) {
    ...marketplaceProductDetailOverview_query @arguments(productId: $productId)
    ...marketplaceProductDetailBookingCard_query @arguments(productId: $productId)
  }
`;

const MarketplaceProductDetail = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<marketplaceProductDetail_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  return (
    <Box sx={{ bgcolor: (theme) => theme.palette.background.default, minHeight: '100vh', pb: 8 }}>
      <Container maxWidth="xl" sx={{ pt: { xs: 3, md: 4 } }}>
        <Button variant="text" onClick={() => router.back()} sx={{ textTransform: 'none', px: 0, mb: 2 }}>
          <StackRow spacing={0.5} sx={{ flexWrap: 'nowrap' }}>
            <ArrowLeftIcon fontSize="small" />
            <BodyIconTypography label="Back" />
          </StackRow>
        </Button>

        <Box
          sx={{
            display: 'grid',
            gap: { xs: 3, lg: 4.5 },
            gridTemplateColumns: { xs: '1fr', lg: 'minmax(0, 1.45fr) minmax(360px, 0.95fr)' },
            alignItems: 'start',
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
