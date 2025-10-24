import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
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
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';
import OrganizationTeamLeftSideNavigationMenuContent from './organization-team-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationTeam_query$key;
  rootDataTeamMembersRelay: organizationTeam_teamMembers_query$key;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
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
  email: string | null | undefined;
  phoneNumber: string | null | undefined;
  role: TeamMemberRole | null | undefined;
  status: boolean;
};

const OrganizationTeam = ({ rootDataRelay, onReloadRequired, rootDataTeamMembersRelay, organizationUniqueAlphanumericName, teamId }: Props) => {
  const rootData = useFragment<organizationTeam_query$key>(
    graphql`
      fragment organizationTeam_query on Query {
        team(id: $teamId) {
          id
          name
          about
          timezone
          primaryFeatureImage {
            original {
              url
              height
              width
            }
            thumbnail {
              url
              height
              width
            }
          }
          primaryLocation {
            id
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
        team(id: $teamId) {
          members(first: $count, after: $cursor, where: { nameContains: $peopleNameSearchText }) @connection(key: "teamMembers_members") {
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
          primaryFeatureImage {
            original {
              url
              height
              width
            }
            thumbnail {
              url
              height
              width
            }
          }
          primaryLocation {
            id
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
  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse | null>(
    rootData.team?.primaryFeatureImage && rootData.team?.primaryFeatureImage.original
      ? {
          id: '',
          original: {
            url: rootData.team?.primaryFeatureImage.original.url,
            height: rootData.team?.primaryFeatureImage.original.height,
            width: rootData.team?.primaryFeatureImage.original.width,
          },
          thumbnail: rootData.team?.primaryFeatureImage.thumbnail
            ? {
                url: rootData.team?.primaryFeatureImage.thumbnail.url,
                height: rootData.team?.primaryFeatureImage.thumbnail.height,
                width: rootData.team?.primaryFeatureImage.thumbnail.width,
              }
            : null,
        }
      : null,
  );
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
    () => rootDataTeamMembers.team?.members?.edges.map(({ node }) => node).find((item) => item.id === selectedMemberId),
    [selectedMemberId, rootDataTeamMembers.team?.members],
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

  const connectionIds = useMemo(() => (rootDataTeamMembers.team ? [rootDataTeamMembers.team.members.__id] : []), [rootDataTeamMembers.team]);
  const members = useMemo(
    () =>
      rootDataTeamMembers.team?.members
        ? rootDataTeamMembers.team.members.edges
            .map(({ node }) => node)
            .sort((a, b) => {
              const name1 = getCustomerFullName(a.customer);
              const name2 = getCustomerFullName(b.customer);

              return name1.localeCompare(name2);
            })
        : [],
    [rootDataTeamMembers.team],
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
    [startTransition, refetchTeamMembers],
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
    const finalPrimaryFeatureImage = primaryFeatureImage
      ? {
          original: primaryFeatureImage.original
            ? { url: primaryFeatureImage.original.url, height: primaryFeatureImage.original.height, width: primaryFeatureImage.original.width }
            : null,
          thumbnail: primaryFeatureImage.thumbnail
            ? { url: primaryFeatureImage.thumbnail.url, height: primaryFeatureImage.thumbnail.height, width: primaryFeatureImage.thumbnail.width }
            : null,
        }
      : null;

    commitUpdateTeam({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: team.id,
          name,
          about,
          timezone,
          primaryFeatureImage: finalPrimaryFeatureImage,
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
            primaryFeatureImage: finalPrimaryFeatureImage,
            primaryLocation: null,
          },
        },
      },
    });
  };

  const handleCloseClick = () => {
    router.push(getOrganizationTeamsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName));
  };

  const handleDeactivateMembersClick = () => {
    const toastId = themedToast(<NotificationContent content={'Deactivating members...'} />, infoNotificationOptions);

    commitChangeTeamMembersStatus({
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
          clientMutationId: uuid(),
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
          clientMutationId: uuid(),
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
          clientMutationId: uuid(),
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
    router.push(getOrganizationBookingsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, { teamId }));
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
          clientMutationId: uuid(),
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

        router.push(getOrganizationTeamsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName));
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
    return null;
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

  const handleFeatureImageUploadCompleted = (response: FileUploadResponse) => {
    setPrimaryFeatureImage(response);
  };

  const team = rootData.team;

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationTeamLeftSideNavigationMenuContent organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} teamId={teamId} hideIcons />
        <Box sx={{ marginLeft: secondDrawerExpandedDrawerWidthPx, flexGrow: 1 }}>
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Team Information">
            <Form
              onSubmit={handleTeamDetailUpdateClick}
              initialValues={{
                name: team.name,
                about: team.about,
                timezone: team.timezone ?? '',
                primaryLocationId: rootData.team.primaryLocation ? rootData.team.primaryLocation.id : null,
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
                      <StackColumn>
                        {primaryFeatureImage?.thumbnail && primaryFeatureImage.original.height && primaryFeatureImage.original.width && (
                          <>
                            {/* eslint-disable-next-line @next/next/no-img-element */}
                            <img src={primaryFeatureImage.original.url} height={200} width={400} alt="" style={{ objectFit: 'cover' }} />
                          </>
                        )}
                        <ImageFileUploader onUploadCompleted={handleFeatureImageUploadCompleted} />
                      </StackColumn>
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
                  <AddOrganizationTeamMemberButton
                    onReloadRequired={onReloadRequired}
                    connectionIds={connectionIds}
                    organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                    teamId={teamId}
                  />
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
