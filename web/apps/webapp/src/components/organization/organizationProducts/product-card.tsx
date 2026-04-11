import { BodyIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@/components/commons';
import { EllipseMenuIcon, ProductIcon } from '@/components/icons';
import { getOrganizationProductBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { emerald, flame } from '@/libs/theme';
import { getRelayErrorMessage } from '@/libs/utils';
import type { productCard_activateProductsMutation } from '@/queries/__generated__/productCard_activateProductsMutation.graphql';
import type { productCard_deactivateProductsMutation } from '@/queries/__generated__/productCard_deactivateProductsMutation.graphql';
import type { productCard_deleteProductsMutation } from '@/queries/__generated__/productCard_deleteProductsMutation.graphql';
import type { productCard_ProductDetails$key } from '@/queries/__generated__/productCard_ProductDetails.graphql';
import type { productCard_query$key } from '@/queries/__generated__/productCard_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
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
  const paletteMode = useContext(PaletteModeContext);
  const router = useRouter();
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
  const primaryFeatureImage = productDetails.featureImages[0]?.thumbnail?.url;
  const previewPricingOptions = pricingOptions.slice(0, 3);

  return (
    <>
      <Card
        sx={{
          width: '100%',
          height: '100%',
          borderRadius: 4,
          border: 1,
          borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
          boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 10px 28px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
          backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.92)' : theme.palette.background.paper),
        }}
      >
        <CardContent sx={{ p: 2, height: '100%' }}>
          <StackColumn spacing={2} sx={{ height: '100%' }}>
            <StackRow sx={{ alignItems: 'flex-start', flexWrap: 'nowrap', gap: 2 }}>
              <Box
                sx={{
                  width: 48,
                  height: 48,
                  borderRadius: 3,
                  border: 1,
                  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  overflow: 'hidden',
                  flexShrink: 0,
                  bgcolor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.04)' : theme.palette.action.hover),
                }}
              >
                {primaryFeatureImage ? (
                  // eslint-disable-next-line @next/next/no-img-element
                  <img src={primaryFeatureImage} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                ) : (
                  <ProductIcon />
                )}
              </Box>

              <StackColumn spacing={0.75} sx={{ minWidth: 0, flexGrow: 1 }}>
                <Link component={NextLink} href={editLink} underline="none" color="inherit">
                  <LeadIconTypography label={productDetails.listingMetadata.title} />
                </Link>
              </StackColumn>

              <StackRow sx={{ gap: 0.5, flexWrap: 'nowrap' }}>
                {!productDetails.inactive && (
                  <IconButton onClick={handleDeactivateProductClick} aria-label="Deactivate product">
                    <Box sx={{ width: 14, height: 14, borderRadius: '50%', backgroundColor: emerald }} />
                  </IconButton>
                )}

                {productDetails.inactive && (
                  <IconButton onClick={handleActivateProductClick} aria-label="Activate product">
                    <Box sx={{ width: 14, height: 14, borderRadius: '50%', backgroundColor: flame }} />
                  </IconButton>
                )}

                {moreActionsOption.length > 0 && (
                  <IconButton onClick={handleMoreActionsMenuClick} aria-label="Open product actions">
                    <EllipseMenuIcon />
                  </IconButton>
                )}
              </StackRow>
            </StackRow>

            <Divider />

            <StackRow sx={{ gap: 1, flexWrap: 'wrap', alignItems: 'center' }}>
              <Chip
                size="small"
                label={productDetails.inactive ? 'Inactive' : 'Active'}
                sx={{
                  bgcolor: productDetails.inactive ? 'rgba(239, 83, 80, 0.12)' : 'rgba(46, 125, 50, 0.12)',
                }}
              />
              <Chip size="small" label={`${pricingOptions.length} offer${pricingOptions.length === 1 ? '' : 's'}`} />
              <Chip size="small" label={currency || productDetails.currency.type} />
            </StackRow>

            <StackColumn spacing={1} sx={{ flexGrow: 1 }}>
              <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center', gap: 1 }}>
                <SubtitleIconTypography label="Offers" />
                {pricingOptions.length > previewPricingOptions.length ? <SmallIconTypography label={`+${pricingOptions.length - previewPricingOptions.length} more`} /> : null}
              </StackRow>

              {previewPricingOptions.length === 0 ? (
                <SmallIconTypography label="No pricing options configured." />
              ) : (
                previewPricingOptions.map((pricingOption) => {
                  const cadence = rootData.productPricingCadences.find((item) => item.type === pricingOption.purchaseCadence)?.name ?? pricingOption.purchaseCadence;

                  return (
                    <Box
                      key={`${productDetails.id}-${pricingOption.index}`}
                      sx={{
                        border: 1,
                        borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
                        borderRadius: 3,
                        p: 1.25,
                        backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.02)' : 'transparent'),
                      }}
                    >
                      <StackColumn spacing={0.75}>
                        <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center', gap: 1 }}>
                          <BodyIconTypography label={pricingOption.listingMetadata.title || `Option ${pricingOption.index + 1}`} />
                          <Chip size="small" label={cadence} />
                        </StackRow>
                        <SmallIconTypography
                          label={`${currency || productDetails.currency.type}${pricingOption.price} • ${pricingOption.isTaxInclusive ? 'Tax included' : 'Tax excluded'}${
                            pricingOption.supportsSubscriptionAutoRenewal ? ' • Auto-renew' : ''
                          }`}
                        />
                      </StackColumn>
                    </Box>
                  );
                })
              )}
            </StackColumn>
          </StackColumn>
        </CardContent>
      </Card>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
    </>
  );
};

export default memo(ProductCard);
