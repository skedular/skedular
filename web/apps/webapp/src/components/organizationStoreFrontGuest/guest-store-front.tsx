import { BodyIconTypography, LargeHeadingIconTypography, MediumHeadingIconTypography, SubtitleIconTypography } from '@/components/commons';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { useKnownParams } from '@/libs/providers';
import type { guestStoreFrontProductsRefetchQuery } from '@/queries/__generated__/guestStoreFrontProductsRefetchQuery.graphql';
import type { guestStoreFrontProducts_query$key } from '@/queries/__generated__/guestStoreFrontProducts_query.graphql';
import type { guestStoreFront_rootQuery } from '@/queries/__generated__/guestStoreFront_rootQuery.graphql';
import Container from '@mui/material/Container';
import { alpha } from '@mui/material/styles';
import Box from '@mui/system/Box';
import { memo, useEffect, useMemo, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import GuestStoreFrontFooter from './guest-store-front-footer';
import GuestStoreFrontLocationsStrip from './guest-store-front-locations-strip';
import GuestStoreFrontProductCard from './guest-store-front-product-card';

type Props = {
  queryReference: PreloadedQuery<guestStoreFront_rootQuery, Record<string, unknown>>;
  organizationUniqueAlphanumericName: string;
};

const RootQuery = graphql`
  query guestStoreFront_rootQuery($organizationUniqueAlphanumericName: String!) {
    organizationPublic(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
      name
      listingMetadata {
        title
        subTitle
      }
      marketplaceListingMetadata {
        title
        subTitle
      }
      featureImages {
        original {
          url
          height
          width
        }
      }
    }
    ...guestStoreFrontProducts_query @arguments(organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName)
    ...guestStoreFrontLocationsStrip_query
    ...guestStoreFrontProductCard_query
    ...guestStoreFrontFooter_query
  }
`;

const GuestStoreFront = ({ queryReference, organizationUniqueAlphanumericName }: Props) => {
  const rootData = usePreloadedQuery<guestStoreFront_rootQuery>(RootQuery, queryReference);
  const [productsData, refetchProducts] = useRefetchableFragment<guestStoreFrontProductsRefetchQuery, guestStoreFrontProducts_query$key>(
    graphql`
      fragment guestStoreFrontProducts_query on Query
      @refetchable(queryName: "guestStoreFrontProductsRefetchQuery")
      @argumentDefinitions(organizationUniqueAlphanumericName: { type: "String!" }, locationSelected: { type: "Boolean", defaultValue: false }) {
        marketplaceLocations(where: { organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName }) @include(if: $locationSelected) {
          edges {
            node {
              id
              products {
                id
                pricingOptions {
                  index
                }
                ...guestStoreFrontProductCard_product
              }
            }
          }
        }
        products(where: { organizationUniqueAlphanumericNames: [$organizationUniqueAlphanumericName], includeInactive: false }) @skip(if: $locationSelected) {
          edges {
            node {
              id
              pricingOptions {
                index
              }
              ...guestStoreFrontProductCard_product
            }
          }
        }
      }
    `,
    rootData,
  );

  const [selectedLocationId, setSelectedLocationId] = useState('');

  const displayedProducts = useMemo(
    () =>
      (selectedLocationId
        ? (productsData.marketplaceLocations?.edges.find((edge) => edge.node.id === selectedLocationId)?.node.products ?? [])
        : (productsData.products?.edges.map((edge) => edge.node) ?? [])
      ).toSorted((left, right) => {
        const leftIndex = left.pricingOptions[0]?.index ?? Number.MAX_SAFE_INTEGER;
        const rightIndex = right.pricingOptions[0]?.index ?? Number.MAX_SAFE_INTEGER;

        return leftIndex - rightIndex;
      }),
    [productsData.marketplaceLocations?.edges, productsData.products?.edges, selectedLocationId],
  );

  useEffect(() => {
    refetchProducts(
      {
        organizationUniqueAlphanumericName,
        locationSelected: selectedLocationId !== '',
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [organizationUniqueAlphanumericName, refetchProducts, selectedLocationId]);

  if (!rootData.organizationPublic) {
    return null;
  }

  return (
    <Box sx={{ bgcolor: (theme) => theme.palette.background.default, minHeight: '100vh' }}>
      <Container maxWidth="xl" sx={{ mt: { xs: 3, md: 4 } }}>
        <GuestStoreFrontLocationsStrip rootDataRelay={rootData} onLocationChange={setSelectedLocationId} />
      </Container>

      <Container maxWidth="xl" sx={{ mt: { xs: 3, md: 5 }, mb: 7 }}>
        <Box
          sx={{
            position: 'relative',
            height: { xs: 340, md: 520 },
            borderRadius: 3,
            overflow: 'hidden',
            border: 1,
            borderColor: (theme) => theme.palette.divider,
          }}
        >
          {rootData.organizationPublic.featureImages.length > 0 && rootData.organizationPublic.featureImages[0].original && (
            <Box
              component="img"
              src={rootData.organizationPublic.featureImages[0].original.url}
              alt={rootData.organizationPublic.name}
              sx={{ width: '100%', height: '100%', objectFit: 'cover', display: 'block' }}
            />
          )}
          <Box
            sx={{
              position: 'absolute',
              inset: 0,
              background: (theme) => `linear-gradient(180deg, ${alpha(theme.palette.common.black, 0.15)} 0%, ${alpha(theme.palette.common.black, 0.7)} 100%)`,
              display: 'flex',
              alignItems: 'flex-end',
            }}
          >
            <Box sx={{ p: { xs: 3, md: 5 }, maxWidth: 850 }}>
              {rootData.organizationPublic.listingMetadata.title && (
                <LargeHeadingIconTypography label={rootData.organizationPublic.listingMetadata.title} sx={{ color: (theme) => theme.palette.common.white, mb: 1 }} />
              )}
              {rootData.organizationPublic.listingMetadata.subTitle && (
                <SubtitleIconTypography
                  label={`${rootData.organizationPublic.listingMetadata.subTitle}`}
                  sx={{ color: (theme) => alpha(theme.palette.common.white, 0.92), mb: 3 }}
                />
              )}
            </Box>
          </Box>
        </Box>
      </Container>

      <Container maxWidth="xl" sx={{ mb: 6 }}>
        <Box sx={{ mb: 4 }}>
          {rootData.organizationPublic.marketplaceListingMetadata.title && (
            <MediumHeadingIconTypography label={rootData.organizationPublic.marketplaceListingMetadata.title} sx={{ mb: 1 }} />
          )}
          {rootData.organizationPublic.marketplaceListingMetadata.subTitle && (
            <BodyIconTypography label={rootData.organizationPublic.marketplaceListingMetadata.subTitle} sx={{ opacity: 0.85 }} />
          )}
        </Box>

        <Box
          sx={{
            display: 'grid',
            gap: 3,
            gridTemplateColumns: {
              xs: '1fr',
              sm: '1fr 1fr',
              lg: 'repeat(4, minmax(0, 1fr))',
            },
          }}
        >
          {displayedProducts.map((product) => (
            <GuestStoreFrontProductCard key={product.id} rootDataRelay={rootData} productRelay={product} />
          ))}
        </Box>
      </Container>

      <GuestStoreFrontFooter rootDataRelay={rootData} />
    </Box>
  );
};

const MemoGuestStoreFront = memo(GuestStoreFront);

const GuestStoreFrontWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<guestStoreFront_rootQuery>(RootQuery);
  const { organizationUniqueAlphanumericName } = useKnownParams();

  if (!organizationUniqueAlphanumericName) {
    throw new Error('organizationUniqueAlphanumericName is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationUniqueAlphanumericName,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationUniqueAlphanumericName]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoGuestStoreFront queryReference={queryReference} organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />
    </ErrorBoundary>
  );
};

export default memo(GuestStoreFrontWithRelay);
