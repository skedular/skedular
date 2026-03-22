import { CardMediaCarousel } from '@/components/carousel';
import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@/components/commons';
import { EllipseMenuIcon, ProductIcon } from '@/components/icons';
import { getOrganizationProductBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, emerald, flame, sandstone } from '@/libs/theme';
import { getRelayErrorMessage } from '@/libs/utils';
import type { productCard_activateProductsMutation } from '@/queries/__generated__/productCard_activateProductsMutation.graphql';
import type { productCard_deactivateProductsMutation } from '@/queries/__generated__/productCard_deactivateProductsMutation.graphql';
import type { productCard_deleteProductsMutation } from '@/queries/__generated__/productCard_deleteProductsMutation.graphql';
import type { productCard_ProductDetails$key } from '@/queries/__generated__/productCard_ProductDetails.graphql';
import type { productCard_query$key } from '@/queries/__generated__/productCard_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Chip from '@mui/material/Chip';
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
  organizationCustomDomain: string;
  connectionIds: string[];
};

const ProductCard = ({ rootDataRelay, productDetailsRelay, organizationCustomDomain, connectionIds }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment productCard_query on Query {
        organization(customDomain: $organizationCustomDomain) {
          canModify
        }
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

  const productDetails = useFragment(
    graphql`
      fragment productCard_ProductDetails on ProductDetails {
        id
        inactive
        listingMetadata {
          title
          subTitle
        }
        featureImages {
          thumbnail {
            url
            height
            width
          }
        }
        currency {
          type
        }
        pricingOptions {
          index
          listingMetadata {
            title
            subTitle
          }
          purchaseCadence
          price
          isTaxInclusive
          supportsSubscriptionAutoRenewal
        }
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
  const editLink = getOrganizationProductBaseLink(integratedPlatrform, organizationCustomDomain, productDetails.id);

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

    const toastId = themedToast(<NotificationContent content={`Removing product ${productDetails.listingMetadata.title}...`} />, infoNotificationOptions);

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
            render: <NotificationContent content={`Failed to remove product ${productDetails.listingMetadata.title}. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product ${productDetails.listingMetadata.title} removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove product ${productDetails.listingMetadata.title}. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleDeactivateProductClick = () => {
    if (!productDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Deactivating product ${productDetails.listingMetadata.title}...`} />, infoNotificationOptions);

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
            render: <NotificationContent content={`Failed to deactivate product ${productDetails.listingMetadata.title}. Error: ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product ${productDetails.listingMetadata.title} deactivated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate product ${productDetails.listingMetadata.title}. Error: ${error.message}.`} />,
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

    const toastId = themedToast(<NotificationContent content={`Activating product ${productDetails.listingMetadata.title}...`} />, infoNotificationOptions);

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
            render: <NotificationContent content={`Failed to activate product ${productDetails.listingMetadata.title}. Error: ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product ${productDetails.listingMetadata.title} activated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate product ${productDetails.listingMetadata.title}. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const pricingOptions = [...productDetails.pricingOptions].sort((left, right) => left.index - right.index);
  const currency = rootData.currencies.find((currency) => currency.type === productDetails.currency.type)?.name;

  return (
    <>
      <Card sx={{ width: { xs: '100%', sm: 600 } }}>
        <CardMediaCarousel images={productDetails.featureImages} showPlaceholderWhenEmpty={false} />
        <CardHeader
          title={
            <StackRow>
              <Link component={NextLink} href={editLink}>
                <LeadIconTypography label={productDetails.listingMetadata.title} startElement={<ProductIcon />} sx={{ flexWrap: undefined }} invertDefaultColor />
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
          <BodyIconTypography label={productDetails.listingMetadata.subTitle} />

          <StackRow sx={{ marginTop: 1 }}>
            <BodyIconTypography label="Pricing Options" />
          </StackRow>

          {pricingOptions.length === 0 ? (
            <SmallIconTypography label="No pricing options configured." />
          ) : (
            pricingOptions.map((pricingOption) => {
              const cadence = rootData.productPricingCadences.find((item) => item.type === pricingOption.purchaseCadence)?.name ?? pricingOption.purchaseCadence;

              return (
                <Box
                  key={`${productDetails.id}-${pricingOption.index}`}
                  sx={{
                    border: 1,
                    borderColor: 'divider',
                    borderRadius: 2,
                    padding: 1.5,
                    marginTop: 1,
                  }}
                >
                  <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
                    <BodyIconTypography label={pricingOption.listingMetadata.title || `Option ${pricingOption.index + 1}`} />
                    <Chip size="small" label={cadence} />
                  </StackRow>

                  <StackRow>
                    <BodyIconTypography label="Price:" />
                    <SmallIconTypography label={`${currency}${pricingOption.price} (${pricingOption.isTaxInclusive ? 'Tax Included' : 'Tax Excluded'})`} />
                  </StackRow>
                </Box>
              );
            })
          )}
        </CardContent>
      </Card>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
    </>
  );
};

export default memo(ProductCard);
