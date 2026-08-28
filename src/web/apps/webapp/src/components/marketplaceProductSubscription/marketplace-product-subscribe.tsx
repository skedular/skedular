import { getMarketplaceProductLink } from '@/components/links';
import { BodyIconTypography } from '@skedular/ui';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@skedular/shared';
import type { marketplaceProductSubscribe_rootQuery } from '@/queries/__generated__/marketplaceProductSubscribe_rootQuery.graphql';
import type { marketplaceProductSubscribeBreadcrumb_query$key } from '@/queries/__generated__/marketplaceProductSubscribeBreadcrumb_query.graphql';
import Breadcrumbs from '@mui/material/Breadcrumbs';
import Container from '@mui/material/Container';
import Link from '@mui/material/Link';
import Box from '@mui/material/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { useIntegratedPlatform } from '@skedular/shared';
import MarketplaceProductSubscribeForm from './marketplace-product-subscribe-form';
import MarketplaceProductSubscribeHero from './marketplace-product-subscribe-hero';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<marketplaceProductSubscribe_rootQuery, Record<string, unknown>>;
};

const RootQuery = graphql`
  query marketplaceProductSubscribe_rootQuery($productId: String!, $organizationCustomDomain: String!) {
    ...marketplaceProductSubscribeBreadcrumb_query @arguments(productId: $productId)
    organization(customDomain: $organizationCustomDomain) {
      spacesPublicBookingAvailability {
        available
        message
      }
    }
    product(id: $productId) {
      ...marketplaceProductSubscribeHero_product
    }
    ...marketplaceProductSubscribeForm_query @arguments(productId: $productId)
  }
`;

const MarketplaceProductSubscribe = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<marketplaceProductSubscribe_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const { integratedPlatform } = useIntegratedPlatform();
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();
  const breadcrumbData = useFragment<marketplaceProductSubscribeBreadcrumb_query$key>(
    graphql`
      fragment marketplaceProductSubscribeBreadcrumb_query on Query @argumentDefinitions(productId: { type: "String!" }) {
        product(id: $productId) {
          id
          listingMetadata {
            title
          }
        }
      }
    `,
    rootData,
  );
  const product = breadcrumbData.product;
  const productTitle = product?.listingMetadata.title ?? 'Product';

  return (
    <Box
      sx={{
        minHeight: '100vh',
        pb: 8,
        background:
          'radial-gradient(circle at top left, rgba(23, 93, 175, 0.14), transparent 28%), radial-gradient(circle at top right, rgba(255, 159, 67, 0.12), transparent 22%)',
      }}
    >
      <Container maxWidth="xl" sx={{ pt: { xs: 3, md: 4 } }}>
        <Breadcrumbs separator="/" sx={{ mb: 3 }}>
          <Link component="button" onClick={() => router.back()} underline="hover" color="text.secondary" sx={{ fontSize: '0.9rem' }}>
            Marketplace
          </Link>
          <Link
            component="button"
            onClick={() => {
              if (product) {
                router.push(getMarketplaceProductLink(integratedPlatform, isCustomDomain, organizationCustomDomain, product.id));
              }
            }}
            underline="hover"
            color="text.secondary"
            sx={{ fontSize: '0.9rem' }}
          >
            {productTitle}
          </Link>
          <BodyIconTypography label="Checkout" color="text.primary" />
        </Breadcrumbs>

        {rootData.product ? <MarketplaceProductSubscribeHero productRelay={rootData.product} /> : null}
        <Box sx={{ mt: 3 }}>
          <MarketplaceProductSubscribeForm
            bookingAvailable={rootData.organization?.spacesPublicBookingAvailability.available ?? false}
            bookingAvailabilityMessage={rootData.organization?.spacesPublicBookingAvailability.message ?? 'Bookings are currently unavailable for this workspace.'}
            rootDataRelay={rootData}
          />
        </Box>
      </Container>
    </Box>
  );
};

const MemoMarketplaceProductSubscribe = memo(MarketplaceProductSubscribe);

const MarketplaceProductSubscribeWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<marketplaceProductSubscribe_rootQuery>(RootQuery);
  const { productId, organizationCustomDomain } = useKnownParams();

  if (!productId || !organizationCustomDomain) {
    throw new Error('productId and organizationCustomDomain are required');
  }

  useEffect(() => {
    loadQuery(
      {
        productId,
        organizationCustomDomain,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain, productId]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoMarketplaceProductSubscribe queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(MarketplaceProductSubscribeWithRelay);
