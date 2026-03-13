import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackRow, SubtitleIconTypography } from '@/components/commons';
import type { marketplaceProductDetailOverview_product$key } from '@/queries/__generated__/marketplaceProductDetailOverview_product.graphql';
import type { marketplaceProductDetailOverview_query$key } from '@/queries/__generated__/marketplaceProductDetailOverview_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Box from '@mui/system/Box';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: marketplaceProductDetailOverview_query$key;
};

const MarketplaceProductDetailOverview = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<marketplaceProductDetailOverview_query$key>(
    graphql`
      fragment marketplaceProductDetailOverview_query on Query @argumentDefinitions(productId: { type: "String!" }) {
        product(id: $productId) {
          ...marketplaceProductDetailOverview_product
        }
      }
    `,
    rootDataRelay,
  );

  const product = useFragment<marketplaceProductDetailOverview_product$key>(
    graphql`
      fragment marketplaceProductDetailOverview_product on ProductDetails {
        name
        listingMetadata {
          about
          includedFeatures
        }
        featureImages {
          original {
            url
          }
        }
        amenities {
          id
          name
          color
        }
      }
    `,
    rootData.product,
  );
  const imageUrls = useMemo(() => (product ? product.featureImages.map((item) => item.original?.url).filter((item): item is string => !!item) : []), [product]);
  const [selectedImageUrl, setSelectedImageUrl] = useState(imageUrls[0] ?? '');
  const effectiveSelectedImageUrl = useMemo(
    () => (imageUrls.some((imageUrl) => imageUrl === selectedImageUrl) ? selectedImageUrl : (imageUrls[0] ?? '')),
    [imageUrls, selectedImageUrl],
  );

  if (!product) {
    return null;
  }

  return (
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
        {effectiveSelectedImageUrl ? (
          <Box component="img" src={effectiveSelectedImageUrl} alt={product.name} sx={{ width: '100%', height: { xs: 260, md: 460 }, objectFit: 'cover', display: 'block' }} />
        ) : (
          <Box sx={{ width: '100%', height: { xs: 260, md: 460 } }} />
        )}
      </Box>

      {imageUrls.length > 0 && (
        <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(3, minmax(0, 1fr))', gap: 1.5 }}>
          {imageUrls.slice(0, 3).map((imageUrl) => (
            <Box
              key={imageUrl}
              component="button"
              type="button"
              onClick={() => setSelectedImageUrl(imageUrl)}
              sx={{
                border: 0,
                p: 0,
                lineHeight: 0,
                borderRadius: 2,
                overflow: 'hidden',
                cursor: 'pointer',
                outline: 'none',
                boxShadow: effectiveSelectedImageUrl === imageUrl ? (theme) => `0 0 0 2px ${theme.palette.primary.main}` : 'none',
              }}
            >
              <Box component="img" src={imageUrl} alt={product.name} sx={{ width: '100%', height: { xs: 90, md: 120 }, objectFit: 'cover', display: 'block' }} />
            </Box>
          ))}
        </Box>
      )}

      <Card variant="outlined" sx={{ borderRadius: 3 }}>
        <CardContent sx={{ p: { xs: 2.5, md: 3.5 }, '&:last-child': { pb: { xs: 2.5, md: 3.5 } } }}>
          <LeadIconTypography label="About this product" sx={{ mb: 1.5 }} />
          <BodyIconTypography label={product.listingMetadata.about ?? ''} sx={{ opacity: 0.85 }} />
        </CardContent>
      </Card>

      {product.amenities.length > 0 && (
        <Card variant="outlined" sx={{ borderRadius: 3 }}>
          <CardContent sx={{ p: { xs: 2.5, md: 3.5 }, '&:last-child': { pb: { xs: 2.5, md: 3.5 } } }}>
            <SubtitleIconTypography label="Amenities" sx={{ mb: 1.5 }} />
            <StackRow spacing={1}>
              {product.amenities.map((amenity) => (
                <CaptionIconTypography key={amenity.id} label={amenity.name} sx={{ px: 1.1, py: 0.65, borderRadius: 1.25, bgcolor: (theme) => theme.palette.action.hover }} />
              ))}
            </StackRow>
          </CardContent>
        </Card>
      )}
    </Box>
  );
};

export default memo(MarketplaceProductDetailOverview);
