import {
  AppBarWithStackColumn,
  BodyIconTypography,
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
import { SingleChoiceCountry } from '@/components/forms';
import { DeleteIcon, NewIcon } from '@/components/icons';
import { getRootLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { myBillingAndPayment_addMyBillingDetailsMutation } from '@/queries/__generated__/myBillingAndPayment_addMyBillingDetailsMutation.graphql';
import type { myBillingAndPayment_customerPaymentMethodsDetails_query$key } from '@/queries/__generated__/myBillingAndPayment_customerPaymentMethodsDetails_query.graphql';
import type { myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment } from '@/queries/__generated__/myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment.graphql';
import type { myBillingAndPayment_removeCustomerPaymentMethodMutation } from '@/queries/__generated__/myBillingAndPayment_removeCustomerPaymentMethodMutation.graphql';
import type { myBillingAndPayment_rootQuery } from '@/queries/__generated__/myBillingAndPayment_rootQuery.graphql';
import type { myBillingAndPayment_updateMyBillingDetailsMutation } from '@/queries/__generated__/myBillingAndPayment_updateMyBillingDetailsMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useRouter } from 'next/navigation';
import { memo, startTransition, useCallback, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';
import AddMyPaymentMethodDialog from './add-my-payment-method-dialog';

type Props = {
  queryReference: PreloadedQuery<myBillingAndPayment_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query myBillingAndPayment_rootQuery {
    me {
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
    ...myBillingAndPayment_customerPaymentMethodsDetails_query
  }
`;

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

const MyBillingAndPayment = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<myBillingAndPayment_rootQuery>(RootQuery, queryReference);
  const [rootDataMyPaymentMethodsDetails, refetchMyPaymentMethodsDetails] = useRefetchableFragment<
    myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment,
    myBillingAndPayment_customerPaymentMethodsDetails_query$key
  >(
    graphql`
      fragment myBillingAndPayment_customerPaymentMethodsDetails_query on Query @refetchable(queryName: "myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment") {
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

  const [commitAddMyBillingDetails] = useMutation<myBillingAndPayment_addMyBillingDetailsMutation>(graphql`
    mutation myBillingAndPayment_addMyBillingDetailsMutation($input: AddMyBillingDetailsInput!) @raw_response_type {
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

  const [commitUpdateMyBillingDetails] = useMutation<myBillingAndPayment_updateMyBillingDetailsMutation>(graphql`
    mutation myBillingAndPayment_updateMyBillingDetailsMutation($input: UpdateMyBillingDetailsInput!) @raw_response_type {
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

  const [commitRemoveCustomerPaymentMethod] = useMutation<myBillingAndPayment_removeCustomerPaymentMethodMutation>(graphql`
    mutation myBillingAndPayment_removeCustomerPaymentMethodMutation($input: RemoveCustomerPaymentMethodInput!) {
      removeCustomerPaymentMethod(input: $input) {
        clientMutationId
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
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

  const handleMyBillingDetailUpdateClick = ({ companyName, email, addressLine1, addressLine2, suburb, city, province, zipcode, country }: CustomerBillingDetails) => {
    const billingDetails = rootData.me.billingDetails;
    if (billingDetails) {
      const toastId = themedToast(<NotificationContent content={`Updating billing...`} />, infoNotificationOptions);

      commitUpdateMyBillingDetails({
        variables: {
          input: {
            clientMutationId: uuid(),
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
      const id = uuid();
      const toastId = themedToast(<NotificationContent content={`Adding billing...`} />, infoNotificationOptions);

      commitAddMyBillingDetails({
        variables: {
          input: {
            clientMutationId: uuid(),
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
          clientMutationId: uuid(),
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
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Billing and Payment">
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

const MemoMyBillingAndPayment = memo(MyBillingAndPayment);

type RelayProps = {
  onReloadRequired: () => void;
};

const MyBillingAndPaymentWithRelay = ({ onReloadRequired }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<myBillingAndPayment_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
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
      setTriggerReloadId(uuid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoMyBillingAndPayment queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(MyBillingAndPaymentWithRelay);
