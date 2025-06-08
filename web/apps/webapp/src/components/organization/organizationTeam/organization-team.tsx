import { CustomerAvatar } from '@/components/avatars';
import {
  AppBarWithStackColumn,
  BodyIconTypography,
  FormFieldLabel,
  FormStackColumn,
  GridContainer,
  PushToRight,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@/components/commons';
import { SingleChoinceTimezone } from '@/components/forms';
import { BookingIcon, DeleteIcon, EllipseMenuIcon } from '@/components/icons';
import { getOrganizationBookingsBaseLink, getOrganizationTeamsBaseLink } from '@/components/links';
import { SingleChoiceLocation } from '@/components/location/locationSelector';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { AddOrganizationTeamMemberButton } from '@/components/organization/addOrganizationTeamMember';
import { Search } from '@/components/search';
import { teamFeatureImageHeight, teamFeatureImageWidth } from '@/components/team';
import { ImageFileUploader } from '@/libs/image-file-uploader';
import { defaultGridRowSelectionModelValue } from '@/libs/mui';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultGridActionPadding, defaultGridStyle, defaultPadding, emerald, flame, secondDrawerExpandedDrawerWidthPx } from '@/libs/theme';
import { getCustomerFullName, joinErrors } from '@/libs/utils';
import type { organizationTeam_changeTeamMemberRoleMutation } from '@/queries/__generated__/organizationTeam_changeTeamMemberRoleMutation.graphql';
import type { organizationTeam_changeTeamMembersStatusMutation } from '@/queries/__generated__/organizationTeam_changeTeamMembersStatusMutation.graphql';
import type { organizationTeam_deleteTeamMutation } from '@/queries/__generated__/organizationTeam_deleteTeamMutation.graphql';
import type { organizationTeam_query$key } from '@/queries/__generated__/organizationTeam_query.graphql';
import type { organizationTeam_removeTeamMembersMutation } from '@/queries/__generated__/organizationTeam_removeTeamMembersMutation.graphql';
import type { organizationTeam_teamMembers_query$key, TeamMemberRole } from '@/queries/__generated__/organizationTeam_teamMembers_query.graphql';
import type { organizationTeam_teamMembers_refetchableFragment } from '@/queries/__generated__/organizationTeam_teamMembers_refetchableFragment.graphql';
import type { organizationTeam_updateTeamMutation } from '@/queries/__generated__/organizationTeam_updateTeamMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import MenuItem from '@mui/material/MenuItem';
import Select from '@mui/material/Select';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import OrganizationTeamLeftSideNavigationMenuContent from './organization-team-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationTeam_query$key;
  rootDataTeamMembersRelay: organizationTeam_teamMembers_query$key;
  onReloadRequired: () => void;
  organizationId: string;
  teamId: string;
};

type TeamDetails = {
  name: string;
  about: string | null;
  timezone?: string;
  primaryLocationId: string | null;
};

const teamSchema = object({
  name: string().min(3, 'Team name must be at least three characters long.').required('Team name is required'),
  about: string().nullable(),
  timezone: string().nullable(),
  primaryLocationId: string().nullable(),
});

type CustomerDetails = {
  uniqueId: string;
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
  email: string | null | undefined;
  phoneNumber: string | null | undefined;
  role: TeamMemberRole | null | undefined;
  status: boolean;
};

const OrganizationTeam = ({ rootDataRelay, onReloadRequired, rootDataTeamMembersRelay, organizationId, teamId }: Props) => {
  const rootData = useFragment<organizationTeam_query$key>(
    graphql`
      fragment organizationTeam_query on Query {
        team(id: $teamId) {
          id
          name
          about
          timezone
          primaryFeatureImageUrl
          primaryLocation {
            uniqueId
            name
          }
        }
        teamMemberRoles
        ...singleChoiceLocation_locations_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataTeamMembers, refetchTeamMembers] = useRefetchableFragment<organizationTeam_teamMembers_refetchableFragment, organizationTeam_teamMembers_query$key>(
    graphql`
      fragment organizationTeam_teamMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationTeam_teamMembers_refetchableFragment") {
        teamMembers(first: $count, after: $cursor, where: { teamId: $teamId, nameContains: $peopleNameSearchText }) @connection(key: "teamMembers_teamMembers") {
          __id
          totalCount
          edges {
            node {
              id
              customer {
                uniqueId
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
    `,
    rootDataTeamMembersRelay,
  );

  const [commitUpdateTeam] = useMutation<organizationTeam_updateTeamMutation>(graphql`
    mutation organizationTeam_updateTeamMutation($input: UpdateTeamInput!) @raw_response_type {
      updateTeam(input: $input) {
        team {
          id
          name
          about
          timezone
          primaryFeatureImageUrl
          primaryLocation {
            uniqueId
            name
          }
        }
      }
    }
  `);

  const [commitChangeTeamMembersStatus] = useMutation<organizationTeam_changeTeamMembersStatusMutation>(graphql`
    mutation organizationTeam_changeTeamMembersStatusMutation($input: ChangeTeamMembersStatusInput!) {
      changeTeamMembersStatus(input: $input) {
        members {
          id
          customer {
            uniqueId
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

  const [commitRemoveTeamMembers] = useMutation<organizationTeam_removeTeamMembersMutation>(graphql`
    mutation organizationTeam_removeTeamMembersMutation($connectionIds: [ID!]!, $input: RemoveTeamMembersInput!) {
      removeTeamMembers(input: $input) {
        members {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitChangeTeamMemberRole] = useMutation<organizationTeam_changeTeamMemberRoleMutation>(graphql`
    mutation organizationTeam_changeTeamMemberRoleMutation($input: ChangeTeamMemberRoleInput!) @raw_response_type {
      changeTeamMemberRole(input: $input) {
        member {
          id
          customer {
            uniqueId
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

  const [commitDeleteTeam] = useMutation<organizationTeam_deleteTeamMutation>(graphql`
    mutation organizationTeam_deleteTeamMutation($input: DeleteTeamInput!) {
      deleteTeam(input: $input) {
        team {
          id
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const [, startTransition] = useTransition();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const [seledctedMembers, setSeledctedMembers] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
  const validate = makeValidate(teamSchema);
  const [primaryFeatureImageUrl, setPrimaryFeatureImageUrl] = useState(rootData.team?.primaryFeatureImageUrl);
  const requiredTeamDetailsFields = makeRequired(teamSchema);
  const [selectedMemberId, setSelectedMemberId] = useState<null | string>(null);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateTeamMember],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateTeamMember],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveTeamMember],
  ];

  const memberDetails = useMemo(
    () => rootDataTeamMembers.teamMembers?.edges.map(({ node }) => node).find((item) => item.id === selectedMemberId),
    [selectedMemberId, rootDataTeamMembers.teamMembers],
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

  const connectionIds = useMemo(() => [rootDataTeamMembers.teamMembers.__id], [rootDataTeamMembers.teamMembers]);
  const members = useMemo(
    () =>
      rootDataTeamMembers.teamMembers
        ? rootDataTeamMembers.teamMembers.edges
            .map(({ node }) => node)
            .sort((a, b) => {
              const name1 = getCustomerFullName(a.customer);
              const name2 = getCustomerFullName(b.customer);

              return name1.localeCompare(name2);
            })
        : [],
    [rootDataTeamMembers.teamMembers],
  );

  const handleRefetchTeamMembers = useCallback(
    (peopleNameSearchText: string) => {
      startTransition(() => {
        refetchTeamMembers(
          {
            peopleNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchTeamMembers],
  );

  const handleSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetchTeamMembers(str);
  };

  const handleSelectedMembersChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedMembers(newRowSelectionModel);
  };

  const handleTeamDetailUpdateClick = ({ name, about, timezone, primaryLocationId }: TeamDetails) => {
    const team = rootData.team;
    if (!team) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating team '${team.name}'...`} />, infoNotificationOptions);

    commitUpdateTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: team.id,
          name,
          about,
          timezone,
          primaryFeatureImageUrl,
          primaryLocationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update team '${team?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Team ${name} details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update team '${team?.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateTeam: {
          team: {
            id: team.id,
            name,
            about,
            timezone,
            primaryFeatureImageUrl,
            primaryLocation: null,
          },
        },
      },
    });
  };

  const handleCloseClick = () => {
    router.push(getOrganizationTeamsBaseLink(integratedPlatrform, organizationId));
  };

  const handleDeactivateMembersClick = () => {
    const toastId = themedToast(<NotificationContent content={'Deactivating members...'} />, infoNotificationOptions);

    commitChangeTeamMembersStatus({
      variables: {
        input: {
          clientMutationId: nanoid(),
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
            render: <NotificationContent content={`Failed to deactivate members. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Members deactivated.'} />,
        });
        setSeledctedMembers(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate members. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateMembersClick = () => {
    const toastId = themedToast(<NotificationContent content={'Activating members...'} />, infoNotificationOptions);

    commitChangeTeamMembersStatus({
      variables: {
        input: {
          clientMutationId: nanoid(),
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
            render: <NotificationContent content={`Failed to activate members. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Members activated.'} />,
        });
        setSeledctedMembers(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate members. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveMembersClick = () => {
    const toastId = themedToast(<NotificationContent content={'Removing members...'} />, infoNotificationOptions);

    commitRemoveTeamMembers({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
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
            render: <NotificationContent content={`Failed to remove members. Error: ${joinErrors(errors)}`} />,
          });
          return;
        }
        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Members removed.'} />,
        });
        setSeledctedMembers(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove members. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.DeactivateTeamMember:
        handleDeactivateMemberClick();
        break;

      case MoreActionsMenuOptionType.ActivateTeamMember:
        handleActivateMemberClick();
        break;

      case MoreActionsMenuOptionType.RemoveTeamMember:
        handleRemoveMemberClick();
        break;
    }
  };

  const handleDeactivateMemberClick = () => {
    if (!memberDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Deactivating member...'} />, infoNotificationOptions);

    commitChangeTeamMembersStatus({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: [memberDetails.id],
          status: 'INACTIVE',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate member. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Member deactivated.'} />,
        });
        setSeledctedMembers(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate member. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateMemberClick = () => {
    if (!memberDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Activating member...'} />, infoNotificationOptions);

    commitChangeTeamMembersStatus({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: [memberDetails.id],
          status: 'ACTIVE',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate member. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Member activated.'} />,
        });
        setSeledctedMembers(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate member. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveMemberClick = () => {
    if (!memberDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Removing member...'} />, infoNotificationOptions);

    commitRemoveTeamMembers({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: [memberDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove member. Error: ${joinErrors(errors)}`} />,
          });
          return;
        }
        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Member removed.'} />,
        });
        setSeledctedMembers(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove member. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRoleChanged = (id: string, roleStr: string) => {
    const member = members.find((member) => member.id === id);
    if (!member) {
      return;
    }

    const role = roleStr as unknown as TeamMemberRole;
    const toastId = themedToast(<NotificationContent content={`Updating role...`} />, infoNotificationOptions);

    commitChangeTeamMemberRole({
      variables: {
        input: {
          clientMutationId: nanoid(),
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
        changeTeamMemberRole: {
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

  const handleViewBookingsClick = () => {
    router.push(getOrganizationBookingsBaseLink(integratedPlatrform, organizationId, { teamId }));
  };

  const handleRemoveTeamClicked = () => {
    const team = rootData.team;
    if (!team) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing team '${team.name}'...`} />, infoNotificationOptions);

    commitDeleteTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: team.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the team '${team.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Team '${team.name}' removed.`} />,
        });

        router.push(getOrganizationTeamsBaseLink(integratedPlatrform, organizationId));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the team '${team.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  if (!rootData.team) {
    return <></>;
  }

  const rows: RowType[] = members.map((member) => ({
    id: member.id,
    avatar: member.customer,
    name: getCustomerFullName(member.customer),
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
          {rootData.teamMemberRoles.map((role) => (
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

  const handleFeatureImageUploadCompleted = (url: string) => {
    setPrimaryFeatureImageUrl(url);
  };

  const team = rootData.team;

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationTeamLeftSideNavigationMenuContent organizationId={organizationId} teamId={teamId} hideIcons />
        <Box sx={{ marginLeft: secondDrawerExpandedDrawerWidthPx, flexGrow: 1 }}>
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Team Information">
            <Form
              onSubmit={handleTeamDetailUpdateClick}
              initialValues={{
                name: team.name,
                about: team.about,
                timezone: team.timezone ?? '',
                primaryLocationId: rootData.team.primaryLocation ? rootData.team.primaryLocation.uniqueId : null,
              }}
              validate={validate}
              render={({ handleSubmit }) => (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn
                    sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                    ref={(divElement) => {
                      sectionRefs.current['setup'] = divElement;
                    }}
                  >
                    <GridContainer sx={{ justifyContent: 'space-between' }}>
                      <Grid>
                        <SectionIconTypography label="Team Setup" />
                        <BodyIconTypography label="Edit your team name and details" />
                      </Grid>

                      <Grid>
                        <Button variant="contained" sx={defaultButtonStyle} startIcon={<BookingIcon />} onClick={handleViewBookingsClick}>
                          View Team Bookings
                        </Button>
                      </Grid>
                    </GridContainer>
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Feature Image">
                      {primaryFeatureImageUrl && (
                        <ImageFileUploader
                          defaultImageUrl={primaryFeatureImageUrl}
                          defaultAspectRatio={teamFeatureImageWidth / teamFeatureImageHeight}
                          previewImageHeight={teamFeatureImageHeight}
                          previewImageWidth={teamFeatureImageWidth}
                          onUploadCompleted={handleFeatureImageUploadCompleted}
                        />
                      )}
                    </FormFieldLabel>

                    <FormFieldLabel label="Name">
                      <TextField name="name" required={requiredTeamDetailsFields.name} />
                    </FormFieldLabel>

                    <FormFieldLabel label="About">
                      <TextField name="about" required={requiredTeamDetailsFields.about} multiline rows={3} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Timezone">
                      <SingleChoinceTimezone name="timezone" required={requiredTeamDetailsFields.timezone} />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn
                    sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                    ref={(divElement) => {
                      sectionRefs.current['location'] = divElement;
                    }}
                  >
                    <SectionIconTypography label="Location Settings" />
                    <BodyIconTypography label="Assign team to locations" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Primary Location">
                      <SingleChoiceLocation rootDataRelay={rootData} id="primaryLocationId" required={requiredTeamDetailsFields.primaryLocationId} />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Update
                      </Button>
                    </StackRow>
                  </StackColumn>
                </FormStackColumn>
              )}
            />

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['members'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Team Members" />
                  <BodyIconTypography label="Manage your team members" />
                </Grid>

                <Grid>
                  <AddOrganizationTeamMemberButton onReloadRequired={onReloadRequired} connectionIds={connectionIds} organizationId={organizationId} teamId={teamId} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search size="small" placeholder="Search for members" defaultValue={peopleNameSearchText} onChange={handleSearchTextChange} />
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
                    <SmallIconTypography label={`${seledctedMembers.ids.size} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="secondary" onClick={handleDeactivateMembersClick} sx={defaultButtonStyle}>
                      Deactivate Member
                    </Button>
                    <Button size="medium" variant="contained" color="secondary" onClick={handleActivateMembersClick} sx={defaultButtonStyle}>
                      Activate Member
                    </Button>
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveMembersClick} sx={{ textTransform: 'none' }}>
                      Remove Member
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedMembers}
                onRowSelectionModelChange={handleSelectedMembersChanged}
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
                localeText={{ noRowsLabel: 'No member found' }}
              />
            </StackRow>

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['manage-team'] = divElement;
              }}
            >
              <SectionIconTypography label="Manage" />
              <BodyIconTypography label="Remove your team" />
              <Divider />
            </StackColumn>

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
              <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveTeamClicked} sx={{ textTransform: 'none' }}>
                Remove Team
              </Button>
            </StackRow>
          </AppBarWithStackColumn>
        </Box>
      </Box>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
    </>
  );
};

export default memo(OrganizationTeam);
