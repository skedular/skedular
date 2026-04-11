import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
import { BodyIconTypography, FormFieldLabel, FormStackColumn, GridContainer, SectionIconTypography, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { SingleChoinceTimezone } from '@/components/forms';
import { DeleteIcon } from '@/components/icons';
import { getOrganizationTeamsBaseLink } from '@/components/links';
import { SingleChoiceLocation } from '@/components/location/locationSelector';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { AddOrganizationTeamMemberButton } from '@/components/organization/addOrganizationTeamMember';
import OrganizationTeamMemberManagementList from '@/components/organization/organizationTeam/organization-team-member-management-list';
import OrganizationTeamSectionNav, { OrganizationTeamSection } from '@/components/organization/organizationTeam/organization-team-section-nav';
import { Search } from '@/components/search';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultPadding } from '@/libs/theme';
import { getCustomerFullName, getRelayErrorMessage } from '@/libs/utils';
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
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import { EditorActionBar, PageHeaderPanel, SettingsSectionCard } from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  rootDataRelay: organizationTeam_query$key;
  rootDataTeamMembersRelay: organizationTeam_teamMembers_query$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
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

const getActiveSection = (value: string | null): OrganizationTeamSection => {
  switch (value) {
    case 'location':
      return 'location';
    case 'members':
      return 'members';
    case 'manage-team':
      return 'manage-team';
    case 'setup':
    default:
      return 'setup';
  }
};

const formColumnSx = {
  width: '100%',
  maxWidth: 760,
};

const OrganizationTeam = ({ rootDataRelay, onReloadRequired, rootDataTeamMembersRelay, organizationCustomDomain, teamId }: Props) => {
  const rootData = useFragment<organizationTeam_query$key>(
    graphql`
      fragment organizationTeam_query on Query {
        team(id: $teamId) {
          id
          name
          about
          timezone
          featureImages {
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
        teamMemberRoles {
          type
          name
        }
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
          featureImages {
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
  const activeSection = getActiveSection(section);
  const [stickyTop, setStickyTop] = useState(0);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const [selectedMemberIds, setSelectedMemberIds] = useState<string[]>([]);
  const validate = makeValidate(teamSchema);
  const [featureImages, setFeatureImages] = useState<FileUploadResponse[]>(
    rootData.team
      ? rootData.team.featureImages
          .filter((item) => !!item.original)
          .map((item) => ({
            id: '',
            original: {
              url: item.original!.url,
              height: item.original!.height,
              width: item.original!.width,
            },
            thumbnail: item.thumbnail
              ? {
                  url: item.thumbnail.url,
                  height: item.thumbnail.height,
                  width: item.thumbnail.width,
                }
              : null,
          }))
      : [],
  );
  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse | null>(featureImages[0] ?? null);

  const requiredTeamDetailsFields = makeRequired(teamSchema);
  const [selectedMemberId, setSelectedMemberId] = useState<null | string>(null);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const [changeRoleAnchorEl, setChangeRoleAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const changeRoleMenuOpen = Boolean(changeRoleAnchorEl);

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
    const updateStickyTop = () => {
      setStickyTop(document.querySelector('.app-bar')?.clientHeight ?? 0);
    };

    updateStickyTop();
    window.addEventListener('resize', updateStickyTop);

    return () => {
      window.removeEventListener('resize', updateStickyTop);
    };
  }, []);

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
  const teamMemberRoleNameByType = useMemo(() => new Map(rootData.teamMemberRoles.map((item) => [item.type, item.name])), [rootData.teamMemberRoles]);

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

  const handleSelectedMembersChanged = (memberId: string) => {
    setSelectedMemberIds((current) => (current.includes(memberId) ? current.filter((id) => id !== memberId) : current.concat(memberId)));
  };

  const handleTeamDetailUpdateClick = ({ name, about, timezone, primaryLocationId }: TeamDetails) => {
    const team = rootData.team;
    if (!team) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating team '${team.name}'...`} />, infoNotificationOptions);
    const finalFeatureImages = featureImages.map((image) => ({
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    }));

    commitUpdateTeam({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: team.id,
          name,
          about,
          timezone,
          featureImages: finalFeatureImages,
          primaryLocationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update team '${team?.name}'. Error: ${getRelayErrorMessage(errors)}.`} />,
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
            featureImages: finalFeatureImages,
            primaryLocation: null,
          },
        },
      },
    });
  };

  const handleDeactivateMembersClick = () => {
    const toastId = themedToast(<NotificationContent content={'Deactivating members...'} />, infoNotificationOptions);

    commitChangeTeamMembersStatus({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: selectedMemberIds,
          status: 'INACTIVE',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate members. Error: ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Members deactivated.'} />,
        });
        setSelectedMemberIds([]);
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
          ids: selectedMemberIds,
          status: 'ACTIVE',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate members. Error: ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Members activated.'} />,
        });
        setSelectedMemberIds([]);
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
          ids: selectedMemberIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove members. Error: ${getRelayErrorMessage(errors)}`} />,
          });
          return;
        }
        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Members removed.'} />,
        });
        setSelectedMemberIds([]);
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

  const handleChangeRoleClick = (memberId: string, target: HTMLElement) => {
    setSelectedMemberId(memberId);
    setChangeRoleAnchorEl(target);
  };

  const handleChangeRoleMenuItemClick = (role: TeamMemberRole) => {
    if (!selectedMemberId) {
      return;
    }

    setChangeRoleAnchorEl(null);
    handleRoleChanged(selectedMemberId, role);
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
            render: <NotificationContent content={`Failed to deactivate member. Error: ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Member deactivated.'} />,
        });
        setSelectedMemberIds([]);
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
            render: <NotificationContent content={`Failed to activate member. Error: ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Member activated.'} />,
        });
        setSelectedMemberIds([]);
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
            render: <NotificationContent content={`Failed to remove member. Error: ${getRelayErrorMessage(errors)}`} />,
          });
          return;
        }
        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Member removed.'} />,
        });
        setSelectedMemberIds([]);
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
            render: <NotificationContent content={`Failed to update role to ${role}. Error: ${getRelayErrorMessage(errors)}.`} />,
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
            role: {
              type: role,
              name: teamMemberRoleNameByType.get(role) ?? role,
            },
          },
        },
      },
    });
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
            render: <NotificationContent content={`Failed to remove the team '${team.name}'. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Team '${team.name}' removed.`} />,
        });

        router.push(getOrganizationTeamsBaseLink(integratedPlatrform, organizationCustomDomain));
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

  const handleFeatureImageUploadCompleted = (response: FileUploadResponse) => {
    setFeatureImages((prev) => [response, ...prev]);
    setPrimaryFeatureImage((prevPrimary) => prevPrimary ?? response);
  };

  const handleRemoveFeatureImage = (image: FileUploadResponse) => {
    setFeatureImages((prev) => {
      const next = prev.filter((item) => item.original?.url !== image.original?.url);

      if (primaryFeatureImage?.original?.url === image.original?.url) {
        setPrimaryFeatureImage(next[0] ?? null);
      }

      return next;
    });
  };

  const handleSetPrimaryFeatureImage = (image: FileUploadResponse) => {
    setPrimaryFeatureImage(image);
    setFeatureImages((prev) => [image, ...prev.filter((item) => item.original?.url !== image.original?.url)]);
  };

  const team = rootData.team;
  if (!team) {
    return null;
  }

  const teamFormInitialValues = {
    name: team.name,
    about: team.about,
    timezone: team.timezone ?? '',
    primaryLocationId: rootData.team.primaryLocation ? rootData.team.primaryLocation.id : null,
  };

  const renderActiveSection = () => {
    switch (activeSection) {
      case 'location':
        return (
          <Form
            onSubmit={handleTeamDetailUpdateClick}
            initialValues={teamFormInitialValues}
            validate={validate}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit} sx={formColumnSx}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Location Settings" />
                  <BodyIconTypography label="Assign team to a primary location." />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Primary Location">
                    <SingleChoiceLocation rootDataRelay={rootData} id="primaryLocationId" required={requiredTeamDetailsFields.primaryLocationId} />
                  </FormFieldLabel>
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding, paddingBottom: defaultPadding }}>
                  <EditorActionBar primaryAction="Update" />
                </StackColumn>
              </FormStackColumn>
            )}
          />
        );
      case 'members':
        return (
          <Box sx={{ p: defaultPadding }}>
            <SettingsSectionCard
              title="Team Members"
              description="Manage membership, roles, and activation status for this team."
              actions={
                <AddOrganizationTeamMemberButton
                  onReloadRequired={onReloadRequired}
                  connectionIds={connectionIds}
                  organizationCustomDomain={organizationCustomDomain}
                  teamId={teamId}
                />
              }
            >
              <StackColumn spacing={2}>
                <StackRow sx={{ justifyContent: 'flex-end' }}>
                  <Search size="small" placeholder="Search for members" defaultValue={peopleNameSearchText} onChange={handleSearchTextChange} />
                </StackRow>

                <OrganizationTeamMemberManagementList
                  items={memberItems}
                  selectedIds={selectedMemberIds}
                  onToggleSelected={handleSelectedMembersChanged}
                  onOpenChangeRole={handleChangeRoleClick}
                  onOpenMoreActions={(memberId, target) => {
                    setSelectedMemberId(memberId);
                    setMoreActionsAnchorEl(target);
                  }}
                  onDeactivateSelected={() => handleDeactivateMembersClick()}
                  onActivateSelected={() => handleActivateMembersClick()}
                  onRemoveSelected={() => handleRemoveMembersClick()}
                />
              </StackColumn>
            </SettingsSectionCard>
          </Box>
        );
      case 'manage-team':
        return (
          <>
            <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
              <SectionIconTypography label="Manage" />
              <BodyIconTypography label="Remove your team" />
              <Divider />
            </StackColumn>

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding, paddingBottom: defaultPadding }}>
              <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveTeamClicked} sx={{ textTransform: 'none' }}>
                Remove Team
              </Button>
            </StackRow>
          </>
        );
      case 'setup':
      default:
        return (
          <Form
            onSubmit={handleTeamDetailUpdateClick}
            initialValues={teamFormInitialValues}
            validate={validate}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit} sx={formColumnSx}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <GridContainer sx={{ justifyContent: 'space-between' }}>
                    <Grid>
                      <SectionIconTypography label="Team Setup" />
                      <BodyIconTypography label="Edit your team name and details" />
                    </Grid>
                  </GridContainer>
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Feature Images">
                    <StackColumn>
                      <Box
                        sx={{
                          display: 'grid',
                          gridTemplateColumns: { xs: 'repeat(auto-fill, minmax(140px, 1fr))', sm: 'repeat(auto-fill, minmax(180px, 1fr))' },
                          gap: 2,
                        }}
                      >
                        {featureImages.map((image, index) => (
                          <Box
                            key={index}
                            sx={{
                              position: 'relative',
                              borderRadius: 2,
                              overflow: 'hidden',
                              border: 1,
                              borderColor: 'divider',
                              backgroundColor: paletteMode === 'dark' ? 'grey.900' : 'grey.50',
                            }}
                          >
                            {/* eslint-disable-next-line @next/next/no-img-element */}
                            <img src={image.original?.url ?? image.thumbnail?.url ?? ''} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                            <StackRow sx={{ position: 'absolute', top: 8, right: 8 }}>
                              <IconButton size="small" aria-label="Remove feature image" onClick={() => handleRemoveFeatureImage(image)}>
                                <DeleteIcon fontSize="small" />
                              </IconButton>
                            </StackRow>
                            <StackRow sx={{ position: 'absolute', left: 8, bottom: 8 }}>
                              {primaryFeatureImage?.original?.url === image.original?.url ? (
                                <Chip size="small" color="success" label="Cover image" />
                              ) : (
                                <Button variant="contained" size="small" onClick={() => handleSetPrimaryFeatureImage(image)} sx={{ textTransform: 'none' }}>
                                  Make cover
                                </Button>
                              )}
                            </StackRow>
                          </Box>
                        ))}
                      </Box>
                      <ImageFileUploaderWithCropper onUploadCompleted={handleFeatureImageUploadCompleted} />
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

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding, paddingBottom: defaultPadding }}>
                  <EditorActionBar primaryAction="Update" />
                </StackColumn>
              </FormStackColumn>
            )}
          />
        );
    }
  };

  return (
    <>
      <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pb: defaultPadding }}>
        <StackColumn
          sx={{
            width: '100%',
            maxWidth: 1120,
            mx: 'auto',
            backgroundColor: 'transparent',
            gap: 2,
          }}
        >
          <PageHeaderPanel eyebrow="Team settings" title={team.name} description="Manage team details, location assignment, members, and lifecycle controls.">
            <StackColumn spacing={0.5}>
              <SmallIconTypography label="Setup & operations" />
              <BodyIconTypography label={team.about || team.name} />
            </StackColumn>
          </PageHeaderPanel>

          <OrganizationTeamSectionNav activeSection={activeSection} organizationCustomDomain={organizationCustomDomain} teamId={teamId} stickyTop={stickyTop} />

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
            {renderActiveSection()}
          </Box>
        </StackColumn>
      </Box>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
      <Menu anchorEl={changeRoleAnchorEl} open={changeRoleMenuOpen} onClose={() => setChangeRoleAnchorEl(null)}>
        {rootData.teamMemberRoles.map((item) => (
          <MenuItem key={item.type} selected={memberDetails?.role.type === item.type} onClick={() => handleChangeRoleMenuItemClick(item.type)}>
            <SmallIconTypography label={item.name} />
          </MenuItem>
        ))}
      </Menu>
    </>
  );
};

export default memo(OrganizationTeam);
