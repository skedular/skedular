import { Address, PhysicalAddress } from '@/components/address';
import { FormStackColumn, StackColumn } from '@skedular/ui';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { physicalAddressSchema, PhysicalAddressDetails } from '@/components/organization/organizationAdmin/organization-admin-shared';
import { getRelayErrorMessage } from '@skedular/shared';
import type { organizationAdminPhysicalAddressSectionQuery } from '@/queries/__generated__/organizationAdminPhysicalAddressSectionQuery.graphql';
import type { organizationAdminPhysicalAddressSection_updateOrganizationMutation } from '@/queries/__generated__/organizationAdminPhysicalAddressSection_updateOrganizationMutation.graphql';
import Box from '@mui/material/Box';
import { SettingsSectionCard } from '@skedular/ui';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { makeRequired, makeValidate } from 'mui-rff';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { PaletteModeContext } from '@skedular/shared';

type Props = {
  organizationCustomDomain: string;
};

type InnerProps = {
  organizationCustomDomain: string;
  queryReference: PreloadedQuery<organizationAdminPhysicalAddressSectionQuery>;
};

const inlinePatchDebounceTimeout = 1000;

const arePhysicalAddressValuesEqual = (left: PhysicalAddressDetails, right: PhysicalAddressDetails) =>
  left.addressLine1 === right.addressLine1 &&
  left.addressLine2 === right.addressLine2 &&
  left.suburb === right.suburb &&
  left.city === right.city &&
  left.province === right.province &&
  left.zipcode === right.zipcode &&
  left.countryCode === right.countryCode;

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
  const [commitUpdateOrganizationPatch] = useMutation<organizationAdminPhysicalAddressSection_updateOrganizationMutation>(graphql`
    mutation organizationAdminPhysicalAddressSection_updateOrganizationMutation($input: UpdateOrganizationInput!) @raw_response_type {
      updateOrganization(input: $input) {
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
  const [physicalAddressCountry, setPhysicalAddressCountry] = useState<string>(organization?.physicalAddress?.country ?? '');
  const initialPhysicalAddressValues = useMemo<PhysicalAddressDetails>(
    () => ({
      addressLine1: organization?.physicalAddress?.addressLine1 ?? '',
      addressLine2: organization?.physicalAddress?.addressLine2 ?? null,
      suburb: organization?.physicalAddress?.suburb ?? null,
      city: organization?.physicalAddress?.city ?? null,
      province: organization?.physicalAddress?.province ?? null,
      zipcode: organization?.physicalAddress?.zipcode ?? '',
      countryCode: organization?.physicalAddress?.countryCode ?? '',
    }),
    [organization],
  );
  const draftPhysicalAddressValues = useRef(initialPhysicalAddressValues);

  const handlePhysicalAddressSelect = (address: Address) => {
    setPhysicalAddressOsmType(address.osmType);
    setPhysicalAddressOsmId(address.osmId);
    setPhysicalAddressPlaceId(address.placeId);
    setPhysicalAddressLongitude(address.longitude);
    setPhysicalAddressLatitude(address.latitude);
    setPhysicalAddressFormattedAddress(address.formattedAddress);
    setPhysicalAddressCountry(address.country ?? '');
  };

  const commitPhysicalAddressPatch = useCallback(
    ({ addressLine1, addressLine2, suburb, city, province, zipcode, countryCode }: PhysicalAddressDetails) => {
      if (!organization || !physicalAddressSchema.isValidSync({ addressLine1, addressLine2, suburb, city, province, zipcode, countryCode })) {
        return;
      }

      const countryData = getCountryData(countryCode as TCountryCode);
      let country = physicalAddressCountry;
      if (countryData) {
        country = countryData.name;
      }

      const physicalAddressId = organization.physicalAddress?.id ?? uuid();
      commitUpdateOrganizationPatch({
        variables: {
          input: {
            clientMutationId: uuid(),
            customDomain: organizationCustomDomain,
            fieldsToUpdate: ['PHYSICAL_ADDRESS'],
            physicalAddress: {
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
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            themedToast(
              <NotificationContent content={`Failed to save organization '${organization.name}' physical address. Error: ${getRelayErrorMessage(errors)}.`} />,
              errorNotificationOptions,
            );
          }
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`Failed to save organization '${organization.name}' physical address. Error: ${error.message}.`} />, errorNotificationOptions);
        },
        optimisticResponse: {
          updateOrganization: {
            organization: {
              id: organization.id,
              physicalAddress: {
                id: physicalAddressId,
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
    },
    [
      commitUpdateOrganizationPatch,
      organization,
      organizationCustomDomain,
      physicalAddressCountry,
      physicalAddressFormattedAddress,
      physicalAddressLatitude,
      physicalAddressLongitude,
      physicalAddressOsmId,
      physicalAddressOsmType,
      physicalAddressPlaceId,
      themedToast,
    ],
  );
  const debouncedCommitPhysicalAddressPatch = useDebounceCallback(commitPhysicalAddressPatch, inlinePatchDebounceTimeout);

  if (!organization) {
    return null;
  }

  return (
    <Form<PhysicalAddressDetails>
      onSubmit={() => undefined}
      initialValues={initialPhysicalAddressValues}
      validate={validatePhysicalAddress}
      render={({ handleSubmit, values, form }) => {
        const formValues = values as PhysicalAddressDetails;

        if (!arePhysicalAddressValuesEqual(draftPhysicalAddressValues.current, formValues)) {
          draftPhysicalAddressValues.current = formValues;
          debouncedCommitPhysicalAddressPatch(formValues);
        }

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
