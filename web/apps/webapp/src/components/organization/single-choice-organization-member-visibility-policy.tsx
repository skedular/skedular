import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceOrganizationMemberVisibilityPolicyquery$key } from '@/queries/__generated__/singleChoiceOrganizationMemberVisibilityPolicyquery.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceOrganizationMemberVisibilityPolicyquery$key;
  name: string;
  required?: boolean;
};

type OrganizationMemberVisibilityPolicyDetails = {
  readonly type: string;
  readonly name: string;
};

const SingleChoiceOrganizationMemberVisibilityPolicy = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceOrganizationMemberVisibilityPolicyquery$key>(
    graphql`
      fragment singleChoiceOrganizationMemberVisibilityPolicyquery on Query {
        organizationMemberVisibilityPolicies {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const organizationTypes = useMemo<OrganizationMemberVisibilityPolicyDetails[]>(
    () => rootData.organizationMemberVisibilityPolicies.map((item) => item),
    [rootData.organizationMemberVisibilityPolicies],
  );
  const filter = createFilterOptions<OrganizationMemberVisibilityPolicyDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={organizationTypes}
      getOptionValue={(option) => (option as OrganizationMemberVisibilityPolicyDetails).type}
      getOptionLabel={(option: string | OrganizationMemberVisibilityPolicyDetails) => (option as OrganizationMemberVisibilityPolicyDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as OrganizationMemberVisibilityPolicyDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as OrganizationMemberVisibilityPolicyDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceOrganizationMemberVisibilityPolicy);
