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
  StackColumn,
  StackRow,
} from '@/components/commons';
import { SingleChoiceCountry, SingleChoinceTimezone } from '@/components/forms';
import { BookingIcon, DeleteIcon } from '@/components/icons';
import { getOrganizationBookingsBaseLink, getOrganizationUsersBaseLink } from '@/components/links';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { TeamCard } from '@/components/organization/organizationTeams';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding, secondDrawerExpandedDrawerWidthPx } from '@/libs/theme';
import { getCustomerFullName, joinErrors } from '@/libs/utils';
import type { organizationUser_changeOrganizationUsersStatusMutation } from '@/queries/__generated__/organizationUser_changeOrganizationUsersStatusMutation.graphql';
import type { organizationUser_query$key } from '@/queries/__generated__/organizationUser_query.graphql';
import type { organizationUser_removeOrganizationUsersMutation } from '@/queries/__generated__/organizationUser_removeOrganizationUsersMutation.graphql';
import type { organizationUser_updateCustomerDetailsMutation } from '@/queries/__generated__/organizationUser_updateCustomerDetailsMutation.graphql';
import type { organizationUser_updateMyBillingContactDetailsMutation } from '@/queries/__generated__/organizationUser_updateMyBillingContactDetailsMutation.graphql';
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

type CustomerBillingDetails = {
  companyName: string;
  email: string;
  addressLine1: string | null;
  addressLine2: string | null;
  suburb: string | null;
  city: string | null;
  province: string | null;
  zipcode: string | null;
  country: string | null;
};

const customerBillingSchema = object({
  companyName: string().nullable(),
  email: string().email(({ value }) => `${value} is not a valid email`),
  addressLine1: string().nullable(),
  addressLine2: string().nullable(),
  suburb: string().nullable(),
  city: string().nullable(),
  province: string().nullable(),
  zipcode: string().nullable(),
  country: string().nullable(),
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
        myBillingContactDetails {
          id
          companyName
          email
          addressLine1
          addressLine2
          suburb
          city
          province
          zipcode
          country
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

  const [commitUpdateMyBillingContactDetails] = useMutation<organizationUser_updateMyBillingContactDetailsMutation>(graphql`
    mutation organizationUser_updateMyBillingContactDetailsMutation($input: UpdateMyBillingContactDetailsInput!) @raw_response_type {
      updateMyBillingContactDetails(input: $input) {
        customerBillingContactDetails {
          id
          companyName
          email
          addressLine1
          addressLine2
          suburb
          city
          province
          zipcode
          country
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
  const teams = useMemo(() => rootData.customerTeams.edges.map((edge) => edge.node), [rootData.customerTeams]);
  const validateCustomerBilling = makeValidate(customerBillingSchema);
  const requiredCustomerBillingFields = makeRequired(customerBillingSchema);
  const connectionIds = useMemo(() => [rootData.customerTeams.__id], [rootData.customerTeams]);
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
    router.push(getOrganizationUsersBaseLink(organizationId));
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

  const handleMyBillingDetailUpdateClick = ({ companyName, email, addressLine1, addressLine2, suburb, city, province, zipcode, country }: CustomerBillingDetails) => {
    const billingDetails = rootData.myBillingContactDetails;
    if (!billingDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating billing...`} />, infoNotificationOptions);

    commitUpdateMyBillingContactDetails({
      variables: {
        input: {
          clientMutationId: nanoid(),
          companyName,
          email,
          addressLine1,
          addressLine2,
          suburb,
          city,
          province,
          zipcode,
          country,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update billing. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Billing updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update billing. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateMyBillingContactDetails: {
          customerBillingContactDetails: {
            id: billingDetails.id,
            companyName,
            email,
            addressLine1,
            addressLine2,
            suburb,
            city,
            province,
            zipcode,
            country,
          },
        },
      },
    });
  };

  const handleViewBookingsClick = () => {
    router.push(getOrganizationBookingsBaseLink(organizationId, { customerId }));
  };

  if (!rootData.customer) {
    return <></>;
  }

  const customer = rootData.customer;

  const billingContactDetails = rootData.myBillingContactDetails;
  const companyName = billingContactDetails.companyName ? billingContactDetails.companyName : '';
  const email = billingContactDetails.email ? billingContactDetails.email : '';
  const addressLine1 = billingContactDetails.addressLine1 ? billingContactDetails.addressLine1 : '';
  const addressLine2 = billingContactDetails.addressLine2 ? billingContactDetails.addressLine2 : '';
  const suburb = billingContactDetails.suburb ? billingContactDetails.suburb : '';
  const city = billingContactDetails.city ? billingContactDetails.city : '';
  const province = billingContactDetails.province ? billingContactDetails.province : '';
  const zipcode = billingContactDetails.zipcode ? billingContactDetails.zipcode : '';
  const country = billingContactDetails.country ? billingContactDetails.country : '';

  return (
    <Box sx={{ display: 'flex' }}>
      <OrganizationUserLeftSideNavigationMenuContent rootDataRelay={rootData} organizationId={organizationId} customerId={customerId} hideIcons />
      <Box sx={{ marginLeft: secondDrawerExpandedDrawerWidthPx, flexGrow: 1 }}>
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
                  connectionIds={connectionIds}
                  teammates={team.members.filter(({ organizationMember }) => !!organizationMember)!.map(({ organizationMember }) => organizationMember!.customer)}
                />
              </Grid>
            ))}
          </GridContainer>

          {customerId === rootData.me?.id && (
            <Form
              onSubmit={handleMyBillingDetailUpdateClick}
              initialValues={{
                companyName,
                email,
                addressLine1,
                addressLine2,
                suburb,
                city,
                province,
                zipcode,
                country,
              }}
              validate={validateCustomerBilling}
              render={({ handleSubmit }) => (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn
                    sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                    ref={(divElement) => {
                      sectionRefs.current['billing-payment-setup'] = divElement;
                    }}
                  >
                    <SectionIconTypography label="Billing & Payment Setup" />
                    <BodyIconTypography label="Edit your billing and payment details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Company">
                      <TextField name="companyName" required={requiredCustomerBillingFields.companyName} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Email">
                      <TextField name="email" required={requiredCustomerBillingFields.email} helperText="Email to send invoice to" />
                    </FormFieldLabel>

                    <FormFieldLabel label="Address line 1">
                      <TextField name="addressLine1" required={requiredCustomerBillingFields.addressLine1} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Address line 2">
                      <TextField name="addressLine2" required={requiredCustomerBillingFields.addressLine2} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Suburb">
                      <TextField name="suburb" required={requiredCustomerBillingFields.suburb} />
                    </FormFieldLabel>

                    <FormFieldLabel label="City">
                      <TextField name="city" required={requiredCustomerBillingFields.city} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Province">
                      <TextField name="province" required={requiredCustomerBillingFields.province} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Zipcode">
                      <TextField name="zipcode" required={requiredCustomerBillingFields.zipcode} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Country">
                      <SingleChoiceCountry name="country" required={requiredCustomerBillingFields.country} />
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
          )}

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
