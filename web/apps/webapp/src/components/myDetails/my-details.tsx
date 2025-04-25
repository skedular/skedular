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
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { getCustomerFullName, joinErrors } from '@/libs/utils';
import type { myDetails_rootQuery } from '@/queries/__generated__/myDetails_rootQuery.graphql';
import type { myDetails_updateCustomerDetailsMutation } from '@/queries/__generated__/myDetails_updateCustomerDetailsMutation.graphql';
import type { myDetails_updateMyBillingContactDetailsMutation } from '@/queries/__generated__/myDetails_updateMyBillingContactDetailsMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<myDetails_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query myDetails_rootQuery {
    me {
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
  }
`;

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
  email: string()
    .email(({ value }) => `${value} is not a valid email`)
    .required('Email is required'),
  addressLine1: string().required('Address line 1 is required'),
  addressLine2: string().nullable(),
  suburb: string().required('Suburb is required'),
  city: string().required('City is required'),
  province: string().nullable(),
  zipcode: string().required('Zipcode is required'),
  country: string().required('Country is required'),
});

const MyDetails = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<myDetails_rootQuery>(RootQuery, queryReference);
  const [commitUpdateCustomerDetails] = useMutation<myDetails_updateCustomerDetailsMutation>(graphql`
    mutation myDetails_updateCustomerDetailsMutation($input: UpdateCustomerDetailsInput!) @raw_response_type {
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

  const [commitUpdateMyBillingContactDetails] = useMutation<myDetails_updateMyBillingContactDetailsMutation>(graphql`
    mutation myDetails_updateMyBillingContactDetailsMutation($input: UpdateMyBillingContactDetailsInput!) @raw_response_type {
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
  const validateProfileDetails = makeValidate(profileDetailsSchema);
  const requiredProfileDetailsFields = makeRequired(profileDetailsSchema);
  const validateCustomerBilling = makeValidate(customerBillingSchema);
  const requiredCustomerBillingFields = makeRequired(customerBillingSchema);

  const handleCloseClick = () => {
    router.push('/');
  };

  const handleProfileDetailUpdateClick = ({ timezone, designation, title, name, givenName, middleName, familyName, phoneNumber }: ProfileDetailsDetails) => {
    const me = rootData.me;
    if (!me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating user profile details'...`} />, infoNotificationOptions);

    commitUpdateCustomerDetails({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: me.id,
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
            id: me.id,
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

  const me = rootData.me;
  if (!me) {
    return <></>;
  }

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
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit My Details">
          <Form
            onSubmit={handleProfileDetailUpdateClick}
            initialValues={{
              timezone: me.timezone,
              designation: me.designation,
              title: me.title,
              name: me.name,
              givenName: me.givenName,
              middleName: me.middleName,
              familyName: me.familyName,
              phoneNumber: me.phoneNumber,
            }}
            validate={validateProfileDetails}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <GridContainer sx={{ justifyContent: 'space-between' }}>
                    <Grid>
                      <StackRow>
                        <CustomerAvatar name={me} photo={{ url: me?.photoUrl }} size="large" />
                        <StackColumn spacing={-0.5}>
                          <LeadIconTypography label={getCustomerFullName(me)} />
                          <CaptionIconTypography label={me.email} />
                        </StackColumn>
                      </StackRow>
                    </Grid>

                    <Grid></Grid>
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
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
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
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

const MemoMyDetails = memo(MyDetails);

type RelayProps = {
  onReloadRequired: () => void;
};

const MyDetailsWithRelay = ({ onReloadRequired }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<myDetails_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoMyDetails queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(MyDetailsWithRelay);
