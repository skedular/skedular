import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@/components/commons';
import { ProductIcon } from '@/components/icons';
import { getOrganizationBookingProductLink } from '@/components/links';
import BookProductButton from '@/components/product/bookProduct/book-product-button';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import type { productCard_ProductDetails$key } from '@/queries/__generated__/productCard_ProductDetails.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import CardMedia from '@mui/material/CardMedia';
import Link from '@mui/material/Link';
import NextLink from 'next/link';
import { memo, useContext } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: productCard_ProductDetails$key;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
};

const ProductCard = ({ rootDataRelay, organizationUniqueAlphanumericName }: Props) => {
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
        primaryFeatureImage {
          thumbnail {
            url
            height
            width
          }
        }
        isPriceTaxInclusive
      }
    `,
    rootDataRelay,
  );

  const { integratedPlatrform } = useIntegratedPlatrform();
  const paletteMode = useContext(PaletteModeContext);

  return (
    <>
      <Card sx={{ width: { xs: '100%', sm: 600 } }}>
        {productDetails.primaryFeatureImage && productDetails.primaryFeatureImage.thumbnail && (
          <CardMedia component="img" image={productDetails.primaryFeatureImage.thumbnail.url} />
        )}
        <CardHeader
          title={
            <StackRow>
              <Link component={NextLink} href={getOrganizationBookingProductLink(integratedPlatrform, organizationUniqueAlphanumericName, productDetails.id)}>
                <LeadIconTypography label={productDetails.name} startElement={<ProductIcon />} sx={{ flexWrap: undefined }} invertDefaultColor />
              </Link>

              <PushToRight />
              <BookProductButton
                organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                productId={productDetails.id}
                label="Book Now"
                hideIcon
                variant="contained"
                size="small"
                sx={{ textTransform: 'none' }}
                invertDefaultColor={paletteMode === 'dark'}
              />
            </StackRow>
          }
        />
        <CardContent>
          <BodyIconTypography label={productDetails.description} />

          <StackRow>
            <BodyIconTypography label="Price:" />
            <SmallIconTypography
              label={`${productDetails.priceToDisplay} - ${productDetails.priceUnit.name}, ${productDetails.isPriceTaxInclusive ? 'Tax Included' : 'Tax Excluded'}`}
            />
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
