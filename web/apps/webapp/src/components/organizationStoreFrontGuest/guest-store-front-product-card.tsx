import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackRow, SubtitleIconTypography } from '@/components/commons';
import { getMarketplaceProductLink } from '@/components/links';
import { useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import type { guestStoreFrontProductCard_product$key } from '@/queries/__generated__/guestStoreFrontProductCard_product.graphql';
import type { guestStoreFrontProductCard_query$key } from '@/queries/__generated__/guestStoreFrontProductCard_query.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Radio from '@mui/material/Radio';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: guestStoreFrontProductCard_query$key;
  productRelay: guestStoreFrontProductCard_product$key;
  organizationUniqueAlphanumericName: string;
};

const GuestStoreFrontProductCard = ({ rootDataRelay, productRelay, organizationUniqueAlphanumericName }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const rootData = useFragment<guestStoreFrontProductCard_query$key>(
    graphql`
      fragment guestStoreFrontProductCard_query on Query {
        productPricingCadences {
          type
          name
        }
        currencies {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const product = useFragment(
    graphql`
      fragment guestStoreFrontProductCard_product on ProductDetails {
        id
        listingMetadata {
          title
          subTitle
        }
        featureImages {
          original {
            url
          }
        }
        currency {
          type
          name
        }
        amenities {
          id
          name
        }
        pricingOptions {
          id
          index
          listingMetadata {
            title
            subTitle
          }
          cadence
          price
          isTaxInclusive
        }
      }
    `,
    productRelay,
  );

  const { isCustomDomain } = useKnownParams();

  const currency = product.currency ? rootData.currencies.find((item) => item.type === product.currency?.type)?.name : null;

  const pricingRows = useMemo(
    () =>
      [...product.pricingOptions]
        .sort((a, b) => a.index - b.index)
        .map((option) => ({
          id: option.id,
          title: option.listingMetadata.title ?? '',
          cadenceLabel: rootData.productPricingCadences.find((cadence) => cadence.type === option.cadence)?.name ?? option.cadence,
          amountLabel: currency ? `${currency} ${option.price}` : `${option.price}`,
          taxLabel: option.isTaxInclusive ? 'incl. tax' : 'excl. tax',
        })),
    [currency, product.pricingOptions, rootData.productPricingCadences],
  );
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
      <CardMedia component="img" image={product.featureImages[0]?.original?.url ?? ''} alt={product.listingMetadata.title ?? ''} sx={{ height: 190 }} />
      <CardContent sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
        <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
          <LeadIconTypography label={product.listingMetadata.title} />
        </StackRow>

        <BodyIconTypography label={product.listingMetadata.subTitle ?? ''} />

        {product.amenities.length > 0 && (
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
            {product.amenities.slice(0, 4).map((amenity) => (
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

        <StackRow spacing={1} sx={{ mt: 'auto', flexWrap: 'nowrap' }}>
          <Button fullWidth variant="contained" onClick={() => {}} disabled={!selectedPricing} sx={{ textTransform: 'none' }}>
            Book now
          </Button>
          <Button
            fullWidth
            variant="outlined"
            onClick={() => router.push(getMarketplaceProductLink(integratedPlatrform, isCustomDomain, organizationUniqueAlphanumericName, product.id))}
            sx={{ textTransform: 'none' }}
          >
            Details
          </Button>
        </StackRow>
      </CardContent>
    </Card>
  );
};

export default memo(GuestStoreFrontProductCard);
