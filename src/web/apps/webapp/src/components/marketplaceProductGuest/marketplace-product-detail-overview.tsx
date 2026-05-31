import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackRow, SubtitleIconTypography } from '@skedular/ui';
import { SelectedTickIcon } from '@/components/icons';
import Chip from '@mui/material/Chip';
import type { marketplaceProductDetailOverview_product$key } from '@/queries/__generated__/marketplaceProductDetailOverview_product.graphql';
import type { marketplaceProductDetailOverview_query$key } from '@/queries/__generated__/marketplaceProductDetailOverview_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Divider from '@mui/material/Divider';
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
        type {
          type
          name
        }
        listingMetadata {
          title
          subTitle
          includedFeatures
        }
        featureImages {
          original {
            url
          }
          thumbnail {
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
  const productImages = useMemo(
    () =>
      product
        ? product.featureImages
            .map((image) => ({
              originalUrl: image.original?.url ?? '',
              thumbnailUrl: image.thumbnail?.url ?? image.original?.url ?? '',
            }))
            .filter((image) => image.originalUrl)
        : [],
    [product],
  );
  const imageUrls = useMemo(() => productImages.map((image) => image.originalUrl), [productImages]);
  const includedFeatures = useMemo(() => product?.listingMetadata.includedFeatures?.filter(Boolean) ?? [], [product?.listingMetadata.includedFeatures]);
  const [selectedImageUrl, setSelectedImageUrl] = useState(imageUrls[0] ?? '');
  const effectiveSelectedImageUrl = useMemo(
    () => (imageUrls.some((imageUrl) => imageUrl === selectedImageUrl) ? selectedImageUrl : (imageUrls[0] ?? '')),
    [imageUrls, selectedImageUrl],
  );

  if (!product) {
    return null;
  }

  const productTypeDescription =
    product.type.type === 'EVENT'
      ? 'This booking reserves all matching resources for the selected time, including across multiple locations when the product tags match.'
      : 'This booking reserves the matching resources required for the selected time.';

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3, minWidth: 0, maxWidth: '100%' }}>
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
            display: 'flex',
            alignItems: 'flex-start',
            justifyContent: 'flex-start',
            width: '100%',
            boxSizing: 'border-box',
            overflow: 'hidden',
          }}
        >
          {effectiveSelectedImageUrl ? (
            <Box
              component="img"
              src={effectiveSelectedImageUrl}
              alt={product.listingMetadata.title ?? ''}
              sx={{
                display: 'block',
                width: { xs: '100%', md: 'auto' },
                boxSizing: 'border-box',
                height: 'auto',
                maxWidth: '100%',
                maxHeight: { md: 460 },
                borderRadius: 3,
                objectFit: 'contain',
              }}
            />
          ) : (
            <Box sx={{ width: '100%', height: { xs: 260, md: 460 } }} />
          )}
        </Box>

        {imageUrls.length > 0 && (
          <Box sx={{ display: 'flex', gap: 1, width: '100%', maxWidth: '100%', overflowX: 'auto', pb: 0.5, scrollbarWidth: 'thin' }}>
            {productImages.map((image, index) => (
              <Box
                key={`${image.originalUrl}-${index}`}
                component="button"
                type="button"
                onClick={() => setSelectedImageUrl(image.originalUrl)}
                sx={{
                  width: { xs: 72, md: 96 },
                  height: { xs: 54, md: 72 },
                  flex: '0 0 auto',
                  border: 2,
                  p: 0,
                  lineHeight: 0,
                  borderRadius: 1.5,
                  overflow: 'hidden',
                  cursor: 'pointer',
                  outline: 'none',
                  borderColor: (theme) => (effectiveSelectedImageUrl === image.originalUrl ? theme.palette.primary.main : theme.palette.divider),
                  bgcolor: (theme) => theme.palette.background.default,
                  opacity: effectiveSelectedImageUrl === image.originalUrl ? 1 : 0.78,
                }}
              >
                <Box
                  component="img"
                  src={image.thumbnailUrl}
                  alt={product.listingMetadata.title ?? ''}
                  sx={{ width: '100%', height: '100%', objectFit: 'contain', display: 'block' }}
                />
              </Box>
            ))}
          </Box>
        )}
      </Box>

      <Card variant="outlined" sx={{ borderRadius: 3 }}>
        <CardContent sx={{ p: { xs: 2.5, md: 3.5 }, '&:last-child': { pb: { xs: 2.5, md: 3.5 } } }}>
          <LeadIconTypography label="About this product" sx={{ mb: 1.5 }} />
          <Box sx={{ mb: 1.5 }}>
            <Chip label={product.type.name} color={product.type.type === 'EVENT' ? 'warning' : 'primary'} variant="outlined" />
          </Box>
          <BodyIconTypography label={product.listingMetadata.subTitle ?? ''} sx={{ opacity: 0.85 }} />
          <BodyIconTypography label={productTypeDescription} sx={{ mt: 1.25, opacity: 0.85 }} />

          {includedFeatures.length > 0 && (
            <>
              <Divider sx={{ my: 3 }} />
              <SubtitleIconTypography label="What's included" sx={{ mb: 2 }} />
              <Box
                sx={{
                  display: 'grid',
                  gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))' },
                  gap: 1.5,
                }}
              >
                {includedFeatures.map((feature) => (
                  <Box key={feature} sx={{ display: 'flex', alignItems: 'flex-start', gap: 1.25 }}>
                    <SelectedTickIcon sx={{ color: 'success.main', fontSize: 20, mt: '2px' }} />
                    <BodyIconTypography label={feature} sx={{ opacity: 0.9 }} />
                  </Box>
                ))}
              </Box>
            </>
          )}
        </CardContent>
      </Card>

      {product.amenities.length > 0 && (
        <Card variant="outlined" sx={{ borderRadius: 3 }}>
          <CardContent sx={{ p: { xs: 2.5, md: 3.5 }, '&:last-child': { pb: { xs: 2.5, md: 3.5 } } }}>
            <SubtitleIconTypography label="Amenities" sx={{ mb: 1.5 }} />
            <StackRow>
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
