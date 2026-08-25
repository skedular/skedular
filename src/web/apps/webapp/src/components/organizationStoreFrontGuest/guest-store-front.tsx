import { RelayError, toRootError } from '@skedular/shared';
import { ArrowLeftIcon, ArrowRightIcon } from '@/components/icons';
import { Loading } from '@/components/loading';

import type { guestStoreFront_rootQuery } from '@/queries/__generated__/guestStoreFront_rootQuery.graphql';
import type { guestStoreFrontProducts_query$key } from '@/queries/__generated__/guestStoreFrontProducts_query.graphql';
import type { guestStoreFrontSelectedLocationProductsQuery } from '@/queries/__generated__/guestStoreFrontSelectedLocationProductsQuery.graphql';
import ButtonBase from '@mui/material/ButtonBase';
import Chip from '@mui/material/Chip';
import Container from '@mui/material/Container';
import IconButton from '@mui/material/IconButton';
import Box from '@mui/system/Box';

import { BodyIconTypography, MediumHeadingIconTypography } from '@skedular/ui';
import { memo, useEffect, useMemo, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import GuestStoreFrontFooter from './guest-store-front-footer';
import GuestStoreFrontLocationsStrip from './guest-store-front-locations-strip';
import GuestStoreFrontProductCard from './guest-store-front-product-card';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<guestStoreFront_rootQuery, Record<string, unknown>>;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query guestStoreFront_rootQuery($organizationCustomDomain: String!) {
    organizationPublic(customDomain: $organizationCustomDomain) {
      name
      marketplaceListingMetadata {
        about
        title
        subTitle
        includedFeatures
      }
      featureImages {
        original {
          url
          height
          width
        }
      }
    }
    ...guestStoreFrontProducts_query @arguments(organizationCustomDomain: $organizationCustomDomain)
    ...guestStoreFrontLocationsStrip_query
    ...guestStoreFrontProductCard_query
    ...guestStoreFrontFooter_query
  }
`;

const SelectedLocationProductsQuery = graphql`
  query guestStoreFrontSelectedLocationProductsQuery($locationId: String!) {
    location(id: $locationId) {
      products {
        id
        pricingOptions {
          index
        }
        ...guestStoreFrontProductCard_product
      }
    }
  }
`;

type ProductListProps = {
  organizationCustomDomain: string;
  products: ReadonlyArray<NonNullable<guestStoreFrontProducts_query$key[' $data']>['products']['edges'][number]['node']>;
  rootData: guestStoreFront_rootQuery['response'];
};

const sortProducts = <T extends { readonly pricingOptions: ReadonlyArray<{ readonly index: number }> }>(products: ReadonlyArray<T>) =>
  products.toSorted((left, right) => {
    const leftIndex = left.pricingOptions[0]?.index ?? Number.MAX_SAFE_INTEGER;
    const rightIndex = right.pricingOptions[0]?.index ?? Number.MAX_SAFE_INTEGER;

    return leftIndex - rightIndex;
  });

const ProductList = ({ products, rootData, organizationCustomDomain }: ProductListProps) => (
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
    {sortProducts(products).map((product) => (
      <GuestStoreFrontProductCard key={product.id} rootDataRelay={rootData} productRelay={product} organizationCustomDomain={organizationCustomDomain} />
    ))}
  </Box>
);

type SelectedLocationProductsProps = {
  queryReference: PreloadedQuery<guestStoreFrontSelectedLocationProductsQuery, Record<string, unknown>>;
  rootData: guestStoreFront_rootQuery['response'];
  organizationCustomDomain: string;
};

const SelectedLocationProducts = ({ queryReference, rootData, organizationCustomDomain }: SelectedLocationProductsProps) => {
  const selectedLocationData = usePreloadedQuery<guestStoreFrontSelectedLocationProductsQuery>(SelectedLocationProductsQuery, queryReference);

  return <ProductList products={selectedLocationData.location?.products ?? []} rootData={rootData} organizationCustomDomain={organizationCustomDomain} />;
};

const GuestStoreFront = ({ queryReference, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<guestStoreFront_rootQuery>(RootQuery, queryReference);
  const productsData = useFragment<guestStoreFrontProducts_query$key>(
    graphql`
      fragment guestStoreFrontProducts_query on Query @argumentDefinitions(organizationCustomDomain: { type: "String!" }) {
        products(where: { organizationCustomDomains: [$organizationCustomDomain], includeInactive: false }) {
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
  const [selectedOrganizationImageIndex, setSelectedOrganizationImageIndex] = useState(0);
  const [selectedLocationProductsQueryReference, loadSelectedLocationProductsQuery, disposeSelectedLocationProductsQuery] =
    useQueryLoader<guestStoreFrontSelectedLocationProductsQuery>(SelectedLocationProductsQuery);
  const defaultProducts = useMemo(() => productsData.products.edges.map((edge) => edge.node), [productsData.products.edges]);
  const organizationImages = useMemo(
    () => rootData.organizationPublic?.featureImages.map((image) => image.original).filter((image): image is NonNullable<typeof image> => Boolean(image?.url)) ?? [],
    [rootData.organizationPublic?.featureImages],
  );
  const safeSelectedOrganizationImageIndex = selectedOrganizationImageIndex < organizationImages.length ? selectedOrganizationImageIndex : 0;
  const selectedOrganizationImage = organizationImages[safeSelectedOrganizationImageIndex] ?? null;

  useEffect(() => {
    if (!selectedLocationId) {
      disposeSelectedLocationProductsQuery();
      return;
    }

    loadSelectedLocationProductsQuery(
      {
        locationId: selectedLocationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [disposeSelectedLocationProductsQuery, loadSelectedLocationProductsQuery, selectedLocationId]);

  if (!rootData.organizationPublic) {
    return null;
  }

  return (
    <Box sx={{ bgcolor: (theme) => theme.palette.background.default, minHeight: '100vh' }}>
      <Container maxWidth="xl" sx={{ mt: { xs: 3, md: 5 }, mb: { xs: 4, md: 6 } }}>
        <Box
          sx={{
            display: 'grid',
            gridTemplateColumns: { xs: '1fr', md: 'minmax(0, 0.95fr) minmax(320px, 0.8fr)' },
            gap: { xs: 2.5, md: 5 },
            alignItems: 'center',
          }}
        >
          <Box
            sx={{
              position: 'relative',
              width: '100%',
              borderRadius: 3,
              overflow: 'hidden',
              minHeight: { xs: 220, md: 330 },
              bgcolor: (theme) => theme.palette.action.hover,
            }}
          >
            {selectedOrganizationImage && (
              <Box
                component="img"
                src={selectedOrganizationImage.url}
                alt={rootData.organizationPublic.name}
                sx={{
                  display: 'block',
                  width: '100%',
                  height: '100%',
                  minHeight: { xs: 220, md: 330 },
                  objectFit: 'cover',
                }}
              />
            )}
            {organizationImages.length > 1 && (
              <Box
                sx={{
                  position: 'absolute',
                  left: 16,
                  right: 16,
                  bottom: 16,
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  gap: 1,
                }}
              >
                <Box sx={{ display: 'flex', gap: 0.75, maxWidth: 'calc(100% - 96px)', overflowX: 'auto', p: 0.25 }}>
                  {organizationImages.map((image, index) => {
                    const isSelected = index === safeSelectedOrganizationImageIndex;

                    return (
                      <ButtonBase
                        key={`${image.url}-${index}`}
                        onClick={() => setSelectedOrganizationImageIndex(index)}
                        aria-label={index === 0 ? 'Show cover image' : `Show image ${index + 1}`}
                        sx={{
                          width: 56,
                          height: 40,
                          flex: '0 0 auto',
                          overflow: 'hidden',
                          border: 2,
                          borderColor: isSelected ? 'common.white' : 'rgba(255, 255, 255, 0.58)',
                          borderRadius: 1,
                          boxShadow: '0 1px 4px rgba(0, 0, 0, 0.4)',
                        }}
                      >
                        <Box component="img" src={image.url} alt={index === 0 ? 'Cover image' : ''} sx={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                      </ButtonBase>
                    );
                  })}
                </Box>

                <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, borderRadius: 99, bgcolor: 'rgba(17, 24, 39, 0.68)', p: 0.25 }}>
                  <IconButton
                    size="small"
                    onClick={() => setSelectedOrganizationImageIndex((index) => (index - 1 + organizationImages.length) % organizationImages.length)}
                    aria-label="Previous image"
                    sx={{ color: 'common.white' }}
                  >
                    <ArrowLeftIcon fontSize="small" />
                  </IconButton>
                  <IconButton
                    size="small"
                    onClick={() => setSelectedOrganizationImageIndex((index) => (index + 1) % organizationImages.length)}
                    aria-label="Next image"
                    sx={{ color: 'common.white' }}
                  >
                    <ArrowRightIcon fontSize="small" />
                  </IconButton>
                </Box>
              </Box>
            )}
          </Box>

          <Box sx={{ minWidth: 0, py: { md: 2 } }}>
            <BodyIconTypography
              label="BROWSE SPACES AND PLANS"
              sx={{ color: 'primary.dark', fontSize: { xs: '0.9rem', md: '1rem' }, fontWeight: 800, mb: 1, letterSpacing: '0.02em' }}
            />
            <MediumHeadingIconTypography
              label={rootData.organizationPublic.marketplaceListingMetadata.title || rootData.organizationPublic.name}
              sx={{ mb: 1.25, fontSize: { xs: '2rem', md: '2.75rem' }, lineHeight: 1.08 }}
            />
            <BodyIconTypography
              label={
                rootData.organizationPublic.marketplaceListingMetadata.subTitle ||
                rootData.organizationPublic.marketplaceListingMetadata.about ||
                'Explore the workspace, compare products, and choose the option that suits your day.'
              }
              sx={{ opacity: 0.8, maxWidth: 560, mb: 2.5, fontSize: { md: '1.05rem' } }}
            />
            {(rootData.organizationPublic.marketplaceListingMetadata.includedFeatures?.length ?? 0) > 0 && (
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                {(rootData.organizationPublic.marketplaceListingMetadata.includedFeatures ?? []).slice(0, 4).map((feature) => (
                  <Chip key={feature} label={feature} size="small" variant="outlined" sx={{ bgcolor: (theme) => theme.palette.background.paper }} />
                ))}
              </Box>
            )}
          </Box>
        </Box>
      </Container>

      <Container maxWidth="xl" sx={{ mb: 6 }}>
        <Box sx={{ mb: 3 }}>
          <GuestStoreFrontLocationsStrip rootDataRelay={rootData} onLocationChange={setSelectedLocationId} />
        </Box>

        <Box sx={{ mb: 3 }}>
          <BodyIconTypography
            label={selectedLocationId ? 'PRODUCTS AT THIS LOCATION' : 'EXPLORE PRODUCTS'}
            sx={{ color: 'primary.dark', fontSize: { xs: '0.9rem', md: '1rem' }, fontWeight: 800, mb: 0.75, letterSpacing: '0.02em' }}
          />
          <MediumHeadingIconTypography label="Find the space that works for you" sx={{ mb: 0.75 }} />
          <BodyIconTypography label="Compare options, choose a plan, and continue when you are ready to purchase." sx={{ opacity: 0.8 }} />
        </Box>

        {selectedLocationId && selectedLocationProductsQueryReference ? (
          <SelectedLocationProducts queryReference={selectedLocationProductsQueryReference} rootData={rootData} organizationCustomDomain={organizationCustomDomain} />
        ) : (
          <ProductList products={defaultProducts} rootData={rootData} organizationCustomDomain={organizationCustomDomain} />
        )}
      </Container>

      <GuestStoreFrontFooter rootDataRelay={rootData} />
    </Box>
  );
};

const MemoGuestStoreFront = memo(GuestStoreFront);

const GuestStoreFrontWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<guestStoreFront_rootQuery>(RootQuery);
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoGuestStoreFront queryReference={queryReference} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(GuestStoreFrontWithRelay);
