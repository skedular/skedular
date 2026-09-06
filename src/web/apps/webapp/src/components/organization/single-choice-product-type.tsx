import { BodyIconTypography } from '@skedular/ui';
import type { singleChoiceProductType_query$key } from '@/queries/__generated__/singleChoiceProductType_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceProductType_query$key;
  name: string;
  required?: boolean;
};

type MembershipTermDetails = {
  type: string;
  name: string;
};

const SingleChoiceProductType = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceProductType_query$key>(
    graphql`
      fragment singleChoiceProductType_query on Query {
        productTypes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<MembershipTermDetails[]>(() => rootData.productTypes.map((item) => item), [rootData.productTypes]);
  const filter = createFilterOptions<MembershipTermDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as MembershipTermDetails).type}
      getOptionLabel={(option: string | MembershipTermDetails) => (option as MembershipTermDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as MembershipTermDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as MembershipTermDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceProductType);
