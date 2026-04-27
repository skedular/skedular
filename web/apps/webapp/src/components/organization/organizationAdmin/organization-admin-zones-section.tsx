import { PushToRight, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { DeleteIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { AddOrganizationZoneButton } from '@/components/organization/addOrganizationZone';
import { EditOrganizationZoneDialog } from '@/components/organization/editOrganizationZone';
import OrganizationAdminTagManagementList from '@/components/organization/organizationAdmin/organization-admin-tag-management-list';
import { Search } from '@/components/search';
import { Zone } from '@/components/zone';
import { PaletteModeContext } from '@skedular/shared';
import { defaultGridActionPadding } from '@skedular/ui';
import { getRelayErrorMessage } from '@skedular/shared';
import type { organizationAdminZonesSectionQuery } from '@/queries/__generated__/organizationAdminZonesSectionQuery.graphql';
import type { organizationAdminZonesSection_addCustomerPreferredOrganizationTagMutation } from '@/queries/__generated__/organizationAdminZonesSection_addCustomerPreferredOrganizationTagMutation.graphql';
import type { organizationAdminZonesSection_deleteZonesMutation } from '@/queries/__generated__/organizationAdminZonesSection_deleteZonesMutation.graphql';
import type { organizationAdminZonesSection_removeCustomerPreferredOrganizationTagMutation } from '@/queries/__generated__/organizationAdminZonesSection_removeCustomerPreferredOrganizationTagMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { SettingsSectionCard } from '@skedular/ui';
import { memo, useContext, useEffect, useMemo, useState } from 'react';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  organizationCustomDomain: string;
};

type InnerProps = {
  organizationCustomDomain: string;
  onSearchTextChange: (value: string) => void;
  onReloadRequired: () => void;
  queryReference: PreloadedQuery<organizationAdminZonesSectionQuery>;
};

const RootQuery = graphql`
  query organizationAdminZonesSectionQuery($organizationCustomDomain: String!, $zoneNameSearchText: String) {
    me {
      id
      preferredZones {
        id
      }
    }
    organization(customDomain: $organizationCustomDomain) {
      zones(first: 100, where: { nameContains: $zoneNameSearchText }) {
        __id
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
  }
`;

const OrganizationAdminZonesSectionContent = ({ organizationCustomDomain, onReloadRequired, onSearchTextChange, queryReference }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationAdminZonesSectionQuery>(RootQuery, queryReference);
  const [commitDeleteZones] = useMutation<organizationAdminZonesSection_deleteZonesMutation>(graphql`
    mutation organizationAdminZonesSection_deleteZonesMutation($connectionIds: [ID!]!, $input: DeleteZonesInput!) {
      deleteZones(input: $input) {
        organizationTags {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);
  const [commitAddCustomerPreferredOrganizationTag] = useMutation<organizationAdminZonesSection_addCustomerPreferredOrganizationTagMutation>(graphql`
    mutation organizationAdminZonesSection_addCustomerPreferredOrganizationTagMutation($input: AddCustomerPreferredOrganizationTagInput!) {
      addCustomerPreferredOrganizationTag(input: $input) {
        customer {
          id
          preferredZones {
            id
          }
        }
      }
    }
  `);
  const [commitRemoveCustomerPreferredOrganizationTag] = useMutation<organizationAdminZonesSection_removeCustomerPreferredOrganizationTagMutation>(graphql`
    mutation organizationAdminZonesSection_removeCustomerPreferredOrganizationTagMutation($input: RemoveCustomerPreferredOrganizationTagInput!) {
      removeCustomerPreferredOrganizationTag(input: $input) {
        customer {
          id
          preferredZones {
            id
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [selectedZoneIds, setSelectedZoneIds] = useState<string[]>([]);
  const [selectedZoneId, setSelectedZoneId] = useState<null | string>(null);
  const [zoneMoreActionsAnchorEl, setZoneMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const zoneMoreActionsMenuOpen = Boolean(zoneMoreActionsAnchorEl);
  const [isEditZoneDialogOpen, setIsEditZoneDialogOpen] = useState(false);
  const preferredZones = useMemo(() => rootData.me?.preferredZones.map(({ id }) => id) ?? [], [rootData.me]);

  const zones = useMemo(() => (rootData.organization ? rootData.organization.zones.edges.map(({ node }) => node) : []), [rootData.organization]);
  const zonesConnectionIds = useMemo(() => (rootData.organization ? [rootData.organization.zones.__id] : []), [rootData.organization]);

  const zoneItems = useMemo(
    () =>
      zones.map((zone) => ({
        id: zone.id,
        name: zone.name ?? '',
        description: zone.description,
        preferred: preferredZones.includes(zone.id),
      })),
    [preferredZones, zones],
  );
  const selectedZoneItem = useMemo(() => zoneItems.find((item) => item.id === selectedZoneId), [selectedZoneId, zoneItems]);
  const zoneMoreActionsOption: MoreActionsMenuItemType[] = useMemo(() => {
    const options: MoreActionsMenuItemType[] = [moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditZone], moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteZone]];

    if (selectedZoneItem) {
      options.splice(
        1,
        0,
        selectedZoneItem.preferred
          ? moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveAsPreferredZone]
          : moreActionsMenuAllOptions[MoreActionsMenuOptionType.SetAsPreferredZone],
      );
    }

    return options;
  }, [selectedZoneItem]);

  const handleSelectedZonesChanged = (id: string) => {
    setSelectedZoneIds((current) => (current.includes(id) ? current.filter((item) => item !== id) : current.concat(id)));
  };

  const handleRemoveZonesClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing zones ..." />, infoNotificationOptions);

    commitDeleteZones({
      variables: {
        connectionIds: zonesConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: selectedZoneIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove zones. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content="Zones removed." />,
        });
        setSelectedZoneIds([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove zones. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveZoneClick = () => {
    if (!selectedZoneId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing zone ..." />, infoNotificationOptions);

    commitDeleteZones({
      variables: {
        connectionIds: zonesConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: [selectedZoneId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove zone. Error: ${getRelayErrorMessage(errors)}.`} />,
          });
          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content="Zone removed." />,
        });
        setSelectedZoneId(null);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove zone. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleSetAsPreferredZoneClicked = (id: string) => {
    const organizationTagDetails = zones.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting zone '${organizationTagDetails.name}' as your preferred zone...`} />, infoNotificationOptions);

    commitAddCustomerPreferredOrganizationTag({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set zone '${organizationTagDetails.name}' as your preferred zone. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone '${organizationTagDetails.name}' has been set as the preferred zone.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set zone '${organizationTagDetails.name}' as your preferred zone. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredZoneClicked = (id: string) => {
    const organizationTagDetails = zones.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing zone '${organizationTagDetails.name}' as your preferred zone...`} />, infoNotificationOptions);

    commitRemoveCustomerPreferredOrganizationTag({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the zone '${organizationTagDetails.name}' as your preferred zone. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone '${organizationTagDetails.name}' has been removed as your preferred zone.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the zone '${organizationTagDetails.name}' as your preferred zone. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleZoneMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setZoneMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditZone:
        setIsEditZoneDialogOpen(true);
        break;
      case MoreActionsMenuOptionType.DeleteZone:
        handleRemoveZoneClick();
        break;
      case MoreActionsMenuOptionType.SetAsPreferredZone:
        if (selectedZoneId) {
          handleSetAsPreferredZoneClicked(selectedZoneId);
        }
        break;
      case MoreActionsMenuOptionType.RemoveAsPreferredZone:
        if (selectedZoneId) {
          handleRemoveAsPreferredZoneClicked(selectedZoneId);
        }
        break;
    }
  };

  return (
    <>
      <Box sx={{ pb: 2 }}>
        <SettingsSectionCard
          title="Zones"
          description="Manage shared place-based tags used for organization filters and preferences."
          actions={<AddOrganizationZoneButton organizationCustomDomain={organizationCustomDomain} connectionIds={zonesConnectionIds} />}
        >
          <StackColumn spacing={2}>
            <StackRow sx={{ justifyContent: 'flex-end' }}>
              <Search size="small" placeholder="Search for zones" defaultValue="" onChange={onSearchTextChange} />
            </StackRow>

            {selectedZoneIds.length > 0 && (
              <Box
                sx={{
                  backgroundColor: 'background.paper',
                  padding: defaultGridActionPadding,
                  border: 1,
                  borderColor: (theme) => theme.palette.divider,
                  borderRadius: 2,
                }}
              >
                <StackRow sx={{ alignItems: 'center' }}>
                  <SmallIconTypography label={`${selectedZoneIds.length} record${selectedZoneIds.length === 1 ? '' : 's'} selected`} />
                  <PushToRight />
                  <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveZonesClick} sx={{ textTransform: 'none' }}>
                    Remove Zone
                  </Button>
                </StackRow>
              </Box>
            )}

            <OrganizationAdminTagManagementList
              items={zoneItems}
              emptyTitle="No zones found"
              emptyDescription="Adjust the search or add a new zone for this organization."
              selectedIds={selectedZoneIds}
              onToggleSelected={handleSelectedZonesChanged}
              onOpenMoreActions={(id, target) => {
                setSelectedZoneId(id);
                setZoneMoreActionsAnchorEl(target);
              }}
              renderPrimary={(item) => {
                const zone = zones.find((entry) => entry.id === item.id);
                return zone ? <Zone zone={zone} showFullName /> : null;
              }}
            />
          </StackColumn>
        </SettingsSectionCard>
      </Box>

      <MoreActionsMenu anchorEl={zoneMoreActionsAnchorEl} open={zoneMoreActionsMenuOpen} onMenuItemClick={handleZoneMoreActionsMenuItemClick} options={zoneMoreActionsOption} />

      {selectedZoneId && (
        <EditOrganizationZoneDialog
          onReloadRequired={onReloadRequired}
          zoneId={selectedZoneId}
          isDialogOpen={isEditZoneDialogOpen}
          onAddClicked={() => setIsEditZoneDialogOpen(false)}
          onCancel={() => setIsEditZoneDialogOpen(false)}
        />
      )}
    </>
  );
};

const OrganizationAdminZonesSection = ({ organizationCustomDomain }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationAdminZonesSectionQuery>(RootQuery);
  const [zoneNameSearchText, setZoneNameSearchText] = useState('');
  const [reloadKey, setReloadKey] = useState(uuid());

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        zoneNameSearchText,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain, reloadKey, zoneNameSearchText]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <OrganizationAdminZonesSectionContent
      key={`${reloadKey}-${zoneNameSearchText}`}
      organizationCustomDomain={organizationCustomDomain}
      onSearchTextChange={setZoneNameSearchText}
      onReloadRequired={() => setReloadKey(uuid())}
      queryReference={queryReference}
    />
  );
};

export default memo(OrganizationAdminZonesSection);
