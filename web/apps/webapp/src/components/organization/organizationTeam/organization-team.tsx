import { SingleChoiceLocation } from '@/components/location/locationSelector';
import type { organizationTeam_changeTeamMembersStatusMutation } from '@/queries/__generated__/organizationTeam_changeTeamMembersStatusMutation.graphql';
import type { organizationTeam_query$key } from '@/queries/__generated__/organizationTeam_query.graphql';
import type { organizationTeam_removeTeamMembersMutation } from '@/queries/__generated__/organizationTeam_removeTeamMembersMutation.graphql';
import type { organizationTeam_teamMembers_query$key, TeamMemberRole } from '@/queries/__generated__/organizationTeam_teamMembers_query.graphql';
import type { organizationTeam_teamMembers_refetchableFragment } from '@/queries/__generated__/organizationTeam_teamMembers_refetchableFragment.graphql';
import type { organizationTeam_updateTeamMutation } from '@/queries/__generated__/organizationTeam_updateTeamMutation.graphql';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Toolbar from '@mui/material/Toolbar';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import {
  BodyIconTypography,
  FormFieldLabel,
  FormStackColumn,
  PushToRight,
  SectionIconTypography,
  SmallHeadingIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@repo/shared/components/commons';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import { DeleteIcon, EllipseMenuIcon } from '@repo/shared/components/icons';
import {
  MoreActionsMenu,
  moreActionsMenuAllOptions,
  MoreActionsMenuItemType,
  MoreActionsMenuOptionType,
} from '@repo/shared/components/moreActionsMenu';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { Search } from '@repo/shared/components/search';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { coal, defaultGridActionPadding, defaultGridStyle, defaultPadding, emerald, flame, maxScreenWidth, sandstone } from '@repo/shared/libs/theme';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import { getModernOrganizationTeamsBaseLink } from '../organization-link';
import { expandedDrawerWidthPx } from './commons';
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
  primaryLocationId?: string;
};

const teamSchema = object({
  name: string().min(3, 'Team name must be at least three charcters long.').required('Team name is required'),
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
  moreActions: string;
};

const OrganizationTeam = ({ rootDataRelay, rootDataTeamMembersRelay, organizationId, teamId }: Props) => {
  const rootData = useFragment<organizationTeam_query$key>(
    graphql`
      fragment organizationTeam_query on Query {
        team(id: $teamId) {
          id
          name
          about
          timezone
          primaryLocation {
            uniqueId
            name
          }
        }
        ...singleChoiceLocation_locations_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataTeamMembers, refetchTeamMembers] = useRefetchableFragment<
    organizationTeam_teamMembers_refetchableFragment,
    organizationTeam_teamMembers_query$key
  >(
    graphql`
      fragment organizationTeam_teamMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationTeam_teamMembers_refetchableFragment") {
        teamMembers(first: $count, after: $cursor, where: { teamId: $teamId, nameContains: $peopleNameSearchText })
          @connection(key: "teamMembers_teamMembers") {
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

  const [, startTransition] = useTransition();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const [seledctedMembers, setSeledctedMembers] = useState<GridRowSelectionModel>([]);
  const validate = makeValidate(teamSchema);
  const requiredFields = makeRequired(teamSchema);
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

  const connectionIds = useMemo(
    () => (rootDataTeamMembers.teamMembers ? [rootDataTeamMembers.teamMembers.__id] : []),
    [rootDataTeamMembers.teamMembers],
  );
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

  const handleDetailUpdateClick = ({ name, about, timezone, primaryLocationId }: TeamDetails) => {
    if (!rootData.team) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating team '${rootData.team.name}'...`} />, infoNotificationOptions);

    commitUpdateTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.team.id,
          name,
          about,
          timezone,
          organizationId,
          primaryLocationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update team '${rootData.team?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Team ${name} details updated.`} />,
        });

        router.push(getModernOrganizationTeamsBaseLink(organizationId));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update team '${rootData.team?.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateTeam: {
          team: {
            id: rootData.team.id,
            name,
            about,
            timezone,
            primaryLocation: null,
          },
        },
      },
    });
  };

  const handleCancelClick = () => {
    router.push(getModernOrganizationTeamsBaseLink(organizationId));
  };

  const handleDeactivateMembersClick = () => {
    const toastId = themedToast(<NotificationContent content={'Deactivating members...'} />, infoNotificationOptions);

    commitChangeTeamMembersStatus({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: seledctedMembers.map((id) => id as string),
          status: 'Inactive',
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
        setSeledctedMembers([]);
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
          ids: seledctedMembers.map((id) => id as string),
          status: 'Active',
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
        setSeledctedMembers([]);
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
          ids: seledctedMembers.map((id) => id as string),
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
        setSeledctedMembers([]);
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
          status: 'Inactive',
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
        setSeledctedMembers([]);
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
          status: 'Active',
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
        setSeledctedMembers([]);
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
        setSeledctedMembers([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove member. Error: ${error.message}.`} />,
        });
      },
    });
  };

  if (!rootData.team) {
    return null;
  }

  const rows: RowType[] = members.map((member) => ({
    id: member.id,
    avatar: member.customer,
    name: getCustomerFullName(member.customer),
    email: member.customer.email,
    phoneNumber: member.customer.phoneNumber,
    role: member.role,
    status: member.status === 'Active',
    moreActions: member.id,
  }));

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'avatar',
      headerName: '',
      editable: false,
      renderCell: (params) => (
        <CustomerAvatar key={params.value?.uniqueId} name={params.value} photo={{ url: params.value?.photoUrl }} size="medium" showFullName />
      ),
      display: 'flex',
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
      minWidth: 250,
    },
    {
      field: 'role',
      headerName: 'Role',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 100,
    },
    {
      field: 'status',
      headerName: 'Status',
      editable: false,
      renderCell: (params) => (
        <StackRow>
          {params.value && (
            <StackRow sx={{ flexWrap: undefined }}>
              <SmallIconTypography label="Active" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: emerald }} />
            </StackRow>
          )}
          {!params.value && (
            <StackRow sx={{ flexWrap: undefined }}>
              <SmallIconTypography label="Deactive" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: flame }} />
            </StackRow>
          )}
        </StackRow>
      ),
      display: 'flex',
    },
    {
      field: 'moreActions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <IconButton
          onClick={(event: React.MouseEvent<HTMLElement>) => {
            setSelectedMemberId(params.value);
            setMoreActionsAnchorEl(event.currentTarget);
          }}
        >
          <EllipseMenuIcon />
        </IconButton>
      ),
    },
  ];

  if (!rootData.team) {
    return <></>;
  }

  const team = rootData.team;

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationTeamLeftSideNavigationMenuContent organizationId={organizationId} teamId={teamId} hideIcons />
        <Box sx={{ marginLeft: expandedDrawerWidthPx, flexGrow: 1 }}>
          <Form
            onSubmit={handleDetailUpdateClick}
            initialValues={{
              name: team.name,
              about: team.about,
              timezone: team.timezone,
              primaryLocationId: rootData.team.primaryLocation ? rootData.team.primaryLocation.uniqueId : null,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit} spacing={0} sx={{ padding: 0 }}>
                <AppBar position="sticky">
                  <Toolbar
                    sx={{
                      backgroundColor: paletteMode === 'dark' ? sandstone : coal,
                      borderBottom: paletteMode === 'dark' ? 1 : undefined,
                      borderColor: (theme) => theme.palette.divider,
                    }}
                  >
                    <SmallHeadingIconTypography label="Edit Team Information" invertDefaultColor />

                    <PushToRight />
                    <StackRow>
                      <Button
                        sx={{
                          border: 1,
                          borderColor: paletteMode === 'dark' ? coal : sandstone,
                          textTransform: 'none',
                          '&:hover': {
                            backgroundColor: 'inherit',
                          },
                        }}
                        variant="contained"
                        color="inherit"
                        onClick={handleCancelClick}
                      >
                        <SmallIconTypography label="Cancel" invertDefaultColor />
                      </Button>

                      <Button variant="contained" color="primary" type="submit" sx={{ textTransform: 'none' }}>
                        <SmallIconTypography label="Save & Exit" />
                      </Button>
                    </StackRow>
                  </Toolbar>
                </AppBar>

                <StackColumn sx={{ maxWidth: maxScreenWidth }}>
                  <StackColumn
                    sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                    ref={(divElement) => {
                      sectionRefs.current['setup'] = divElement;
                    }}
                  >
                    <SectionIconTypography label="Team Setup" />
                    <BodyIconTypography label="Edit your team name and details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Name">
                      <TextField name="name" required={requiredFields.name} />
                    </FormFieldLabel>

                    <FormFieldLabel label="About">
                      <TextField name="about" required={requiredFields.about} multiline={true} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Timezone">
                      <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn
                    sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                    ref={(divElement) => {
                      sectionRefs.current['members'] = divElement;
                    }}
                  >
                    <SectionIconTypography label="Team Members" />
                    <BodyIconTypography label="Manage your team members" />
                    <Divider />
                  </StackColumn>

                  <StackRow sx={{ padding: defaultPadding }}>
                    <PushToRight />
                    <Search size="small" placeholder="Search for members" defaultValue={peopleNameSearchText} onChange={handleSearchTextChange} />
                  </StackRow>

                  <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
                    <Box
                      sx={{
                        backgroundColor: (theme) => theme.palette.background.paper,
                        padding: defaultGridActionPadding,
                        border: 1,
                        borderColor: (theme) => theme.palette.divider,
                        borderRadius: 2,
                        flexGrow: 1,
                      }}
                    >
                      <StackRow sx={{ alignItems: 'center' }}>
                        <SmallIconTypography label={`${seledctedMembers.length} records selected`} />
                        <PushToRight />
                        <Button
                          size="medium"
                          variant="contained"
                          color="secondary"
                          disabled={seledctedMembers.length === 0}
                          onClick={handleDeactivateMembersClick}
                        >
                          Deactuvate Member
                        </Button>
                        <Button
                          size="medium"
                          variant="contained"
                          color="secondary"
                          disabled={seledctedMembers.length === 0}
                          onClick={handleActivateMembersClick}
                        >
                          Activate Member
                        </Button>
                        <Button
                          size="medium"
                          variant="contained"
                          color="warning"
                          startIcon={<DeleteIcon />}
                          disabled={seledctedMembers.length === 0}
                          onClick={handleRemoveMembersClick}
                        >
                          Remove Member
                        </Button>
                      </StackRow>
                    </Box>
                  </StackRow>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
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
                    />
                  </StackColumn>
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
                    <SingleChoiceLocation rootDataRelay={rootData} id="primaryLocationId" required={requiredFields.primaryLocationId} />
                  </FormFieldLabel>
                </StackColumn>
              </FormStackColumn>
            )}
          />
        </Box>
      </Box>

      <MoreActionsMenu
        anchorEl={moreActionsAnchorEl}
        open={moreActionsMenuOpen}
        onMenuItemClick={handleMoreActionsMenuItemClick}
        options={moreActionsOption}
      />
    </>
  );
};

export default memo(OrganizationTeam);
