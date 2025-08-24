import { Address, PhysicalAddress } from '@/components/address';
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
import { DeleteIcon, NewIcon } from '@/components/icons';
import { getRootLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors, keyboardTextFieldDebounceTimeout } from '@/libs/utils';
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
import { useDebounceCallback } from 'usehooks-ts';
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
        osmType
        osmId
        placeId
        longitude
        latitude
        formattedAddress
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
  companyName: string | null;
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
            osmType
            osmId
            placeId
            longitude
            latitude
            formattedAddress
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
            osmType
            osmId
            placeId
            longitude
            latitude
            formattedAddress
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
  const [billingCompanyName, setBillingCompanyName] = useState(rootData.me.billingDetails?.companyName);
  const debounceSetBillingCompanyName = useDebounceCallback(setBillingCompanyName, keyboardTextFieldDebounceTimeout);
  const [billingEmail, setBillingEmail] = useState<string>(rootData.me.billingDetails?.email ?? '');
  const debounceSetBillingEmail = useDebounceCallback(setBillingEmail, keyboardTextFieldDebounceTimeout);
  const [billingOsmType, setBillingOsmType] = useState(rootData.me.billingDetails?.osmType);
  const [billingOsmId, setBillingOsmId] = useState(rootData.me.billingDetails?.osmId);
  const [billingPlaceId, setBillingPlaceId] = useState(rootData.me.billingDetails?.placeId);
  const [billingLongitude, setBillingLongitude] = useState(rootData.me.billingDetails?.longitude);
  const [billingLatitude, setBillingLatitude] = useState(rootData.me.billingDetails?.latitude);
  const [billingFormattedAddress, setBillingFormattedAddress] = useState(rootData.me.billingDetails?.formattedAddress);
  const [billingAddressLine1, setBillingAddressLine1] = useState<string>(rootData.me.billingDetails?.addressLine1 ?? '');
  const debounceSetBillingAddressLine1 = useDebounceCallback(setBillingAddressLine1, keyboardTextFieldDebounceTimeout);
  const [billingAddressLine2, setBillingAddressLine2] = useState(rootData.me.billingDetails?.addressLine2);
  const debounceSetBillingAddressLine2 = useDebounceCallback(setBillingAddressLine2, keyboardTextFieldDebounceTimeout);
  const [billingSuburb, setBillingSuburb] = useState<string>(rootData.me.billingDetails?.suburb ?? '');
  const debounceSetBillingSuburb = useDebounceCallback(setBillingSuburb, keyboardTextFieldDebounceTimeout);
  const [billingCity, setBillingCity] = useState<string>(rootData.me.billingDetails?.city ?? '');
  const debounceSetBillingCity = useDebounceCallback(setBillingCity, keyboardTextFieldDebounceTimeout);
  const [billingProvince, setBillingProvince] = useState(rootData.me.billingDetails?.province);
  const debounceSetBillingProvince = useDebounceCallback(setBillingProvince, keyboardTextFieldDebounceTimeout);
  const [billingZipcode, setBillingZipcode] = useState<string>(rootData.me.billingDetails?.zipcode ?? '');
  const debounceSetBillingZipcode = useDebounceCallback(setBillingZipcode, keyboardTextFieldDebounceTimeout);
  const [billingCountry, setBillingCountry] = useState<string>(rootData.me.billingDetails?.country ?? '');
  const debounceSetBillingCountry = useDebounceCallback(setBillingCountry, keyboardTextFieldDebounceTimeout);

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

  const handleBillingAddressSelect = (address: Address) => {
    setBillingOsmType(address.osmType);
    setBillingOsmId(address.osmId);
    setBillingPlaceId(address.placeId);
    setBillingLongitude(address.longitude);
    setBillingLatitude(address.latitude);
    setBillingFormattedAddress(address.formattedAddress);
    setBillingAddressLine1(address.addressLine1 ?? '');
    setBillingAddressLine2(address.addressLine2 ?? '');
    setBillingSuburb(address.suburb ?? '');
    setBillingCity(address.city ?? '');
    setBillingProvince(address.province ?? '');
    setBillingZipcode(address.zipcode ?? '');
    setBillingCountry(address.countryCode ?? '');
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
            osmType: billingOsmType,
            osmId: billingOsmId,
            placeId: billingPlaceId,
            longitude: billingLongitude,
            latitude: billingLatitude,
            formattedAddress: billingFormattedAddress,
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
                osmType: billingOsmType,
                osmId: billingOsmId,
                placeId: billingPlaceId,
                longitude: billingLongitude,
                latitude: billingLatitude,
                formattedAddress: billingFormattedAddress,
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
            osmType: billingOsmType,
            osmId: billingOsmId,
            placeId: billingPlaceId,
            longitude: billingLongitude,
            latitude: billingLatitude,
            formattedAddress: billingFormattedAddress,
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
                osmType: billingOsmType,
                osmId: billingOsmId,
                placeId: billingPlaceId,
                longitude: billingLongitude,
                latitude: billingLatitude,
                formattedAddress: billingFormattedAddress,
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
                companyName: billingCompanyName,
                email: billingEmail,
                addressLine1: billingAddressLine1,
                addressLine2: billingAddressLine2,
                suburb: billingSuburb,
                city: billingCity,
                province: billingProvince,
                zipcode: billingZipcode,
                country: billingCountry,
              }}
              validate={validateCustomerBilling}
              render={({ handleSubmit, values }) => {
                debounceSetBillingCompanyName(values!.companyName);
                debounceSetBillingEmail(values!.email);
                debounceSetBillingAddressLine1(values!.addressLine1);
                debounceSetBillingAddressLine2(values!.addressLine2);
                debounceSetBillingSuburb(values!.suburb);
                debounceSetBillingCity(values!.city);
                debounceSetBillingProvince(values!.province);
                debounceSetBillingZipcode(values!.zipcode);
                debounceSetBillingCountry(values!.country);

                return (
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
                    </StackColumn>

                    <PhysicalAddress
                      addressLine1Name="addressLine1"
                      addressLine1Required={requiredCustomerBillingFields.addressLine1}
                      addressLine2Name="addressLine2"
                      addressLine2Required={requiredCustomerBillingFields.addressLine2}
                      suburbName="suburb"
                      suburbRequired={requiredCustomerBillingFields.suburb}
                      cityName="city"
                      cityRequired={requiredCustomerBillingFields.city}
                      provinceName="province"
                      provinceRequired={requiredCustomerBillingFields.province}
                      zipcodeName="zipcode"
                      zipcodeRequired={requiredCustomerBillingFields.zipcode}
                      countryName="country"
                      countryRequired={requiredCustomerBillingFields.country}
                      onSelect={handleBillingAddressSelect}
                    />

                    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                      <StackRow>
                        <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                          Update
                        </Button>
                      </StackRow>
                    </StackColumn>
                  </FormStackColumn>
                );
              }}
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
