import { BodyIconTypography } from '@skedular/ui';
import type { singleChoiceOrganizationXeroBillingMode_query$key } from '@/queries/__generated__/singleChoiceOrganizationXeroBillingMode_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceOrganizationXeroBillingMode_query$key;
  name: string;
  required?: boolean;
};

type OrganizationXeroBillingModeDetails = {
  type: string;
  name: string;
};

const SingleChoiceOrganizationXeroBillingMode = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceOrganizationXeroBillingMode_query$key>(
    graphql`
      fragment singleChoiceOrganizationXeroBillingMode_query on Query {
        organizationXeroBillingModes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<OrganizationXeroBillingModeDetails[]>(() => rootData.organizationXeroBillingModes.map((item) => item), [rootData.organizationXeroBillingModes]);
  const filter = createFilterOptions<OrganizationXeroBillingModeDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as OrganizationXeroBillingModeDetails).type}
      getOptionLabel={(option: string | OrganizationXeroBillingModeDetails) => (option as OrganizationXeroBillingModeDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as OrganizationXeroBillingModeDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as OrganizationXeroBillingModeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceOrganizationXeroBillingMode);
