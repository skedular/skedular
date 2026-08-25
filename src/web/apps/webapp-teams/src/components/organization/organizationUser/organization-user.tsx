import { CustomerAvatar } from '@/components/avatars';
import { SingleChoinceTimezone } from '@/components/forms';
import { DeleteIcon } from '@/components/icons';
import { getOrganizationUsersBaseLink } from '@/components/links';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { SingleChoiceUserPersonalInformationVisibility } from '@/components/user';
import type { organizationUser_changeOrganizationUsersStatusMutation } from '@/queries/__generated__/organizationUser_changeOrganizationUsersStatusMutation.graphql';
import type { organizationUser_query$key } from '@/queries/__generated__/organizationUser_query.graphql';
import type { organizationUser_removeOrganizationUsersMutation } from '@/queries/__generated__/organizationUser_removeOrganizationUsersMutation.graphql';
import type {
  CustomerDetailsPatchField,
  organizationUser_updateCustomerDetailsMutation,
  PersonalInformationVisibility,
} from '@/queries/__generated__/organizationUser_updateCustomerDetailsMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import ExpandMoreRoundedIcon from '@mui/icons-material/ExpandMoreRounded';
import { getCustomerFullName, getRelayErrorMessage, PaletteModeContext, useIntegratedPlatform } from '@skedular/shared';
import {
  CaptionIconTypography,
  defaultButtonStyle,
  defaultPadding,
  EditorActionBar,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  PageHeaderPanel,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useMemo, useRef, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';
import OrganizationUserSectionNav, { OrganizationUserSection } from './organization-user-section-nav';
import OrganizationUserTeamList, { OrganizationUserTeamListItem } from './organization-user-team-list';

type Props = {
  rootDataRelay: organizationUser_query$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  customerId: string;
};

type ProfileDetailsDetails = {
  designation: string | null;
  title: string | null;
  name: string | null;
  givenName: string | null;
  middleName: string | null;
  familyName: string | null;
  timezone: string;
  phoneNumber: string | null;
  personalInformationVisibility: string;
};

const profileDetailsSchema = object({
  designation: string().nullable(),
  title: string().nullable(),
  name: string().nullable(),
  givenName: string().nullable(),
  middleName: string().nullable(),
  familyName: string().nullable(),
  timezone: string().required('Timezone is required'),
  phoneNumber: string().nullable(),
  personalInformationVisibility: string().required('Personal Information Visibility is required'),
});

const getActiveSection = (tab: string | null, section: string | null): OrganizationUserSection => {
  if (tab === 'profile' || section === 'identity' || section === 'profile') {
    return 'profile';
  }
  if (tab === 'teams' || section === 'manage-teams') return 'manage-teams';
  if (tab === 'manage' || section === 'manage-user') return 'manage-user';

  return 'profile';
};

const formColumnSx = {
  width: '100%',
  maxWidth: 'none',
};

const inlinePatchDebounceTimeout = 1000;

const profilePatchFields: Record<keyof ProfileDetailsDetails, CustomerDetailsPatchField> = {
  designation: 'DESIGNATION',
  title: 'TITLE',
  name: 'NAME',
  givenName: 'GIVEN_NAME',
  middleName: 'MIDDLE_NAME',
  familyName: 'FAMILY_NAME',
  timezone: 'TIMEZONE',
  phoneNumber: 'PHONE_NUMBER',
  personalInformationVisibility: 'PERSONAL_INFORMATION_VISIBILITY',
};

const getChangedProfileFields = (left: ProfileDetailsDetails, right: ProfileDetailsDetails) =>
  (Object.keys(profilePatchFields) as (keyof ProfileDetailsDetails)[]).filter((field) => left[field] !== right[field]).map((field) => profilePatchFields[field]);

const getValidProfilePatchFields = (fieldsToUpdate: CustomerDetailsPatchField[], values: ProfileDetailsDetails): CustomerDetailsPatchField[] =>
  fieldsToUpdate.filter((patchField) => {
    const formField = (Object.entries(profilePatchFields) as [keyof ProfileDetailsDetails, CustomerDetailsPatchField][]).find(([, field]) => field === patchField)?.[0];
    if (!formField) {
      return false;
    }

    try {
      profileDetailsSchema.validateSyncAt(formField, values);
      return true;
    } catch {
      return false;
    }
  });

const OrganizationUser = ({ rootDataRelay, organizationCustomDomain, customerId }: Props) => {
  const rootData = useFragment<organizationUser_query$key>(
    graphql`
      fragment organizationUser_query on Query @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null }) {
        me {
          id
        }
        customer(id: $customerId) {
          id
          email
          photoUrl
          designation
          title
          name
          givenName
          middleName
          familyName
          timezone
          phoneNumber
          personalInformationVisibility {
            type
            name
          }
        }
        customerTeams(first: $count, after: $cursor, where: { organizationCustomDomain: $organizationCustomDomain, customerId: $customerId }, orderBy: $teamsSortingValues)
          @connection(key: "organizationUser_customerTeams") {
          __id
          totalCount
          edges {
            node {
              id
              name
              organization {
                id
              }
              featureImages {
                thumbnail {
                  url
                }
              }
              members {
                edges {
                  node {
                    organizationMember {
                      uniqueId
                      customer {
                        id
                        givenName
                        middleName
                        familyName
                        name
                        photoUrl
                        personalInformationVisibility {
                          type
                          name
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        organization(customDomain: $organizationCustomDomain) {
          members(where: { customerId: $customerId }) {
            __id
            totalCount
            edges {
              node {
                id
                status {
                  type
                  name
                }
              }
            }
          }
        }
        ...organizationUserLeftSideNavigationMenuContent_query
        ...singleChoiceUserPersonalInformationVisibility_query
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateCustomerDetails] = useMutation<organizationUser_updateCustomerDetailsMutation>(graphql`
    mutation organizationUser_updateCustomerDetailsMutation($input: UpdateCustomerDetailsInput!) @raw_response_type {
      updateCustomerDetails(input: $input) {
        customer {
          id
          timezone
          designation
          title
          name
          givenName
          middleName
          familyName
          phoneNumber
          personalInformationVisibility {
            type
            name
          }
        }
      }
    }
  `);

  const [commitChangeOrganizationMembersStatus] = useMutation<organizationUser_changeOrganizationUsersStatusMutation>(graphql`
    mutation organizationUser_changeOrganizationUsersStatusMutation($input: ChangeOrganizationMembersStatusInput!) {
      changeOrganizationMembersStatus(input: $input) {
        members {
          id
          status {
            type
            name
          }
        }
      }
    }
  `);

  const [commitRemoveOrganizationMembers] = useMutation<organizationUser_removeOrganizationUsersMutation>(graphql`
    mutation organizationUser_removeOrganizationUsersMutation($connectionIds: [ID!]!, $input: RemoveOrganizationMembersInput!) {
      removeOrganizationMembers(input: $input) {
        members {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const activeSection = useMemo(() => getActiveSection(searchParams.get('tab'), searchParams.get('section')), [searchParams]);
  const [expandedProfileSection, setExpandedProfileSection] = useState<'identity' | 'contact' | 'privacy' | false>(() => {
    const section = searchParams.get('section');
    return section === 'contact' || section === 'privacy' || section === 'identity' ? section : 'identity';
  });
  const updateProfileSection = (section: 'identity' | 'contact' | 'privacy' | false) => {
    setExpandedProfileSection(section);
    const params = new URLSearchParams(searchParams.toString());
    params.set('tab', 'profile');
    if (section) params.set('section', section);
    else params.delete('section');
    router.replace(`?${params.toString()}`, { scroll: false });
  };
  const validateProfileDetails = makeValidate(profileDetailsSchema);
  const requiredProfileDetailsFields = makeRequired(profileDetailsSchema);
  const teams = useMemo(() => rootData.customerTeams.edges.map((edge) => edge.node), [rootData.customerTeams]);
  const teamItems = useMemo<OrganizationUserTeamListItem[]>(
    () =>
      teams.map((team) => ({
        id: team.id,
        name: team.name,
        featureImageUrl: team.featureImages[0]?.thumbnail?.url,
        members: team.members.edges
          .map(({ node }) => node.organizationMember?.customer)
          .filter((member): member is NonNullable<typeof member> => !!member)
          .map((member) => ({
            id: member.id,
            givenName: member.givenName,
            middleName: member.middleName,
            familyName: member.familyName,
            name: member.name,
            photoUrl: member.photoUrl,
          })),
      })),
    [teams],
  );
  const teamsConnectionIds = useMemo(() => [rootData.customerTeams.__id], [rootData.customerTeams]);
  const member = useMemo(
    () => (rootData.organization?.members && rootData.organization.members.edges.length > 0 ? rootData.organization.members.edges[0]?.node : null),
    [rootData.organization],
  );

  const initialProfileValues = useMemo<ProfileDetailsDetails | null>(
    () =>
      rootData.customer
        ? {
            timezone: rootData.customer.timezone ?? '',
            designation: rootData.customer.designation ?? null,
            title: rootData.customer.title ?? null,
            name: rootData.customer.name ?? null,
            givenName: rootData.customer.givenName ?? null,
            middleName: rootData.customer.middleName ?? null,
            familyName: rootData.customer.familyName ?? null,
            phoneNumber: rootData.customer.phoneNumber ?? null,
            personalInformationVisibility: rootData.customer.personalInformationVisibility.type,
          }
        : null,
    [rootData.customer],
  );
  const draftProfileValues = useRef(initialProfileValues);
  const submittedProfileValues = useRef(initialProfileValues);
  const commitProfilePatch = useCallback(
    (fieldsToUpdate: CustomerDetailsPatchField[], values: ProfileDetailsDetails) => {
      const validFieldsToUpdate = getValidProfilePatchFields(fieldsToUpdate, values);
      if (!rootData.customer || rootData.customer.id !== rootData.me?.id || validFieldsToUpdate.length === 0) {
        return;
      }

      const previousValues = submittedProfileValues.current;
      if (!previousValues || getChangedProfileFields(previousValues, values).filter((field) => validFieldsToUpdate.includes(field)).length === 0) {
        return;
      }
      submittedProfileValues.current = values;

      commitUpdateCustomerDetails({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: customerId,
            fieldsToUpdate: validFieldsToUpdate,
            ...values,
            personalInformationVisibility: values.personalInformationVisibility as PersonalInformationVisibility,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            submittedProfileValues.current = previousValues;
            themedToast(<NotificationContent content={`We couldn't update this user's profile details. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          }
        },
        onError: (error) => {
          submittedProfileValues.current = previousValues;
          themedToast(<NotificationContent content={`We couldn't update this user's profile details. ${error.message}`} />, errorNotificationOptions);
        },
        optimisticResponse: {
          updateCustomerDetails: {
            customer: {
              id: customerId,
              ...values,
              personalInformationVisibility: {
                type: values.personalInformationVisibility as PersonalInformationVisibility,
                name: '',
              },
            },
          },
        },
      });
    },
    [commitUpdateCustomerDetails, customerId, rootData.customer, rootData.me?.id, themedToast],
  );
  const debouncedCommitProfilePatch = useDebounceCallback(commitProfilePatch, inlinePatchDebounceTimeout);

  const handleDeactivateUserClick = () => {
    if (!member) {
      return;
    }

    commitChangeOrganizationMembersStatus({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: [member.id],
          status: 'INACTIVE',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't deactivate this user. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't deactivate this user. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleActivateUserClick = () => {
    if (!member) {
      return;
    }

    commitChangeOrganizationMembersStatus({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: [member.id],
          status: 'ACTIVE',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't activate this user. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't activate this user. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleRemoveUserClick = () => {
    if (!member) {
      return;
    }

    commitRemoveOrganizationMembers({
      variables: {
        connectionIds: teamsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: [member.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't remove this user. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        router.push(getOrganizationUsersBaseLink(integratedPlatform, organizationCustomDomain));
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove this user. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const customer = rootData.customer;
  if (!customer) {
    return null;
  }

  const isItMe = customer.id === rootData.me?.id;

  const renderProfileSection = () => (
    <StackColumn spacing={2}>
      <Form
        onSubmit={() => undefined}
        initialValues={initialProfileValues ?? undefined}
        validate={validateProfileDetails}
        render={({ handleSubmit, values }) => {
          const formValues = values as ProfileDetailsDetails;
          const changedFields = draftProfileValues.current ? getChangedProfileFields(draftProfileValues.current, formValues) : [];
          if (isItMe && changedFields.length > 0) {
            draftProfileValues.current = formValues;
            debouncedCommitProfilePatch(changedFields, formValues);
          }

          return (
            <FormStackColumn onSubmit={handleSubmit} sx={formColumnSx}>
              <StackColumn spacing={2}>
                <Accordion
                  expanded={expandedProfileSection === 'identity'}
                  onChange={() => updateProfileSection(expandedProfileSection === 'identity' ? false : 'identity')}
                  disableGutters
                  elevation={0}
                  sx={{ border: 1, borderColor: 'divider', borderRadius: '12px !important', overflow: 'hidden', '&::before': { display: 'none' } }}
                >
                  <AccordionSummary expandIcon={<ExpandMoreRoundedIcon />} sx={{ px: 2, minHeight: 56 }}>
                    <StackColumn spacing={0.25}>
                      <LeadIconTypography label="Identity" />
                      <SmallIconTypography label="Names, title, and designation" />
                    </StackColumn>
                  </AccordionSummary>
                  <AccordionDetails sx={{ borderTop: 1, borderColor: 'divider', p: 2 }}>
                    <StackColumn spacing={2}>
                      <FormFieldLabel label="Designation">
                        {isItMe && <TextField name="designation" required={requiredProfileDetailsFields.designation} />}
                        {!isItMe && <SmallIconTypography label={customer.designation} />}
                      </FormFieldLabel>

                      <FormFieldLabel label="Title">
                        {isItMe && <TextField name="title" required={requiredProfileDetailsFields.title} />}
                        {!isItMe && <SmallIconTypography label={customer.title} />}
                      </FormFieldLabel>

                      <FormFieldLabel label="Name">
                        {isItMe && <TextField name="name" required={requiredProfileDetailsFields.name} />}
                        {!isItMe && <SmallIconTypography label={customer.name} />}
                      </FormFieldLabel>

                      <FormFieldLabel label="Given Name">
                        {isItMe && <TextField name="givenName" required={requiredProfileDetailsFields.givenName} />}
                        {!isItMe && <SmallIconTypography label={customer.givenName} />}
                      </FormFieldLabel>

                      <FormFieldLabel label="Middle Name">
                        {isItMe && <TextField name="middleName" required={requiredProfileDetailsFields.middleName} />}
                        {!isItMe && <SmallIconTypography label={customer.middleName} />}
                      </FormFieldLabel>

                      <FormFieldLabel label="Family Name">
                        {isItMe && <TextField name="familyName" required={requiredProfileDetailsFields.familyName} />}
                        {!isItMe && <SmallIconTypography label={customer.familyName} />}
                      </FormFieldLabel>
                    </StackColumn>
                  </AccordionDetails>
                </Accordion>

                <Accordion
                  expanded={expandedProfileSection === 'contact'}
                  onChange={() => updateProfileSection(expandedProfileSection === 'contact' ? false : 'contact')}
                  disableGutters
                  elevation={0}
                  sx={{ border: 1, borderColor: 'divider', borderRadius: '12px !important', overflow: 'hidden', '&::before': { display: 'none' } }}
                >
                  <AccordionSummary expandIcon={<ExpandMoreRoundedIcon />} sx={{ px: 2, minHeight: 56 }}>
                    <StackColumn spacing={0.25}>
                      <LeadIconTypography label="Contact" />
                      <SmallIconTypography label="Timezone and phone number" />
                    </StackColumn>
                  </AccordionSummary>
                  <AccordionDetails sx={{ borderTop: 1, borderColor: 'divider', p: 2 }}>
                    <StackColumn spacing={2}>
                      <FormFieldLabel label="Timezone">
                        {isItMe && <SingleChoinceTimezone name="timezone" required={requiredProfileDetailsFields.timezone} />}
                        {!isItMe && <SmallIconTypography label={customer.timezone} />}
                      </FormFieldLabel>

                      <FormFieldLabel label="Phone Number">
                        {isItMe && <TextField name="phoneNumber" required={requiredProfileDetailsFields.phoneNumber} />}
                        {!isItMe && <SmallIconTypography label={customer.phoneNumber} />}
                      </FormFieldLabel>
                    </StackColumn>
                  </AccordionDetails>
                </Accordion>

                <Accordion
                  expanded={expandedProfileSection === 'privacy'}
                  onChange={() => updateProfileSection(expandedProfileSection === 'privacy' ? false : 'privacy')}
                  disableGutters
                  elevation={0}
                  sx={{ border: 1, borderColor: 'divider', borderRadius: '12px !important', overflow: 'hidden', '&::before': { display: 'none' } }}
                >
                  <AccordionSummary expandIcon={<ExpandMoreRoundedIcon />} sx={{ px: 2, minHeight: 56 }}>
                    <StackColumn spacing={0.25}>
                      <LeadIconTypography label="Privacy" />
                      <SmallIconTypography label="Personal information visibility" />
                    </StackColumn>
                  </AccordionSummary>
                  <AccordionDetails sx={{ borderTop: 1, borderColor: 'divider', p: 2 }}>
                    <FormFieldLabel label="Personal Information Visibility" required={requiredProfileDetailsFields.personalInformationVisibility}>
                      <SingleChoiceUserPersonalInformationVisibility
                        rootDataRelay={rootData}
                        name="personalInformationVisibility"
                        required={requiredProfileDetailsFields.personalInformationVisibility}
                      />
                    </FormFieldLabel>
                  </AccordionDetails>
                </Accordion>
              </StackColumn>
            </FormStackColumn>
          );
        }}
      />
    </StackColumn>
  );

  const renderTeamsSection = () => (
    <Box sx={{ p: defaultPadding }}>
      <OrganizationUserTeamList items={teamItems} />
    </Box>
  );

  const renderManageSection = () => (
    <Box sx={{ p: defaultPadding }}>
      <StackColumn spacing={2}>
        <StackColumn spacing={0.5}>
          <LeadIconTypography label="Manage User" />
          <SmallIconTypography label="Change this user's status or remove them from this organization." />
        </StackColumn>

        <Divider />

        {member && (
          <>
            <SmallIconTypography label={`Current status: ${member.status.name}`} />
            <EditorActionBar
              primaryAction={
                <StackRow>
                  {member.status.type === 'ACTIVE' && (
                    <Button size="medium" variant="contained" color="secondary" onClick={handleDeactivateUserClick} sx={defaultButtonStyle}>
                      Deactivate User
                    </Button>
                  )}
                  {member.status.type === 'INACTIVE' && (
                    <Button size="medium" variant="contained" color="secondary" onClick={handleActivateUserClick} sx={defaultButtonStyle}>
                      Activate User
                    </Button>
                  )}
                  <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveUserClick} sx={{ textTransform: 'none' }}>
                    Remove User
                  </Button>
                </StackRow>
              }
            />
          </>
        )}
      </StackColumn>
    </Box>
  );

  const renderActiveSection = () => {
    switch (activeSection) {
      case 'manage-teams':
        return renderTeamsSection();
      case 'manage-user':
        return renderManageSection();
      case 'profile':
      default:
        return renderProfileSection();
    }
  };

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pt: { xs: 1, sm: 1, md: 2 }, pb: defaultPadding }}>
      <StackColumn
        sx={{
          width: '100%',
          maxWidth: 1200,
          mx: 'auto',
          backgroundColor: 'transparent',
          gap: 2,
        }}
      >
        <PageHeaderPanel eyebrow="User profile" title="User profile" description="Manage profile details, team membership, booking context, and lifecycle controls.">
          <StackRow sx={{ alignItems: 'center', gap: 1.5 }}>
            <CustomerAvatar name={customer} photo={{ url: customer.photoUrl }} size="large" />
            <StackColumn spacing={0.25}>
              <LeadIconTypography label={getCustomerFullName(customer)} />
              <CaptionIconTypography label={customer.email} />
            </StackColumn>
          </StackRow>
          <OrganizationUserSectionNav activeSection={activeSection} organizationCustomDomain={organizationCustomDomain} customerId={customerId} />
        </PageHeaderPanel>

        <Box
          sx={{
            borderRadius: activeSection === 'profile' ? 0 : 4,
            border: activeSection === 'profile' ? 0 : 1,
            borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
            bgcolor: activeSection === 'profile' ? 'transparent' : (theme) => (theme.palette.mode === 'light' ? 'common.white' : theme.palette.background.paper),
            boxShadow: activeSection === 'profile' ? 'none' : (theme) => (theme.palette.mode === 'light' ? '0 12px 32px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
            overflow: activeSection === 'profile' ? 'visible' : 'hidden',
          }}
        >
          {renderActiveSection()}
        </Box>
      </StackColumn>
    </Box>
  );
};

export default memo(OrganizationUser);
