import { FormFieldLabel } from '@/components/commons';
import { SingleChoiceCountry } from '@/components/forms';
import { TextField } from 'mui-rff';
import { memo } from 'react';
import GoogleAddressLookup, { GooglePlaceDetails } from './google-address-lookup';

type Props = {
  addressLine1Name: string;
  addressLine1Required: boolean;
  addressLine2Name: string;
  addressLine2Required: boolean;
  suburbName: string;
  suburbRequired: boolean;
  cityName: string;
  cityRequired: boolean;
  provinceName: string;
  provinceRequired: boolean;
  zipcodeName: string;
  zipcodeRequired: boolean;
  countryName: string;
  countryRequired: boolean;
  onSelect: (address: Address) => void;
};

export type Address = {
  osmType?: string;
  osmId?: string;
  placeId?: string;
  latitude?: number;
  longitude?: number;
  formattedAddress?: string;
  addressLine1?: string;
  addressLine2?: string;
  suburb?: string;
  city?: string;
  province?: string;
  zipcode?: string;
  country?: string;
  countryCode?: string;
};

const getComponentLongName = (components: GooglePlaceDetails['addressComponents'], targetTypes: string[]) =>
  components.find((component) => targetTypes.some((type) => component.types.includes(type)))?.long_name;

const getComponentShortName = (components: GooglePlaceDetails['addressComponents'], targetTypes: string[]) =>
  components.find((component) => targetTypes.some((type) => component.types.includes(type)))?.short_name;

const PhysicalAddress = ({
  addressLine1Name,
  addressLine1Required,
  addressLine2Name,
  addressLine2Required,
  suburbName,
  suburbRequired,
  cityName,
  cityRequired,
  provinceName,
  provinceRequired,
  zipcodeName,
  zipcodeRequired,
  countryName,
  countryRequired,
  onSelect,
}: Props) => {
  const handleSelect = (place: GooglePlaceDetails) => {
    const components = place.addressComponents ?? [];
    const streetNumber = getComponentLongName(components, ['street_number']);
    const route = getComponentLongName(components, ['route']);
    const subpremise = getComponentLongName(components, ['subpremise']);
    const neighbourhood = getComponentLongName(components, ['neighborhood', 'sublocality_level_2']);
    const suburb = getComponentLongName(components, ['sublocality', 'sublocality_level_1']);
    const locality = getComponentLongName(components, ['locality', 'postal_town', 'administrative_area_level_2']);
    const adminAreaLevel1 = getComponentLongName(components, ['administrative_area_level_1']);
    const postalCode = getComponentLongName(components, ['postal_code']);
    const country = getComponentLongName(components, ['country']);
    const countryCode = getComponentShortName(components, ['country']);

    onSelect({
      osmType: 'google_place',
      osmId: place.placeId,
      placeId: place.placeId,
      latitude: place.latitude,
      longitude: place.longitude,
      formattedAddress: place.formattedAddress ?? '',
      addressLine1: [streetNumber, route ?? place.name].filter(Boolean).join(' '),
      addressLine2: subpremise ?? neighbourhood ?? undefined,
      suburb: suburb ?? neighbourhood ?? undefined,
      city: locality ?? suburb ?? undefined,
      province: adminAreaLevel1 ?? undefined,
      zipcode: postalCode ?? undefined,
      country: country ?? undefined,
      countryCode: countryCode ? countryCode.toUpperCase() : undefined,
    });
  };

  return (
    <>
      <FormFieldLabel label="">
        <GoogleAddressLookup onSelect={handleSelect} />
      </FormFieldLabel>

      <FormFieldLabel label="Address line 1">
        <TextField name={addressLine1Name} required={addressLine1Required} />
      </FormFieldLabel>

      <FormFieldLabel label="Address line 2">
        <TextField name={addressLine2Name} required={addressLine2Required} />
      </FormFieldLabel>

      <FormFieldLabel label="Suburb">
        <TextField name={suburbName} required={suburbRequired} />
      </FormFieldLabel>

      <FormFieldLabel label="City">
        <TextField name={cityName} required={cityRequired} />
      </FormFieldLabel>

      <FormFieldLabel label="Province">
        <TextField name={provinceName} required={provinceRequired} />
      </FormFieldLabel>

      <FormFieldLabel label="Zipcode">
        <TextField name={zipcodeName} required={zipcodeRequired} />
      </FormFieldLabel>

      <FormFieldLabel label="Country">
        <SingleChoiceCountry name={countryName} required={countryRequired} />
      </FormFieldLabel>
    </>
  );
};

export default memo(PhysicalAddress);
