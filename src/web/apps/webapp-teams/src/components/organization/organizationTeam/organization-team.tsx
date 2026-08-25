import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { SingleChoinceTimezone } from '@/components/forms';
import { DeleteIcon } from '@/components/icons';
import { getOrganizationTeamsBaseLink } from '@/components/links';
import { SingleChoiceLocation } from '@/components/location/locationSelector';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { AddOrganizationTeamMemberButton } from '@/components/organization/addOrganizationTeamMember';
import OrganizationTeamMemberManagementList from '@/components/organization/organizationTeam/organization-team-member-management-list';
import OrganizationTeamSectionNav, { OrganizationTeamSection } from '@/components/organization/organizationTeam/organization-team-section-nav';
import { Search } from '@/components/search';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import type { organizationTeam_changeTeamMemberRoleMutation } from '@/queries/__generated__/organizationTeam_changeTeamMemberRoleMutation.graphql';
import type { organizationTeam_changeTeamMembersStatusMutation } from '@/queries/__generated__/organizationTeam_changeTeamMembersStatusMutation.graphql';
import type { organizationTeam_deleteTeamMutation } from '@/queries/__generated__/organizationTeam_deleteTeamMutation.graphql';
import type { organizationTeam_query$key } from '@/queries/__generated__/organizationTeam_query.graphql';
import type { organizationTeam_removeTeamMembersMutation } from '@/queries/__generated__/organizationTeam_removeTeamMembersMutation.graphql';
import type { organizationTeam_teamMembers_query$key, TeamMemberRole } from '@/queries/__generated__/organizationTeam_teamMembers_query.graphql';
import type { organizationTeam_teamMembers_refetchableFragment } from '@/queries/__generated__/organizationTeam_teamMembers_refetchableFragment.graphql';
import type { organizationTeam_updateTeamMutation, TeamPatchField } from '@/queries/__generated__/organizationTeam_updateTeamMutation.graphql';
import Box from '@mui/material/Box';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import ExpandMoreRoundedIcon from '@mui/icons-material/ExpandMoreRounded';
import { getCustomerFullName, getRelayErrorMessage, PaletteModeContext, useIntegratedPlatform } from '@skedular/shared';
import {
  BodyIconTypography,
  FeatureImageGallery,
  defaultPadding,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  PageHeaderPanel,
  SectionIconTypography,
  SettingsSectionCard,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
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
  maxWidth: 'none',
};
const teamAutosaveDebounceTimeout = 1000;

const teamPatchFields: Record<keyof TeamDetails, TeamPatchField> = {
  name: 'NAME',
  about: 'ABOUT',
  timezone: 'TIMEZONE',
  primaryLocationId: 'PRIMARY_LOCATION',
};

const getChangedTeamFields = (left: TeamDetails | null, right: TeamDetails): TeamPatchField[] => {
  if (!left) return [];
  return (Object.keys(teamPatchFields) as (keyof TeamDetails)[]).filter((field) => left[field] !== right[field]).map((field) => teamPatchFields[field]);
};

const getValidTeamPatchFields = (fieldsToUpdate: TeamPatchField[], values: TeamDetails): TeamPatchField[] =>
  fieldsToUpdate.filter((patchField) => {
    if (patchField === 'FEATURE_IMAGES') {
      return true;
    }

    const formField = (Object.entries(teamPatchFields) as [keyof TeamDetails, TeamPatchField][]).find(([, field]) => field === patchField)?.[0];
    if (!formField) {
      return false;
    }

    try {
      teamSchema.validateSyncAt(formField, values);
      return true;
    } catch {
      return false;
    }
  });

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

  const { integratedPlatform } = useIntegratedPlatform();
  const [, startTransition] = useTransition();
  const router = useRouter();
  const pathname = usePathname();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const activeSection = getActiveSection(section);
  const presentationExpanded = section !== '';
  const [stickyTop, setStickyTop] = useState(0);
  const togglePresentation = () => {
    const params = new URLSearchParams(searchParams.toString());
    if (presentationExpanded) params.set('section', '');
    else params.set('section', 'setup');
    router.replace(`${pathname}?${params.toString()}`, { scroll: false });
  };
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
  const initialTeamValues = useMemo<TeamDetails | null>(
    () =>
      rootData.team
        ? {
            name: rootData.team.name,
            about: rootData.team.about ?? null,
            timezone: rootData.team.timezone ?? '',
            primaryLocationId: rootData.team.primaryLocation ? rootData.team.primaryLocation.id : null,
          }
        : null,
    [rootData.team],
  );
  const previousTeamValues = useRef<TeamDetails | null>(initialTeamValues);
  const previousFeatureImages = useRef<FileUploadResponse[]>(featureImages);
  const featureImagesRef = useRef<FileUploadResponse[]>(featureImages);

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

  const handleTeamDetailUpdateClick = (fieldsToUpdate: TeamPatchField[], values: TeamDetails) => {
    const { name, about, timezone, primaryLocationId } = values;
    const team = rootData.team;
    const validFieldsToUpdate = getValidTeamPatchFields(fieldsToUpdate, values);
    if (!team || validFieldsToUpdate.length === 0) {
      return;
    }

    const finalFeatureImages = featureImagesRef.current.map((image) => ({
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    }));

    commitUpdateTeam({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: team.id,
          fieldsToUpdate: validFieldsToUpdate,
          name,
          about,
          timezone,
          featureImages: finalFeatureImages,
          primaryLocationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't update team '${team?.name}'. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't update team '${team?.name}'. ${error.message}`} />, errorNotificationOptions);
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
  const debouncedTeamDetailUpdate = useDebounceCallback(handleTeamDetailUpdateClick, teamAutosaveDebounceTimeout);

  const handleDeactivateMembersClick = () => {
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
          themedToast(<NotificationContent content={`We couldn't deactivate those team members. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedMemberIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't deactivate those team members. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleActivateMembersClick = () => {
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
          themedToast(<NotificationContent content={`We couldn't activate those team members. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedMemberIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't activate those team members. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleRemoveMembersClick = () => {
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
          themedToast(<NotificationContent content={`We couldn't remove those team members. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          return;
        }

        setSelectedMemberIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove those team members. ${error.message}`} />, errorNotificationOptions);
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
          themedToast(<NotificationContent content={`We couldn't deactivate this team member. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedMemberIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't deactivate this team member. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleActivateMemberClick = () => {
    if (!memberDetails) {
      return;
    }

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
          themedToast(<NotificationContent content={`We couldn't activate this team member. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        setSelectedMemberIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't activate this team member. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleRemoveMemberClick = () => {
    if (!memberDetails) {
      return;
    }

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
          themedToast(<NotificationContent content={`We couldn't remove this team member. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          return;
        }

        setSelectedMemberIds([]);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove this team member. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleRoleChanged = (id: string, roleStr: string) => {
    const member = members.find((member) => member.id === id);
    if (!member) {
      return;
    }

    const role = roleStr as unknown as TeamMemberRole;
    const roleName = teamMemberRoleNameByType.get(role) ?? role;
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
          themedToast(<NotificationContent content={`We couldn't change this team member's role to ${roleName}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't change this team member's role to ${roleName}. ${error.message}`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        changeTeamMemberRole: {
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

  const handleRemoveTeamClicked = () => {
    const team = rootData.team;
    if (!team) {
      return;
    }

    commitDeleteTeam({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: team.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't remove team '${team.name}'. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        router.push(getOrganizationTeamsBaseLink(integratedPlatform, organizationCustomDomain));
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove team '${team.name}'. ${error.message}`} />, errorNotificationOptions);
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
    const next = [response, ...featureImagesRef.current];
    featureImagesRef.current = next;
    setFeatureImages(next);
    setPrimaryFeatureImage((prevPrimary) => prevPrimary ?? response);
  };

  const handleRemoveFeatureImage = (image: FileUploadResponse) => {
    const next = featureImagesRef.current.filter((item) => item.original?.url !== image.original?.url);
    featureImagesRef.current = next;
    setFeatureImages(next);
    if (primaryFeatureImage?.original?.url === image.original?.url) setPrimaryFeatureImage(next[0] ?? null);
  };

  const handleSetPrimaryFeatureImage = (image: FileUploadResponse) => {
    setPrimaryFeatureImage(image);
    setFeatureImages((prev) => [image, ...prev.filter((item) => item.original?.url !== image.original?.url)]);
  };

  const team = rootData.team;
  if (!team) {
    return null;
  }

  const renderActiveSection = () => {
    switch (activeSection) {
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
          <Accordion
            disableGutters
            elevation={0}
            expanded={presentationExpanded}
            onChange={togglePresentation}
            sx={{ border: 1, borderColor: 'divider', borderRadius: '16px !important', overflow: 'hidden', '&::before': { display: 'none' } }}
          >
            <AccordionSummary expandIcon={<ExpandMoreRoundedIcon />} sx={{ px: 2.5, py: 0.75, minHeight: 72, '& .MuiAccordionSummary-content': { my: 1 } }}>
              <StackColumn spacing={0.35} sx={{ minWidth: 0 }}>
                <LeadIconTypography label="Presentation" />
                <BodyIconTypography label={presentationExpanded ? 'Shape the customer-facing identity and settings for this team.' : 'Team profile and presentation details'} />
              </StackColumn>
            </AccordionSummary>
            <AccordionDetails sx={{ borderTop: 1, borderColor: 'divider', p: 0 }}>
              <Form
                onSubmit={() => undefined}
                initialValues={initialTeamValues ?? undefined}
                validate={validate}
                render={({ handleSubmit, values }) => {
                  const teamValues = values as TeamDetails;
                  const changedFormFields = getChangedTeamFields(previousTeamValues.current, teamValues);
                  const extraFields: TeamPatchField[] = featureImages !== previousFeatureImages.current ? ['FEATURE_IMAGES'] : [];
                  const fieldsToUpdate: TeamPatchField[] = [...changedFormFields, ...extraFields];
                  if (fieldsToUpdate.length > 0) {
                    previousTeamValues.current = teamValues;
                    previousFeatureImages.current = featureImages;
                    debouncedTeamDetailUpdate(fieldsToUpdate, teamValues);
                  }

                  return (
                    <FormStackColumn onSubmit={handleSubmit} sx={formColumnSx}>
                      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'minmax(0, 0.95fr) minmax(0, 1.05fr)' }, gap: { xs: 2, md: 4 }, p: { xs: 1.5, sm: 2 } }}>
                        <StackColumn spacing={2}>
                          <StackColumn spacing={0.5}>
                            <LeadIconTypography label="Cover and gallery" />
                            <BodyIconTypography label="Use a strong cover image to help customers recognize this team." />
                          </StackColumn>
                          <FormFieldLabel label="Feature Images">
                            <FeatureImageGallery
                              images={featureImages}
                              coverImage={primaryFeatureImage}
                              onRemove={handleRemoveFeatureImage}
                              onMakeCover={handleSetPrimaryFeatureImage}
                              uploadControl={<ImageFileUploaderWithCropper onUploadCompleted={handleFeatureImageUploadCompleted} />}
                            />
                          </FormFieldLabel>
                        </StackColumn>
                        <StackColumn spacing={2}>
                          <StackColumn spacing={0.5}>
                            <LeadIconTypography label="Team details" />
                            <BodyIconTypography label="Use concise language to describe this team." />
                          </StackColumn>
                          <FormFieldLabel label="Name">
                            <TextField name="name" required={requiredTeamDetailsFields.name} />
                          </FormFieldLabel>

                          <FormFieldLabel label="About">
                            <TextField name="about" required={requiredTeamDetailsFields.about} multiline rows={3} />
                          </FormFieldLabel>

                          <FormFieldLabel label="Timezone">
                            <SingleChoinceTimezone name="timezone" required={requiredTeamDetailsFields.timezone} />
                          </FormFieldLabel>

                          <FormFieldLabel label="Primary Location">
                            <SingleChoiceLocation rootDataRelay={rootData} id="primaryLocationId" required={requiredTeamDetailsFields.primaryLocationId} />
                          </FormFieldLabel>
                        </StackColumn>
                      </Box>
                    </FormStackColumn>
                  );
                }}
              />
            </AccordionDetails>
          </Accordion>
        );
    }
  };

  return (
    <>
      <Box
        sx={{
          width: '100%',
          maxWidth: '100vw',
          minWidth: 0,
          display: 'flex',
          justifyContent: 'center',
          overflowX: 'hidden',
          boxSizing: 'border-box',
          px: { xs: 0, sm: 1, md: 2 },
          pt: { xs: 1, sm: 1, md: 2 },
          pb: defaultPadding,
        }}
      >
        <StackColumn
          sx={{
            width: '100%',
            maxWidth: 1200,
            minWidth: 0,
            mx: 'auto',
            overflowX: 'hidden',
            backgroundColor: 'transparent',
            gap: 2,
          }}
        >
          <PageHeaderPanel
            eyebrow="Team settings"
            title={team.name}
            description="Manage team details, location assignment, members, and lifecycle controls."
            sx={{ width: '100%', minWidth: 0, maxWidth: '100%' }}
          >
            <OrganizationTeamSectionNav activeSection={activeSection} organizationCustomDomain={organizationCustomDomain} teamId={teamId} stickyTop={stickyTop} />
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
