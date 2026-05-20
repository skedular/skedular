import { BodyIconTypography, StackRow } from '@skedular/ui';
import { ArrowLeftIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { startOfDay, toOpeningHoursFromTime } from '@skedular/shared';
import type { marketplaceProductBooking_rootQuery } from '@/queries/__generated__/marketplaceProductBooking_rootQuery.graphql';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Box from '@mui/material/Box';
import type { DateRange } from '@mui/x-date-pickers-pro/models';
import { Dayjs } from 'dayjs';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import MarketplaceProductBookingForm from './marketplace-product-booking-form';
import MarketplaceProductBookingHero from './marketplace-product-booking-hero';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<marketplaceProductBooking_rootQuery, Record<string, unknown>>;
  selectedDate: Dayjs;
  setSelectedDate: (value: Dayjs) => void;
  setTimeRange: (value: DateRange<Dayjs>) => void;
  timeRange: DateRange<Dayjs>;
};

const RootQuery = graphql`
  query marketplaceProductBooking_rootQuery($productId: String!) {
    product(id: $productId) {
      ...marketplaceProductBookingHero_product
    }
    ...marketplaceProductBookingForm_query @arguments(productId: $productId)
  }
`;

const MarketplaceProductBooking = ({ queryReference, selectedDate, setSelectedDate, setTimeRange, timeRange }: Props) => {
  const rootData = usePreloadedQuery<marketplaceProductBooking_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

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
        <Button variant="text" onClick={() => router.back()} sx={{ textTransform: 'none', px: 0, mb: 2 }}>
          <StackRow spacing={0.5} sx={{ flexWrap: 'nowrap' }}>
            <ArrowLeftIcon fontSize="small" />
            <BodyIconTypography label="Back" />
          </StackRow>
        </Button>

        {rootData.product ? <MarketplaceProductBookingHero productRelay={rootData.product} /> : null}
        <Box sx={{ mt: 1 }}>
          <MarketplaceProductBookingForm
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

const MarketplaceProductBookingWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<marketplaceProductBooking_rootQuery>(RootQuery);
  const { productId } = useKnownParams();
  const [selectedDate, setSelectedDate] = useState<Dayjs>(startOfDay());
  const [timeRange, setTimeRange] = useState<DateRange<Dayjs>>([toOpeningHoursFromTime('08:00'), toOpeningHoursFromTime('17:00')]);

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
      <MemoMarketplaceProductBooking
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
