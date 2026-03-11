import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, SubtitleIconTypography } from '@/components/commons';
import type { GuestStoreFrontProduct } from './types';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Chip from '@mui/material/Chip';
import Stack from '@mui/material/Stack';
import Box from '@mui/system/Box';
import { memo } from 'react';

type Props = {
  product: GuestStoreFrontProduct;
};

const GuestStoreFrontProductCard = ({ product }: Props) => {
  const lowestPricing = product.pricingOptions.reduce((lowest, next) => (next.price < lowest.price ? next : lowest), product.pricingOptions[0]);

  return (
    <Card
      sx={{
        border: 1,
        borderColor: (theme) => theme.palette.divider,
        backgroundColor: (theme) => theme.palette.background.paper,
        borderRadius: 3,
        height: '100%',
      }}
    >
      <CardMedia component="img" image={product.imageUrl} alt={product.name} sx={{ height: 190 }} />
      <CardContent sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
        <Stack direction="row" justifyContent="space-between" alignItems="center">
          <LeadIconTypography label={product.name} />
          <Chip size="small" label={`${product.availableCount} available`} color="primary" />
        </Stack>

        <BodyIconTypography label={product.type} sx={{ opacity: 0.85 }} />
        <BodyIconTypography label={product.description} />

        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
          {product.amenities.slice(0, 4).map((amenity) => (
            <Chip key={amenity} size="small" variant="outlined" label={amenity} />
          ))}
        </Box>

        <Box sx={{ mt: 1, borderTop: 1, borderColor: (theme) => theme.palette.divider, pt: 1.5 }}>
          <CaptionIconTypography label="Starting from" sx={{ opacity: 0.75 }} />
          <SubtitleIconTypography label={`$${lowestPricing.price} ${lowestPricing.periodLabel}`} fontWeight={600} />
        </Box>
      </CardContent>
    </Card>
  );
};

export default memo(GuestStoreFrontProductCard);
