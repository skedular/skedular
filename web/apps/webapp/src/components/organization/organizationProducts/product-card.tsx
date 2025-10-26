import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@/components/commons';
import { EllipseMenuIcon, ProductIcon } from '@/components/icons';
import { getOrganizationProductBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import BookProductButton from '@/components/product/bookProduct/book-product-button';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import type { productCard_ProductDetails$key } from '@/queries/__generated__/productCard_ProductDetails.graphql';
import type { productCard_query$key } from '@/queries/__generated__/productCard_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import CardMedia from '@mui/material/CardMedia';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import 'leaflet/dist/leaflet.css';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
type Props = {
  rootDataRelay: productCard_query$key;
  productDetailsRelay: productCard_ProductDetails$key;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
};

const ProductCard = ({ rootDataRelay, productDetailsRelay, organizationUniqueAlphanumericName }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment productCard_query on Query {
        organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
          canModify
        }
      }
    `,
    rootDataRelay,
  );

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
          id
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
    productDetailsRelay,
  );

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  let moreActionsOption: MoreActionsMenuItemType[] = [];
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const editLink = getOrganizationProductBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, productDetails.id);

  if (rootData.organization?.canModify) {
    moreActionsOption = moreActionsOption.concat(
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditProduct],
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteProduct],
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateProduct],
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateProduct],
    );
  }

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditProduct:
        router.push(editLink);
        break;
    }
  };

  return (
    <>
      <Card sx={{ width: { xs: '100%', sm: 600 } }}>
        {productDetails.primaryFeatureImage && productDetails.primaryFeatureImage.thumbnail && (
          <CardMedia component="img" image={productDetails.primaryFeatureImage.thumbnail.url} />
        )}
        <CardHeader
          title={
            <StackRow>
              <Link component={NextLink} href={editLink}>
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
          action={
            <>
              {moreActionsOption.length > 0 && (
                <Box color={paletteMode === 'dark' ? coal : sandstone} sx={{ paddingTop: 0.5 }}>
                  <IconButton onClick={handleMoreActionsMenuClick} color="inherit">
                    <EllipseMenuIcon />
                  </IconButton>
                </Box>
              )}
            </>
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

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
    </>
  );
};

export default memo(ProductCard);
