import { NewIcon } from '@/components/icons';
import { getOrganizationLocationAddResourceBaseLink, getOrganizationLocationBulkAddResourcesBaseLink, getOrganizationLocationResourceBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { CustomTagSelector } from '@/components/organization/customTagSelector';
import OrganizationLocationResourceManagementList from '@/components/organization/organizationLocation/organization-location-resource-management-list';
import { ZoneSelector } from '@/components/organization/zoneSelector';
import { Search } from '@/components/search';
import type { organizationLocationManageResourcesSectionQuery } from '@/queries/__generated__/organizationLocationManageResourcesSectionQuery.graphql';
import type { organizationLocationManageResourcesSection_activateResourcesMutation } from '@/queries/__generated__/organizationLocationManageResourcesSection_activateResourcesMutation.graphql';
import type { organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation } from '@/queries/__generated__/organizationLocationManageResourcesSection_addCustomerPreferredResourceMutation.graphql';
import type { organizationLocationManageResourcesSection_deactivateResourcesMutation } from '@/queries/__generated__/organizationLocationManageResourcesSection_deactivateResourcesMutation.graphql';
import type { organizationLocationManageResourcesSection_deleteResourcesMutation } from '@/queries/__generated__/organizationLocationManageResourcesSection_deleteResourcesMutation.graphql';
import type { organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation } from '@/queries/__generated__/organizationLocationManageResourcesSection_removeCustomerPreferredResourceMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { getRelayErrorMessage, PaletteModeContext, useIntegratedPlatform } from '@skedular/shared';
import { defaultPadding, LeadIconTypography, PushToRight, SettingsSectionCard, StackColumn, StackRow } from '@skedular/ui';
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

type ResourceManagementItem = {
  id: string;
  resource: ResourceDetails;
  resourceType: ResourceTypeDetails;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
  productTags: ProductTagDetails[];
  isActive: boolean;
  isPreferred: boolean;
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

const OrganizationLocationManageResourcesSection = ({ organizationCustomDomain, locationId }: Props) => {
  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [resourceNameSearchText, setResourceNameSearchText] = useState('');
  const [resourceCustomTagIds, setResourceCustomTagIds] = useState<string[]>([]);
  const [resourceZoneIds, setResourceZoneIds] = useState<string[]>([]);
  const [selectedResourceId, setSelectedResourceId] = useState<null | string>(null);
  const [selectedResourceIds, setSelectedResourceIds] = useState<string[]>([]);
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

  const resourceItems: ResourceManagementItem[] = resources.map((resource) => ({
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
    isActive: !resource.inactive,
    isPreferred: preferredResources.includes(resource.id),
    capacity: resource.capacity,
  }));
  const resourceDetails = useMemo(() => resources.find((item) => item.id === selectedResourceId), [resources, selectedResourceId]);
  const selectedResourceItem = useMemo(() => resourceItems.find((item) => item.id === selectedResourceId), [resourceItems, selectedResourceId]);
  const resourceMoreActionsOption: MoreActionsMenuItemType[] = useMemo(() => {
    const options: MoreActionsMenuItemType[] = [
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditResource],
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateResource],
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateResource],
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteResource],
    ];

    if (selectedResourceItem) {
      options.splice(
        1,
        0,
        selectedResourceItem.isPreferred
          ? moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveAsPreferredResource]
          : moreActionsMenuAllOptions[MoreActionsMenuOptionType.SetAsPreferredResource],
      );
    }

    return options;
  }, [selectedResourceItem]);

  const handleResourceNameSearchTextChange = (value: string) => {
    setResourceNameSearchText(value);
  };

  const handleResourceCustomTagChanged = (id?: string) => {
    setResourceCustomTagIds(id ? [id] : []);
  };

  const handleResourceZoneTypeChanged = (id?: string) => {
    setResourceZoneIds(id ? [id] : []);
  };

  const handleDeactivateResourcesClick = (ids: string[]) => {
    commitDeactivateResources({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't deactivate those resources. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedResourceIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't deactivate those resources. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleActivateResourcesClick = (ids: string[]) => {
    commitActivateResources({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't activate those resources. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedResourceIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't activate those resources. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleDeleteResourcesClick = (ids: string[]) => {
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
          themedToast(<NotificationContent content={`We couldn't remove those resources. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedResourceIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove those resources. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleSetAsPreferredResourceClicked = (id: string) => {
    const selectedResource = resources.find((item) => item.id === id);
    if (!selectedResource) {
      return;
    }
    commitAddCustomerPreferredResource({
      variables: {
        input: {
          clientMutationId: uuid(),
          resourceId: selectedResource.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`We couldn't make '${selectedResource.name}' your preferred resource. ${getRelayErrorMessage(errors)}`} />,
            errorNotificationOptions,
          );

          return;
        }

        setPreferredResources((current) => current.concat(selectedResource.id));
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't make '${selectedResource.name}' your preferred resource. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleRemoveAsPreferredResourceClicked = (id: string) => {
    const selectedResource = resources.find((item) => item.id === id);
    if (!selectedResource) {
      return;
    }
    commitRemoveCustomerPreferredResource({
      variables: {
        input: {
          clientMutationId: uuid(),
          resourceId: selectedResource.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`We couldn't remove '${selectedResource.name}' from your preferred resources. ${getRelayErrorMessage(errors)}`} />,
            errorNotificationOptions,
          );

          return;
        }

        setPreferredResources((current) => current.filter((item) => item !== selectedResource.id));
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove '${selectedResource.name}' from your preferred resources. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleResourceMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setResourceMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditResource:
        if (resourceDetails) {
          router.push(getOrganizationLocationResourceBaseLink(integratedPlatform, organizationCustomDomain, locationId, resourceDetails.id));
        }
        break;
      case MoreActionsMenuOptionType.DeactivateResource:
        if (resourceDetails) {
          handleDeactivateResourcesClick([resourceDetails.id]);
        }
        break;
      case MoreActionsMenuOptionType.SetAsPreferredResource:
        if (resourceDetails) {
          handleSetAsPreferredResourceClicked(resourceDetails.id);
        }
        break;
      case MoreActionsMenuOptionType.RemoveAsPreferredResource:
        if (resourceDetails) {
          handleRemoveAsPreferredResourceClicked(resourceDetails.id);
        }
        break;
      case MoreActionsMenuOptionType.ActivateResource:
        if (resourceDetails) {
          handleActivateResourcesClick([resourceDetails.id]);
        }
        break;
      case MoreActionsMenuOptionType.DeleteResource:
        if (resourceDetails) {
          handleDeleteResourcesClick([resourceDetails.id]);
        }
        break;
    }
  };

  const handleSelectedResourceToggle = (resourceId: string) => {
    setSelectedResourceIds((current) => (current.includes(resourceId) ? current.filter((id) => id !== resourceId) : current.concat(resourceId)));
  };

  const handleOpenResource = (resourceId: string) => {
    router.push(getOrganizationLocationResourceBaseLink(integratedPlatform, organizationCustomDomain, locationId, resourceId));
  };

  return (
    <>
      <Box sx={{ pb: defaultPadding }}>
        <SettingsSectionCard
          title="Manage Resources"
          description="Search, filter, and operate on the resources assigned to this location."
          actions={
            <StackRow sx={{ gap: 1 }}>
              <Button
                variant="text"
                onClick={() => router.push(getOrganizationLocationAddResourceBaseLink(integratedPlatform, organizationCustomDomain, locationId))}
                sx={{ textTransform: 'none' }}
              >
                <LeadIconTypography label="Add Resource" endElement={<NewIcon fontSize="large" />} />
              </Button>
              <Button
                variant="text"
                onClick={() => router.push(getOrganizationLocationBulkAddResourcesBaseLink(integratedPlatform, organizationCustomDomain, locationId))}
                sx={{ textTransform: 'none' }}
              >
                <LeadIconTypography label="Bulk Add Resources" endElement={<NewIcon fontSize="large" />} />
              </Button>
            </StackRow>
          }
        >
          <StackColumn spacing={2}>
            <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
              <ZoneSelector rootDataRelay={rootData} onChange={handleResourceZoneTypeChanged} />
              <CustomTagSelector rootDataRelay={rootData} onChange={handleResourceCustomTagChanged} />
              <PushToRight />
              <Search size="small" placeholder="Search for resources" defaultValue={resourceNameSearchText} onChange={handleResourceNameSearchTextChange} />
            </StackRow>

            <OrganizationLocationResourceManagementList
              items={resourceItems.map((resourceItem) => ({
                id: resourceItem.id,
                resourceName: resourceItem.resource.name ?? 'Unnamed resource',
                resourceType: resourceItem.resourceType,
                customTags: resourceItem.customTags,
                zones: resourceItem.zones,
                productTags: resourceItem.productTags,
                isActive: resourceItem.isActive,
                isPreferred: resourceItem.isPreferred,
                capacity: resourceItem.capacity,
              }))}
              selectedIds={selectedResourceIds}
              onToggleSelected={handleSelectedResourceToggle}
              onOpenResource={handleOpenResource}
              onOpenMoreActions={(resourceId, target) => {
                setSelectedResourceId(resourceId);
                setResourceMoreActionsAnchorEl(target);
              }}
              onDeactivateSelected={(ids) => handleDeactivateResourcesClick(ids)}
              onActivateSelected={(ids) => handleActivateResourcesClick(ids)}
              onDeleteSelected={(ids) => handleDeleteResourcesClick(ids)}
            />
          </StackColumn>
        </SettingsSectionCard>
      </Box>

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
