import { PhysicalAddress } from '@/components/address';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { PhysicalAddressDetails, physicalAddressSchema } from '@/components/organization/organizationAdmin/organization-admin-shared';
import type { organizationAdminPhysicalAddressSectionQuery } from '@/queries/__generated__/organizationAdminPhysicalAddressSectionQuery.graphql';
import type { organizationAdminPhysicalAddressSection_updateOrganizationMutation } from '@/queries/__generated__/organizationAdminPhysicalAddressSection_updateOrganizationMutation.graphql';
import Box from '@mui/material/Box';
import { getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
import { FormStackColumn, SettingsSectionCard, StackColumn } from '@skedular/ui';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { makeRequired, makeValidate } from 'mui-rff';
import { memo, useCallback, useContext, useEffect, useMemo, useRef } from 'react';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';

type Props = {
  organizationCustomDomain: string;
  embedded?: boolean;
};

type InnerProps = {
  organizationCustomDomain: string;
  queryReference: PreloadedQuery<organizationAdminPhysicalAddressSectionQuery>;
  embedded?: boolean;
};

const inlinePatchDebounceTimeout = 1000;

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

const OrganizationAdminPhysicalAddressSectionContent = ({ organizationCustomDomain, queryReference, embedded }: InnerProps) => {
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

  const initialPhysicalAddressValues = useMemo<PhysicalAddressDetails>(
    () => ({
      osmType: organization?.physicalAddress?.osmType,
      osmId: organization?.physicalAddress?.osmId,
      placeId: organization?.physicalAddress?.placeId,
      longitude: organization?.physicalAddress?.longitude,
      latitude: organization?.physicalAddress?.latitude,
      formattedAddress: organization?.physicalAddress?.formattedAddress,
      country: organization?.physicalAddress?.country ?? '',
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
  const submittedPhysicalAddressKey = useRef<string | null>(null);

  const commitPhysicalAddressPatch = useCallback(
    ({
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
    }: PhysicalAddressDetails) => {
      if (!organization || !physicalAddressSchema.isValidSync({ addressLine1, addressLine2, suburb, city, province, zipcode, countryCode })) {
        return;
      }

      const countryData = getCountryData(countryCode as TCountryCode);
      let country = lookupCountry;
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
    [commitUpdateOrganizationPatch, organization, organizationCustomDomain, themedToast],
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
        const nextPhysicalAddressKey = JSON.stringify(formValues);

        if (submittedPhysicalAddressKey.current === null) {
          submittedPhysicalAddressKey.current = nextPhysicalAddressKey;
        } else if (nextPhysicalAddressKey !== submittedPhysicalAddressKey.current) {
          submittedPhysicalAddressKey.current = nextPhysicalAddressKey;
          debouncedCommitPhysicalAddressPatch(formValues);
        }

        return (
          <FormStackColumn onSubmit={handleSubmit}>
            <Box sx={{ pb: embedded ? 0 : 2 }}>
              <SettingsSectionCard bare={embedded} title="Physical address" description="Update the organization address used for internal records and operational context.">
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
            </Box>
          </FormStackColumn>
        );
      }}
    />
  );
};

const OrganizationAdminPhysicalAddressSection = ({ organizationCustomDomain, embedded }: Props) => {
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

  return <OrganizationAdminPhysicalAddressSectionContent organizationCustomDomain={organizationCustomDomain} queryReference={queryReference} embedded={embedded} />;
};

export default memo(OrganizationAdminPhysicalAddressSection);
