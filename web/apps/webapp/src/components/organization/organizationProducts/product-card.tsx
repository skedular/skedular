import { BodyIconTypography, DefaultDialogTitle, LeadIconTypography, SmallIconTypography, StackRow, TwoButtonsDialogActions } from '@/components/commons';
import { EllipseMenuIcon, ProductIcon } from '@/components/icons';
import { getOrganizationProductBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { productCard_deleteProductsMutation } from '@/queries/__generated__/productCard_deleteProductsMutation.graphql';
import type { productCard_ProductDetails$key } from '@/queries/__generated__/productCard_ProductDetails.graphql';
import { Divider } from '@mui/material';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import { nanoid } from 'nanoid';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';

type Props = {
  rootDataRelay: productCard_ProductDetails$key;
  onReloadRequired: () => void;
  connectionIds: string[];
};

const ProductCard = ({ rootDataRelay, connectionIds }: Props) => {
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

  const [commitDeleteProducts] = useMutation<productCard_deleteProductsMutation>(graphql`
    mutation productCard_deleteProductsMutation($connectionIds: [ID!]!, $input: DeleteProductsInput!) {
      deleteProducts(input: $input) {
        products {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [productRemoveConfirmationDialogOpen, setProductRemoveConfirmationDialogOpen] = useState(false);
  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditProduct],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteProduct],
  ];

  const editLink = getOrganizationProductBaseLink(productDetails.organization.uniqueId!, productDetails.id);

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
        handleRemoveTeamClicked();
        break;
    }
  };

  const handleRemoveTeamClicked = () => {
    setProductRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingProductClick = () => {
    setProductRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingProductClick = () => {
    const toastId = themedToast(<NotificationContent content={`Removing product '${productDetails.name}'...`} />, infoNotificationOptions);

    commitDeleteProducts({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: [productDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove product '${productDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product '${productDetails.name}' has been successfully removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove product '${productDetails.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  return (
    <>
      <Card sx={{ width: { xs: '100%', sm: 600 } }}>
        <CardHeader
          title={
            <StackRow>
              <Link component={NextLink} href={editLink}>
                <LeadIconTypography label={productDetails.name} startElement={<ProductIcon />} sx={{ flexWrap: undefined }} invertDefaultColor />
              </Link>
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
          <Divider />
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

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />

      <Dialog slots={{ transition: DialogTransition }} open={productRemoveConfirmationDialogOpen} onClose={handleCancelRemovingProductClick}>
        <DefaultDialogTitle title="Remove Product" />
        <DialogContent sx={{ marginTop: 2 }}>
          <DialogContentText>{`Are you sure you want to remove the product "${productDetails.name}"?`}</DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmRemovingProductClick}
            onSecondaryClicked={handleCancelRemovingProductClick}
            primaryLabel="Remove"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(ProductCard);
