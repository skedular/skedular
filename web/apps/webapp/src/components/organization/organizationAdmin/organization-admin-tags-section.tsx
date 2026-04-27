import { PushToRight, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { CustomTag } from '@/components/customTag';
import { DeleteIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { AddOrganizationCustomTagButton } from '@/components/organization/addOrganizationCustomTag';
import { EditOrganizationCustomTagDialog } from '@/components/organization/editOrganizationCustomTag';
import OrganizationAdminTagManagementList from '@/components/organization/organizationAdmin/organization-admin-tag-management-list';
import { Search } from '@/components/search';
import { PaletteModeContext } from '@skedular/shared';
import { defaultGridActionPadding } from '@skedular/ui';
import { getRelayErrorMessage } from '@skedular/shared';
import type { organizationAdminTagsSectionQuery } from '@/queries/__generated__/organizationAdminTagsSectionQuery.graphql';
import type { organizationAdminTagsSection_addCustomerPreferredOrganizationTagMutation } from '@/queries/__generated__/organizationAdminTagsSection_addCustomerPreferredOrganizationTagMutation.graphql';
import type { organizationAdminTagsSection_deleteCustomTagsMutation } from '@/queries/__generated__/organizationAdminTagsSection_deleteCustomTagsMutation.graphql';
import type { organizationAdminTagsSection_removeCustomerPreferredOrganizationTagMutation } from '@/queries/__generated__/organizationAdminTagsSection_removeCustomerPreferredOrganizationTagMutation.graphql';
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
  queryReference: PreloadedQuery<organizationAdminTagsSectionQuery>;
};

const RootQuery = graphql`
  query organizationAdminTagsSectionQuery($organizationCustomDomain: String!, $customTagNameSearchText: String) {
    me {
      id
      preferredCustomTags {
        id
      }
    }
    organization(customDomain: $organizationCustomDomain) {
      customTags(first: 100, where: { nameContains: $customTagNameSearchText }, orderBy: [{ direction: ASCENDING, field: NAME }]) {
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

const OrganizationAdminTagsSectionContent = ({ organizationCustomDomain, onReloadRequired, onSearchTextChange, queryReference }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationAdminTagsSectionQuery>(RootQuery, queryReference);
  const [commitDeleteCustomTags] = useMutation<organizationAdminTagsSection_deleteCustomTagsMutation>(graphql`
    mutation organizationAdminTagsSection_deleteCustomTagsMutation($connectionIds: [ID!]!, $input: DeleteCustomTagsInput!) {
      deleteCustomTags(input: $input) {
        organizationTags {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);
  const [commitAddCustomerPreferredOrganizationTag] = useMutation<organizationAdminTagsSection_addCustomerPreferredOrganizationTagMutation>(graphql`
    mutation organizationAdminTagsSection_addCustomerPreferredOrganizationTagMutation($input: AddCustomerPreferredOrganizationTagInput!) {
      addCustomerPreferredOrganizationTag(input: $input) {
        customer {
          id
          preferredCustomTags {
            id
          }
        }
      }
    }
  `);
  const [commitRemoveCustomerPreferredOrganizationTag] = useMutation<organizationAdminTagsSection_removeCustomerPreferredOrganizationTagMutation>(graphql`
    mutation organizationAdminTagsSection_removeCustomerPreferredOrganizationTagMutation($input: RemoveCustomerPreferredOrganizationTagInput!) {
      removeCustomerPreferredOrganizationTag(input: $input) {
        customer {
          id
          preferredCustomTags {
            id
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [selectedCustomTagIds, setSelectedCustomTagIds] = useState<string[]>([]);
  const [selectedCustomTagId, setSelectedCustomTagId] = useState<null | string>(null);
  const [customTagMoreActionsAnchorEl, setCustomTagMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const customTagMoreActionsMenuOpen = Boolean(customTagMoreActionsAnchorEl);
  const [isEditCustomTagDialogOpen, setIsEditCustomTagDialogOpen] = useState(false);
  const preferredCustomTags = useMemo(() => rootData.me?.preferredCustomTags.map(({ id }) => id) ?? [], [rootData.me]);

  const customTags = useMemo(() => (rootData.organization ? rootData.organization.customTags.edges.map(({ node }) => node) : []), [rootData.organization]);
  const customTagsConnectionIds = useMemo(() => (rootData.organization ? [rootData.organization.customTags.__id] : []), [rootData.organization]);

  const customTagItems = useMemo(
    () =>
      customTags.map((customTag) => ({
        id: customTag.id,
        name: customTag.name ?? '',
        description: customTag.description,
        preferred: preferredCustomTags.includes(customTag.id),
      })),
    [customTags, preferredCustomTags],
  );
  const selectedCustomTagItem = useMemo(() => customTagItems.find((item) => item.id === selectedCustomTagId), [customTagItems, selectedCustomTagId]);
  const customTagMoreActionsOption: MoreActionsMenuItemType[] = useMemo(() => {
    const options: MoreActionsMenuItemType[] = [
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditCustomTag],
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteCustomTag],
    ];

    if (selectedCustomTagItem) {
      options.splice(
        1,
        0,
        selectedCustomTagItem.preferred
          ? moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveAsPreferredCustomTag]
          : moreActionsMenuAllOptions[MoreActionsMenuOptionType.SetAsPreferredCustomTag],
      );
    }

    return options;
  }, [selectedCustomTagItem]);

  const handleSelectedCustomTagsChanged = (id: string) => {
    setSelectedCustomTagIds((current) => (current.includes(id) ? current.filter((item) => item !== id) : current.concat(id)));
  };

  const handleRemoveCustomTagsClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing tags ..." />, infoNotificationOptions);

    commitDeleteCustomTags({
      variables: {
        connectionIds: customTagsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: selectedCustomTagIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't remove those tags. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content="Those tags have been removed." />,
        });
        setSelectedCustomTagIds([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't remove those tags. ${error.message}`} />,
        });
      },
    });
  };

  const handleRemoveCustomTagClick = () => {
    if (!selectedCustomTagId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing tag ..." />, infoNotificationOptions);

    commitDeleteCustomTags({
      variables: {
        connectionIds: customTagsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: [selectedCustomTagId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't remove that tag. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content="That tag has been removed." />,
        });

        setSelectedCustomTagId(null);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't remove that tag. ${error.message}`} />,
        });
      },
    });
  };

  const handleSetAsPreferredCustomTagClicked = (id: string) => {
    const organizationTagDetails = customTags.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting tag '${organizationTagDetails.name}' as your preferred tag...`} />, infoNotificationOptions);

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
            render: <NotificationContent content={`We couldn't make '${organizationTagDetails.name}' your preferred tag. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`'${organizationTagDetails.name}' is now your preferred tag.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't make '${organizationTagDetails.name}' your preferred tag. ${error.message}`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredCustomTagClicked = (id: string) => {
    const organizationTagDetails = customTags.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing tag '${organizationTagDetails.name}' as your preferred tag...`} />, infoNotificationOptions);

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
            render: <NotificationContent content={`We couldn't remove '${organizationTagDetails.name}' from your preferred tags. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`'${organizationTagDetails.name}' is no longer one of your preferred tags.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't remove '${organizationTagDetails.name}' from your preferred tags. ${error.message}`} />,
        });
      },
    });
  };

  const handleCustomTagMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setCustomTagMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditCustomTag:
        setIsEditCustomTagDialogOpen(true);
        break;
      case MoreActionsMenuOptionType.DeleteCustomTag:
        handleRemoveCustomTagClick();
        break;
      case MoreActionsMenuOptionType.SetAsPreferredCustomTag:
        if (selectedCustomTagId) {
          handleSetAsPreferredCustomTagClicked(selectedCustomTagId);
        }
        break;
      case MoreActionsMenuOptionType.RemoveAsPreferredCustomTag:
        if (selectedCustomTagId) {
          handleRemoveAsPreferredCustomTagClicked(selectedCustomTagId);
        }
        break;
    }
  };

  return (
    <>
      <Box sx={{ pb: 2 }}>
        <SettingsSectionCard
          title="Tags"
          description="Manage custom tags used to classify bookings, spaces, and member preferences."
          actions={<AddOrganizationCustomTagButton organizationCustomDomain={organizationCustomDomain} connectionIds={customTagsConnectionIds} />}
        >
          <StackColumn spacing={2}>
            <StackRow sx={{ justifyContent: 'flex-end' }}>
              <Search size="small" placeholder="Search for tags" defaultValue="" onChange={onSearchTextChange} />
            </StackRow>

            {selectedCustomTagIds.length > 0 && (
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
                  <SmallIconTypography label={`${selectedCustomTagIds.length} record${selectedCustomTagIds.length === 1 ? '' : 's'} selected`} />
                  <PushToRight />
                  <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveCustomTagsClick} sx={{ textTransform: 'none' }}>
                    Remove Tag
                  </Button>
                </StackRow>
              </Box>
            )}

            <OrganizationAdminTagManagementList
              items={customTagItems}
              emptyTitle="No tags found"
              emptyDescription="Adjust the search or add a new custom tag for this organization."
              selectedIds={selectedCustomTagIds}
              onToggleSelected={handleSelectedCustomTagsChanged}
              onOpenMoreActions={(id, target) => {
                setSelectedCustomTagId(id);
                setCustomTagMoreActionsAnchorEl(target);
              }}
              renderPrimary={(item) => {
                const customTag = customTags.find((entry) => entry.id === item.id);
                return customTag ? <CustomTag customTag={customTag} showFullName /> : null;
              }}
            />
          </StackColumn>
        </SettingsSectionCard>
      </Box>

      <MoreActionsMenu
        anchorEl={customTagMoreActionsAnchorEl}
        open={customTagMoreActionsMenuOpen}
        onMenuItemClick={handleCustomTagMoreActionsMenuItemClick}
        options={customTagMoreActionsOption}
      />

      {selectedCustomTagId && (
        <EditOrganizationCustomTagDialog
          onReloadRequired={onReloadRequired}
          customTagId={selectedCustomTagId}
          isDialogOpen={isEditCustomTagDialogOpen}
          onAddClicked={() => setIsEditCustomTagDialogOpen(false)}
          onCancel={() => setIsEditCustomTagDialogOpen(false)}
        />
      )}
    </>
  );
};

const OrganizationAdminTagsSection = ({ organizationCustomDomain }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationAdminTagsSectionQuery>(RootQuery);
  const [customTagNameSearchText, setCustomTagNameSearchText] = useState('');
  const [reloadKey, setReloadKey] = useState(uuid());

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        customTagNameSearchText,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [customTagNameSearchText, loadQuery, organizationCustomDomain, reloadKey]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <OrganizationAdminTagsSectionContent
      key={`${reloadKey}-${customTagNameSearchText}`}
      organizationCustomDomain={organizationCustomDomain}
      onSearchTextChange={setCustomTagNameSearchText}
      onReloadRequired={() => setReloadKey(uuid())}
      queryReference={queryReference}
    />
  );
};

export default memo(OrganizationAdminTagsSection);
