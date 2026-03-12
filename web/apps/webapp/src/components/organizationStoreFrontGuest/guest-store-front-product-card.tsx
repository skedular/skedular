import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, SubtitleIconTypography } from '@/components/commons';
import type { guestStoreFrontProductCard_product$key } from '@/queries/__generated__/guestStoreFrontProductCard_product.graphql';
import type { guestStoreFrontProductCard_query$key } from '@/queries/__generated__/guestStoreFrontProductCard_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Stack from '@mui/material/Stack';
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

  const lowestPricing =
    product.pricingOptions.length > 0 ? product.pricingOptions.reduce((lowest, next) => (next.price < lowest.price ? next : lowest), product.pricingOptions[0]) : null;

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
        <Stack direction="row" justifyContent="space-between" alignItems="center">
          <LeadIconTypography label={product.name} />
        </Stack>

        <BodyIconTypography label={product.description ?? ''} />

        <Box sx={{ mt: 1, borderTop: 1, borderColor: (theme) => theme.palette.divider, pt: 1.5 }}>
          <CaptionIconTypography label="Starting from" sx={{ opacity: 0.75 }} />
          <SubtitleIconTypography label={lowestPricing ? `$${lowestPricing.price}` : 'Contact for pricing'} fontWeight={600} />
        </Box>
      </CardContent>
    </Card>
  );
};

export default memo(GuestStoreFrontProductCard);
