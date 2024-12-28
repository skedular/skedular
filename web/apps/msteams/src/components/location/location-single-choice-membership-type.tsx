import { createFilterOptions } from '@mui/material/useAutocomplete';
import { BodyIconTypography } from '@repo/shared/components/commons';
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
        locationMembershipTypes
      }
    `,
    rootDataRelay,
  );

  const locationMembershipTypes = useMemo<string[]>(
    () => rootData.locationMembershipTypes.map((locationMembershipType) => locationMembershipType),
    [rootData.locationMembershipTypes],
  );

  const filter = createFilterOptions<string>();

  return (
    <Autocomplete
      label="Membership"
      name={name}
      multiple={false}
      required={required}
      options={locationMembershipTypes}
      getOptionValue={(option) => option as string}
      getOptionLabel={(option: string | string) => convertStringToLowercaseExceptFirstLetter(option as string)}
      renderOption={(props, option) => {
        const castedOption = option as string;

        return (
          <li {...props}>
            <BodyIconTypography label={convertStringToLowercaseExceptFirstLetter(castedOption)} />
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
