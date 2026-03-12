import { BodyIconTypography, StackRow } from '@/components/commons';
import { ArrowLeftIcon } from '@/components/icons';
import { useKnownParams } from '@/libs/providers';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useMemo, useState } from 'react';
import MarketplaceProductDetailBookingCard from './marketplace-product-detail-booking-card';
import MarketplaceProductDetailOverview from './marketplace-product-detail-overview';
import { marketplaceProductDetailMock } from './mock-data';

const MarketplaceProductDetail = () => {
  const router = useRouter();
  const { productId } = useKnownParams();

  const product = useMemo(
    () =>
      productId && productId !== ''
        ? {
            ...marketplaceProductDetailMock,
            id: productId,
          }
        : marketplaceProductDetailMock,
    [productId],
  );

  const [selectedImageUrl, setSelectedImageUrl] = useState(product.imageUrls[0] ?? '');

  return (
    <Box sx={{ bgcolor: (theme) => theme.palette.background.default, minHeight: '100vh', pb: 8 }}>
      <Container maxWidth="xl" sx={{ pt: { xs: 3, md: 4 } }}>
        <Button variant="text" onClick={() => router.back()} sx={{ textTransform: 'none', px: 0, mb: 2 }}>
          <StackRow spacing={0.5} sx={{ flexWrap: 'nowrap' }}>
            <ArrowLeftIcon fontSize="small" />
            <BodyIconTypography label="Back to products" />
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
          <MarketplaceProductDetailOverview product={product} selectedImageUrl={selectedImageUrl} onImageSelect={setSelectedImageUrl} />
          <MarketplaceProductDetailBookingCard product={product} />
        </Box>
      </Container>
    </Box>
  );
};

export default memo(MarketplaceProductDetail);
