import type { organizationSingleChoiceMembershipType_query$key } from '@/queries/__generated__/organizationSingleChoiceMembershipType_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { BodyIconTypography } from '@repo/shared/components/commons';
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
        organizationMembershipTypes
      }
    `,
    rootDataRelay,
  );

  const organizationMembershipTypes = useMemo<string[]>(
    () => rootData.organizationMembershipTypes.map((organizationMembershipType) => organizationMembershipType),
    [rootData.organizationMembershipTypes],
  );

  const filter = createFilterOptions<string>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={organizationMembershipTypes}
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

export default memo(OrganizationSingleChoiceMembershipType);
