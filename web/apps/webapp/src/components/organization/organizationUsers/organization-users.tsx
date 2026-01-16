import { CustomerAvatar } from '@/components/avatars';
import { AppBarWithStackColumn, BodyIconTypography, GridContainer, PushToRight, SectionIconTypography, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { DeleteIcon, EllipseMenuIcon } from '@/components/icons';
import { getOrganizationBaseLink, getOrganizationBookingsBaseLink, getOrganizationUserProfileBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { InvitePeopleToJoinOrganizationButton } from '@/components/organization/invitePeopleToJoinOrganization';
import { RelayError, toRootError } from '@/components/relayError';
import { Search } from '@/components/search';
import { TeamSelector } from '@/components/team/teamSelector';
import { defaultGridRowSelectionModelValue } from '@/libs/mui';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultGridActionPadding, defaultGridStyle, defaultPadding, emerald, flame, secondDrawerExpandedDrawerWidthPx } from '@/libs/theme';
import { getCustomerFullName, joinErrors } from '@/libs/utils';
import type { organizationUsers_changeOrganizationMemberRoleMutation } from '@/queries/__generated__/organizationUsers_changeOrganizationMemberRoleMutation.graphql';
import type { organizationUsers_changeOrganizationUsersStatusMutation } from '@/queries/__generated__/organizationUsers_changeOrganizationUsersStatusMutation.graphql';
import type { OrganizationMemberRole, organizationUsers_organizationMembers_query$key } from '@/queries/__generated__/organizationUsers_organizationMembers_query.graphql';
import type { organizationUsers_organizationUsers_refetchableFragment } from '@/queries/__generated__/organizationUsers_organizationUsers_refetchableFragment.graphql';
import type { organizationUsers_removeOrganizationUsersMutation } from '@/queries/__generated__/organizationUsers_removeOrganizationUsersMutation.graphql';
import type { organizationUsers_rootQuery } from '@/queries/__generated__/organizationUsers_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import MenuItem from '@mui/material/MenuItem';
import Select from '@mui/material/Select';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { useRouter } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import OrganizationUsersLeftSideNavigationMenuContent from './organization-users-left-side-navigation-menu-content';

type Props = {
  queryReference: PreloadedQuery<organizationUsers_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
};

const RootQuery = graphql`
  query organizationUsers_rootQuery($organizationUniqueAlphanumericName: String!, $peopleNameSearchText: String) {
    organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
      canInvitePeople
    }
    teams(where: { organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName }) {
      __id
      totalCount
      edges {
        node {
          id
          name
          members {
            edges {
              node {
                organizationMember {
                  uniqueId
                  customer {
                    id
                  }
                }
              }
            }
          }
          ...teamCard_TeamDetails
        }
      }
    }
    organizationMemberRoles
    ...teamSelector_allTeams_query
    ...organizationUsers_organizationMembers_query
  }
`;

type CustomerDetails = {
  id: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
  phoneNumber?: string | null | undefined;
};

type RowType = {
  id: string;
  avatar: CustomerDetails;
  name: string;
  teams: string;
  email: string | null | undefined;
  phoneNumber: string | null | undefined;
  role: OrganizationMemberRole | null | undefined;
  status: boolean;
};

const OrganizationUsers = ({ queryReference, organizationUniqueAlphanumericName }: Props) => {
  const rootData = usePreloadedQuery<organizationUsers_rootQuery>(RootQuery, queryReference);
  const [rootDataOrganizationUsers, refetchOrganizationUsers] = useRefetchableFragment<
    organizationUsers_organizationUsers_refetchableFragment,
    organizationUsers_organizationMembers_query$key
  >(
    graphql`
      fragment organizationUsers_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationUsers_organizationUsers_refetchableFragment") {
        organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
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
                status
                role
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
          status
          role
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
          status
          role
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const [, startTransition] = useTransition();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const router = useRouter();
  const [teamIds, setTeamIds] = useState<string[]>([]);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const [seledctedMembers, setSeledctedMembers] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
  const [selectedMemberId, setSelectedMemberId] = useState<null | string>(null);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);

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
    const members = rootDataOrganizationUsers.organization
      ? rootDataOrganizationUsers.organization.members.edges
          .map(({ node }) => node)
          .sort((a, b) => {
            const name1 = getCustomerFullName(a.customer);
            const name2 = getCustomerFullName(b.customer);

            return name1.localeCompare(name2);
          })
          .map((member) => {
            const teams = rootData.teams
              ? rootData.teams.edges
                  .map(({ node }) => node)
                  .filter((item) => item.members.edges.map(({ node }) => node).some(({ organizationMember }) => organizationMember?.customer.id === member.customer.id))
              : [];

            return {
              ...member,
              teams,
            };
          })
      : [];

    return members.filter((member) => {
      if (teamIds.length === 0) {
        return true;
      }

      return member.teams.some((team) => teamIds.includes(team.id));
    });
  }, [rootData.teams, rootDataOrganizationUsers.organization, teamIds]);

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

  const handlTeamChanged = (id?: string) => {
    setTeamIds(id ? [id] : []);
  };

  const handleSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetchOrganizationUsers(str);
  };

  const handleSelectedUsersChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedMembers(newRowSelectionModel);
  };

  const handleDeactivateUsersClick = () => {
    const toastId = themedToast(<NotificationContent content={'Deactivating users...'} />, infoNotificationOptions);

    commitChangeOrganizationMembersStatus({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: seledctedMembers.ids
            .values()
            .map((id) => id as string)
            .toArray(),
          status: 'INACTIVE',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate users. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Users deactivated.'} />,
        });
        setSeledctedMembers(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate users. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateUsersClick = () => {
    const toastId = themedToast(<NotificationContent content={'Activating users...'} />, infoNotificationOptions);

    commitChangeOrganizationMembersStatus({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: seledctedMembers.ids
            .values()
            .map((id) => id as string)
            .toArray(),
          status: 'ACTIVE',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate users. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Users activated.'} />,
        });
        setSeledctedMembers(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate users. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveUsersClick = () => {
    const toastId = themedToast(<NotificationContent content={'Removing users...'} />, infoNotificationOptions);

    commitRemoveOrganizationMembers({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          ids: seledctedMembers.ids
            .values()
            .map((id) => id as string)
            .toArray(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove users. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Users removed.'} />,
        });
        setSeledctedMembers(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove users. Error: ${error.message}.`} />,
        });
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

        router.push(getOrganizationUserProfileBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, memberDetails.customer.id));
        break;

      case MoreActionsMenuOptionType.ViewUserBookings:
        if (!memberDetails) {
          return;
        }

        router.push(getOrganizationBookingsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, { customerId: memberDetails.customer.id }));
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

    const toastId = themedToast(<NotificationContent content={'Deactivating user...'} />, infoNotificationOptions);

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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate user. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'User deactivated.'} />,
        });
        setSeledctedMembers(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate user. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateUserClick = () => {
    if (!memberDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Activating user...'} />, infoNotificationOptions);

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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate user. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'User activated.'} />,
        });
        setSeledctedMembers(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate user. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveUserClick = () => {
    if (!memberDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Removing user...'} />, infoNotificationOptions);

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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove user. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'User removed.'} />,
        });
        setSeledctedMembers(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove user. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRoleChanged = (id: string, roleStr: string) => {
    const member = members.find((member) => member.id === id);
    if (!member) {
      return;
    }

    const role = roleStr as unknown as OrganizationMemberRole;
    const toastId = themedToast(<NotificationContent content={`Updating role...`} />, infoNotificationOptions);

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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update role to ${role}. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Role updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update role to '${role}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        changeOrganizationMemberRole: {
          member: {
            id: member.id,
            customer: member.customer,
            status: member.status,
            role,
          },
        },
      },
    });
  };

  const handleCloseClick = () => {
    router.push(getOrganizationBaseLink(integratedPlatrform, organizationUniqueAlphanumericName));
  };

  const rows: RowType[] = members.map((member) => ({
    id: member.id,
    avatar: member.customer,
    name: getCustomerFullName(member.customer),
    teams: member.teams.map((team) => team.name).join(', '),
    email: member.customer.email,
    phoneNumber: member.customer.phoneNumber,
    role: member.role,
    status: member.status === 'ACTIVE',
  }));

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'avatar',
      headerName: '',
      editable: false,
      renderCell: (params) => <CustomerAvatar name={params.value} photo={{ url: params.value?.photoUrl }} size="medium" showFullName />,
      display: 'flex',
      maxWidth: 20,
    },
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'teams',
      headerName: 'Team',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 300,
    },
    {
      field: 'email',
      headerName: 'Email',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 300,
    },
    {
      field: 'phoneNumber',
      headerName: 'Phone',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'role',
      headerName: 'Role',
      editable: false,
      renderCell: (params) => (
        <Select
          value={params.value}
          onChange={(event) => handleRoleChanged(params.id as string, event.target.value as string)}
          size="small"
          sx={{
            borderRadius: 2,
            width: 150,
            margin: 0.5,
          }}
          renderValue={(selectedRole) => <SmallIconTypography label={selectedRole} />}
        >
          {rootData.organizationMemberRoles.map((role) => (
            <MenuItem key={role} value={role}>
              <SmallIconTypography label={role} />
            </MenuItem>
          ))}
        </Select>
      ),
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'status',
      headerName: 'Status',
      editable: false,
      renderCell: (params) => (
        <StackRow>
          {params.value && (
            <StackRow sx={{ justifyContent: 'space-between', width: 76 }}>
              <SmallIconTypography label="Active" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: emerald }} />
            </StackRow>
          )}
          {!params.value && (
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
      field: 'More Actions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedMemberId(params.id as string);
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

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationUsersLeftSideNavigationMenuContent organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} hideIcons />
        <Box sx={{ marginLeft: secondDrawerExpandedDrawerWidthPx, flexGrow: 1 }}>
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Organization Users">
            <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Organization Users" />
                  <BodyIconTypography label="View users in your organization" />
                </Grid>

                <Grid>
                  <InvitePeopleToJoinOrganizationButton organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <TeamSelector rootDataRelay={rootData} onChange={handlTeamChanged} />
              <PushToRight />
              <Search size="small" placeholder="Search for users" defaultValue={peopleNameSearchText} onChange={handleSearchTextChange} />
            </GridContainer>

            {seledctedMembers.ids.size > 0 && (
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
                    <SmallIconTypography label={`${seledctedMembers.ids.size} records selected`} invertDefaultColor={paletteMode === 'dark'} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="secondary" onClick={handleDeactivateUsersClick} sx={defaultButtonStyle}>
                      Deactivate User
                    </Button>
                    <Button size="medium" variant="contained" color="secondary" onClick={handleActivateUsersClick} sx={defaultButtonStyle}>
                      Activate User
                    </Button>
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveUsersClick} sx={{ textTransform: 'none' }}>
                      Remove User
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedMembers}
                onRowSelectionModelChange={handleSelectedUsersChanged}
                rows={rows}
                columns={columns}
                hideFooterPagination={rows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: rows.length,
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
                localeText={{ noRowsLabel: 'No user found' }}
              />
            </StackRow>
          </AppBarWithStackColumn>
        </Box>
      </Box>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
    </>
  );
};

const MemoOrganizationUsers = memo(OrganizationUsers);

type RelayProps = {
  organizationUniqueAlphanumericName: string;
};

const OrganizationUsersWithRelay = ({ organizationUniqueAlphanumericName }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationUsers_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationUniqueAlphanumericName,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationUniqueAlphanumericName]);

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
      <MemoOrganizationUsers queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationUsersWithRelay);
