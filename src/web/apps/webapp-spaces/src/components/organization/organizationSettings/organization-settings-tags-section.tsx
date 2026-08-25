import { PaletteModeContext, getRelayErrorMessage, useIntegratedPlatform } from '@skedular/shared';
import { PushToRight, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { DeleteIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { getOrganizationSettingsEditCustomTagBaseLink } from '@/components/links';
import { AddOrganizationCustomTagButton } from '@/components/organization/addOrganizationCustomTag';
import OrganizationSettingsTagManagementList from '@/components/organization/organizationSettings/organization-settings-tag-management-list';
import { Search } from '@/components/search';

import { defaultGridActionPadding } from '@skedular/ui';

import type { organizationSettingsTagsSectionQuery } from '@/queries/__generated__/organizationSettingsTagsSectionQuery.graphql';
import type { organizationSettingsTagsSection_addCustomerPreferredOrganizationTagMutation } from '@/queries/__generated__/organizationSettingsTagsSection_addCustomerPreferredOrganizationTagMutation.graphql';
import type { organizationSettingsTagsSection_deleteCustomTagsMutation } from '@/queries/__generated__/organizationSettingsTagsSection_deleteCustomTagsMutation.graphql';
import type { organizationSettingsTagsSection_removeCustomerPreferredOrganizationTagMutation } from '@/queries/__generated__/organizationSettingsTagsSection_removeCustomerPreferredOrganizationTagMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { SettingsSectionCard } from '@skedular/ui';
import { memo, useContext, useEffect, useMemo, useState } from 'react';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  organizationCustomDomain: string;
};

type InnerProps = {
  organizationCustomDomain: string;
  onSearchTextChange: (value: string) => void;
  searchText: string;
  queryReference: PreloadedQuery<organizationSettingsTagsSectionQuery>;
};

const RootQuery = graphql`
  query organizationSettingsTagsSectionQuery($organizationCustomDomain: String!, $customTagNameSearchText: String) {
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

const OrganizationSettingsTagsSectionContent = ({ organizationCustomDomain, onSearchTextChange, searchText, queryReference }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationSettingsTagsSectionQuery>(RootQuery, queryReference);
  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const [commitDeleteCustomTags] = useMutation<organizationSettingsTagsSection_deleteCustomTagsMutation>(graphql`
    mutation organizationSettingsTagsSection_deleteCustomTagsMutation($connectionIds: [ID!]!, $input: DeleteCustomTagsInput!) {
      deleteCustomTags(input: $input) {
        organizationTags {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);
  const [commitAddCustomerPreferredOrganizationTag] = useMutation<organizationSettingsTagsSection_addCustomerPreferredOrganizationTagMutation>(graphql`
    mutation organizationSettingsTagsSection_addCustomerPreferredOrganizationTagMutation($input: AddCustomerPreferredOrganizationTagInput!) {
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
  const [commitRemoveCustomerPreferredOrganizationTag] = useMutation<organizationSettingsTagsSection_removeCustomerPreferredOrganizationTagMutation>(graphql`
    mutation organizationSettingsTagsSection_removeCustomerPreferredOrganizationTagMutation($input: RemoveCustomerPreferredOrganizationTagInput!) {
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
          themedToast(<NotificationContent content={`We couldn't remove those tags. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedCustomTagIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove those tags. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleRemoveCustomTagClick = () => {
    if (!selectedCustomTagId) {
      return;
    }

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
          themedToast(<NotificationContent content={`We couldn't remove that tag. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedCustomTagId(null);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove that tag. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleSetAsPreferredCustomTagClicked = (id: string) => {
    const organizationTagDetails = customTags.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    commitAddCustomerPreferredOrganizationTag({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`We couldn't make '${organizationTagDetails.name}' your preferred tag. ${getRelayErrorMessage(errors)}`} />,
            errorNotificationOptions,
          );

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't make '${organizationTagDetails.name}' your preferred tag. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleRemoveAsPreferredCustomTagClicked = (id: string) => {
    const organizationTagDetails = customTags.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    commitRemoveCustomerPreferredOrganizationTag({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`We couldn't remove '${organizationTagDetails.name}' from your preferred tags. ${getRelayErrorMessage(errors)}`} />,
            errorNotificationOptions,
          );

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove '${organizationTagDetails.name}' from your preferred tags. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleCustomTagMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setCustomTagMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditCustomTag:
        if (selectedCustomTagId) {
          const currentQuery = searchParams.toString();
          const redirectUrl = currentQuery ? `${pathname}?${currentQuery}` : pathname;
          router.push(getOrganizationSettingsEditCustomTagBaseLink(integratedPlatform, organizationCustomDomain, selectedCustomTagId, { redirectUrl }));
        }
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

  const handleOpenCustomTag = (id: string) => {
    const currentQuery = searchParams.toString();
    const redirectUrl = currentQuery ? `${pathname}?${currentQuery}` : pathname;
    router.push(getOrganizationSettingsEditCustomTagBaseLink(integratedPlatform, organizationCustomDomain, id, { redirectUrl }));
  };

  return (
    <>
      <Box sx={{ pb: 2 }}>
        <SettingsSectionCard
          title="Tags"
          description="Manage custom tags used to classify bookings, spaces, and member preferences."
          actions={<AddOrganizationCustomTagButton organizationCustomDomain={organizationCustomDomain} />}
        >
          <StackColumn spacing={2}>
            <StackRow sx={{ justifyContent: 'flex-end' }}>
              <Search size="small" placeholder="Search for tags" defaultValue={searchText} onChange={onSearchTextChange} />
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

            <OrganizationSettingsTagManagementList
              items={customTagItems}
              emptyTitle="No tags found"
              emptyDescription="Adjust the search or add a new custom tag for this organization."
              selectedIds={selectedCustomTagIds}
              onToggleSelected={handleSelectedCustomTagsChanged}
              onOpenMoreActions={(id, target) => {
                setSelectedCustomTagId(id);
                setCustomTagMoreActionsAnchorEl(target);
              }}
              onOpenItem={handleOpenCustomTag}
              variant="plain"
              renderPrimary={(item) => {
                return <SmallIconTypography label={item.name} />;
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
    </>
  );
};

const OrganizationSettingsTagsSection = ({ organizationCustomDomain }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationSettingsTagsSectionQuery>(RootQuery);
  const [reloadKey] = useState(uuid());
  const router = useRouter();
  const searchParams = useSearchParams();
  const customTagNameSearchText = searchParams.get('tagSearch') ?? '';
  const setCustomTagNameSearchText = (value: string) => {
    const params = new URLSearchParams(window.location.search);
    if (value) params.set('tagSearch', value);
    else params.delete('tagSearch');
    router.push(`?${params.toString()}`);
  };

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
    <OrganizationSettingsTagsSectionContent
      organizationCustomDomain={organizationCustomDomain}
      onSearchTextChange={setCustomTagNameSearchText}
      searchText={customTagNameSearchText}
      queryReference={queryReference}
    />
  );
};

export default memo(OrganizationSettingsTagsSection);
