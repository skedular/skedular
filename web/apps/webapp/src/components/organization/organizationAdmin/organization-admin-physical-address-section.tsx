import { Address, PhysicalAddress } from '@/components/address';
import { FormStackColumn, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { physicalAddressSchema, PhysicalAddressDetails } from '@/components/organization/organizationAdmin/organization-admin-shared';
import { keyboardTextFieldDebounceTimeout } from '@/libs/utils';
import { getRelayErrorMessage } from '@/libs/utils';
import type { organizationAdminPhysicalAddressSectionQuery } from '@/queries/__generated__/organizationAdminPhysicalAddressSectionQuery.graphql';
import type { organizationAdminPhysicalAddressSection_addOrganizationPhysicalAddressMutation } from '@/queries/__generated__/organizationAdminPhysicalAddressSection_addOrganizationPhysicalAddressMutation.graphql';
import type { organizationAdminPhysicalAddressSection_updateOrganizationPhysicalAddressMutation } from '@/queries/__generated__/organizationAdminPhysicalAddressSection_updateOrganizationPhysicalAddressMutation.graphql';
import Box from '@mui/material/Box';
import { EditorActionBar, SettingsSectionCard } from '@skedular/ui';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { makeRequired, makeValidate } from 'mui-rff';
import { memo, useContext, useEffect, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { PaletteModeContext } from '@/libs/providers';

type Props = {
  organizationCustomDomain: string;
};

type InnerProps = {
  organizationCustomDomain: string;
  queryReference: PreloadedQuery<organizationAdminPhysicalAddressSectionQuery>;
};

const RootQuery = graphql`
  query organizationAdminPhysicalAddressSectionQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      id
      name
      physicalAddress {
        id
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
`;

const OrganizationAdminPhysicalAddressSectionContent = ({ organizationCustomDomain, queryReference }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationAdminPhysicalAddressSectionQuery>(RootQuery, queryReference);
  const [commitAddOrganizationPhysicalAddress] = useMutation<organizationAdminPhysicalAddressSection_addOrganizationPhysicalAddressMutation>(graphql`
    mutation organizationAdminPhysicalAddressSection_addOrganizationPhysicalAddressMutation($input: AddOrganizationPhysicalAddressInput!) @raw_response_type {
      addOrganizationPhysicalAddress(input: $input) {
        organization {
          id
          physicalAddress {
            id
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
  const [commitUpdateOrganizationPhysicalAddress] = useMutation<organizationAdminPhysicalAddressSection_updateOrganizationPhysicalAddressMutation>(graphql`
    mutation organizationAdminPhysicalAddressSection_updateOrganizationPhysicalAddressMutation($input: UpdateOrganizationPhysicalAddressInput!) @raw_response_type {
      updateOrganizationPhysicalAddress(input: $input) {
        organization {
          id
          physicalAddress {
            id
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

  const organization = rootData.organization;
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validatePhysicalAddress = makeValidate(physicalAddressSchema);
  const requiredPhysicalAddressFields = makeRequired(physicalAddressSchema);
  const formColumnSx = {
    width: '100%',
    maxWidth: 760,
  };

  const [physicalAddressOsmType, setPhysicalAddressOsmType] = useState(organization?.physicalAddress?.osmType);
  const [physicalAddressOsmId, setPhysicalAddressOsmId] = useState(organization?.physicalAddress?.osmId);
  const [physicalAddressPlaceId, setPhysicalAddressPlaceId] = useState(organization?.physicalAddress?.placeId);
  const [physicalAddressLongitude, setPhysicalAddressLongitude] = useState(organization?.physicalAddress?.longitude);
  const [physicalAddressLatitude, setPhysicalAddressLatitude] = useState(organization?.physicalAddress?.latitude);
  const [physicalAddressFormattedAddress, setPhysicalAddressFormattedAddress] = useState(organization?.physicalAddress?.formattedAddress);
  const [physicalAddressAddressLine1, setPhysicalAddressAddressLine1] = useState<string>(organization?.physicalAddress?.addressLine1 ?? '');
  const debounceSetPhysicalAddressAddressLine1 = useDebounceCallback(setPhysicalAddressAddressLine1, keyboardTextFieldDebounceTimeout);
  const [physicalAddressAddressLine2, setPhysicalAddressAddressLine2] = useState(organization?.physicalAddress?.addressLine2);
  const debounceSetPhysicalAddressAddressLine2 = useDebounceCallback(setPhysicalAddressAddressLine2, keyboardTextFieldDebounceTimeout);
  const [physicalAddressSuburb, setPhysicalAddressSuburb] = useState(organization?.physicalAddress?.suburb);
  const debounceSetPhysicalAddressSuburb = useDebounceCallback(setPhysicalAddressSuburb, keyboardTextFieldDebounceTimeout);
  const [physicalAddressCity, setPhysicalAddressCity] = useState(organization?.physicalAddress?.city);
  const debounceSetPhysicalAddressCity = useDebounceCallback(setPhysicalAddressCity, keyboardTextFieldDebounceTimeout);
  const [physicalAddressProvince, setPhysicalAddressProvince] = useState(organization?.physicalAddress?.province);
  const debounceSetPhysicalAddressProvince = useDebounceCallback(setPhysicalAddressProvince, keyboardTextFieldDebounceTimeout);
  const [physicalAddressZipcode, setPhysicalAddressZipcode] = useState<string>(organization?.physicalAddress?.zipcode ?? '');
  const debounceSetPhysicalAddressZipcode = useDebounceCallback(setPhysicalAddressZipcode, keyboardTextFieldDebounceTimeout);
  const [physicalAddressCountry, setPhysicalAddressCountry] = useState<string>(organization?.physicalAddress?.country ?? '');
  const [physicalAddressCountryCode, setPhysicalAddressCountryCode] = useState<string>(organization?.physicalAddress?.countryCode ?? '');
  const debounceSetPhysicalAddressCountryCode = useDebounceCallback(setPhysicalAddressCountryCode, keyboardTextFieldDebounceTimeout);

  if (!organization) {
    return null;
  }

  const handlePhysicalAddressSelect = (address: Address) => {
    setPhysicalAddressOsmType(address.osmType);
    setPhysicalAddressOsmId(address.osmId);
    setPhysicalAddressPlaceId(address.placeId);
    setPhysicalAddressLongitude(address.longitude);
    setPhysicalAddressLatitude(address.latitude);
    setPhysicalAddressFormattedAddress(address.formattedAddress);
    setPhysicalAddressAddressLine1(address.addressLine1 ?? '');
    setPhysicalAddressAddressLine2(address.addressLine2 ?? '');
    setPhysicalAddressSuburb(address.suburb ?? '');
    setPhysicalAddressCity(address.city ?? '');
    setPhysicalAddressProvince(address.province ?? '');
    setPhysicalAddressZipcode(address.zipcode ?? '');
    setPhysicalAddressCountry(address.country ?? '');
    setPhysicalAddressCountryCode(address.countryCode ?? '');
  };

  const handlePhysicalAddressUpdateClick = ({ addressLine1, addressLine2, suburb, city, province, zipcode, countryCode }: PhysicalAddressDetails) => {
    const countryData = getCountryData(countryCode as TCountryCode);
    let country = physicalAddressCountry;
    if (countryData) {
      country = countryData.name;
    }

    const physicalAddress = organization.physicalAddress;

    if (physicalAddress) {
      const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' physical address...`} />, infoNotificationOptions);

      commitUpdateOrganizationPhysicalAddress({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: physicalAddress.id,
            osmType: physicalAddressOsmType,
            osmId: physicalAddressOsmId,
            placeId: physicalAddressPlaceId,
            longitude: physicalAddressLongitude,
            latitude: physicalAddressLatitude,
            formattedAddress: physicalAddressFormattedAddress,
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
              render: <NotificationContent content={`Failed to update organization '${organization.name}' physical address. Error: ${getRelayErrorMessage(errors)}.`} />,
            });

            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Organization '${organization.name}' physical address updated.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${organization.name}' physical address. Error: ${error.message}.`} />,
          });
        },
        optimisticResponse: {
          updateOrganizationPhysicalAddress: {
            organization: {
              id: organization.id,
              physicalAddress: {
                id: physicalAddress.id,
                osmType: physicalAddressOsmType,
                osmId: physicalAddressOsmId,
                placeId: physicalAddressPlaceId,
                longitude: physicalAddressLongitude,
                latitude: physicalAddressLatitude,
                formattedAddress: physicalAddressFormattedAddress,
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
    const toastId = themedToast(<NotificationContent content={`Adding organization '${organization.name}' physical address...`} />, infoNotificationOptions);

    commitAddOrganizationPhysicalAddress({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
          id,
          osmType: physicalAddressOsmType,
          osmId: physicalAddressOsmId,
          placeId: physicalAddressPlaceId,
          longitude: physicalAddressLongitude,
          latitude: physicalAddressLatitude,
          formattedAddress: physicalAddressFormattedAddress,
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
            render: <NotificationContent content={`Failed to add organization '${organization.name}' physical address. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization '${organization.name}' physical address added.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add organization '${organization.name}' physical address. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addOrganizationPhysicalAddress: {
          organization: {
            id: organization.id,
            physicalAddress: {
              id,
              osmType: physicalAddressOsmType,
              osmId: physicalAddressOsmId,
              placeId: physicalAddressPlaceId,
              longitude: physicalAddressLongitude,
              latitude: physicalAddressLatitude,
              formattedAddress: physicalAddressFormattedAddress,
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

  return (
    <Form
      onSubmit={handlePhysicalAddressUpdateClick}
      initialValues={{
        addressLine1: physicalAddressAddressLine1,
        addressLine2: physicalAddressAddressLine2,
        suburb: physicalAddressSuburb,
        city: physicalAddressCity,
        province: physicalAddressProvince,
        zipcode: physicalAddressZipcode,
        countryCode: physicalAddressCountryCode,
      }}
      validate={validatePhysicalAddress}
      render={({ handleSubmit, values, form }) => {
        const formValues = values!;

        debounceSetPhysicalAddressAddressLine1(formValues.addressLine1);
        debounceSetPhysicalAddressAddressLine2(formValues.addressLine2);
        debounceSetPhysicalAddressSuburb(formValues.suburb);
        debounceSetPhysicalAddressCity(formValues.city);
        debounceSetPhysicalAddressProvince(formValues.province);
        debounceSetPhysicalAddressZipcode(formValues.zipcode);
        debounceSetPhysicalAddressCountryCode(formValues.countryCode);

        return (
          <FormStackColumn onSubmit={handleSubmit}>
            <Box sx={{ pb: 2 }}>
              <SettingsSectionCard title="Physical address" description="Update the organization address used for internal records and operational context.">
                <StackColumn sx={formColumnSx}>
                  <PhysicalAddress
                    addressLine1Name="addressLine1"
                    addressLine1Required={requiredPhysicalAddressFields.addressLine1}
                    addressLine2Name="addressLine2"
                    addressLine2Required={requiredPhysicalAddressFields.addressLine2}
                    suburbName="suburb"
                    suburbRequired={requiredPhysicalAddressFields.suburb}
                    cityName="city"
                    cityRequired={requiredPhysicalAddressFields.city}
                    provinceName="province"
                    provinceRequired={requiredPhysicalAddressFields.province}
                    zipcodeName="zipcode"
                    zipcodeRequired={requiredPhysicalAddressFields.zipcode}
                    countryName="countryCode"
                    countryRequired={requiredPhysicalAddressFields.countryCode}
                    onSelect={(address) => {
                      handlePhysicalAddressSelect(address);
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
            </Box>
          </FormStackColumn>
        );
      }}
    />
  );
};

const OrganizationAdminPhysicalAddressSection = ({ organizationCustomDomain }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationAdminPhysicalAddressSectionQuery>(RootQuery);

  useEffect(() => {
    loadQuery(
      { organizationCustomDomain },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain]);

  if (!queryReference) {
    return <Loading />;
  }

  return <OrganizationAdminPhysicalAddressSectionContent organizationCustomDomain={organizationCustomDomain} queryReference={queryReference} />;
};

export default memo(OrganizationAdminPhysicalAddressSection);
