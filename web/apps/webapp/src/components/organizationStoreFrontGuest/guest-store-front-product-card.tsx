import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackRow, SubtitleIconTypography } from '@/components/commons';
import type { guestStoreFrontProductCard_product$key } from '@/queries/__generated__/guestStoreFrontProductCard_product.graphql';
import type { guestStoreFrontProductCard_query$key } from '@/queries/__generated__/guestStoreFrontProductCard_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Box from '@mui/system/Box';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: guestStoreFrontProductCard_query$key;
  productRelay: guestStoreFrontProductCard_product$key;
};

const GuestStoreFrontProductCard = ({ rootDataRelay, productRelay }: Props) => {
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
        name
        description
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
          cadence
          price
          isTaxInclusive
        }
      }
    `,
    productRelay,
  );

  const currency = product.currency ? rootData.currencies.find((item) => item.type === product.currency?.type)?.name : null;

  const pricingRows = [...product.pricingOptions]
    .sort((a, b) => a.index - b.index)
    .map((option) => ({
      id: option.id,
      cadenceLabel: rootData.productPricingCadences.find((cadence) => cadence.type === option.cadence)?.name ?? option.cadence,
      amountLabel: currency ? `${currency} ${option.price}` : `${option.price}`,
      taxLabel: option.isTaxInclusive ? 'incl. tax' : 'excl. tax',
    }));

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
      <CardMedia component="img" image={product.featureImages[0]?.original?.url ?? ''} alt={product.name} sx={{ height: 190 }} />
      <CardContent sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
        <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
          <LeadIconTypography label={product.name} />
        </StackRow>

        <BodyIconTypography label={product.description ?? ''} />

        {product.amenities.length > 0 && (
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
            {product.amenities.slice(0, 4).map((amenity) => (
              <CaptionIconTypography key={amenity.id} label={amenity.name} sx={{ px: 1, py: 0.5, borderRadius: 1, bgcolor: (theme) => theme.palette.action.hover }} />
            ))}
          </Box>
        )}

        <Box sx={{ mt: 1, borderTop: 1, borderColor: (theme) => theme.palette.divider, pt: 1.5 }}>
          <CaptionIconTypography label="Pricing" sx={{ opacity: 0.75, mb: 1 }} />
          {pricingRows.length > 0 ? (
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 0.75 }}>
              {pricingRows.map((row) => (
                <StackRow key={row.id} sx={{ justifyContent: 'space-between', alignItems: 'baseline', flexWrap: 'nowrap' }}>
                  <CaptionIconTypography label={row.cadenceLabel} sx={{ opacity: 0.9 }} />
                  <StackRow spacing={0.75} sx={{ alignItems: 'baseline', flexWrap: 'nowrap' }}>
                    <SubtitleIconTypography label={row.amountLabel} fontWeight={600} />
                    <CaptionIconTypography label={row.taxLabel} sx={{ opacity: 0.65 }} />
                  </StackRow>
                </StackRow>
              ))}
            </Box>
          ) : (
            <CaptionIconTypography label="Contact for pricing" sx={{ opacity: 0.85 }} />
          )}
        </Box>
      </CardContent>
    </Card>
  );
};

export default memo(GuestStoreFrontProductCard);
