import { FormFieldLabel, StackColumn } from '@/components/commons';
import { SingleChoiceCountry } from '@/components/forms';
import { AddressJsonV2 } from '@/libs/address/nominatim';
import { defaultPadding } from '@/libs/theme';
import { TextField } from 'mui-rff';
import { memo } from 'react';
import AddressSearch from './address-search';

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
  const handleSelect = (address: AddressJsonV2) => {
    onSelect({
      osmType: address.osm_type,
      osmId: String(address.osm_id),
      placeId: String(address.place_id),
      latitude: parseFloat(address.lat),
      longitude: parseFloat(address.lon),
      formattedAddress: address.display_name,
      addressLine1: [address.address.house_number, address.address.road].filter(Boolean).join(' '),
      addressLine2: address.address.neighbourhood,
      suburb: address.address.suburb,
      city: address.address.city ? address.address.city : address.address.town,
      province: address.address.state ? address.address.state : address.address['ISO3166-2-lvl4'],
      zipcode: address.address.postcode,
      country: address.address.country,
      countryCode: address.address.country_code?.toLocaleUpperCase(),
    });
  };

  return (
    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
      <FormFieldLabel label="">
        <AddressSearch onSelect={handleSelect} />
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
    </StackColumn>
  );
};

export default memo(PhysicalAddress);
