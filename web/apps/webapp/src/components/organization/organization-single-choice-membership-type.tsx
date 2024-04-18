import type { organizationSingleChoiceMembershipType_query$key } from '@/queries/__generated__/organizationSingleChoiceMembershipType_query.graphql';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { convertStringToLowercaseExceptFirstLetter } from '@repo/shared/libs/utils';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationSingleChoiceMembershipType_query$key;
  name: string;
  required?: boolean;
};

const OrganizationSingleChoiceMembershipType = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment organizationSingleChoiceMembershipType_query on Query {
        organizationMemberMembershipTypes
      }
    `,
    rootDataRelay,
  );

  const organizationMemberMembershipTypes = useMemo<string[]>(
    () => rootData.organizationMemberMembershipTypes.map((organizationMemberMembershipType) => organizationMemberMembershipType),
    [rootData.organizationMemberMembershipTypes],
  );

  const filter = createFilterOptions<string>();

  return (
    <Autocomplete
      label="Membership"
      name={name}
      multiple={false}
      required={required}
      options={organizationMemberMembershipTypes}
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

export default memo(OrganizationSingleChoiceMembershipType);
