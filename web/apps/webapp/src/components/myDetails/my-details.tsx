import { CustomerAvatar } from '@/components/avatars';
import {
  AppBarWithStackColumn,
  BodyIconTypography,
  CaptionIconTypography,
  CreditCard,
  FormFieldLabel,
  FormStackColumn,
  GridContainer,
  LeadIconTypography,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@/components/commons';
import { SingleChoiceCountry, SingleChoinceTimezone } from '@/components/forms';
import { DeleteIcon, NewIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { getCustomerFullName, joinErrors } from '@/libs/utils';
import type { myDetails_addMyBillingDetailsMutation } from '@/queries/__generated__/myDetails_addMyBillingDetailsMutation.graphql';
import type { myDetails_customerPaymentMethodsDetails_query$key } from '@/queries/__generated__/myDetails_customerPaymentMethodsDetails_query.graphql';
import type { myDetails_customerPaymentMethodsDetails_refetchableFragment } from '@/queries/__generated__/myDetails_customerPaymentMethodsDetails_refetchableFragment.graphql';
import type { myDetails_removeCustomerPaymentMethodMutation } from '@/queries/__generated__/myDetails_removeCustomerPaymentMethodMutation.graphql';
import type { myDetails_rootQuery } from '@/queries/__generated__/myDetails_rootQuery.graphql';
import type { myDetails_updateCustomerDetailsMutation } from '@/queries/__generated__/myDetails_updateCustomerDetailsMutation.graphql';
import type { myDetails_updateMyBillingDetailsMutation } from '@/queries/__generated__/myDetails_updateMyBillingDetailsMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, startTransition, useCallback, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import { getRootLink } from '../links';
import AddMyPaymentMethodDialog from './add-my-payment-method-dialog';

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
      billingDetails {
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
    ...myDetails_customerPaymentMethodsDetails_query
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
  addressLine1: string;
  addressLine2: string | null;
  suburb: string;
  city: string;
  province: string | null;
  zipcode: string;
  country: string;
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
  const [rootDataMyPaymentMethodsDetails, refetchMyPaymentMethodsDetails] = useRefetchableFragment<
    myDetails_customerPaymentMethodsDetails_refetchableFragment,
    myDetails_customerPaymentMethodsDetails_query$key
  >(
    graphql`
      fragment myDetails_customerPaymentMethodsDetails_query on Query @refetchable(queryName: "myDetails_customerPaymentMethodsDetails_refetchableFragment") {
        me {
          paymentMethods {
            id
            cardBrand
            cardExpiryMonth
            cardExpiryYear
            cardLastFourDigit
          }
        }
      }
    `,
    rootData,
  );

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

  const [commitAddMyBillingDetails] = useMutation<myDetails_addMyBillingDetailsMutation>(graphql`
    mutation myDetails_addMyBillingDetailsMutation($input: AddMyBillingDetailsInput!) @raw_response_type {
      addMyBillingDetails(input: $input) {
        customer {
          id
          billingDetails {
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
    }
  `);

  const [commitUpdateMyBillingDetails] = useMutation<myDetails_updateMyBillingDetailsMutation>(graphql`
    mutation myDetails_updateMyBillingDetailsMutation($input: UpdateMyBillingDetailsInput!) @raw_response_type {
      updateMyBillingDetails(input: $input) {
        customer {
          id
          billingDetails {
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
    }
  `);

  const [commitRemoveCustomerPaymentMethod] = useMutation<myDetails_removeCustomerPaymentMethodMutation>(graphql`
    mutation myDetails_removeCustomerPaymentMethodMutation($input: RemoveCustomerPaymentMethodInput!) {
      removeCustomerPaymentMethod(input: $input) {
        clientMutationId
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateProfileDetails = makeValidate(profileDetailsSchema);
  const requiredProfileDetailsFields = makeRequired(profileDetailsSchema);
  const validateCustomerBilling = makeValidate(customerBillingSchema);
  const requiredCustomerBillingFields = makeRequired(customerBillingSchema);
  const [isAddPaymentMethodDialogOpen, setIsAddPaymentMethodDialogOpen] = useState(false);

  const handleRefetchMyPaymentMethodsDetails = useCallback(() => {
    startTransition(() => {
      refetchMyPaymentMethodsDetails(
        {},
        {
          fetchPolicy: 'store-and-network',
        },
      );
    });
  }, [refetchMyPaymentMethodsDetails]);

  const handleCloseClick = () => {
    router.push(getRootLink(integratedPlatrform));
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
    if (!rootData.me) {
      return;
    }

    const billingDetails = rootData.me.billingDetails;
    if (billingDetails) {
      const toastId = themedToast(<NotificationContent content={`Updating billing...`} />, infoNotificationOptions);

      commitUpdateMyBillingDetails({
        variables: {
          input: {
            clientMutationId: nanoid(),
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
          updateMyBillingDetails: {
            customer: {
              id: rootData.me.id,
              billingDetails: {
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
        },
      });
    } else {
      const id = nanoid();
      const toastId = themedToast(<NotificationContent content={`Adding billing...`} />, infoNotificationOptions);

      commitAddMyBillingDetails({
        variables: {
          input: {
            clientMutationId: nanoid(),
            id,
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
              render: <NotificationContent content={`Failed to add billing. Error: ${joinErrors(errors)}.`} />,
            });

            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Billing added.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update add. Error: ${error.message}.`} />,
          });
        },
        optimisticResponse: {
          addMyBillingDetails: {
            customer: {
              id: rootData.me.id,
              billingDetails: {
                id,
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
        },
      });
    }
  };

  const handleAddPaymentMethodClicked = () => {
    setIsAddPaymentMethodDialogOpen(true);
  };

  const handleAddPaymentMethodCancel = () => {
    setIsAddPaymentMethodDialogOpen(false);
  };

  const handleRemovePaymentMethodClick = (id: string) => {
    const toastId = themedToast(<NotificationContent content={`Removing payment method...`} />, infoNotificationOptions);

    commitRemoveCustomerPaymentMethod({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove payment method. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Payment method removed.`} />,
        });

        handleRefetchMyPaymentMethodsDetails();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove payment method. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const me = rootData.me;
  if (!me) {
    return <></>;
  }

  const billingDetails = rootData.me.billingDetails;
  const companyName = billingDetails?.companyName ? billingDetails.companyName : '';
  const email = billingDetails?.email ? billingDetails.email : '';
  const addressLine1 = billingDetails?.addressLine1 ? billingDetails.addressLine1 : '';
  const addressLine2 = billingDetails?.addressLine2 ? billingDetails.addressLine2 : '';
  const suburb = billingDetails?.suburb ? billingDetails.suburb : '';
  const city = billingDetails?.city ? billingDetails.city : '';
  const province = billingDetails?.province ? billingDetails.province : '';
  const zipcode = billingDetails?.zipcode ? billingDetails.zipcode : '';
  const country = billingDetails?.country ? billingDetails.country : '';
  const paymentMethodExist = rootDataMyPaymentMethodsDetails.me?.paymentMethods && rootDataMyPaymentMethodsDetails.me.paymentMethods.length > 0;

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <Box sx={{ flexGrow: 1 }}>
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit My Details">
            <Form
              onSubmit={handleProfileDetailUpdateClick}
              initialValues={{
                timezone: me.timezone ?? '',
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

            <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Payment Method" />
                  <BodyIconTypography label="Edit your payment method" />
                </Grid>

                <Grid>
                  {!paymentMethodExist && (
                    <Button variant="text" onClick={handleAddPaymentMethodClicked} sx={{ textTransform: 'none' }}>
                      <LeadIconTypography label="Add Payment Method" endElement={<NewIcon fontSize="large" />} />
                    </Button>
                  )}
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            {paymentMethodExist && (
              <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                <StackRow>
                  {rootDataMyPaymentMethodsDetails.me?.paymentMethods.map((item) => (
                    <StackColumn key={item.id}>
                      <CreditCard lastFourDigits={item.cardLastFourDigit} expiryDate={`${item.cardExpiryMonth}/${item.cardExpiryYear}`} cardBrand={item.cardBrand} />
                      <Button variant="contained" color="warning" onClick={() => handleRemovePaymentMethodClick(item.id)}>
                        <BodyIconTypography label="Remove Payment Method" invertDefaultColor={paletteMode === 'dark'} startElement={<DeleteIcon />} />
                      </Button>
                    </StackColumn>
                  ))}
                </StackRow>
              </StackColumn>
            )}

            {!paymentMethodExist && (
              <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                <SmallIconTypography label="No payment method setup yet" />
              </StackColumn>
            )}

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
      {!paymentMethodExist && isAddPaymentMethodDialogOpen && <AddMyPaymentMethodDialog isDialogOpen={isAddPaymentMethodDialogOpen} onCancel={handleAddPaymentMethodCancel} />}
    </>
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
