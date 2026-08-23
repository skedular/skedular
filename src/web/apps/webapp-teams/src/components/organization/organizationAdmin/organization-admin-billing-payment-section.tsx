import { Address, PhysicalAddress } from '@/components/address';
import { DeleteIcon, NewIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { AddOrganizationPaymentMethodDialog } from '@/components/organization/addOrganizationPaymentMethod';
import { BillingDetails, billingSchema } from '@/components/organization/organizationAdmin/organization-admin-shared';
import type { organizationAdminBillingPaymentSectionQuery } from '@/queries/__generated__/organizationAdminBillingPaymentSectionQuery.graphql';
import type { organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation } from '@/queries/__generated__/organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation.graphql';
import type { organizationAdminBillingPaymentSection_updateOrganizationBillingDetailsMutation } from '@/queries/__generated__/organizationAdminBillingPaymentSection_updateOrganizationBillingDetailsMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
import { BodyIconTypography, FormFieldLabel, FormStackColumn, SettingsSectionCard, StackColumn, StackRow } from '@skedular/ui';
import CreditCard from '@skedular/ui/commons/credit-card';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';

type Props = {
  organizationCustomDomain: string;
  section?: 'billing-details' | 'payment-methods' | 'all';
};

type InnerProps = {
  organizationCustomDomain: string;
  onRefetchRequired: () => void;
  queryReference: PreloadedQuery<organizationAdminBillingPaymentSectionQuery>;
  section: 'billing-details' | 'payment-methods' | 'all';
};

type BillingDetailsPatchField = 'COMPANY_NAME' | 'EMAIL' | 'BILLING_ADDRESS';

const inlinePatchDebounceTimeout = 1000;
const billingPatchFieldNames: Record<BillingDetailsPatchField, ReadonlyArray<keyof BillingDetails>> = {
  COMPANY_NAME: ['companyName'],
  EMAIL: ['email'],
  BILLING_ADDRESS: ['addressLine1', 'addressLine2', 'suburb', 'city', 'province', 'zipcode', 'countryCode'],
};

const getValidBillingDetailsPatchFields = (fieldsToUpdate: BillingDetailsPatchField[], values: BillingDetails): BillingDetailsPatchField[] =>
  fieldsToUpdate.filter((patchField) => {
    try {
      for (const formField of billingPatchFieldNames[patchField]) {
        billingSchema.validateSyncAt(formField, values);
      }

      return true;
    } catch {
      return false;
    }
  });

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

const OrganizationAdminBillingPaymentSectionContent = ({ organizationCustomDomain, onRefetchRequired, queryReference, section }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationAdminBillingPaymentSectionQuery>(RootQuery, queryReference);
  const [commitUpdateOrganizationBillingDetailsPatch] = useMutation<organizationAdminBillingPaymentSection_updateOrganizationBillingDetailsMutation>(graphql`
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

  const [billingOsmType, setBillingOsmType] = useState(organization?.billingDetails?.osmType);
  const [billingOsmId, setBillingOsmId] = useState(organization?.billingDetails?.osmId);
  const [billingPlaceId, setBillingPlaceId] = useState(organization?.billingDetails?.placeId);
  const [billingLongitude, setBillingLongitude] = useState(organization?.billingDetails?.longitude);
  const [billingLatitude, setBillingLatitude] = useState(organization?.billingDetails?.latitude);
  const [billingFormattedAddress, setBillingFormattedAddress] = useState(organization?.billingDetails?.formattedAddress);
  const [billingCountry, setBillingCountry] = useState<string>(organization?.billingDetails?.country ?? '');
  const [isAddPaymentMethodDialogOpen, setIsAddPaymentMethodDialogOpen] = useState(false);
  const initialBillingValues = useMemo<BillingDetails>(
    () => ({
      companyName: organization?.billingDetails?.companyName ?? null,
      email: organization?.billingDetails?.email ?? '',
      osmType: billingOsmType,
      osmId: billingOsmId,
      placeId: billingPlaceId,
      longitude: billingLongitude,
      latitude: billingLatitude,
      formattedAddress: billingFormattedAddress,
      country: billingCountry,
      addressLine1: organization?.billingDetails?.addressLine1 ?? '',
      addressLine2: organization?.billingDetails?.addressLine2 ?? null,
      suburb: organization?.billingDetails?.suburb ?? null,
      city: organization?.billingDetails?.city ?? null,
      province: organization?.billingDetails?.province ?? null,
      zipcode: organization?.billingDetails?.zipcode ?? '',
      countryCode: organization?.billingDetails?.countryCode ?? '',
    }),
    [billingCountry, billingFormattedAddress, billingLatitude, billingLongitude, billingOsmId, billingOsmType, billingPlaceId, organization],
  );
  const draftBillingValues = useRef(initialBillingValues);
  const submittedBillingAddressKey = useRef<string | null>(null);

  const handleBillingAddressSelect = (address: Address) => {
    setBillingOsmType(address.osmType);
    setBillingOsmId(address.osmId);
    setBillingPlaceId(address.placeId);
    setBillingLongitude(address.longitude);
    setBillingLatitude(address.latitude);
    setBillingFormattedAddress(address.formattedAddress);
    setBillingCountry(address.country ?? '');
  };

  const commitBillingDetailsPatch = useCallback(
    (fieldsToUpdate: BillingDetailsPatchField[], values: BillingDetails) => {
      const {
        companyName,
        email,
        osmType,
        osmId,
        placeId,
        longitude,
        latitude,
        formattedAddress,
        country: lookupCountry,
        addressLine1,
        addressLine2,
        suburb,
        city,
        province,
        zipcode,
        countryCode,
      } = values;
      const validFieldsToUpdate = getValidBillingDetailsPatchFields(fieldsToUpdate, values);
      if (!organization || validFieldsToUpdate.length === 0) {
        return;
      }

      const countryData = getCountryData(countryCode as TCountryCode);
      let country = lookupCountry;
      if (countryData) {
        country = countryData.name;
      }

      const billingDetailsId = organization.billingDetails?.id ?? uuid();
      commitUpdateOrganizationBillingDetailsPatch({
        variables: {
          input: {
            clientMutationId: uuid(),
            organizationCustomDomain,
            fieldsToUpdate: validFieldsToUpdate,
            companyName,
            email,
            osmType,
            osmId,
            placeId,
            longitude,
            latitude,
            formattedAddress,
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
            themedToast(
              <NotificationContent content={`We couldn't update billing for organization '${organization.name}'. ${getRelayErrorMessage(errors)}`} />,
              errorNotificationOptions,
            );
          }
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`We couldn't update billing for organization '${organization.name}'. ${error.message}`} />, errorNotificationOptions);
        },
        optimisticResponse: {
          updateOrganizationBillingDetails: {
            organization: {
              id: organization.id,
              billingDetails: {
                id: billingDetailsId,
                companyName,
                email,
                osmType,
                osmId,
                placeId,
                longitude,
                latitude,
                formattedAddress,
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
    },
    [commitUpdateOrganizationBillingDetailsPatch, organization, organizationCustomDomain, themedToast],
  );
  const debouncedCommitBillingDetailsPatch = useDebounceCallback(commitBillingDetailsPatch, inlinePatchDebounceTimeout);

  if (!organization) {
    return null;
  }

  const paymentMethodExist = organization.paymentMethods.length > 0;

  const handleAddPaymentMethodClicked = () => {
    setIsAddPaymentMethodDialogOpen(true);
  };

  const handleAddPaymentMethodCancel = () => {
    setIsAddPaymentMethodDialogOpen(false);
    onRefetchRequired();
  };

  const handleRemovePaymentMethodClick = (id: string) => {
    commitRemoveOrganizationPaymentMethod({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't remove that payment method. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        onRefetchRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove that payment method. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  return (
    <>
      <Form<BillingDetails>
        onSubmit={() => undefined}
        initialValues={initialBillingValues}
        validate={validateOrganizationBilling}
        render={({ handleSubmit, values, form }) => {
          const formValues = values as BillingDetails;
          const nextBillingAddressKey = JSON.stringify({
            values: {
              osmType: formValues.osmType,
              osmId: formValues.osmId,
              placeId: formValues.placeId,
              longitude: formValues.longitude,
              latitude: formValues.latitude,
              formattedAddress: formValues.formattedAddress,
              country: formValues.country,
              addressLine1: formValues.addressLine1,
              addressLine2: formValues.addressLine2,
              suburb: formValues.suburb,
              city: formValues.city,
              province: formValues.province,
              zipcode: formValues.zipcode,
              countryCode: formValues.countryCode,
            },
          });

          const changedFields: BillingDetailsPatchField[] = [];
          if (draftBillingValues.current.companyName !== formValues.companyName) {
            changedFields.push('COMPANY_NAME');
          }
          if (draftBillingValues.current.email !== formValues.email) {
            changedFields.push('EMAIL');
          }
          if (submittedBillingAddressKey.current === null) {
            submittedBillingAddressKey.current = nextBillingAddressKey;
          } else if (nextBillingAddressKey !== submittedBillingAddressKey.current) {
            changedFields.push('BILLING_ADDRESS');
          }
          if (changedFields.length > 0) {
            draftBillingValues.current = formValues;
            submittedBillingAddressKey.current = nextBillingAddressKey;
            debouncedCommitBillingDetailsPatch(changedFields, formValues);
          }

          return (
            <FormStackColumn onSubmit={handleSubmit}>
              <Box sx={{ pb: 2 }}>
                <StackColumn spacing={2}>
                  {section !== 'payment-methods' && (
                    <SettingsSectionCard
                      bare={section !== 'all'}
                      title="Billing details"
                      description="Control invoice recipients and the legal billing address used for the organization."
                    >
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
                              form.change('osmType', address.osmType);
                              form.change('osmId', address.osmId);
                              form.change('placeId', address.placeId);
                              form.change('longitude', address.longitude);
                              form.change('latitude', address.latitude);
                              form.change('formattedAddress', address.formattedAddress);
                              form.change('country', address.country ?? '');
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
                    </SettingsSectionCard>
                  )}

                  {section !== 'billing-details' && (
                    <SettingsSectionCard
                      bare={section !== 'all'}
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
                  )}
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

const OrganizationAdminBillingPaymentSection = ({ organizationCustomDomain, section = 'all' }: Props) => {
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
      section={section}
      onRefetchRequired={() => setReloadKey(uuid())}
      queryReference={queryReference}
    />
  );
};

export default memo(OrganizationAdminBillingPaymentSection);
