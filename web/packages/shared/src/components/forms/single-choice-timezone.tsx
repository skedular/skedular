import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { BodyIconTypography } from '../commons';

type Props = {
  name: string;
  required?: boolean;
};

interface TimezoneDetails {
  id: string;
  label: string;
}

const SingleChoinceTimezone = ({ name, required }: Props) => {
  const timezones = useMemo<TimezoneDetails[]>(
    () =>
      Intl.supportedValuesOf('timeZone').map((item) => ({
        id: item,
        label: item,
      })),
    [],
  );

  const filter = createFilterOptions<TimezoneDetails>();

  return (
    <Autocomplete
      label="Timezone"
      name={name}
      multiple={false}
      required={required}
      options={timezones}
      getOptionValue={(option) => (option as TimezoneDetails).id}
      getOptionLabel={(option: string | TimezoneDetails) => (option as TimezoneDetails).label}
      renderOption={(props, option) => {
        const castedOption = option as TimezoneDetails;

        return (
          <li {...props}>
            <BodyIconTypography label={castedOption.label} />
          </li>
        );
      }}
      disableCloseOnSelect={false}
      freeSolo={true}
      filterOptions={(options, params) => filter(options as TimezoneDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoinceTimezone);
