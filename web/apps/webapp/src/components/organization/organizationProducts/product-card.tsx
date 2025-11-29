import { CardMediaCarousel } from '@/components/carousel';
import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@/components/commons';
import { EllipseMenuIcon, ProductIcon } from '@/components/icons';
import { getOrganizationProductBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import BookProductButton from '@/components/product/bookProduct/book-product-button';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, emerald, flame, sandstone } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { productCard_activateProductsMutation } from '@/queries/__generated__/productCard_activateProductsMutation.graphql';
import type { productCard_deactivateProductsMutation } from '@/queries/__generated__/productCard_deactivateProductsMutation.graphql';
import type { productCard_deleteProductsMutation } from '@/queries/__generated__/productCard_deleteProductsMutation.graphql';
import type { productCard_ProductDetails$key } from '@/queries/__generated__/productCard_ProductDetails.graphql';
import type { productCard_query$key } from '@/queries/__generated__/productCard_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  rootDataRelay: productCard_query$key;
  productDetailsRelay: productCard_ProductDetails$key;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
  connectionIds: string[];
};

const ProductCard = ({ rootDataRelay, productDetailsRelay, organizationUniqueAlphanumericName, connectionIds }: Props) => {
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
        inactive
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
        featureImages {
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

  const [commitDeleteProduct] = useMutation<productCard_deleteProductsMutation>(graphql`
    mutation productCard_deleteProductsMutation($connectionIds: [ID!]!, $input: DeleteProductsInput!) {
      deleteProducts(input: $input) {
        products {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitActivateProducts] = useMutation<productCard_activateProductsMutation>(graphql`
    mutation productCard_activateProductsMutation($input: ActivateProductsInput!) @raw_response_type {
      activateProducts(input: $input) {
        products {
          id
          inactive
        }
      }
    }
  `);

  const [commitDeactivateProducts] = useMutation<productCard_deactivateProductsMutation>(graphql`
    mutation productCard_deactivateProductsMutation($input: DeactivateProductsInput!) @raw_response_type {
      deactivateProducts(input: $input) {
        products {
          id
          inactive
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  let moreActionsOption: MoreActionsMenuItemType[] = [];
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const editLink = getOrganizationProductBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, productDetails.id);

  if (rootData.organization?.canModify) {
    moreActionsOption = moreActionsOption.concat(
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditProduct],
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteProduct],
    );

    if (productDetails.inactive) {
      moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateProduct]);
    } else {
      moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateProduct]);
    }
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

      case MoreActionsMenuOptionType.DeleteProduct:
        handleRemoveProductClick();
        break;

      case MoreActionsMenuOptionType.ActivateProduct:
        handleActivateProductClick();
        break;

      case MoreActionsMenuOptionType.DeactivateProduct:
        handleDeactivateProductClick();
    }
  };
  const handleRemoveProductClick = () => {
    if (!productDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing product ${productDetails.name}...`} />, infoNotificationOptions);

    commitDeleteProduct({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          ids: [productDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove product ${productDetails.name}. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product ${productDetails.name} removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove product ${productDetails.name}. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleDeactivateProductClick = () => {
    if (!productDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Deactivating product ${productDetails.name}...`} />, infoNotificationOptions);

    commitDeactivateProducts({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: [productDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate product ${productDetails.name}. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product ${productDetails.name} deactivated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate product ${productDetails.name}. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        deactivateProducts: {
          products: [
            {
              id: productDetails.id,
              inactive: true,
            },
          ],
        },
      },
    });
  };

  const handleActivateProductClick = () => {
    if (!productDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Activating product ${productDetails.name}...`} />, infoNotificationOptions);

    commitActivateProducts({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: [productDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate product ${productDetails.name}. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product ${productDetails.name} activated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate product ${productDetails.name}. Error: ${error.message}.`} />,
        });
      },
    });
  };

  return (
    <>
      <Card sx={{ width: { xs: '100%', sm: 600 } }}>
        <CardMediaCarousel images={productDetails.featureImages ?? []} />
        <CardHeader
          title={
            <StackRow>
              <Link component={NextLink} href={editLink}>
                <LeadIconTypography label={productDetails.name} startElement={<ProductIcon />} sx={{ flexWrap: undefined }} invertDefaultColor />
              </Link>

              <PushToRight />

              {!productDetails.inactive && (
                <IconButton onClick={handleDeactivateProductClick}>
                  <StackRow sx={{ justifyContent: 'space-between', width: 76 }}>
                    <SmallIconTypography label="Active" invertDefaultColor />
                    <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: emerald }} />
                  </StackRow>
                </IconButton>
              )}

              {productDetails.inactive && (
                <IconButton onClick={handleActivateProductClick}>
                  <StackRow sx={{ justifyContent: 'space-between', width: 76 }}>
                    <SmallIconTypography label="Inactive" invertDefaultColor />
                    <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: flame }} />
                  </StackRow>
                </IconButton>
              )}

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
