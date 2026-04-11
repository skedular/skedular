import { BodyIconTypography } from '@/components/commons';
import { Autocomplete } from '@/components/forms';
import { convertStringToLowercaseExceptFirstLetter } from '@/libs/utils';
import type { organizationSingleChoiceMemberRole_query$key } from '@/queries/__generated__/organizationSingleChoiceMemberRole_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationSingleChoiceMemberRole_query$key;
  name: string;
  required?: boolean;
};

const OrganizationSingleChoiceMemberRole = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment organizationSingleChoiceMemberRole_query on Query {
        organizationMemberRoles
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<string[]>(() => rootData.organizationMemberRoles.map((role) => role), [rootData.organizationMemberRoles]);
  const filter = createFilterOptions<string>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => option as string}
      getOptionLabel={(option: string | string) => convertStringToLowercaseExceptFirstLetter(option as string)}
      renderOption={(props, option) => {
        const castedOption = option as string;

        return (
          <li {...props} key={castedOption}>
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

export default memo(OrganizationSingleChoiceMemberRole);
