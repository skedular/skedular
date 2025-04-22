import { BodyIconTypography } from '@/components/commons';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import type { TCountryCode } from 'countries-list';
import { countries as countriesList, getCountryCode } from 'countries-list';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';

type Props = {
  name: string;
  required?: boolean;
};

interface CountryDetails {
  code: TCountryCode;
  name: string;
}

const SingleChoiceCountry = ({ name, required }: Props) => {
  const countries = useMemo<CountryDetails[]>(() => Object.entries(countriesList).map(([, { name }]) => ({ code: getCountryCode(name) as TCountryCode, name })), []);
  const filter = createFilterOptions<CountryDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={countries}
      getOptionValue={(option) => (option as CountryDetails).code}
      getOptionLabel={(option: string | CountryDetails) => (option as CountryDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as CountryDetails;

        return (
          <li {...props} key={castedOption.name}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as CountryDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceCountry);
