import { Address, PhysicalAddress } from '@/components/address';
import { CreditCard, FormFieldLabel, FormStackColumn, LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { DeleteIcon, NewIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import { PaletteModeContext } from '@skedular/shared';
import { defaultButtonStyle, defaultPadding, EditorActionBar, PageHeaderPanel } from '@skedular/ui';
import { getRelayErrorMessage, keyboardTextFieldDebounceTimeout } from '@skedular/shared';
import type { myBillingAndPayment_addMyBillingDetailsMutation } from '@/queries/__generated__/myBillingAndPayment_addMyBillingDetailsMutation.graphql';
import type { myBillingAndPayment_customerPaymentMethodsDetails_query$key } from '@/queries/__generated__/myBillingAndPayment_customerPaymentMethodsDetails_query.graphql';
import type { myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment } from '@/queries/__generated__/myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment.graphql';
import type { myBillingAndPayment_removeCustomerPaymentMethodMutation } from '@/queries/__generated__/myBillingAndPayment_removeCustomerPaymentMethodMutation.graphql';
import type { myBillingAndPayment_rootQuery } from '@/queries/__generated__/myBillingAndPayment_rootQuery.graphql';
import type { myBillingAndPayment_updateMyBillingDetailsMutation } from '@/queries/__generated__/myBillingAndPayment_updateMyBillingDetailsMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useSearchParams } from 'next/navigation';
import { memo, startTransition, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';
import AddMyPaymentMethodDialog from './add-my-payment-method-dialog';
import MyBillingAndPaymentSectionNav, { MyBillingAndPaymentSection } from './my-billing-and-payment-section-nav';

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
        countryCode
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
  suburb: string | null;
  city: string | null;
  province: string | null;
  zipcode: string;
  countryCode: string;
};

const customerBillingSchema = object({
  companyName: string().nullable(),
  email: string()
    .email(({ value }) => `${value} is not a valid email`)
    .required('Email is required'),
  addressLine1: string().required('Address line 1 is required'),
  addressLine2: string().nullable(),
  suburb: string().nullable(),
  city: string().nullable(),
  province: string().nullable(),
  zipcode: string().required('Zipcode is required'),
  countryCode: string().required('Country is required'),
});

const validSections: MyBillingAndPaymentSection[] = ['payment-methods', 'billing-details'];

const getActiveSection = (value: string | null): MyBillingAndPaymentSection => {
  if (value && validSections.includes(value as MyBillingAndPaymentSection)) {
    return value as MyBillingAndPaymentSection;
  }

  return 'payment-methods';
};

const formColumnSx = {
  width: '100%',
  maxWidth: 760,
};

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
            countryCode
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
            countryCode
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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const activeSection = useMemo(() => getActiveSection(searchParams.get('section')), [searchParams]);
  const [stickyTop, setStickyTop] = useState(0);
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
  const [billingSuburb, setBillingSuburb] = useState(rootData.me.billingDetails?.suburb);
  const debounceSetBillingSuburb = useDebounceCallback(setBillingSuburb, keyboardTextFieldDebounceTimeout);
  const [billingCity, setBillingCity] = useState(rootData.me.billingDetails?.city);
  const debounceSetBillingCity = useDebounceCallback(setBillingCity, keyboardTextFieldDebounceTimeout);
  const [billingProvince, setBillingProvince] = useState(rootData.me.billingDetails?.province);
  const debounceSetBillingProvince = useDebounceCallback(setBillingProvince, keyboardTextFieldDebounceTimeout);
  const [billingZipcode, setBillingZipcode] = useState<string>(rootData.me.billingDetails?.zipcode ?? '');
  const debounceSetBillingZipcode = useDebounceCallback(setBillingZipcode, keyboardTextFieldDebounceTimeout);
  const [billingCountry, setBillingCountry] = useState<string>(rootData.me.billingDetails?.country ?? '');
  const [billingCountryCode, setBillingCountryCode] = useState<string>(rootData.me.billingDetails?.countryCode ?? '');
  const debounceSetBillingCountryCode = useDebounceCallback(setBillingCountryCode, keyboardTextFieldDebounceTimeout);

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
    setBillingCountry(address.country ?? '');
    setBillingCountryCode(address.countryCode ?? '');
  };

  const handleMyBillingDetailUpdateClick = ({ companyName, email, addressLine1, addressLine2, suburb, city, province, zipcode, countryCode }: CustomerBillingDetails) => {
    const countryData = getCountryData(countryCode as TCountryCode);
    let country = billingCountry;
    if (countryData) {
      country = countryData.name;
    }

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
            countryCode,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to update billing. Error: ${getRelayErrorMessage(errors)}.`} />,
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
                countryCode,
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
            countryCode,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to add billing. Error: ${getRelayErrorMessage(errors)}.`} />,
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
                countryCode,
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
            render: <NotificationContent content={`Failed to remove payment method. Error: ${getRelayErrorMessage(errors)}.`} />,
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

  const renderPaymentMethodsSection = () => (
    <Box sx={{ p: defaultPadding }}>
      <StackColumn spacing={2}>
        <StackRow sx={{ alignItems: 'flex-start', justifyContent: 'space-between', gap: 2, flexWrap: 'wrap' }}>
          <StackColumn spacing={0.5}>
            <LeadIconTypography label="Payment Methods" />
            <SmallIconTypography label="Manage the card used for user billing and checkout payments." />
          </StackColumn>
          {!paymentMethodExist && (
            <Button variant="contained" startIcon={<NewIcon />} onClick={handleAddPaymentMethodClicked} sx={defaultButtonStyle}>
              Add Payment Method
            </Button>
          )}
        </StackRow>

        <Divider />

        {paymentMethodExist ? (
          <StackColumn spacing={0}>
            {rootDataMyPaymentMethodsDetails.me?.paymentMethods.map((item, index) => (
              <StackColumn key={item.id} spacing={2}>
                {index > 0 && <Divider />}
                <StackColumn sx={{ alignItems: 'flex-start' }} spacing={1.5}>
                  <CreditCard lastFourDigits={item.cardLastFourDigit} expiryDate={`${item.cardExpiryMonth}/${item.cardExpiryYear}`} cardBrand={item.cardBrand} />
                  <Button variant="contained" color="error" startIcon={<DeleteIcon />} onClick={() => handleRemovePaymentMethodClick(item.id)} sx={{ textTransform: 'none' }}>
                    Remove Payment Method
                  </Button>
                </StackColumn>
              </StackColumn>
            ))}
          </StackColumn>
        ) : (
          <StackColumn spacing={0.5}>
            <LeadIconTypography label="No payment method set up yet" />
            <SmallIconTypography label="Add a card before paying invoices or bookings that require saved payment details." />
          </StackColumn>
        )}
      </StackColumn>
    </Box>
  );

  const renderBillingDetailsSection = () => (
    <Box sx={{ p: defaultPadding }}>
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
          countryCode: billingCountryCode,
        }}
        validate={validateCustomerBilling}
        render={({ handleSubmit, values, form }) => {
          debounceSetBillingCompanyName(values!.companyName);
          debounceSetBillingEmail(values!.email);
          debounceSetBillingAddressLine1(values!.addressLine1);
          debounceSetBillingAddressLine2(values!.addressLine2);
          debounceSetBillingSuburb(values!.suburb);
          debounceSetBillingCity(values!.city);
          debounceSetBillingProvince(values!.province);
          debounceSetBillingZipcode(values!.zipcode);
          debounceSetBillingCountryCode(values!.countryCode);

          return (
            <FormStackColumn onSubmit={handleSubmit} sx={formColumnSx}>
              <StackColumn spacing={2}>
                <StackColumn spacing={0.5}>
                  <LeadIconTypography label="Billing Details" />
                  <SmallIconTypography label="Set the invoice recipient and billing address used for user payments." />
                </StackColumn>

                <Divider />

                <FormFieldLabel label="Company">
                  <TextField name="companyName" required={requiredCustomerBillingFields.companyName} />
                </FormFieldLabel>

                <FormFieldLabel label="Email">
                  <TextField name="email" required={requiredCustomerBillingFields.email} helperText="Email address invoices should be sent to" />
                </FormFieldLabel>

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
                  countryName="countryCode"
                  countryRequired={requiredCustomerBillingFields.countryCode}
                  onSelect={(address) => {
                    handleBillingAddressSelect(address);
                    form.batch(() => {
                      form.change('addressLine1', address.addressLine1 ?? '');
                      form.change('addressLine2', address.addressLine2 ?? '');
                      form.change('suburb', address.suburb ?? '');
                      form.change('city', address.city ?? '');
                      form.change('province', address.province ?? '');
                      form.change('zipcode', address.zipcode ?? '');
                      form.change('countryCode', address.countryCode ?? '');
                    });
                  }}
                />
              </StackColumn>

              <EditorActionBar
                primaryAction={
                  <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                    Update
                  </Button>
                }
              />
            </FormStackColumn>
          );
        }}
      />
    </Box>
  );

  const renderActiveSection = () => {
    switch (activeSection) {
      case 'billing-details':
        return renderBillingDetailsSection();
      case 'payment-methods':
      default:
        return renderPaymentMethodsSection();
    }
  };

  return (
    <>
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
          <PageHeaderPanel eyebrow="Billing and payment" title="Billing & Payment" description="Manage saved payment methods and invoice details for your user account.">
            <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
              <Chip size="small" label={paymentMethodExist ? 'Payment method ready' : 'No payment method'} />
              <Chip size="small" label={rootData.me.billingDetails ? 'Billing details saved' : 'Billing details missing'} />
            </StackRow>
          </PageHeaderPanel>

          <MyBillingAndPaymentSectionNav activeSection={activeSection} stickyTop={stickyTop} />

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
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoMyBillingAndPayment queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(MyBillingAndPaymentWithRelay);
