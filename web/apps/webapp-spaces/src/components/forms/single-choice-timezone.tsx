import { BodyIconTypography, HelperText } from '@skedular/ui';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';

type Props = {
  name: string;
  required?: boolean;
  helperText?: string;
};

interface TimezoneDetails {
  id: string;
  label: string;
}

const SingleChoinceTimezone = ({ name, required, helperText }: Props) => {
  const items = useMemo<TimezoneDetails[]>(
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
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as TimezoneDetails).id}
      getOptionLabel={(option: string | TimezoneDetails) => (option as TimezoneDetails).label}
      renderOption={(props, option) => {
        const castedOption = option as TimezoneDetails;

        return (
          <li {...props} key={castedOption.id}>
            <BodyIconTypography label={castedOption.label} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as TimezoneDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
      helperText={<HelperText text={helperText} />}
    />
  );
};

export default memo(SingleChoinceTimezone);
