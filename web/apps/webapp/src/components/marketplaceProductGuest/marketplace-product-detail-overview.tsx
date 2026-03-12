import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackRow, SubtitleIconTypography } from '@/components/commons';
import { TickIcon } from '@/components/icons';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Box from '@mui/system/Box';
import { memo } from 'react';
import type { MarketplaceProductDetail } from './types';

type Props = {
  product: MarketplaceProductDetail;
  selectedImageUrl: string;
  onImageSelect: (imageUrl: string) => void;
};

const MarketplaceProductDetailOverview = ({ product, selectedImageUrl, onImageSelect }: Props) => (
  <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
    <Box
      sx={{
        borderRadius: 3,
        overflow: 'hidden',
        border: 1,
        borderColor: (theme) => theme.palette.divider,
        bgcolor: (theme) => theme.palette.background.paper,
      }}
    >
      <Box component="img" src={selectedImageUrl} alt={product.title} sx={{ width: '100%', height: { xs: 260, md: 460 }, objectFit: 'cover', display: 'block' }} />
    </Box>

    <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(3, minmax(0, 1fr))', gap: 1.5 }}>
      {product.imageUrls.slice(0, 3).map((imageUrl) => (
        <Box
          key={imageUrl}
          component="button"
          type="button"
          onClick={() => onImageSelect(imageUrl)}
          sx={{
            border: 0,
            p: 0,
            lineHeight: 0,
            borderRadius: 2,
            overflow: 'hidden',
            cursor: 'pointer',
            outline: 'none',
            boxShadow: selectedImageUrl === imageUrl ? (theme) => `0 0 0 2px ${theme.palette.primary.main}` : 'none',
          }}
        >
          <Box component="img" src={imageUrl} alt={product.title} sx={{ width: '100%', height: { xs: 90, md: 120 }, objectFit: 'cover', display: 'block' }} />
        </Box>
      ))}
    </Box>

    <Card variant="outlined" sx={{ borderRadius: 3 }}>
      <CardContent sx={{ p: { xs: 2.5, md: 3.5 }, '&:last-child': { pb: { xs: 2.5, md: 3.5 } } }}>
        <LeadIconTypography label="About this workspace" sx={{ mb: 1.5 }} />
        <BodyIconTypography label={product.longDescription} sx={{ opacity: 0.85, mb: 3 }} />
        <LeadIconTypography label="What is included" sx={{ mb: 1.5 }} />
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 1.25 }}>
          {product.features.map((feature) => (
            <CaptionIconTypography
              key={feature}
              label={feature}
              startElement={<TickIcon sx={{ color: (theme) => theme.palette.success.main, fontSize: 16 }} />}
              sx={{ opacity: 0.9 }}
            />
          ))}
        </Box>
      </CardContent>
    </Card>

    <Card variant="outlined" sx={{ borderRadius: 3 }}>
      <CardContent sx={{ p: { xs: 2.5, md: 3.5 }, '&:last-child': { pb: { xs: 2.5, md: 3.5 } } }}>
        <SubtitleIconTypography label="Amenities" sx={{ mb: 1.5 }} />
        <StackRow spacing={1}>
          {product.amenities.map((amenity) => (
            <CaptionIconTypography key={amenity} label={amenity} sx={{ px: 1.1, py: 0.65, borderRadius: 1.25, bgcolor: (theme) => theme.palette.action.hover }} />
          ))}
        </StackRow>
      </CardContent>
    </Card>
  </Box>
);

export default memo(MarketplaceProductDetailOverview);
