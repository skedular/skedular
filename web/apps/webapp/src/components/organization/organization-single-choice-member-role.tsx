import { BodyIconTypography } from '@/components/commons';
import type { organizationSingleChoiceMemberRole_query$key } from '@/queries/__generated__/organizationSingleChoiceMemberRole_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationSingleChoiceMemberRole_query$key;
  name: string;
  required?: boolean;
};

type MemberRoleDetails = {
  type: string;
  name: string;
};

const OrganizationSingleChoiceMemberRole = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment organizationSingleChoiceMemberRole_query on Query {
        organizationMemberRoles {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<MemberRoleDetails[]>(() => rootData.organizationMemberRoles.map((item) => item), [rootData.organizationMemberRoles]);
  const filter = createFilterOptions<MemberRoleDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as MemberRoleDetails).type}
      getOptionLabel={(option: string | MemberRoleDetails) => (option as MemberRoleDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as MemberRoleDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as MemberRoleDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(OrganizationSingleChoiceMemberRole);
