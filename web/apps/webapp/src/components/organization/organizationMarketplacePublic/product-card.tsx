import { BodyIconTypography, LeadIconTypography, SmallIconTypography, StackRow } from '@/components/commons';
import { ProductIcon } from '@/components/icons';
import type { productCard_ProductDetails$key } from '@/queries/__generated__/productCard_ProductDetails.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: productCard_ProductDetails$key;
  onReloadRequired: () => void;
};

const ProductCard = ({ rootDataRelay }: Props) => {
  const productDetails = useFragment(
    graphql`
      fragment productCard_ProductDetails on ProductDetails {
        id
        name
        description
        priceToDisplay
        priceUnit {
          name
        }
        numberOfResourcesToBook
        minDurationMinutes
        maxDurationMinutes
        requireConsecutiveDays
        maxBookingSpreadDays
        organization {
          uniqueId
        }
      }
    `,
    rootDataRelay,
  );

  return (
    <>
      <Card sx={{ width: { xs: '100%', sm: 600 } }}>
        <CardHeader
          title={
            <StackRow>
              <LeadIconTypography label={productDetails.name} startElement={<ProductIcon />} sx={{ flexWrap: undefined }} invertDefaultColor />
            </StackRow>
          }
        />
        <CardContent>
          <BodyIconTypography label={productDetails.description} />

          <StackRow>
            <BodyIconTypography label="Price:" />
            <SmallIconTypography label={`${productDetails.priceToDisplay} - ${productDetails.priceUnit.name}`} />
          </StackRow>

          {productDetails.minDurationMinutes && (
            <StackRow>
              <BodyIconTypography label="Min duration:" />
              <SmallIconTypography label={productDetails.minDurationMinutes ? `${productDetails.minDurationMinutes} minutes` : 'No limit'} />
            </StackRow>
          )}

          <StackRow>
            <BodyIconTypography label="Max duration:" />
            <SmallIconTypography label={productDetails.maxDurationMinutes ? `${productDetails.maxDurationMinutes} minutes` : 'No limit'} />
          </StackRow>

          <StackRow>
            <BodyIconTypography label="Must book consecutive days:" />
            <SmallIconTypography label={productDetails.requireConsecutiveDays ? 'Yes' : 'No'} />
          </StackRow>

          <StackRow>
            <BodyIconTypography label="Max booking spread days:" />
            <SmallIconTypography label={productDetails.maxBookingSpreadDays ? productDetails.maxBookingSpreadDays.toString() : 'No limit'} />
          </StackRow>
        </CardContent>
      </Card>
    </>
  );
};

export default memo(ProductCard);
