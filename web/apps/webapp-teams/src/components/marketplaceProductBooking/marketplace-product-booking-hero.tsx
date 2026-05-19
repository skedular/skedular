import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackRow, SubtitleIconTypography } from '@skedular/ui';
import type { marketplaceProductBookingHero_product$key } from '@/queries/__generated__/marketplaceProductBookingHero_product.graphql';
import Box from '@mui/material/Box';
import Chip from '@mui/material/Chip';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  productRelay: marketplaceProductBookingHero_product$key;
};

const MarketplaceProductBookingHero = ({ productRelay }: Props) => {
  const product = useFragment(
    graphql`
      fragment marketplaceProductBookingHero_product on ProductDetails {
        listingMetadata {
          title
          subTitle
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
        }
      }
    `,
    productRelay,
  );
  const includedFeatures = useMemo(() => product.listingMetadata.includedFeatures?.filter(Boolean) ?? [], [product.listingMetadata.includedFeatures]);

  return (
    <Box
      sx={{
        position: 'relative',
        overflow: 'hidden',
        borderRadius: 4,
        minHeight: 320,
        color: 'common.white',
        backgroundImage: `linear-gradient(140deg, rgba(11,20,37,0.92), rgba(17,57,89,0.8)), url(${product.featureImages[0]?.original?.url ?? ''})`,
        backgroundSize: 'cover',
        backgroundPosition: 'center',
        p: { xs: 3, md: 4 },
        display: 'flex',
        alignItems: 'flex-end',
      }}
    >
      <Box
        sx={{
          position: 'absolute',
          inset: 0,
          background: 'linear-gradient(140deg, rgba(11,20,37,0.78), rgba(17,57,89,0.56) 48%, rgba(255,255,255,0.08) 100%)',
          pointerEvents: 'none',
        }}
      />
      <Box
        sx={{
          position: 'relative',
          zIndex: 1,
          maxWidth: 720,
          px: { xs: 0, md: 1 },
          py: 1.25,
          borderRadius: 3,
          bgcolor: 'rgba(7, 14, 28, 0.34)',
          backdropFilter: 'blur(10px)',
          boxShadow: '0 12px 32px rgba(3, 8, 19, 0.18)',
        }}
      >
        <CaptionIconTypography label="Workspace booking" sx={{ letterSpacing: '0.14em', textTransform: 'uppercase', opacity: 0.86, color: 'common.white' }} />
        <LeadIconTypography
          label={product.listingMetadata.title ?? ''}
          sx={{
            mt: 1,
            mb: 0.75,
            fontSize: { xs: '1.8rem', md: '2.4rem' },
            lineHeight: 1.05,
            color: 'common.white',
            textShadow: '0 2px 18px rgba(0,0,0,0.35)',
          }}
        />
        <SubtitleIconTypography
          label={product.listingMetadata.subTitle ?? ''}
          sx={{ opacity: 0.96, mb: 1.5, color: 'rgba(255,255,255,0.96)', textShadow: '0 1px 14px rgba(0,0,0,0.3)' }}
        />
        <BodyIconTypography
          label={product.listingMetadata.about ?? ''}
          sx={{ opacity: 0.88, maxWidth: 620, color: 'rgba(255,255,255,0.9)', textShadow: '0 1px 10px rgba(0,0,0,0.22)' }}
        />
        <StackRow sx={{ mt: 2 }}>
          {product.amenities.slice(0, 5).map((amenity) => (
            <Chip
              key={amenity.id}
              label={amenity.name}
              sx={{
                bgcolor: 'rgba(255,255,255,0.12)',
                color: 'common.white',
                borderRadius: 999,
                backdropFilter: 'blur(10px)',
              }}
            />
          ))}
          {includedFeatures.slice(0, 3).map((feature) => (
            <Chip
              key={feature}
              label={feature}
              variant="outlined"
              sx={{
                borderColor: 'rgba(255,255,255,0.25)',
                color: 'common.white',
                borderRadius: 999,
              }}
            />
          ))}
        </StackRow>
      </Box>
    </Box>
  );
};

export default memo(MarketplaceProductBookingHero);
