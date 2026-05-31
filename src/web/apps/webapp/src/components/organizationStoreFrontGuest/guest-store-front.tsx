import { RelayError, convertCalendarDayToStartOfDay, endOfWeek, toRootError } from '@skedular/shared';
import { Loading } from '@/components/loading';

import type { guestStoreFront_rootQuery } from '@/queries/__generated__/guestStoreFront_rootQuery.graphql';
import type { guestStoreFrontProducts_query$key } from '@/queries/__generated__/guestStoreFrontProducts_query.graphql';
import type { guestStoreFrontSelectedLocationProductsQuery } from '@/queries/__generated__/guestStoreFrontSelectedLocationProductsQuery.graphql';
import ButtonBase from '@mui/material/ButtonBase';
import Container from '@mui/material/Container';
import Box from '@mui/system/Box';

import { BodyIconTypography, MediumHeadingIconTypography } from '@skedular/ui';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import dayjs from 'dayjs';
import { memo, useEffect, useMemo, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import GuestStoreFrontActiveSubscriptionsStrip from './guest-store-front-active-subscriptions-strip';
import GuestStoreFrontFooter from './guest-store-front-footer';
import GuestStoreFrontLocationsStrip from './guest-store-front-locations-strip';
import GuestStoreFrontProductCard from './guest-store-front-product-card';
import GuestStoreFrontUpcomingBookingsStrip from './guest-store-front-upcoming-bookings-strip';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<guestStoreFront_rootQuery, Record<string, unknown>>;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query guestStoreFront_rootQuery(
    $organizationCustomDomain: String!
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
    $includeUpcomingBookings: Boolean!
    $includeActiveSubscriptions: Boolean!
  ) {
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
    ...guestStoreFrontUpcomingBookingsStrip_query
      @arguments(
        bookingsSearchCriteriaFrom: $bookingsSearchCriteriaFrom
        bookingsSearchCriteriaTo: $bookingsSearchCriteriaTo
        includeUpcomingBookings: $includeUpcomingBookings
        organizationCustomDomain: $organizationCustomDomain
      )
    ...guestStoreFrontActiveSubscriptionsStrip_query @arguments(includeActiveSubscriptions: $includeActiveSubscriptions, organizationCustomDomain: $organizationCustomDomain)
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
      <Container maxWidth="xl" sx={{ mt: { xs: 3, md: 4 } }}>
        <Box
          sx={{
            display: 'grid',
            gap: 2,
            gridTemplateColumns: {
              xs: '1fr',
              lg: 'minmax(0, 1fr) minmax(0, 1fr)',
            },
            alignItems: 'start',
          }}
        >
          <GuestStoreFrontUpcomingBookingsStrip rootDataRelay={rootData} />
          <GuestStoreFrontActiveSubscriptionsStrip rootDataRelay={rootData} />
        </Box>
      </Container>

      <Container maxWidth="xl" sx={{ mt: { xs: 2, md: 3 } }}>
        <GuestStoreFrontLocationsStrip rootDataRelay={rootData} onLocationChange={setSelectedLocationId} />
      </Container>

      <Container maxWidth="xl" sx={{ mt: { xs: 3, md: 5 }, mb: 7 }}>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'column',
            gap: 0.75,
            minWidth: 0,
            maxWidth: '100%',
          }}
        >
          <Box
            sx={{
              position: 'relative',
              width: '100%',
              boxSizing: 'border-box',
              display: 'flex',
              alignItems: 'flex-start',
              justifyContent: 'flex-start',
              borderRadius: 3,
              overflow: 'hidden',
            }}
          >
            {selectedOrganizationImage && (
              <Box
                component="img"
                src={selectedOrganizationImage.url}
                alt={rootData.organizationPublic.name}
                sx={{
                  display: 'block',
                  width: { xs: '100%', md: 'auto' },
                  boxSizing: 'border-box',
                  height: 'auto',
                  maxWidth: '100%',
                  maxHeight: { md: 520 },
                  objectFit: 'contain',
                  borderRadius: 3,
                }}
              />
            )}
          </Box>

          {organizationImages.length > 1 && (
            <Box
              sx={{
                display: 'flex',
                gap: 1,
                width: '100%',
                maxWidth: '100%',
                overflowX: 'auto',
                pb: 0.5,
                scrollbarWidth: 'thin',
              }}
            >
              {organizationImages.map((image, index) => {
                const isSelected = index === safeSelectedOrganizationImageIndex;

                return (
                  <ButtonBase
                    key={`${image.url}-${index}`}
                    onClick={() => setSelectedOrganizationImageIndex(index)}
                    aria-label={`Show ${rootData.organizationPublic?.name ?? 'organisation'} image ${index + 1}`}
                    sx={{
                      width: { xs: 72, md: 96 },
                      height: { xs: 54, md: 72 },
                      flex: '0 0 auto',
                      borderRadius: 1.5,
                      overflow: 'hidden',
                      border: 2,
                      borderColor: (theme) => (isSelected ? theme.palette.primary.main : theme.palette.divider),
                      bgcolor: (theme) => theme.palette.background.default,
                      opacity: isSelected ? 1 : 0.78,
                    }}
                  >
                    <Box
                      component="img"
                      src={image.url}
                      alt=""
                      sx={{
                        display: 'block',
                        width: '100%',
                        height: '100%',
                        objectFit: 'contain',
                      }}
                    />
                  </ButtonBase>
                );
              })}
            </Box>
          )}
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
  const { user, loading } = useAuth();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  useEffect(() => {
    if (loading) {
      return;
    }

    const today = convertCalendarDayToStartOfDay(dayjs());

    loadQuery(
      {
        organizationCustomDomain,
        bookingsSearchCriteriaFrom: today.toISOString(),
        bookingsSearchCriteriaTo: endOfWeek(today).add(-1, 'milliseconds').toISOString(),
        includeUpcomingBookings: !!user,
        includeActiveSubscriptions: !!user,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, loading, organizationCustomDomain, user]);

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
