import { BodyIconTypography, PushToRight, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { DeleteIcon, EllipseMenuIcon, NotPreferredIcon, PreferredIcon } from '@/components/icons';
import { getOrganizationLocationResourceBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { CustomTagSelector } from '@/components/organization/customTagSelector';
import { ZoneSelector } from '@/components/organization/zoneSelector';
import { ProductTags } from '@/components/productTag';
import { Resource } from '@/components/resource';
import { AddResourceButton } from '@/components/resource/addResource';
import { ResourceType } from '@/components/resourceType';
import { Search } from '@/components/search';
import { Zones } from '@/components/zone';
import { defaultGridRowSelectionModelValue } from '@/libs/mui';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultGridActionPadding, defaultGridStyle, defaultPadding, emerald, flame } from '@/libs/theme';
import { getRelayErrorMessage } from '@/libs/utils';
import type { organizationLocationManageResourcesSectionQuery } from '@/queries/__generated__/organizationLocationManageResourcesSectionQuery.graphql';
import type { organizationLocationManageResourcesSection_activateResourcesMutation } from '@/queries/__generated__/organizationLocationManageResourcesSection_activateResourcesMutation.graphql';
import type { organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation } from '@/queries/__generated__/organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation.graphql';
import type { organizationLocationManageResourcesSection_deactivateResourcesMutation } from '@/queries/__generated__/organizationLocationManageResourcesSection_deactivateResourcesMutation.graphql';
import type { organizationLocationManageResourcesSection_deleteResourcesMutation } from '@/queries/__generated__/organizationLocationManageResourcesSection_deleteResourcesMutation.graphql';
import type { organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation } from '@/queries/__generated__/organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { useRouter } from 'next/navigation';
import { memo, useContext, useMemo, useState } from 'react';
import { graphql, useLazyLoadQuery, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  locationId: string;
};

type ResourceTypeDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type ResourceDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type CustomTagDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type ZoneDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type ProductTagDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type ResourceRowType = {
  id: string;
  resource: ResourceDetails;
  resourceType: ResourceTypeDetails;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
  productTags: ProductTagDetails[];
  status: boolean;
  preferred: boolean;
  capacity: number;
};

const ResourcesSectionQuery = graphql`
  query organizationLocationManageResourcesSectionQuery(
    $organizationCustomDomain: String!
    $locationId: String!
    $resourceNameSearchText: String
    $resourceZoneIds: [String!]
    $resourceCustomTagIds: [String!]
    $zonesSortingValues: [OrganizationTagOrderInput!]
    $customTagsSortingValues: [OrganizationTagOrderInput!]
    $resourcesSortingValues: [ResourceOrderInput!]
  ) {
    me {
      id
      preferredResources {
        id
      }
    }
    location(id: $locationId) {
      resources(where: { nameContains: $resourceNameSearchText, customTagIds: $resourceCustomTagIds, zoneIds: $resourceZoneIds }, orderBy: $resourcesSortingValues) {
        __id
        edges {
          node {
            id
            name
            inactive
            requireBookingApproval
            color
            capacity
            customTags {
              id
              name
              color
            }
            zones {
              id
              name
              color
            }
            productTags {
              id
              name
              color
            }
            resourceType {
              id
              name
              color
            }
          }
        }
      }
    }
    ...customTagSelector_allCustomTags_query
    ...zoneSelector_allZones_query
  }
`;

const OrganizationLocationManageResourcesSection = ({ onReloadRequired, organizationCustomDomain, locationId }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [resourceNameSearchText, setResourceNameSearchText] = useState('');
  const [resourceCustomTagIds, setResourceCustomTagIds] = useState<string[]>([]);
  const [resourceZoneIds, setResourceZoneIds] = useState<string[]>([]);
  const [selectedResourceId, setSelectedResourceId] = useState<null | string>(null);
  const [selectedResources, setSelectedResources] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
  const [resourceMoreActionsAnchorEl, setResourceMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const resourceMoreActionsMenuOpen = Boolean(resourceMoreActionsAnchorEl);

  const rootData = useLazyLoadQuery<organizationLocationManageResourcesSectionQuery>(
    ResourcesSectionQuery,
    {
      organizationCustomDomain,
      locationId,
      resourceNameSearchText,
      resourceCustomTagIds,
      resourceZoneIds,
      zonesSortingValues: [
        {
          direction: 'ASCENDING',
          field: 'NAME',
        },
      ],
      customTagsSortingValues: [
        {
          direction: 'ASCENDING',
          field: 'NAME',
        },
      ],
      resourcesSortingValues: [
        {
          direction: 'ASCENDING',
          field: 'NAME',
        },
      ],
    },
    {
      fetchPolicy: 'store-and-network',
    },
  );

  const [preferredResources, setPreferredResources] = useState(rootData.me?.preferredResources.map(({ id }) => id) ?? []);
  const [commitDeleteResources] = useMutation<organizationLocationManageResourcesSection_deleteResourcesMutation>(graphql`
    mutation organizationLocationManageResourcesSection_deleteResourcesMutation($connectionIds: [ID!]!, $input: DeleteResourcesInput!) {
      deleteResources(input: $input) {
        resources {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);
  const [commitActivateResources] = useMutation<organizationLocationManageResourcesSection_activateResourcesMutation>(graphql`
    mutation organizationLocationManageResourcesSection_activateResourcesMutation($input: ActivateResourcesInput!) {
      activateResources(input: $input) {
        resources {
          id
          inactive
        }
      }
    }
  `);
  const [commitDeactivateResources] = useMutation<organizationLocationManageResourcesSection_deactivateResourcesMutation>(graphql`
    mutation organizationLocationManageResourcesSection_deactivateResourcesMutation($input: DeactivateResourcesInput!) {
      deactivateResources(input: $input) {
        resources {
          id
          inactive
        }
      }
    }
  `);
  const [commitAddCustomerPreferredResource] = useMutation<organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation>(graphql`
    mutation organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation($input: AddCustomerPreferredResourceInput!) {
      addCustomerPreferredResource(input: $input) {
        customer {
          id
          preferredResources {
            id
          }
        }
      }
    }
  `);
  const [commitRemoveCustomerPreferredResource] = useMutation<organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation>(graphql`
    mutation organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation($input: RemoveCustomerPreferredResourceInput!) {
      removeCustomerPreferredResource(input: $input) {
        customer {
          id
          preferredResources {
            id
          }
        }
      }
    }
  `);

  const resources = useMemo(() => (rootData.location ? rootData.location.resources.edges.map(({ node }) => node) : []), [rootData.location]);
  const resourcesConnectionIds = useMemo(() => (rootData.location ? [rootData.location.resources.__id] : []), [rootData.location]);
  const resourceDetails = useMemo(() => resources.find((item) => item.id === selectedResourceId), [resources, selectedResourceId]);
  const resourceMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditResource],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateResource],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateResource],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteResource],
  ];

  const resourceRows: ResourceRowType[] = resources.map((resource) => ({
    id: resource.id,
    resource: {
      id: resource.id,
      name: resource.name,
      color: resource.color,
    },
    resourceType: {
      id: resource.resourceType.id,
      name: resource.resourceType.name,
      color: resource.resourceType.color,
    },
    customTags: resource.customTags.map((item) => ({ id: item.id, name: item.name, color: item.color })),
    zones: resource.zones.map((item) => ({ id: item.id, name: item.name, color: item.color })),
    productTags: resource.productTags.map((item) => ({ id: item.id, name: item.name, color: item.color })),
    status: !resource.inactive,
    preferred: preferredResources.includes(resource.id),
    capacity: resource.capacity,
  }));

  const selectedResourceIds = useMemo(() => Array.from(selectedResources.ids).map((id) => id as string), [selectedResources.ids]);

  const handleSelectedResourcesChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSelectedResources(newRowSelectionModel);
  };

  const handleResourceNameSearchTextChange = (value: string) => {
    setResourceNameSearchText(value);
  };

  const handleResourceCustomTagChanged = (id?: string) => {
    setResourceCustomTagIds(id ? [id] : []);
  };

  const handleResourceZoneTypeChanged = (id?: string) => {
    setResourceZoneIds(id ? [id] : []);
  };

  const handleDeactivateResourcesClick = (ids: string[], successMessage: string, pendingMessage: string) => {
    const toastId = themedToast(<NotificationContent content={pendingMessage} />, infoNotificationOptions);

    commitDeactivateResources({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate resources. Error: ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={successMessage} />,
        });
        setSelectedResources(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate resources. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateResourcesClick = (ids: string[], successMessage: string, pendingMessage: string) => {
    const toastId = themedToast(<NotificationContent content={pendingMessage} />, infoNotificationOptions);

    commitActivateResources({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate resources. Error: ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={successMessage} />,
        });
        setSelectedResources(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate resources. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleDeleteResourcesClick = (ids: string[], successMessage: string, pendingMessage: string) => {
    const toastId = themedToast(<NotificationContent content={pendingMessage} />, infoNotificationOptions);

    commitDeleteResources({
      variables: {
        connectionIds: resourcesConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove resources. Error: ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={successMessage} />,
        });
        setSelectedResources(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove resources. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleSetAsPreferredResourceClicked = (id: string) => {
    const selectedResource = resources.find((item) => item.id === id);
    if (!selectedResource) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting resource '${selectedResource.name}' as your preferred resource...`} />, infoNotificationOptions);
    commitAddCustomerPreferredResource({
      variables: {
        input: {
          clientMutationId: uuid(),
          resourceId: selectedResource.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set resource '${selectedResource.name}' as your preferred resource. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Resource '${selectedResource.name}' has been set as the preferred resource.`} />,
        });
        setPreferredResources((current) => current.concat(selectedResource.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set resource '${selectedResource.name}' as your preferred resource. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredResourceClicked = (id: string) => {
    const selectedResource = resources.find((item) => item.id === id);
    if (!selectedResource) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing resource '${selectedResource.name}' as your preferred resource...`} />, infoNotificationOptions);
    commitRemoveCustomerPreferredResource({
      variables: {
        input: {
          clientMutationId: uuid(),
          resourceId: selectedResource.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent content={`Failed to remove the resource '${selectedResource.name}' as your preferred resource. Error: ${getRelayErrorMessage(errors)}.`} />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Resource '${selectedResource.name}' has been removed as your preferred resource.`} />,
        });
        setPreferredResources((current) => current.filter((item) => item !== selectedResource.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the resource '${selectedResource.name}' as your preferred resource. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleResourceMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setResourceMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditResource:
        if (resourceDetails) {
          router.push(getOrganizationLocationResourceBaseLink(integratedPlatrform, organizationCustomDomain, locationId, resourceDetails.id));
        }
        break;
      case MoreActionsMenuOptionType.DeactivateResource:
        if (resourceDetails) {
          handleDeactivateResourcesClick([resourceDetails.id], 'Resource deactivated.', 'Deactivating resource...');
        }
        break;
      case MoreActionsMenuOptionType.ActivateResource:
        if (resourceDetails) {
          handleActivateResourcesClick([resourceDetails.id], 'Resource activated.', 'Activating resource...');
        }
        break;
      case MoreActionsMenuOptionType.DeleteResource:
        if (resourceDetails) {
          handleDeleteResourcesClick([resourceDetails.id], 'Resource removed.', 'Removing resource...');
        }
        break;
    }
  };

  const resourceColumns: GridColDef<(typeof resourceRows)[number]>[] = [
    {
      field: 'resource',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => <Resource resource={params.value} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'resourceType',
      headerName: 'Type',
      editable: false,
      renderCell: (params) => <ResourceType resourceType={params.value} />,
      display: 'flex',
      minWidth: 50,
    },
    {
      field: 'capacity',
      headerName: 'Capacity',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 100,
    },
    {
      field: 'customTags',
      headerName: 'Tags',
      editable: false,
      renderCell: (params) => <CustomTags customTags={params.value} hideIcon />,
      display: 'flex',
      minWidth: 250,
    },
    {
      field: 'zones',
      headerName: 'Zones',
      editable: false,
      renderCell: (params) => <Zones zones={params.value} hideIcon />,
      display: 'flex',
      minWidth: 250,
    },
    {
      field: 'productTags',
      headerName: 'Product Tags',
      editable: false,
      renderCell: (params) => <ProductTags productTags={params.value} hideIcon />,
      display: 'flex',
      minWidth: 250,
    },
    {
      field: 'status',
      headerName: 'Status',
      editable: false,
      renderCell: (params) => (
        <StackRow>
          {params.value ? (
            <StackRow sx={{ justifyContent: 'space-between', width: 76 }}>
              <SmallIconTypography label="Active" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: emerald }} />
            </StackRow>
          ) : (
            <StackRow sx={{ justifyContent: 'space-between', width: 76 }}>
              <SmallIconTypography label="Inactive" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: flame }} />
            </StackRow>
          )}
        </StackRow>
      ),
      display: 'flex',
    },
    {
      field: 'preferred',
      headerName: 'Preferred?',
      editable: false,
      renderCell: (params) => {
        const resourceId = params.id as string;
        return params.value ? (
          <IconButton onClick={() => handleRemoveAsPreferredResourceClicked(resourceId)}>
            <PreferredIcon />
          </IconButton>
        ) : (
          <IconButton onClick={() => handleSetAsPreferredResourceClicked(resourceId)}>
            <NotPreferredIcon />
          </IconButton>
        );
      },
      display: 'flex',
    },
    {
      field: 'More Actions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedResourceId(params.id as string);
              setResourceMoreActionsAnchorEl(event.currentTarget);
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
      <StackColumn sx={{ padding: defaultPadding }}>
        <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
          <Grid>
            <BodyIconTypography label="Manage your location resources details" />
          </Grid>

          <Grid>
            <AddResourceButton
              onReloadRequired={onReloadRequired}
              organizationCustomDomain={organizationCustomDomain}
              locationId={locationId}
              connectionIds={resourcesConnectionIds}
            />
          </Grid>
        </StackRow>
        <Divider />
      </StackColumn>

      <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding, gap: 1, flexWrap: 'wrap' }}>
        <ZoneSelector rootDataRelay={rootData} onChange={handleResourceZoneTypeChanged} />
        <CustomTagSelector rootDataRelay={rootData} onChange={handleResourceCustomTagChanged} />
        <PushToRight />
        <Search size="small" placeholder="Search for resources" defaultValue={resourceNameSearchText} onChange={handleResourceNameSearchTextChange} />
      </StackRow>

      {selectedResourceIds.length > 0 && (
        <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: 1 }}>
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
              <SmallIconTypography label={`${selectedResourceIds.length} records selected`} />
              <PushToRight />
              <Button
                size="medium"
                variant="contained"
                color="secondary"
                onClick={() => handleDeactivateResourcesClick(selectedResourceIds, 'Resources deactivated.', 'Deactivating resources...')}
              >
                Deactivate Resource
              </Button>
              <Button
                size="medium"
                variant="contained"
                color="secondary"
                onClick={() => handleActivateResourcesClick(selectedResourceIds, 'Resources activated.', 'Activating resources...')}
              >
                Activate Resource
              </Button>
              <Button
                size="medium"
                variant="contained"
                color="warning"
                startIcon={<DeleteIcon />}
                onClick={() => handleDeleteResourcesClick(selectedResourceIds, 'Resources removed.', 'Removing resources...')}
                sx={{ textTransform: 'none' }}
              >
                Remove Resource
              </Button>
            </StackRow>
          </Box>
        </StackRow>
      )}

      <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: 1 }}>
        <DataGrid
          checkboxSelection
          rowSelectionModel={selectedResources}
          onRowSelectionModelChange={handleSelectedResourcesChanged}
          rows={resourceRows}
          columns={resourceColumns}
          hideFooterPagination={resourceRows.length <= 10}
          initialState={{
            pagination: {
              rowCount: resourceRows.length,
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
          localeText={{ noRowsLabel: 'No resource found' }}
        />
      </StackRow>

      <MoreActionsMenu
        anchorEl={resourceMoreActionsAnchorEl}
        open={resourceMoreActionsMenuOpen}
        onMenuItemClick={handleResourceMoreActionsMenuItemClick}
        options={resourceMoreActionsOption}
      />
    </>
  );
};

export default memo(OrganizationLocationManageResourcesSection);
