import { getOrganizationUsersBaseLink } from '@/components/links';
import { TeamCard } from '@/components/organization/organizationTeams';
import type { organizationUser_changeOrganizationUsersStatusMutation } from '@/queries/__generated__/organizationUser_changeOrganizationUsersStatusMutation.graphql';
import type { organizationUser_query$key } from '@/queries/__generated__/organizationUser_query.graphql';
import type { organizationUser_removeOrganizationUsersMutation } from '@/queries/__generated__/organizationUser_removeOrganizationUsersMutation.graphql';
import type { organizationUser_updateCustomerDetailsMutation } from '@/queries/__generated__/organizationUser_updateCustomerDetailsMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import {
  AppBarWithStackColumn,
  BodyIconTypography,
  CaptionIconTypography,
  FormFieldLabel,
  FormStackColumn,
  GridContainer,
  LeadIconTypography,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@repo/shared/components/commons';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import { DeleteIcon } from '@repo/shared/components/icons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@repo/shared/libs/theme';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useContext, useEffect, useMemo, useRef } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import { expandedDrawerWidthPx } from './commons';
import OrganizationUserLeftSideNavigationMenuContent from './organization-user-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationUser_query$key;
  onReloadRequired: () => void;
  organizationId: string;
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
});

const OrganizationUser = ({ rootDataRelay, organizationId, customerId }: Props) => {
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
        }
        customerTeams(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, customerId: $customerId }
          orderBy: $teamsSortingValues
        ) @connection(key: "organizationUser_customerTeams") {
          __id
          totalCount
          edges {
            node {
              id
              name
              organization {
                uniqueId
              }
              members {
                organizationMember {
                  uniqueId
                  customer {
                    uniqueId
                    givenName
                    middleName
                    familyName
                    name
                    photoUrl
                  }
                }
              }
              ...teamCard_TeamDetails
            }
          }
        }
        organizationMembers(where: { organizationId: $organizationId, customerId: $customerId }) {
          __id
          totalCount
          edges {
            node {
              id
              status
              role
            }
          }
        }
        ...teamCard_query
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
        }
      }
    }
  `);

  const [commitChangeOrganizationMembersStatus] = useMutation<organizationUser_changeOrganizationUsersStatusMutation>(graphql`
    mutation organizationUser_changeOrganizationUsersStatusMutation($input: ChangeOrganizationMembersStatusInput!) {
      changeOrganizationMembersStatus(input: $input) {
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

  const [commitRemoveOrganizationMembers] = useMutation<organizationUser_removeOrganizationUsersMutation>(graphql`
    mutation organizationUser_removeOrganizationUsersMutation($connectionIds: [ID!]!, $input: RemoveOrganizationMembersInput!) {
      removeOrganizationMembers(input: $input) {
        members {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const validateProfileDetails = makeValidate(profileDetailsSchema);
  const requiredProfileDetailsFields = makeRequired(profileDetailsSchema);
  const connectionIds = useMemo(() => (rootData.customerTeams ? [rootData.customerTeams.__id] : []), [rootData.customerTeams]);
  const teams = useMemo(() => (rootData.customerTeams ? rootData.customerTeams.edges.map((edge) => edge.node) : []), [rootData.customerTeams]);
  const member = useMemo(
    () => (rootData.organizationMembers && rootData.organizationMembers.edges.length > 0 ? rootData.organizationMembers.edges[0]?.node : null),
    [rootData.organizationMembers],
  );

  useEffect(() => {
    if (!section || section === 'profile') {
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

  const handleCloseClick = () => {
    router.push(getOrganizationUsersBaseLink(organizationId));
  };

  const handleProfileDetailUpdateClick = ({
    timezone,
    designation,
    title,
    name,
    givenName,
    middleName,
    familyName,
    phoneNumber,
  }: ProfileDetailsDetails) => {
    if (!rootData.customer) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating user profile details'...`} />, infoNotificationOptions);

    commitUpdateCustomerDetails({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: customerId,
          timezone,
          designation,
          title,
          name,
          givenName,
          middleName,
          familyName,
          phoneNumber,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update user profile details. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`User profile details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update user profile details. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateCustomerDetails: {
          customer: {
            id: customerId,
            timezone,
            designation,
            title,
            name,
            givenName,
            middleName,
            familyName,
            phoneNumber,
          },
        },
      },
    });
  };

  const handleDeactivateUserClick = () => {
    if (!member) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Deactivating user...'} />, infoNotificationOptions);

    commitChangeOrganizationMembersStatus({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: [member.id],
          status: 'Inactive',
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
    if (!member) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Activating user...'} />, infoNotificationOptions);

    commitChangeOrganizationMembersStatus({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: [member.id],
          status: 'Active',
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
    if (!member) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Removing user...'} />, infoNotificationOptions);

    commitRemoveOrganizationMembers({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: [member.id],
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

        router.push(getOrganizationUsersBaseLink(organizationId));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove user. Error: ${error.message}.`} />,
        });
      },
    });
  };

  if (!rootData.customer) {
    return <></>;
  }

  const customer = rootData.customer;

  return (
    <Box sx={{ display: 'flex' }}>
      <OrganizationUserLeftSideNavigationMenuContent organizationId={organizationId} customerId={customerId} hideIcons />
      <Box sx={{ marginLeft: expandedDrawerWidthPx, flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit User Details">
          <Form
            onSubmit={handleProfileDetailUpdateClick}
            initialValues={{
              timezone: customer.timezone,
              designation: customer.designation,
              title: customer.title,
              name: customer.name,
              givenName: customer.givenName,
              middleName: customer.middleName,
              familyName: customer.familyName,
              phoneNumber: customer.phoneNumber,
            }}
            validate={validateProfileDetails}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <StackColumn
                  sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                  ref={(divElement) => {
                    sectionRefs.current['profile'] = divElement;
                  }}
                >
                  <StackRow>
                    <CustomerAvatar name={customer} photo={{ url: customer?.photoUrl }} size="large" />
                    <StackColumn spacing={-0.5}>
                      <LeadIconTypography label={getCustomerFullName(customer)} />
                      <CaptionIconTypography label={customer.email} />
                    </StackColumn>
                  </StackRow>
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Designation">
                    <TextField name="designation" required={requiredProfileDetailsFields.designation} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Title">
                    <TextField name="title" required={requiredProfileDetailsFields.title} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Name">
                    <TextField name="name" required={requiredProfileDetailsFields.name} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Given Name">
                    <TextField name="givenName" required={requiredProfileDetailsFields.givenName} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Middle Name">
                    <TextField name="middleName" required={requiredProfileDetailsFields.middleName} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Family Name">
                    <TextField name="familyName" required={requiredProfileDetailsFields.familyName} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Timezone">
                    <SingleChoinceTimezone name="timezone" required={requiredProfileDetailsFields.timezone} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Phone Number">
                    <TextField name="phoneNumber" required={requiredProfileDetailsFields.phoneNumber} />
                  </FormFieldLabel>
                </StackColumn>

                {rootData.customer?.id === rootData.me?.id && (
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" color="primary" type="submit" sx={{ textTransform: 'none' }}>
                        <SmallIconTypography label="Update" />
                      </Button>
                    </StackRow>
                  </StackColumn>
                )}
              </FormStackColumn>
            )}
          />

          <StackColumn
            sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
            ref={(divElement) => {
              sectionRefs.current['manage-teams'] = divElement;
            }}
          >
            <SectionIconTypography label="User Teams" />
            <BodyIconTypography label="Teams in your organization this user is a part of" />
            <Divider />
          </StackColumn>

          <GridContainer sx={{ padding: defaultPadding }}>
            {teams.map((team) => (
              <Grid key={team.id}>
                <TeamCard
                  rootDataRelay={rootData}
                  teamDetailsRelay={team}
                  connectionIds={connectionIds}
                  teammates={team.members
                    .filter(({ organizationMember }) => !!organizationMember)!
                    .map(({ organizationMember }) => organizationMember!.customer)}
                />
              </Grid>
            ))}
          </GridContainer>

          <StackColumn
            sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
            ref={(divElement) => {
              sectionRefs.current['manage-user'] = divElement;
            }}
          >
            <SectionIconTypography label="Manage This User" />
            <BodyIconTypography label="Change the status of this user or remove them from your organization" />
            <Divider />
          </StackColumn>

          {member && (
            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
              {member.status === 'Active' && (
                <Button size="medium" variant="contained" color="secondary" onClick={handleDeactivateUserClick} sx={defaultButtonStyle}>
                  Deactivate User
                </Button>
              )}
              {member.status === 'Inactive' && (
                <Button size="medium" variant="contained" color="secondary" onClick={handleActivateUserClick} sx={defaultButtonStyle}>
                  Activate User
                </Button>
              )}
              <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveUserClick}>
                Remove User
              </Button>
            </StackRow>
          )}
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

export default memo(OrganizationUser);
