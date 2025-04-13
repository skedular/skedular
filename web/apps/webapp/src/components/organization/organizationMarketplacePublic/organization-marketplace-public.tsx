import { DefaultDialogTitle, GridContainer, SectionIconTypography, StackColumn, TwoButtonsDialogActions } from '@/components/commons';
import { getOrganizationProductBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { defaultPadding, maxScreenWidth } from '@/libs/theme';
import { joinErrors, startOfDay } from '@/libs/utils';
import type { organizationMarketplacePublic_deleteProductsMutation } from '@/queries/__generated__/organizationMarketplacePublic_deleteProductsMutation.graphql';
import type { organizationMarketplacePublic_products_query$key } from '@/queries/__generated__/organizationMarketplacePublic_products_query.graphql';
import type { organizationMarketplacePublic_products_refetchableFragment } from '@/queries/__generated__/organizationMarketplacePublic_products_refetchableFragment.graphql';
import type { organizationMarketplacePublic_rootQuery } from '@/queries/__generated__/organizationMarketplacePublic_rootQuery.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import Box from '@mui/system/Box';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import ProductCard from './product-card';

type Props = {
  queryReference: PreloadedQuery<organizationMarketplacePublic_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationMarketplacePublic_rootQuery($organizationId: String!, $productsSortingValues: [ProductOrderInput!]) {
    ...organizationMarketplacePublic_products_query
  }
`;

const OrganizationMarketplacePublic = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootData = usePreloadedQuery<organizationMarketplacePublic_rootQuery>(RootQuery, queryReference);
  const [rootDataRefetchable] = useRefetchableFragment<organizationMarketplacePublic_products_refetchableFragment, organizationMarketplacePublic_products_query$key>(
    graphql`
      fragment organizationMarketplacePublic_products_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationMarketplacePublic_products_refetchableFragment") {
        products(first: $count, after: $cursor, where: { organizationIds: [$organizationId], includeInactive: false }, orderBy: $productsSortingValues)
          @connection(key: "organizationMarketplacePublic_products") {
          __id
          totalCount
          edges {
            node {
              id
              name
              organization {
                uniqueId
              }
              ...productCard_ProductDetails
            }
          }
        }
      }
    `,
    rootData,
  );

  const [commitDeleteProducts] = useMutation<organizationMarketplacePublic_deleteProductsMutation>(graphql`
    mutation organizationMarketplacePublic_deleteProductsMutation($connectionIds: [ID!]!, $input: DeleteProductsInput!) {
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
  const connectionIds = useMemo(() => [rootDataRefetchable.products.__id], [rootDataRefetchable.products]);
  const [selectedProductId, setSelectedProductId] = useState<null | string>(null);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [productRemoveConfirmationDialogOpen, setProductRemoveConfirmationDialogOpen] = useState(false);

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditProduct],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteProduct],
  ];

  const products = useMemo(() => rootDataRefetchable.products.edges.map((edge) => edge.node).sort((a, b) => a.name.localeCompare(b.name)), [rootDataRefetchable.products]);
  const productDetails = useMemo(() => products.find((item) => item.id === selectedProductId), [selectedProductId, products]);

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditProduct:
        if (!productDetails) {
          return;
        }

        router.push(getOrganizationProductBaseLink(productDetails.organization?.uniqueId!, productDetails.id));
        break;

      case MoreActionsMenuOptionType.DeleteProduct:
        handleRemoveProductClicked();
        break;
    }
  };

  const handleRemoveProductClicked = () => {
    setProductRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingProductClick = () => {
    setProductRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingProductClick = () => {
    if (!productDetails) {
      return;
    }

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

  if (!rootDataRefetchable.products) {
    return <></>;
  }

  return (
    <>
      <StackColumn sx={{ maxWidth: maxScreenWidth }}>
        <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
          <SectionIconTypography label="Products" />
          <Divider />
          <Box sx={{ paddingBottom: defaultPadding }} />

          <GridContainer>
            {products.map((product) => (
              <Grid key={product.id}>
                <ProductCard rootDataRelay={product} onReloadRequired={onReloadRequired} connectionIds={connectionIds} />
              </Grid>
            ))}
          </GridContainer>
        </StackColumn>
      </StackColumn>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />

      {productDetails && (
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
      )}
    </>
  );
};

const MemoOrganizationMarketplacePublic = memo(OrganizationMarketplacePublic);

type RelayProps = {
  organizationId: string;
};

const OrganizationMarketplacePublicWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationMarketplacePublic_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const today = startOfDay();

    loadQuery(
      {
        organizationId,
        productsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationMarketplacePublic queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationMarketplacePublicWithRelay);
