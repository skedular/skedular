import { BodyIconTypography } from '@/components/commons';
import { Autocomplete } from '@/components/forms';
import type { singleChoiceOrganizationBillingCycle_query$key } from '@/queries/__generated__/singleChoiceOrganizationBillingCycle_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceOrganizationBillingCycle_query$key;
  name: string;
  required?: boolean;
};

type OrganizationBillingCycleDetails = {
  type: string;
  name: string;
};

const SingleChoiceOrganizationBillingCycle = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceOrganizationBillingCycle_query$key>(
    graphql`
      fragment singleChoiceOrganizationBillingCycle_query on Query {
        organizationBillingCycles {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<OrganizationBillingCycleDetails[]>(() => rootData.organizationBillingCycles.map((item) => item), [rootData.organizationBillingCycles]);
  const filter = createFilterOptions<OrganizationBillingCycleDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as OrganizationBillingCycleDetails).type}
      getOptionLabel={(option: string | OrganizationBillingCycleDetails) => (option as OrganizationBillingCycleDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as OrganizationBillingCycleDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as OrganizationBillingCycleDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceOrganizationBillingCycle);
