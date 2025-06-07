import { CustomerAvatar } from '@/components/avatars';
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
} from '@/components/commons';
import { SingleChoinceTimezone } from '@/components/forms';
import { BookingIcon, DeleteIcon } from '@/components/icons';
import { getOrganizationBookingsBaseLink, getOrganizationUsersBaseLink } from '@/components/links';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { TeamCard } from '@/components/organization/organizationTeams';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding, secondDrawerExpandedDrawerWidthPx } from '@/libs/theme';
import { getCustomerFullName, joinErrors } from '@/libs/utils';
import type { organizationUser_changeOrganizationUsersStatusMutation } from '@/queries/__generated__/organizationUser_changeOrganizationUsersStatusMutation.graphql';
import type { organizationUser_query$key } from '@/queries/__generated__/organizationUser_query.graphql';
import type { organizationUser_removeOrganizationUsersMutation } from '@/queries/__generated__/organizationUser_removeOrganizationUsersMutation.graphql';
import type { organizationUser_updateCustomerDetailsMutation } from '@/queries/__generated__/organizationUser_updateCustomerDetailsMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useContext, useEffect, useMemo, useRef } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
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
        customerTeams(first: $count, after: $cursor, where: { organizationId: $organizationId, customerId: $customerId }, orderBy: $teamsSortingValues)
          @connection(key: "organizationUser_customerTeams") {
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
        ...organizationUserLeftSideNavigationMenuContent_query
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

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const validateProfileDetails = makeValidate(profileDetailsSchema);
  const requiredProfileDetailsFields = makeRequired(profileDetailsSchema);
  const teams = useMemo(() => rootData.customerTeams.edges.map((edge) => edge.node), [rootData.customerTeams]);
  const teamsConnectionIds = useMemo(() => [rootData.customerTeams.__id], [rootData.customerTeams]);
  const member = useMemo(() => (rootData.organizationMembers.edges.length > 0 ? rootData.organizationMembers.edges[0]?.node : null), [rootData.organizationMembers]);

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
    router.push(getOrganizationUsersBaseLink(integratedPlatrform, organizationId));
  };

  const handleProfileDetailUpdateClick = ({ timezone, designation, title, name, givenName, middleName, familyName, phoneNumber }: ProfileDetailsDetails) => {
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
        connectionIds: teamsConnectionIds,
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

        router.push(getOrganizationUsersBaseLink(integratedPlatrform, organizationId));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove user. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleViewBookingsClick = () => {
    router.push(getOrganizationBookingsBaseLink(integratedPlatrform, organizationId, { customerId }));
  };

  const customer = rootData.customer;
  if (!customer) {
    return <></>;
  }

  const isItMe = customer.id === rootData.me?.id;

  return (
    <Box sx={{ display: 'flex' }}>
      <OrganizationUserLeftSideNavigationMenuContent rootDataRelay={rootData} organizationId={organizationId} customerId={customerId} hideIcons />
      <Box sx={{ marginLeft: secondDrawerExpandedDrawerWidthPx, flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit User Details">
          <Form
            onSubmit={handleProfileDetailUpdateClick}
            initialValues={{
              timezone: customer.timezone ?? '',
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
                  <GridContainer sx={{ justifyContent: 'space-between' }}>
                    <Grid>
                      <StackRow>
                        <CustomerAvatar name={customer} photo={{ url: customer?.photoUrl }} size="large" />
                        <StackColumn spacing={-0.5}>
                          <LeadIconTypography label={getCustomerFullName(customer)} />
                          <CaptionIconTypography label={customer.email} />
                        </StackColumn>
                      </StackRow>
                    </Grid>

                    <Grid>
                      <Button variant="contained" sx={defaultButtonStyle} startIcon={<BookingIcon />} onClick={handleViewBookingsClick}>
                        View User Bookings
                      </Button>
                    </Grid>
                  </GridContainer>
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
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

                  <FormFieldLabel label="Timezone">
                    {isItMe && <SingleChoinceTimezone name="timezone" required={requiredProfileDetailsFields.timezone} />}
                    {!isItMe && <SmallIconTypography label={customer.timezone} />}
                  </FormFieldLabel>

                  <FormFieldLabel label="Phone Number">
                    {isItMe && <TextField name="phoneNumber" required={requiredProfileDetailsFields.phoneNumber} />}
                    {!isItMe && <SmallIconTypography label={customer.phoneNumber} />}
                  </FormFieldLabel>
                </StackColumn>

                {isItMe && (
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Update
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
                  connectionIds={teamsConnectionIds}
                  teammates={team.members.filter(({ organizationMember }) => !!organizationMember)!.map(({ organizationMember }) => organizationMember!.customer)}
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
              {member.status === 'ACTIVE' && (
                <Button size="medium" variant="contained" color="secondary" onClick={handleDeactivateUserClick} sx={defaultButtonStyle}>
                  Deactivate User
                </Button>
              )}
              {member.status === 'INACTIVE' && (
                <Button size="medium" variant="contained" color="secondary" onClick={handleActivateUserClick} sx={defaultButtonStyle}>
                  Activate User
                </Button>
              )}
              <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveUserClick} sx={{ textTransform: 'none' }}>
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
