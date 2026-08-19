import { PaletteModeContext, RelayError, getCustomerFullName, getRelayErrorMessage, toRootError, useIntegratedPlatform } from '@skedular/shared';
import { getOrganizationBookingsBaseLink, getOrganizationUserProfileBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { InvitePeopleToJoinOrganizationButton } from '@/components/organization/invitePeopleToJoinOrganization';
import OrganizationUserManagementList from '@/components/organization/organizationUsers/organization-user-management-list';

import { Search } from '@/components/search';
import type { organizationUsers_changeOrganizationMemberRoleMutation } from '@/queries/__generated__/organizationUsers_changeOrganizationMemberRoleMutation.graphql';
import type { organizationUsers_changeOrganizationUsersStatusMutation } from '@/queries/__generated__/organizationUsers_changeOrganizationUsersStatusMutation.graphql';
import type { OrganizationMemberRole, organizationUsers_organizationMembers_query$key } from '@/queries/__generated__/organizationUsers_organizationMembers_query.graphql';
import type { organizationUsers_organizationUsers_refetchableFragment } from '@/queries/__generated__/organizationUsers_organizationUsers_refetchableFragment.graphql';
import type { organizationUsers_removeOrganizationUsersMutation } from '@/queries/__generated__/organizationUsers_removeOrganizationUsersMutation.graphql';
import type { organizationUsers_rootQuery } from '@/queries/__generated__/organizationUsers_rootQuery.graphql';
import Box from '@mui/material/Box';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';

import { BodyIconTypography, defaultPadding, PageHeaderPanel, SettingsSectionCard, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<organizationUsers_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query organizationUsers_rootQuery($organizationCustomDomain: String!, $peopleNameSearchText: String) {
    organization(customDomain: $organizationCustomDomain) {
      canInvitePeople
    }
    organizationMemberRoles {
      type
      name
    }
    ...organizationUsers_organizationMembers_query
  }
`;

const OrganizationUsers = ({ queryReference, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<organizationUsers_rootQuery>(RootQuery, queryReference);
  const [rootDataOrganizationUsers, refetchOrganizationUsers] = useRefetchableFragment<
    organizationUsers_organizationUsers_refetchableFragment,
    organizationUsers_organizationMembers_query$key
  >(
    graphql`
      fragment organizationUsers_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationUsers_organizationUsers_refetchableFragment") {
        organization(customDomain: $organizationCustomDomain) {
          members(first: $count, after: $cursor, where: { nameContains: $peopleNameSearchText }) @connection(key: "organizationMembers_members") {
            __id
            totalCount
            edges {
              node {
                id
                customer {
                  id
                  email
                  name
                  givenName
                  middleName
                  familyName
                  photoUrl
                  phoneNumber
                }
                status {
                  type
                  name
                }
                role {
                  type
                  name
                }
              }
            }
          }
        }
      }
    `,
    rootData,
  );

  const [commitChangeOrganizationMembersStatus] = useMutation<organizationUsers_changeOrganizationUsersStatusMutation>(graphql`
    mutation organizationUsers_changeOrganizationUsersStatusMutation($input: ChangeOrganizationMembersStatusInput!) {
      changeOrganizationMembersStatus(input: $input) {
        members {
          id
          customer {
            id
            email
            name
            givenName
            middleName
            familyName
            photoUrl
            phoneNumber
          }
          status {
            type
            name
          }
          role {
            type
            name
          }
        }
      }
    }
  `);

  const [commitRemoveOrganizationMembers] = useMutation<organizationUsers_removeOrganizationUsersMutation>(graphql`
    mutation organizationUsers_removeOrganizationUsersMutation($connectionIds: [ID!]!, $input: RemoveOrganizationMembersInput!) {
      removeOrganizationMembers(input: $input) {
        members {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitChangeOrganizationMemberRole] = useMutation<organizationUsers_changeOrganizationMemberRoleMutation>(graphql`
    mutation organizationUsers_changeOrganizationMemberRoleMutation($input: ChangeOrganizationMemberRoleInput!) @raw_response_type {
      changeOrganizationMemberRole(input: $input) {
        member {
          id
          customer {
            id
            email
            name
            givenName
            middleName
            familyName
            photoUrl
            phoneNumber
          }
          status {
            type
            name
          }
          role {
            type
            name
          }
        }
      }
    }
  `);

  const { integratedPlatform } = useIntegratedPlatform();
  const [, startTransition] = useTransition();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const router = useRouter();
  const searchParams = useSearchParams();
  const peopleNameSearchText = searchParams.get('search') ?? '';
  const [selectedMemberIds, setSelectedMemberIds] = useState<string[]>([]);
  const [selectedMemberId, setSelectedMemberId] = useState<null | string>(null);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const [changeRoleAnchorEl, setChangeRoleAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const changeRoleMenuOpen = Boolean(changeRoleAnchorEl);

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditOrganizationUser],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ViewUserBookings],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateOrganizationUser],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateOrganizationUser],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveOrganizationUser],
  ];

  const memberDetails = useMemo(
    () => rootDataOrganizationUsers.organization?.members?.edges.map(({ node }) => node).find((item) => item.id === selectedMemberId),
    [selectedMemberId, rootDataOrganizationUsers.organization?.members],
  );
  const connectionIds = useMemo(
    () => (rootDataOrganizationUsers.organization ? [rootDataOrganizationUsers.organization.members.__id] : []),
    [rootDataOrganizationUsers.organization],
  );
  const members = useMemo(() => {
    return rootDataOrganizationUsers.organization
      ? rootDataOrganizationUsers.organization.members.edges
          .map(({ node }) => node)
          .sort((a, b) => {
            const name1 = getCustomerFullName(a.customer);
            const name2 = getCustomerFullName(b.customer);

            return name1.localeCompare(name2);
          })
      : [];
  }, [rootDataOrganizationUsers.organization]);
  const organizationMemberRoleNameByType = useMemo(() => new Map(rootData.organizationMemberRoles.map((item) => [item.type, item.name])), [rootData.organizationMemberRoles]);

  const handleRefetchOrganizationUsers = useCallback(
    (peopleNameSearchText: string) => {
      startTransition(() => {
        refetchOrganizationUsers(
          {
            peopleNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetchOrganizationUsers],
  );

  const handleSearchTextChange = (str: string) => {
    const params = new URLSearchParams(window.location.search);
    if (str) params.set('search', str);
    else params.delete('search');
    router.push(`?${params.toString()}`);
  };

  useEffect(() => {
    handleRefetchOrganizationUsers(peopleNameSearchText);
  }, [handleRefetchOrganizationUsers, peopleNameSearchText]);

  const handleSelectedUsersChanged = (memberId: string) => {
    setSelectedMemberIds((current) => (current.includes(memberId) ? current.filter((id) => id !== memberId) : current.concat(memberId)));
  };

  const handleDeactivateUsersClick = () => {
    commitChangeOrganizationMembersStatus({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: selectedMemberIds,
          status: 'INACTIVE',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't deactivate those users. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedMemberIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't deactivate those users. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleActivateUsersClick = () => {
    commitChangeOrganizationMembersStatus({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: selectedMemberIds,
          status: 'ACTIVE',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't activate those users. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedMemberIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't activate those users. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleRemoveUsersClick = () => {
    commitRemoveOrganizationMembers({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          ids: selectedMemberIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't remove those users. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedMemberIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove those users. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditOrganizationUser:
        if (!memberDetails) {
          return;
        }

        router.push(getOrganizationUserProfileBaseLink(integratedPlatform, organizationCustomDomain, memberDetails.customer.id));
        break;

      case MoreActionsMenuOptionType.ViewUserBookings:
        if (!memberDetails) {
          return;
        }

        router.push(getOrganizationBookingsBaseLink(integratedPlatform, organizationCustomDomain, { customerId: memberDetails.customer.id }));
        break;

      case MoreActionsMenuOptionType.DeactivateOrganizationUser:
        handleDeactivateUserClick();
        break;

      case MoreActionsMenuOptionType.ActivateOrganizationUser:
        handleActivateUserClick();
        break;

      case MoreActionsMenuOptionType.RemoveOrganizationUser:
        handleRemoveUserClick();
        break;
    }
  };

  const handleDeactivateUserClick = () => {
    if (!memberDetails) {
      return;
    }

    commitChangeOrganizationMembersStatus({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: [memberDetails.id],
          status: 'INACTIVE',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't deactivate this user. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedMemberIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't deactivate this user. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleActivateUserClick = () => {
    if (!memberDetails) {
      return;
    }

    commitChangeOrganizationMembersStatus({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: [memberDetails.id],
          status: 'ACTIVE',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't activate this user. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedMemberIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't activate this user. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleRemoveUserClick = () => {
    if (!memberDetails) {
      return;
    }

    commitRemoveOrganizationMembers({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          ids: [memberDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't remove this user. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedMemberIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove this user. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleRoleChanged = (id: string, roleStr: string) => {
    const member = members.find((member) => member.id === id);
    if (!member) {
      return;
    }

    const role = roleStr as unknown as OrganizationMemberRole;
    const roleName = organizationMemberRoleNameByType.get(role) ?? role;
    commitChangeOrganizationMemberRole({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          role,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't change this user's role to ${roleName}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't change this user's role to ${roleName}. ${error.message}`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        changeOrganizationMemberRole: {
          member: {
            id: member.id,
            customer: member.customer,
            status: member.status,
            role: {
              type: role,
              name: roleName,
            },
          },
        },
      },
    });
  };

  const handleChangeRoleClick = (memberId: string, target: HTMLElement) => {
    setSelectedMemberId(memberId);
    setChangeRoleAnchorEl(target);
  };

  const handleChangeRoleMenuItemClick = (role: OrganizationMemberRole) => {
    if (!selectedMemberId) {
      return;
    }

    setChangeRoleAnchorEl(null);
    handleRoleChanged(selectedMemberId, role);
  };

  const memberItems = members.map((member) => ({
    id: member.id,
    customer: member.customer,
    name: getCustomerFullName(member.customer),
    email: member.customer.email,
    phoneNumber: member.customer.phoneNumber,
    role: member.role.name,
    statusName: member.status.name,
    isActive: member.status.type === 'ACTIVE',
  }));

  return (
    <>
      <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pb: defaultPadding }}>
        <StackColumn
          sx={{
            width: '100%',
            maxWidth: 1200,
            mx: 'auto',
            pt: { xs: 1, sm: 1, md: 2 },
            backgroundColor: 'transparent',
            gap: 2,
          }}
        >
          <PageHeaderPanel eyebrow="User management" title="Organization Users" description="Manage membership, roles, and account lifecycle controls for your organization.">
            <BodyIconTypography label="Search, change roles, and open individual user profiles from one place." />
          </PageHeaderPanel>

          <Box
            sx={{
              borderRadius: 4,
              border: 1,
              borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
              bgcolor: (theme) => (theme.palette.mode === 'light' ? 'common.white' : theme.palette.background.paper),
              boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 12px 32px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
              overflow: 'hidden',
            }}
          >
            <SettingsSectionCard
              title="Users"
              description="Browse users, review contact details, and manage organization access."
              actions={<InvitePeopleToJoinOrganizationButton organizationCustomDomain={organizationCustomDomain} />}
            >
              <StackColumn spacing={2}>
                <StackRow sx={{ gap: 1, flexWrap: 'wrap', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Search key={peopleNameSearchText} size="small" placeholder="Search for users" defaultValue={peopleNameSearchText} onChange={handleSearchTextChange} />
                </StackRow>

                <OrganizationUserManagementList
                  items={memberItems}
                  selectedIds={selectedMemberIds}
                  onToggleSelected={handleSelectedUsersChanged}
                  onOpenProfile={(memberId) => {
                    const member = members.find((item) => item.id === memberId);
                    if (!member) {
                      return;
                    }

                    router.push(getOrganizationUserProfileBaseLink(integratedPlatform, organizationCustomDomain, member.customer.id));
                  }}
                  onOpenChangeRole={handleChangeRoleClick}
                  onOpenMoreActions={(memberId, target) => {
                    setSelectedMemberId(memberId);
                    setMoreActionsAnchorEl(target);
                  }}
                  onDeactivateSelected={() => handleDeactivateUsersClick()}
                  onActivateSelected={() => handleActivateUsersClick()}
                  onRemoveSelected={() => handleRemoveUsersClick()}
                />
              </StackColumn>
            </SettingsSectionCard>
          </Box>
        </StackColumn>
      </Box>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
      <Menu anchorEl={changeRoleAnchorEl} open={changeRoleMenuOpen} onClose={() => setChangeRoleAnchorEl(null)}>
        {rootData.organizationMemberRoles.map((item) => (
          <MenuItem key={item.type} selected={memberDetails?.role.type === item.type} onClick={() => handleChangeRoleMenuItemClick(item.type)}>
            <SmallIconTypography label={item.name} />
          </MenuItem>
        ))}
      </Menu>
    </>
  );
};

const MemoOrganizationUsers = memo(OrganizationUsers);

type RelayProps = {
  organizationCustomDomain: string;
};

const OrganizationUsersWithRelay = ({ organizationCustomDomain }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationUsers_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoOrganizationUsers queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationUsersWithRelay);
