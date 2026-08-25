import { ArrowLeftIcon, ArrowRightIcon, SelectedTickIcon } from '@/components/icons';
import type { marketplaceProductDetailOverview_product$key } from '@/queries/__generated__/marketplaceProductDetailOverview_product.graphql';
import type { marketplaceProductDetailOverview_query$key } from '@/queries/__generated__/marketplaceProductDetailOverview_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Box from '@mui/system/Box';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackRow, SubtitleIconTypography } from '@skedular/ui';
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
      ? 'This booking reserves all matching resources for the selected time, including across multiple locations when the booking groups match.'
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
            <Box sx={{ position: 'relative', width: '100%', height: { xs: 300, md: 460 }, borderRadius: 3, overflow: 'hidden', bgcolor: (theme) => theme.palette.action.hover }}>
              <Box
                component="img"
                src={effectiveSelectedImageUrl}
                alt={product.listingMetadata.title ?? ''}
                sx={{ display: 'block', width: '100%', height: '100%', objectFit: 'cover' }}
              />

              {imageUrls.length > 1 && (
                <Box sx={{ position: 'absolute', left: 16, right: 16, bottom: 16, display: 'flex', justifyContent: 'space-between', alignItems: 'center', gap: 1 }}>
                  <Box sx={{ display: 'flex', gap: 0.75, maxWidth: 'calc(100% - 96px)', overflowX: 'auto', p: 0.25 }}>
                    {productImages.map((image, index) => {
                      const isSelected = effectiveSelectedImageUrl === image.originalUrl;

                      return (
                        <Box
                          key={`${image.originalUrl}-${index}`}
                          component="button"
                          type="button"
                          onClick={() => setSelectedImageUrl(image.originalUrl)}
                          aria-label={index === 0 ? 'Show cover image' : `Show image ${index + 1}`}
                          sx={{
                            width: 56,
                            height: 40,
                            flex: '0 0 auto',
                            border: 2,
                            p: 0,
                            lineHeight: 0,
                            borderRadius: 1,
                            overflow: 'hidden',
                            cursor: 'pointer',
                            borderColor: isSelected ? 'common.white' : 'rgba(255, 255, 255, 0.58)',
                            boxShadow: '0 1px 4px rgba(0, 0, 0, 0.4)',
                          }}
                        >
                          <Box
                            component="img"
                            src={image.thumbnailUrl}
                            alt={index === 0 ? 'Cover image' : ''}
                            sx={{ width: '100%', height: '100%', objectFit: 'cover', display: 'block' }}
                          />
                        </Box>
                      );
                    })}
                  </Box>

                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, borderRadius: 99, bgcolor: 'rgba(17, 24, 39, 0.68)', p: 0.25 }}>
                    <IconButton
                      size="small"
                      onClick={() => {
                        const selectedIndex = imageUrls.indexOf(effectiveSelectedImageUrl);
                        setSelectedImageUrl(imageUrls[(selectedIndex - 1 + imageUrls.length) % imageUrls.length] ?? imageUrls[0] ?? '');
                      }}
                      aria-label="Previous image"
                      sx={{ color: 'common.white' }}
                    >
                      <ArrowLeftIcon fontSize="small" />
                    </IconButton>
                    <IconButton
                      size="small"
                      onClick={() => {
                        const selectedIndex = imageUrls.indexOf(effectiveSelectedImageUrl);
                        setSelectedImageUrl(imageUrls[(selectedIndex + 1) % imageUrls.length] ?? imageUrls[0] ?? '');
                      }}
                      aria-label="Next image"
                      sx={{ color: 'common.white' }}
                    >
                      <ArrowRightIcon fontSize="small" />
                    </IconButton>
                  </Box>
                </Box>
              )}
            </Box>
          ) : (
            <Box sx={{ width: '100%', height: { xs: 260, md: 460 } }} />
          )}
        </Box>
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
