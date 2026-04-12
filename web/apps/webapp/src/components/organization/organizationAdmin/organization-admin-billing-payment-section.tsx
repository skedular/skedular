import { Address, PhysicalAddress } from '@/components/address';
import { BodyIconTypography, CreditCard, FormFieldLabel, FormStackColumn, StackColumn, StackRow } from '@/components/commons';
import { DeleteIcon, NewIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { AddOrganizationPaymentMethodDialog } from '@/components/organization/addOrganizationPaymentMethod';
import { BillingDetails, billingSchema } from '@/components/organization/organizationAdmin/organization-admin-shared';
import { PaletteModeContext } from '@/libs/providers';
import { getRelayErrorMessage, keyboardTextFieldDebounceTimeout } from '@/libs/utils';
import type { organizationAdminBillingPaymentSectionQuery } from '@/queries/__generated__/organizationAdminBillingPaymentSectionQuery.graphql';
import type { organizationAdminBillingPaymentSection_addOrganizationBillingDetailsMutation } from '@/queries/__generated__/organizationAdminBillingPaymentSection_addOrganizationBillingDetailsMutation.graphql';
import type { organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation } from '@/queries/__generated__/organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation.graphql';
import type { organizationAdminBillingPaymentSection_updateOrganizationBillingDetailsMutation } from '@/queries/__generated__/organizationAdminBillingPaymentSection_updateOrganizationBillingDetailsMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { EditorActionBar, SettingsSectionCard } from '@skedular/ui';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useEffect, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';

type Props = {
  organizationCustomDomain: string;
};

type InnerProps = {
  organizationCustomDomain: string;
  onRefetchRequired: () => void;
  queryReference: PreloadedQuery<organizationAdminBillingPaymentSectionQuery>;
};

const RootQuery = graphql`
  query organizationAdminBillingPaymentSectionQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      id
      name
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
      paymentMethods {
        id
        cardBrand
        cardExpiryMonth
        cardExpiryYear
        cardLastFourDigit
      }
    }
  }
`;

const OrganizationAdminBillingPaymentSectionContent = ({ organizationCustomDomain, onRefetchRequired, queryReference }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationAdminBillingPaymentSectionQuery>(RootQuery, queryReference);
  const [commitAddOrganizationBillingDetails] = useMutation<organizationAdminBillingPaymentSection_addOrganizationBillingDetailsMutation>(graphql`
    mutation organizationAdminBillingPaymentSection_addOrganizationBillingDetailsMutation($input: AddOrganizationBillingDetailsInput!) @raw_response_type {
      addOrganizationBillingDetails(input: $input) {
        organization {
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
  const [commitUpdateOrganizationBillingDetails] = useMutation<organizationAdminBillingPaymentSection_updateOrganizationBillingDetailsMutation>(graphql`
    mutation organizationAdminBillingPaymentSection_updateOrganizationBillingDetailsMutation($input: UpdateOrganizationBillingDetailsInput!) @raw_response_type {
      updateOrganizationBillingDetails(input: $input) {
        organization {
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
  const [commitRemoveOrganizationPaymentMethod] = useMutation<organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation>(graphql`
    mutation organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation($input: RemoveOrganizationPaymentMethodInput!) {
      removeOrganizationPaymentMethod(input: $input) {
        clientMutationId
      }
    }
  `);

  const organization = rootData.organization;
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateOrganizationBilling = makeValidate(billingSchema);
  const requiredBillingFields = makeRequired(billingSchema);
  const formColumnSx = {
    width: '100%',
    maxWidth: 760,
  };

  const [billingCompanyName, setBillingCompanyName] = useState(organization?.billingDetails?.companyName);
  const debounceSetBillingCompanyName = useDebounceCallback(setBillingCompanyName, keyboardTextFieldDebounceTimeout);
  const [billingEmail, setBillingEmail] = useState<string>(organization?.billingDetails?.email ?? '');
  const debounceSetBillingEmail = useDebounceCallback(setBillingEmail, keyboardTextFieldDebounceTimeout);
  const [billingOsmType, setBillingOsmType] = useState(organization?.billingDetails?.osmType);
  const [billingOsmId, setBillingOsmId] = useState(organization?.billingDetails?.osmId);
  const [billingPlaceId, setBillingPlaceId] = useState(organization?.billingDetails?.placeId);
  const [billingLongitude, setBillingLongitude] = useState(organization?.billingDetails?.longitude);
  const [billingLatitude, setBillingLatitude] = useState(organization?.billingDetails?.latitude);
  const [billingFormattedAddress, setBillingFormattedAddress] = useState(organization?.billingDetails?.formattedAddress);
  const [billingAddressLine1, setBillingAddressLine1] = useState<string>(organization?.billingDetails?.addressLine1 ?? '');
  const debounceSetBillingAddressLine1 = useDebounceCallback(setBillingAddressLine1, keyboardTextFieldDebounceTimeout);
  const [billingAddressLine2, setBillingAddressLine2] = useState(organization?.billingDetails?.addressLine2);
  const debounceSetBillingAddressLine2 = useDebounceCallback(setBillingAddressLine2, keyboardTextFieldDebounceTimeout);
  const [billingSuburb, setBillingSuburb] = useState(organization?.billingDetails?.suburb);
  const debounceSetBillingSuburb = useDebounceCallback(setBillingSuburb, keyboardTextFieldDebounceTimeout);
  const [billingCity, setBillingCity] = useState(organization?.billingDetails?.city);
  const debounceSetBillingCity = useDebounceCallback(setBillingCity, keyboardTextFieldDebounceTimeout);
  const [billingProvince, setBillingProvince] = useState(organization?.billingDetails?.province);
  const debounceSetBillingProvince = useDebounceCallback(setBillingProvince, keyboardTextFieldDebounceTimeout);
  const [billingZipcode, setBillingZipcode] = useState<string>(organization?.billingDetails?.zipcode ?? '');
  const debounceSetBillingZipcode = useDebounceCallback(setBillingZipcode, keyboardTextFieldDebounceTimeout);
  const [billingCountry, setBillingCountry] = useState<string>(organization?.billingDetails?.country ?? '');
  const [billingCountryCode, setBillingCountryCode] = useState<string>(organization?.billingDetails?.countryCode ?? '');
  const debounceSetBillingCountryCode = useDebounceCallback(setBillingCountryCode, keyboardTextFieldDebounceTimeout);
  const [isAddPaymentMethodDialogOpen, setIsAddPaymentMethodDialogOpen] = useState(false);

  if (!organization) {
    return null;
  }

  const paymentMethodExist = organization.paymentMethods.length > 0;

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

  const handleBillingDetailUpdateClick = ({ companyName, email, addressLine1, addressLine2, suburb, city, province, zipcode, countryCode }: BillingDetails) => {
    const countryData = getCountryData(countryCode as TCountryCode);
    let country = billingCountry;
    if (countryData) {
      country = countryData.name;
    }

    const billingDetails = organization.billingDetails;

    if (billingDetails) {
      const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' billing...`} />, infoNotificationOptions);

      commitUpdateOrganizationBillingDetails({
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
              render: <NotificationContent content={`We couldn't update billing for organisation '${organization.name}'. ${getRelayErrorMessage(errors)}`} />,
            });

            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Billing for organisation '${organization.name}' has been updated.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't update billing for organisation '${organization.name}'. ${error.message}`} />,
          });
        },
        optimisticResponse: {
          updateOrganizationBillingDetails: {
            organization: {
              id: organization.id,
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

      return;
    }

    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Adding billing for organisation '${organization.name}'...`} />, infoNotificationOptions);

    commitAddOrganizationBillingDetails({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
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
            render: <NotificationContent content={`We couldn't add billing for organisation '${organization.name}'. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Billing for organisation '${organization.name}' has been added.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't add billing for organisation '${organization.name}'. ${error.message}`} />,
        });
      },
      optimisticResponse: {
        addOrganizationBillingDetails: {
          organization: {
            id: organization.id,
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
  };

  const handleAddPaymentMethodClicked = () => {
    setIsAddPaymentMethodDialogOpen(true);
  };

  const handleAddPaymentMethodCancel = () => {
    setIsAddPaymentMethodDialogOpen(false);
    onRefetchRequired();
  };

  const handleRemovePaymentMethodClick = (id: string) => {
    const toastId = themedToast(<NotificationContent content="Removing payment method..." />, infoNotificationOptions);

    commitRemoveOrganizationPaymentMethod({
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
            render: <NotificationContent content={`We couldn't remove that payment method. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content="The payment method has been removed." />,
        });
        onRefetchRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't remove that payment method. ${error.message}`} />,
        });
      },
    });
  };

  return (
    <>
      <Form
        onSubmit={handleBillingDetailUpdateClick}
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
        validate={validateOrganizationBilling}
        render={({ handleSubmit, values, form }) => {
          const formValues = values!;

          debounceSetBillingCompanyName(formValues.companyName);
          debounceSetBillingEmail(formValues.email);
          debounceSetBillingAddressLine1(formValues.addressLine1);
          debounceSetBillingAddressLine2(formValues.addressLine2);
          debounceSetBillingSuburb(formValues.suburb);
          debounceSetBillingCity(formValues.city);
          debounceSetBillingProvince(formValues.province);
          debounceSetBillingZipcode(formValues.zipcode);
          debounceSetBillingCountryCode(formValues.countryCode);

          return (
            <FormStackColumn onSubmit={handleSubmit}>
              <Box sx={{ pb: 2 }}>
                <StackColumn spacing={2}>
                  <SettingsSectionCard title="Billing details" description="Control invoice recipients and the legal billing address used for the organization.">
                    <StackColumn sx={formColumnSx}>
                      <FormFieldLabel label="Company name">
                        <TextField name="companyName" required={requiredBillingFields.companyName} />
                      </FormFieldLabel>

                      <FormFieldLabel label="Email">
                        <TextField name="email" required={requiredBillingFields.email} helperText="Email to send invoice to" />
                      </FormFieldLabel>

                      <PhysicalAddress
                        addressLine1Name="addressLine1"
                        addressLine1Required={requiredBillingFields.addressLine1}
                        addressLine2Name="addressLine2"
                        addressLine2Required={requiredBillingFields.addressLine2}
                        suburbName="suburb"
                        suburbRequired={requiredBillingFields.suburb}
                        cityName="city"
                        cityRequired={requiredBillingFields.city}
                        provinceName="province"
                        provinceRequired={requiredBillingFields.province}
                        zipcodeName="zipcode"
                        zipcodeRequired={requiredBillingFields.zipcode}
                        countryName="countryCode"
                        countryRequired={requiredBillingFields.countryCode}
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

                      <EditorActionBar primaryAction="Update" />
                    </StackColumn>
                  </SettingsSectionCard>

                  <SettingsSectionCard
                    title="Payment method"
                    description={
                      paymentMethodExist
                        ? 'The active payment methods available for subscriptions and upgrades.'
                        : 'Attach a payment method before upgrading or changing paid offerings.'
                    }
                    actions={
                      !paymentMethodExist ? (
                        <Button variant="text" onClick={handleAddPaymentMethodClicked} sx={{ textTransform: 'none' }}>
                          <BodyIconTypography label="Add Payment Method" endElement={<NewIcon fontSize="large" />} />
                        </Button>
                      ) : undefined
                    }
                  >
                    {paymentMethodExist ? (
                      <StackRow sx={{ gap: 2, flexWrap: 'wrap' }}>
                        {organization.paymentMethods.map((item) => (
                          <StackColumn key={item.id}>
                            <CreditCard lastFourDigits={item.cardLastFourDigit} expiryDate={`${item.cardExpiryMonth}/${item.cardExpiryYear}`} cardBrand={item.cardBrand} />
                            <Button variant="contained" color="warning" onClick={() => handleRemovePaymentMethodClick(item.id)}>
                              <BodyIconTypography label="Remove Payment Method" invertDefaultColor={paletteMode === 'dark'} startElement={<DeleteIcon />} />
                            </Button>
                          </StackColumn>
                        ))}
                      </StackRow>
                    ) : (
                      <BodyIconTypography label="No payment method setup yet" />
                    )}
                  </SettingsSectionCard>
                </StackColumn>
              </Box>
            </FormStackColumn>
          );
        }}
      />

      {!paymentMethodExist && isAddPaymentMethodDialogOpen && (
        <AddOrganizationPaymentMethodDialog
          organizationCustomDomain={organizationCustomDomain}
          isDialogOpen={isAddPaymentMethodDialogOpen}
          onCancel={handleAddPaymentMethodCancel}
        />
      )}
    </>
  );
};

const OrganizationAdminBillingPaymentSection = ({ organizationCustomDomain }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationAdminBillingPaymentSectionQuery>(RootQuery);
  const [reloadKey, setReloadKey] = useState(uuid());

  useEffect(() => {
    loadQuery(
      { organizationCustomDomain },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain, reloadKey]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <OrganizationAdminBillingPaymentSectionContent
      key={reloadKey}
      organizationCustomDomain={organizationCustomDomain}
      onRefetchRequired={() => setReloadKey(uuid())}
      queryReference={queryReference}
    />
  );
};

export default memo(OrganizationAdminBillingPaymentSection);
