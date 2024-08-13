import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { convertStringToLowercaseExceptFirstLetter } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { useFragment } from 'react-relay';
import type { locationSingleChoiceMembershipType_query$key } from './__generated__/locationSingleChoiceMembershipType_query.graphql';

type Props = {
  rootDataRelay: locationSingleChoiceMembershipType_query$key;
  name: string;
  required?: boolean;
};

const LocationSingleChoiceMembershipType = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment locationSingleChoiceMembershipType_query on Query {
        locationMemberMembershipTypes
      }
    `,
    rootDataRelay,
  );

  const locationMemberMembershipTypes = useMemo<string[]>(
    () => rootData.locationMemberMembershipTypes.map((locationMemberMembershipType) => locationMemberMembershipType),
    [rootData.locationMemberMembershipTypes],
  );

  const filter = createFilterOptions<string>();

  return (
    <Autocomplete
      label="Membership"
      name={name}
      multiple={false}
      required={required}
      options={locationMemberMembershipTypes}
      getOptionValue={(option) => option as string}
      getOptionLabel={(option: string | string) => convertStringToLowercaseExceptFirstLetter(option as string)}
      renderOption={(props, option) => {
        const castedOption = option as string;

        return (
          <li {...props}>
            <Stack sx={{ flex: 1 }} direction="row" spacing={2}>
              <Typography variant="body1">{convertStringToLowercaseExceptFirstLetter(castedOption)}</Typography>
            </Stack>
          </li>
        );
      }}
      disableCloseOnSelect={false}
      freeSolo={true}
      filterOptions={(options, params) => filter(options as string[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(LocationSingleChoiceMembershipType);
