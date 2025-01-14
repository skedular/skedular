import { createFilterOptions } from '@mui/material/useAutocomplete';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { convertStringToLowercaseExceptFirstLetter } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { useFragment } from 'react-relay';
import type { locationSingleChoiceMemberRole_query$key } from './__generated__/locationSingleChoiceMemberRole_query.graphql';

type Props = {
  rootDataRelay: locationSingleChoiceMemberRole_query$key;
  name: string;
  required?: boolean;
};

const LocationSingleChoiceMemberRole = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment locationSingleChoiceMemberRole_query on Query {
        locationMemberRoles
      }
    `,
    rootDataRelay,
  );

  const roles = useMemo<string[]>(() => rootData.locationMemberRoles.map((role) => role), [rootData.locationMemberRoles]);
  const filter = createFilterOptions<string>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={roles}
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
      filterOptions={(options, params) => filter(options as string[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(LocationSingleChoiceMemberRole);
