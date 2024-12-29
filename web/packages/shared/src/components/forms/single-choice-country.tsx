import { createFilterOptions } from '@mui/material/useAutocomplete';
import { countries as countriesList } from 'countries-list';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { BodyIconTypography } from '../commons';

type Props = {
  name: string;
  required?: boolean;
};

interface CountryDetails {
  name: string;
}

const SingleChoiceCountry = ({ name, required }: Props) => {
  const countries = useMemo<CountryDetails[]>(() => Object.entries(countriesList).map(([, { name }]) => ({ name })), []);
  const filter = createFilterOptions<CountryDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={countries}
      getOptionValue={(option) => (option as CountryDetails).name}
      getOptionLabel={(option: string | CountryDetails) => (option as CountryDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as CountryDetails;

        return (
          <li {...props}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      disableCloseOnSelect={false}
      freeSolo={true}
      filterOptions={(options, params) => filter(options as CountryDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceCountry);
