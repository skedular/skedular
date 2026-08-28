import { getMarketplaceProductLink } from '@/components/links';
import { RelayError, startOfDay, toOpeningHoursFromTime, toRootError, useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography } from '@skedular/ui';
import { Loading } from '@/components/loading';

import type { marketplaceProductBooking_rootQuery } from '@/queries/__generated__/marketplaceProductBooking_rootQuery.graphql';
import type { marketplaceProductBookingBreadcrumb_query$key } from '@/queries/__generated__/marketplaceProductBookingBreadcrumb_query.graphql';
import Breadcrumbs from '@mui/material/Breadcrumbs';
import Container from '@mui/material/Container';
import Link from '@mui/material/Link';
import Box from '@mui/material/Box';
import type { DateRange } from '@mui/x-date-pickers-pro/models';
import { Dayjs } from 'dayjs';
import { useRouter } from 'next/navigation';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import MarketplaceProductBookingForm from './marketplace-product-booking-form';
import MarketplaceProductBookingHero from './marketplace-product-booking-hero';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<marketplaceProductBooking_rootQuery, Record<string, unknown>>;
  selectedDate: Dayjs;
  setSelectedDate: (value: Dayjs) => void;
  setTimeRange: (value: DateRange<Dayjs>) => void;
  timeRange: DateRange<Dayjs>;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query marketplaceProductBooking_rootQuery($productId: String!, $organizationCustomDomain: String!) {
    ...marketplaceProductBookingBreadcrumb_query @arguments(productId: $productId)
    organization(customDomain: $organizationCustomDomain) {
      spacesPublicBookingAvailability {
        available
        message
      }
    }
    product(id: $productId) {
      ...marketplaceProductBookingHero_product
    }
    ...marketplaceProductBookingForm_query @arguments(productId: $productId)
  }
`;

const MarketplaceProductBooking = ({ organizationCustomDomain, queryReference, selectedDate, setSelectedDate, setTimeRange, timeRange }: Props) => {
  const rootData = usePreloadedQuery<marketplaceProductBooking_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const { integratedPlatform } = useIntegratedPlatform();
  const { isCustomDomain } = useKnownParams();
  const breadcrumbData = useFragment<marketplaceProductBookingBreadcrumb_query$key>(
    graphql`
      fragment marketplaceProductBookingBreadcrumb_query on Query @argumentDefinitions(productId: { type: "String!" }) {
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

        {rootData.product ? <MarketplaceProductBookingHero productRelay={rootData.product} /> : null}
        <Box sx={{ mt: 1 }}>
          <MarketplaceProductBookingForm
            bookingAvailable={rootData.organization?.spacesPublicBookingAvailability.available ?? false}
            bookingAvailabilityMessage={rootData.organization?.spacesPublicBookingAvailability.message ?? 'Bookings are currently unavailable for this workspace.'}
            onDateChange={setSelectedDate}
            onTimeRangeChange={setTimeRange}
            rootDataRelay={rootData}
            selectedDate={selectedDate}
            timeRange={timeRange}
          />
        </Box>
      </Container>
    </Box>
  );
};

const MemoMarketplaceProductBooking = memo(MarketplaceProductBooking);

type MarketplaceProductBookingWithRelayProps = {
  organizationCustomDomain?: string;
  productId?: string;
};

const MarketplaceProductBookingWithRelay = ({
  organizationCustomDomain: organizationCustomDomainOverride,
  productId: productIdOverride,
}: MarketplaceProductBookingWithRelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<marketplaceProductBooking_rootQuery>(RootQuery);
  const { productId: routeProductId, organizationCustomDomain: routeOrganizationCustomDomain } = useKnownParams();
  const productId = productIdOverride ?? routeProductId;
  const organizationCustomDomain = organizationCustomDomainOverride ?? routeOrganizationCustomDomain;
  const [selectedDate, setSelectedDate] = useState<Dayjs>(startOfDay());
  const [timeRange, setTimeRange] = useState<DateRange<Dayjs>>([toOpeningHoursFromTime('09:00'), toOpeningHoursFromTime('10:00')]);

  if (!productId || !organizationCustomDomain) {
    throw new Error('productId and organizationCustomDomain are required');
  }

  const reloadQuery = useCallback(() => {
    loadQuery(
      {
        productId,
        organizationCustomDomain,
      },
      {
        fetchPolicy: 'network-only',
      },
    );
  }, [loadQuery, organizationCustomDomain, productId]);

  useEffect(() => {
    reloadQuery();
  }, [reloadQuery]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoMarketplaceProductBooking
        organizationCustomDomain={organizationCustomDomain}
        queryReference={queryReference}
        selectedDate={selectedDate}
        setSelectedDate={setSelectedDate}
        setTimeRange={setTimeRange}
        timeRange={timeRange}
      />
    </ErrorBoundary>
  );
};

export default memo(MarketplaceProductBookingWithRelay);
