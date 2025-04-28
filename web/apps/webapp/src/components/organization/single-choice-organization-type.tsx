import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceOrganizationType_query$key } from '@/queries/__generated__/singleChoiceOrganizationType_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceOrganizationType_query$key;
  name: string;
  required?: boolean;
};

type OrganizationTypeDetails = {
  readonly type: string;
  readonly name: string;
};

const SingleChoiceOrganizationType = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceOrganizationType_query$key>(
    graphql`
      fragment singleChoiceOrganizationType_query on Query {
        organizationTypes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const organizationTypes = useMemo<OrganizationTypeDetails[]>(() => rootData.organizationTypes.map((item) => item), [rootData.organizationTypes]);
  const filter = createFilterOptions<OrganizationTypeDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={organizationTypes}
      getOptionValue={(option) => (option as OrganizationTypeDetails).type}
      getOptionLabel={(option: string | OrganizationTypeDetails) => (option as OrganizationTypeDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as OrganizationTypeDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as OrganizationTypeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceOrganizationType);
