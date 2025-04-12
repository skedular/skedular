import { AppBarWithStackColumn, BodyIconTypography, GridContainer, PushToRight, SectionIconTypography, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { DeleteIcon, EllipseMenuIcon } from '@/components/icons';
import { getOrganizationBaseLink } from '@/components/links';
import { LocationTag } from '@/components/locationTag';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { AddOrganizationLocationTagButton } from '@/components/organization/addOrganizationLocationTag';
import { AddOrganizationProductTagButton } from '@/components/organization/addOrganizationProductTag';
import { EditOrganizationLocationTagDialog } from '@/components/organization/editOrganizationLocationTag';
import { EditOrganizationProductTagDialog } from '@/components/organization/editOrganizationProductTag';
import { ProductTag } from '@/components/productTag';
import { Search } from '@/components/search';
import { PaletteModeContext } from '@/libs/providers';
import { defaultGridActionPadding, defaultGridStyle, defaultPadding, secondDrawerExpandedDrawerWidthPx } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { organizationMarketplaceSetup_deleteLocationTagsMutation } from '@/queries/__generated__/organizationMarketplaceSetup_deleteLocationTagsMutation.graphql';
import type { organizationMarketplaceSetup_deleteProductTagsMutation } from '@/queries/__generated__/organizationMarketplaceSetup_deleteProductTagsMutation.graphql';
import type { organizationMarketplaceSetup_locationTags_query$key } from '@/queries/__generated__/organizationMarketplaceSetup_locationTags_query.graphql';
import type { organizationMarketplaceSetup_locationTags_refetchableFragment } from '@/queries/__generated__/organizationMarketplaceSetup_locationTags_refetchableFragment.graphql';
import type { organizationMarketplaceSetup_productTags_query$key } from '@/queries/__generated__/organizationMarketplaceSetup_productTags_query.graphql';
import type { organizationMarketplaceSetup_productTags_refetchableFragment } from '@/queries/__generated__/organizationMarketplaceSetup_productTags_refetchableFragment.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { graphql, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import OrganizationMarketplaceSetupLeftSideNavigationMenuContent from './organization-marketplace-setup-left-side-navigation-menu-content';

type Props = {
  rootDataProductTagsRelay: organizationMarketplaceSetup_productTags_query$key;
  rootDataLocationTagsRelay: organizationMarketplaceSetup_locationTags_query$key;
  onReloadRequired: () => void;
  organizationId: string;
};

type ProductTagRowType = {
  id: string;
  name: string;
  description: string | null | undefined;
};

type LocationTagRowType = {
  id: string;
  name: string;
  description: string | null | undefined;
};

const OrganizationMarketplaceSetup = ({ rootDataProductTagsRelay, rootDataLocationTagsRelay, onReloadRequired, organizationId }: Props) => {
  const [rootDataProductTags, refetchProductTags] = useRefetchableFragment<
    organizationMarketplaceSetup_productTags_refetchableFragment,
    organizationMarketplaceSetup_productTags_query$key
  >(
    graphql`
      fragment organizationMarketplaceSetup_productTags_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationMarketplaceSetup_productTags_refetchableFragment") {
        productTags(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $productTagNameSearchText }
          orderBy: [{ direction: Ascending, field: Name }]
        ) @connection(key: "organizationMarketplaceSetup_productTags") {
          __id
          totalCount
          edges {
            node {
              id
              name
              description
              color
            }
          }
        }
      }
    `,
    rootDataProductTagsRelay,
  );

  const [rootDataLocationTags, refetchLocationTags] = useRefetchableFragment<
    organizationMarketplaceSetup_locationTags_refetchableFragment,
    organizationMarketplaceSetup_locationTags_query$key
  >(
    graphql`
      fragment organizationMarketplaceSetup_locationTags_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationMarketplaceSetup_locationTags_refetchableFragment") {
        locationTags(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $locationTagNameSearchText }
          orderBy: [{ direction: Ascending, field: Name }]
        ) @connection(key: "organizationMarketplaceSetup_locationTags") {
          __id
          totalCount
          edges {
            node {
              id
              name
              description
              color
            }
          }
        }
      }
    `,
    rootDataLocationTagsRelay,
  );

  const [commitDeleteProductTags] = useMutation<organizationMarketplaceSetup_deleteProductTagsMutation>(graphql`
    mutation organizationMarketplaceSetup_deleteProductTagsMutation($connectionIds: [ID!]!, $input: DeleteProductTagsInput!) {
      deleteProductTags(input: $input) {
        organizationTags {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitDeleteLocationTags] = useMutation<organizationMarketplaceSetup_deleteLocationTagsMutation>(graphql`
    mutation organizationMarketplaceSetup_deleteLocationTagsMutation($connectionIds: [ID!]!, $input: DeleteLocationTagsInput!) {
      deleteLocationTags(input: $input) {
        organizationTags {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [, startTransition] = useTransition();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const router = useRouter();
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});

  const [productTagNameSearchText, setProductTagNameSearchText] = useState<string>('');
  const [seledctedProductTags, setSeledctedProductTags] = useState<GridRowSelectionModel>([]);
  const [selectedProductTagId, setSelectedProductTagId] = useState<null | string>(null);
  const [productTagMoreActionsAnchorEl, setProductTagMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const productTagMoreActionsMenuOpen = Boolean(productTagMoreActionsAnchorEl);
  const [isEditProductTagDialogOpen, setIsEditProductTagDialogOpen] = useState(false);
  const productTags = useMemo(() => rootDataProductTags.productTags.edges.map(({ node }) => node), [rootDataProductTags.productTags]);
  const productTagsConnectionIds = useMemo(() => [rootDataProductTags.productTags.__id], [rootDataProductTags.productTags]);
  const productTagMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditProductTag],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteProductTag],
  ];

  const handleRefetchProductTags = useCallback(
    (productTagNameSearchText: string) => {
      startTransition(() => {
        refetchProductTags(
          {
            productTagNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchProductTags],
  );

  const [locationTagNameSearchText, setLocationTagNameSearchText] = useState<string>('');
  const [seledctedLocationTags, setSeledctedLocationTags] = useState<GridRowSelectionModel>([]);
  const [selectedLocationTagId, setSelectedLocationTagId] = useState<null | string>(null);
  const [locationTagMoreActionsAnchorEl, setLocationTagMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const locationTagMoreActionsMenuOpen = Boolean(locationTagMoreActionsAnchorEl);
  const [isEditLocationTagDialogOpen, setIsEditLocationTagDialogOpen] = useState(false);
  const locationTags = useMemo(() => rootDataLocationTags.locationTags.edges.map(({ node }) => node), [rootDataLocationTags.locationTags]);
  const locationTagsConnectionIds = useMemo(() => [rootDataLocationTags.locationTags.__id], [rootDataLocationTags.locationTags]);
  const locationTagMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditLocationTag],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteLocationTag],
  ];

  const handleRefetchLocationTags = useCallback(
    (locationTagNameSearchText: string) => {
      startTransition(() => {
        refetchLocationTags(
          {
            locationTagNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchLocationTags],
  );

  useEffect(() => {
    if (!section || section === 'setup') {
      return;
    }

    const element = sectionRefs.current[section];
    if (!element) {
      return;
    }

    const appBarHeight = document.querySelector('.app-bar')?.clientHeight || 0;
    const elementTop = element.getBoundingClientRect().top + window.scrollY;
    window.scrollTo({
      top: elementTop - appBarHeight,
      behavior: 'smooth',
    });
  }, [section]);

  const handleProductTagsSearchTextChange = (str: string) => {
    setProductTagNameSearchText(str);

    handleRefetchProductTags(str);
  };

  const handleSelectedProductTagsChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedProductTags(newRowSelectionModel);
  };

  const handleProductTagMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setProductTagMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditProductTag:
        setIsEditProductTagDialogOpen(true);
        break;

      case MoreActionsMenuOptionType.DeleteProductTag:
        handleRemoveProductTagClick();
        break;
    }
  };

  const handleEditProductTagClick = () => {
    setIsEditProductTagDialogOpen(false);
  };

  const handleEditProductTagCancel = () => {
    setIsEditProductTagDialogOpen(false);
  };

  const handleRemoveProductTagsClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing product tags ..." />, infoNotificationOptions);

    commitDeleteProductTags({
      variables: {
        connectionIds: productTagsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: seledctedProductTags.map((id) => id as string),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove product tags. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product tags removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove product tags. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveProductTagClick = () => {
    if (!selectedProductTagId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing product tag ..." />, infoNotificationOptions);

    commitDeleteProductTags({
      variables: {
        connectionIds: productTagsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: [selectedProductTagId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove product tag. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product tag removed.`} />,
        });

        setSelectedProductTagId(null);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove product tag. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleLocationTagsSearchTextChange = (str: string) => {
    setLocationTagNameSearchText(str);

    handleRefetchLocationTags(str);
  };

  const handleSelectedLocationTagsChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedLocationTags(newRowSelectionModel);
  };

  const handleLocationTagMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setLocationTagMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditLocationTag:
        setIsEditLocationTagDialogOpen(true);
        break;

      case MoreActionsMenuOptionType.DeleteLocationTag:
        handleRemoveLocationTagClick();
        break;
    }
  };

  const handleEditLocationTagClick = () => {
    setIsEditLocationTagDialogOpen(false);
  };

  const handleEditLocationTagCancel = () => {
    setIsEditLocationTagDialogOpen(false);
  };

  const handleRemoveLocationTagsClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing location tags ..." />, infoNotificationOptions);

    commitDeleteLocationTags({
      variables: {
        connectionIds: locationTagsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: seledctedLocationTags.map((id) => id as string),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove location tags. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location tags removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove location tags. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveLocationTagClick = () => {
    if (!selectedLocationTagId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing location tag ..." />, infoNotificationOptions);

    commitDeleteLocationTags({
      variables: {
        connectionIds: locationTagsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: [selectedLocationTagId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove location tag. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location tag removed.`} />,
        });

        setSelectedLocationTagId(null);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove location tag. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleCloseClick = () => {
    router.push(getOrganizationBaseLink(organizationId));
  };

  const productTagRows: ProductTagRowType[] = productTags.map((productTag) => ({
    id: productTag.id,
    name: productTag.name,
    description: productTag.description,
  }));

  const productTagColumns: GridColDef<(typeof productTagRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => {
        const productTag = productTags.find((productTag) => productTag.id === (params.id as string));
        if (!productTag) {
          return <></>;
        }

        return <ProductTag productTag={productTag} showFullName />;
      },
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'description',
      headerName: 'Description',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
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
              setSelectedProductTagId(params.id as string);
              setProductTagMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  const locationTagRows: LocationTagRowType[] = locationTags.map((locationTag) => ({
    id: locationTag.id,
    name: locationTag.name,
    description: locationTag.description,
  }));

  const locationTagColumns: GridColDef<(typeof locationTagRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => {
        const locationTag = locationTags.find((locationTag) => locationTag.id === (params.id as string));
        if (!locationTag) {
          return <></>;
        }

        return <LocationTag locationTag={locationTag} showFullName />;
      },
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'description',
      headerName: 'Description',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
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
              setSelectedLocationTagId(params.id as string);
              setLocationTagMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationMarketplaceSetupLeftSideNavigationMenuContent organizationId={organizationId} hideIcons />
        <Box sx={{ marginLeft: secondDrawerExpandedDrawerWidthPx, flexGrow: 1 }}>
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Marketplace Information">
            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['product-tags-setup'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Product Tags" />
                  <BodyIconTypography label="Edit your organization product tags details" />
                </Grid>

                <Grid>
                  <AddOrganizationProductTagButton organizationId={organizationId} connectionIds={productTagsConnectionIds} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search size="small" placeholder="Search for product tags" defaultValue={productTagNameSearchText} onChange={handleProductTagsSearchTextChange} />
            </GridContainer>

            {seledctedProductTags.length > 0 && (
              <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
                <Box
                  sx={{
                    backgroundColor: 'white',
                    padding: defaultGridActionPadding,
                    border: 1,
                    borderColor: (theme) => theme.palette.divider,
                    borderRadius: 2,
                    flexGrow: 1,
                  }}
                >
                  <StackRow sx={{ alignItems: 'center' }}>
                    <SmallIconTypography label={`${seledctedProductTags.length} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveProductTagsClick} sx={{ textTransform: 'none' }}>
                      Remove Tag
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedProductTags}
                onRowSelectionModelChange={handleSelectedProductTagsChanged}
                rows={productTagRows}
                columns={productTagColumns}
                hideFooterPagination={productTagRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: productTagRows.length,
                    paginationModel: {
                      pageSize: 10,
                    },
                  },
                }}
                pageSizeOptions={[10]}
                ignoreDiacritics
                disableRowSelectionOnClick
                getRowHeight={() => 'auto'}
                rowSpacingType="margin"
                getRowSpacing={() => ({ top: 3, bottom: 3 })}
                sx={defaultGridStyle}
                localeText={{ noRowsLabel: 'No product tag found' }}
              />
            </StackRow>

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['location-tags-setup'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Location Tags" />
                  <BodyIconTypography label="Edit your organization location tags details" />
                </Grid>

                <Grid>
                  <AddOrganizationLocationTagButton organizationId={organizationId} connectionIds={locationTagsConnectionIds} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search size="small" placeholder="Search for location tags" defaultValue={locationTagNameSearchText} onChange={handleLocationTagsSearchTextChange} />
            </GridContainer>

            {seledctedLocationTags.length > 0 && (
              <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
                <Box
                  sx={{
                    backgroundColor: 'white',
                    padding: defaultGridActionPadding,
                    border: 1,
                    borderColor: (theme) => theme.palette.divider,
                    borderRadius: 2,
                    flexGrow: 1,
                  }}
                >
                  <StackRow sx={{ alignItems: 'center' }}>
                    <SmallIconTypography label={`${seledctedLocationTags.length} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveLocationTagsClick} sx={{ textTransform: 'none' }}>
                      Remove Tag
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedLocationTags}
                onRowSelectionModelChange={handleSelectedLocationTagsChanged}
                rows={locationTagRows}
                columns={locationTagColumns}
                hideFooterPagination={locationTagRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: locationTagRows.length,
                    paginationModel: {
                      pageSize: 10,
                    },
                  },
                }}
                pageSizeOptions={[10]}
                ignoreDiacritics
                disableRowSelectionOnClick
                getRowHeight={() => 'auto'}
                rowSpacingType="margin"
                getRowSpacing={() => ({ top: 3, bottom: 3 })}
                sx={defaultGridStyle}
                localeText={{ noRowsLabel: 'No location tag found' }}
              />
            </StackRow>
          </AppBarWithStackColumn>
        </Box>
      </Box>

      <MoreActionsMenu
        anchorEl={productTagMoreActionsAnchorEl}
        open={productTagMoreActionsMenuOpen}
        onMenuItemClick={handleProductTagMoreActionsMenuItemClick}
        options={productTagMoreActionsOption}
      />

      <MoreActionsMenu
        anchorEl={locationTagMoreActionsAnchorEl}
        open={locationTagMoreActionsMenuOpen}
        onMenuItemClick={handleLocationTagMoreActionsMenuItemClick}
        options={locationTagMoreActionsOption}
      />

      {selectedProductTagId && (
        <EditOrganizationProductTagDialog
          onReloadRequired={onReloadRequired}
          productTagId={selectedProductTagId}
          isDialogOpen={isEditProductTagDialogOpen}
          onAddClicked={handleEditProductTagClick}
          onCancel={handleEditProductTagCancel}
        />
      )}

      {selectedLocationTagId && (
        <EditOrganizationLocationTagDialog
          onReloadRequired={onReloadRequired}
          locationTagId={selectedLocationTagId}
          isDialogOpen={isEditLocationTagDialogOpen}
          onAddClicked={handleEditLocationTagClick}
          onCancel={handleEditLocationTagCancel}
        />
      )}
    </>
  );
};

export default memo(OrganizationMarketplaceSetup);
