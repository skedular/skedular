import { DefaultDialogTitle, GridContainer, PushToRight, SectionIconTypography, SmallIconTypography, StackColumn, TwoButtonsDialogActions } from '@/components/commons';
import { EllipseMenuIcon } from '@/components/icons';
import { getOrganizationProductSetupBaseLink } from '@/components/links';
import ListGridToggle from '@/components/listGridToggle/list-grid-toggle';
import { Loading } from '@/components/loading';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { NewProductButton } from '@/components/product/addProduct';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { defaultGridStyle, defaultPadding, maxScreenWidth } from '@/libs/theme';
import { joinErrors, startOfDay } from '@/libs/utils';
import type { organizationProducts_deleteProductMutation } from '@/queries/__generated__/organizationProducts_deleteProductMutation.graphql';
import type { organizationProducts_products_query$key } from '@/queries/__generated__/organizationProducts_products_query.graphql';
import type { organizationProducts_products_refetchableFragment } from '@/queries/__generated__/organizationProducts_products_refetchableFragment.graphql';
import type { organizationProducts_rootQuery } from '@/queries/__generated__/organizationProducts_rootQuery.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import Box from '@mui/system/Box';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import ProductCard from './product-card';

type Props = {
  queryReference: PreloadedQuery<organizationProducts_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationProducts_rootQuery($organizationId: String!, $productsSortingValues: [ProductOrderInput!]) {
    ...organizationProducts_products_query
  }
`;

type ProductDetails = {
  name: string;
};

type RowType = {
  id: string;
  product: ProductDetails;
};

const OrganizationProducts = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootData = usePreloadedQuery<organizationProducts_rootQuery>(RootQuery, queryReference);
  const [rootDataRefetchable, refetch] = useRefetchableFragment<organizationProducts_products_refetchableFragment, organizationProducts_products_query$key>(
    graphql`
      fragment organizationProducts_products_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationProducts_products_refetchableFragment") {
        products(first: $count, after: $cursor, where: { organizationIds: [$organizationId], includeInactive: true }, orderBy: $productsSortingValues)
          @connection(key: "organizationProducts_products") {
          __id
          totalCount
          edges {
            node {
              id
              inactive
              name
              description
              price
              priceUnit {
                type
                name
              }
              currency {
                type
                name
              }
              numberOfResourcesToBook
              minDurationMinutes
              maxDurationMinutes
              bookAllLocationResources
              recurrenceWindowDays
              requireConsecutiveDays
              maxBookingSpreadDays
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

  const [commitDeleteProduct] = useMutation<organizationProducts_deleteProductMutation>(graphql`
    mutation organizationProducts_deleteProductMutation($connectionIds: [ID!]!, $input: DeleteProductInput!) {
      deleteProduct(input: $input) {
        product {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [viewMode, setViewMode] = useState<'list' | 'grid'>('grid');
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

        router.push(getOrganizationProductSetupBaseLink(productDetails.organization?.uniqueId!, productDetails.id));
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

    commitDeleteProduct({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: productDetails.id,
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

  const handlViewModeChanged = (newViewMode: 'list' | 'grid') => {
    setViewMode(newViewMode);
  };

  const rows: RowType[] = products.map((product) => {
    return {
      id: product.id,
      product,
    };
  });

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'product',
      headerName: 'Product',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value.name} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'moreActions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedProductId(params.id as string);
              setMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  if (!rootDataRefetchable.products) {
    return <></>;
  }

  return (
    <>
      <StackColumn sx={{ maxWidth: maxScreenWidth }}>
        <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
          <ListGridToggle defaultValue={viewMode} onChange={handlViewModeChanged} />
          <PushToRight />
          <NewProductButton organizationId={organizationId} />
        </GridContainer>
        <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
          <SectionIconTypography label="Products" />
          <Divider />
          <Box sx={{ paddingBottom: defaultPadding }} />

          {viewMode === 'grid' && (
            <GridContainer>
              {products.map((product) => (
                <Grid key={product.id}>
                  <ProductCard rootDataRelay={product} onReloadRequired={onReloadRequired} connectionIds={connectionIds} />
                </Grid>
              ))}
            </GridContainer>
          )}

          {viewMode === 'list' && (
            <DataGrid
              rows={rows}
              columns={columns}
              ignoreDiacritics
              disableRowSelectionOnClick
              hideFooter
              getRowHeight={() => 'auto'}
              rowSpacingType="margin"
              getRowSpacing={() => ({ top: 3, bottom: 3 })}
              sx={defaultGridStyle}
              localeText={{ noRowsLabel: 'No product found' }}
            />
          )}
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

const MemoOrganizationProducts = memo(OrganizationProducts);

type RelayProps = {
  organizationId: string;
};

const OrganizationProductsWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationProducts_rootQuery>(RootQuery);
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
      <MemoOrganizationProducts queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationProductsWithRelay);
