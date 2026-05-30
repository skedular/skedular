import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackRow, SubtitleIconTypography } from '@skedular/ui';
import { getMarketplaceProductBookingLink, getMarketplaceProductLink, getMarketplaceProductSubscribeLink } from '@/components/links';
import { isSubscriptionCadence } from '@/components/marketplaceProductSubscription/subscription-utils';
import { useIntegratedPlatform } from '@skedular/shared';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Radio from '@mui/material/Radio';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useMemo, useState } from 'react';
import useKnownParams from '@/hooks/use-known-params';

type PricingRow = {
  amountLabel: string;
  cadence: string;
  cadenceLabel: string;
  id: string;
  taxLabel: string;
  title: string;
};

type Amenity = {
  id: string;
  name: string;
};

type Props = {
  amenities: readonly Amenity[];
  imageUrl: string;
  organizationCustomDomain: string;
  pricingRows: readonly PricingRow[];
  productId: string;
  subTitle: string;
  title: string;
};

const MarketplaceProductCard = ({ amenities, imageUrl, organizationCustomDomain, pricingRows, productId, subTitle, title }: Props) => {
  const { integratedPlatform } = useIntegratedPlatform();
  const { isCustomDomain } = useKnownParams();
  const router = useRouter();
  const [selectedPricingId, setSelectedPricingId] = useState(pricingRows[0]?.id ?? '');
  const effectiveSelectedPricingId = pricingRows.some((row) => row.id === selectedPricingId) ? selectedPricingId : (pricingRows[0]?.id ?? '');
  const selectedPricing = useMemo(() => pricingRows.find((row) => row.id === effectiveSelectedPricingId), [effectiveSelectedPricingId, pricingRows]);

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
      <CardMedia component="img" image={imageUrl} alt={title} sx={{ height: 190, bgcolor: (theme) => theme.palette.action.hover }} />
      <CardContent sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
        <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
          <LeadIconTypography label={title} />
        </StackRow>

        <BodyIconTypography label={subTitle} />

        {amenities.length > 0 && (
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
            {amenities.slice(0, 4).map((amenity) => (
              <CaptionIconTypography
                key={amenity.id}
                label={amenity.name}
                sx={{ px: 1.25, py: 0.5, borderRadius: 1.25, bgcolor: (theme) => theme.palette.action.hover, fontSize: '0.8rem' }}
              />
            ))}
          </Box>
        )}

        <Box sx={{ mt: 1, borderTop: 1, borderColor: (theme) => theme.palette.divider, pt: 1.5, flex: 1 }}>
          <CaptionIconTypography label="Choose your plan" sx={{ opacity: 0.7, mb: 1, textTransform: 'uppercase', letterSpacing: '0.04em' }} />
          {pricingRows.length > 0 ? (
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 0.75 }}>
              {pricingRows.map((row) => (
                <Box
                  key={row.id}
                  onClick={() => setSelectedPricingId(row.id)}
                  sx={{
                    border: 1,
                    borderColor: (theme) => (row.id === effectiveSelectedPricingId ? theme.palette.primary.main : theme.palette.divider),
                    borderRadius: 2,
                    px: 1.25,
                    py: 1,
                    minHeight: 72,
                    cursor: 'pointer',
                    bgcolor: (theme) => (row.id === effectiveSelectedPricingId ? theme.palette.action.selected : theme.palette.background.paper),
                    transition: 'border-color 120ms ease, background-color 120ms ease',
                  }}
                >
                  <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center', flexWrap: 'nowrap', height: '100%' }}>
                    <StackRow spacing={0.75} sx={{ flexWrap: 'nowrap', alignItems: 'flex-start', minWidth: 0 }}>
                      <Radio
                        checked={row.id === effectiveSelectedPricingId}
                        value={row.id}
                        size="small"
                        sx={{
                          p: 0,
                          '& .MuiSvgIcon-root': {
                            fontSize: 18,
                          },
                        }}
                      />
                      <Box sx={{ minWidth: 0 }}>
                        <CaptionIconTypography label={row.cadenceLabel} sx={{ opacity: 0.9, display: 'block', fontWeight: 500 }} />
                        <CaptionIconTypography label={row.title} sx={{ opacity: 0.65, display: 'block' }} />
                      </Box>
                    </StackRow>
                    <Box sx={{ textAlign: 'right', flexShrink: 0 }}>
                      <SubtitleIconTypography label={row.amountLabel} fontWeight={600} sx={{ whiteSpace: 'nowrap', display: 'block', lineHeight: 1.15 }} />
                      <CaptionIconTypography label={row.taxLabel} sx={{ opacity: 0.65, display: 'block' }} />
                    </Box>
                  </StackRow>
                </Box>
              ))}
            </Box>
          ) : (
            <CaptionIconTypography label="Contact for pricing" sx={{ opacity: 0.85 }} />
          )}
        </Box>

        <StackRow sx={{ mt: 'auto', flexWrap: 'nowrap' }}>
          <Button
            fullWidth
            variant="contained"
            onClick={() => {
              if (!selectedPricing) {
                return;
              }

              router.push(
                isSubscriptionCadence(selectedPricing.cadence)
                  ? getMarketplaceProductSubscribeLink(integratedPlatform, isCustomDomain, organizationCustomDomain, productId, selectedPricing.id)
                  : getMarketplaceProductBookingLink(integratedPlatform, isCustomDomain, organizationCustomDomain, productId, selectedPricing.id),
              );
            }}
            disabled={!selectedPricing}
            sx={{ textTransform: 'none' }}
          >
            {selectedPricing && isSubscriptionCadence(selectedPricing.cadence) ? 'Choose plan' : 'Book now'}
          </Button>
          <Button
            fullWidth
            variant="outlined"
            onClick={() => router.push(getMarketplaceProductLink(integratedPlatform, isCustomDomain, organizationCustomDomain, productId))}
            sx={{ textTransform: 'none' }}
          >
            Details
          </Button>
        </StackRow>
      </CardContent>
    </Card>
  );
};

export default memo(MarketplaceProductCard);
