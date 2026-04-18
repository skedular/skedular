import { BodyIconTypography } from '@/components/commons';
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

type ProductPricingCadenceDetails = {
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

  const items = useMemo<ProductPricingCadenceDetails[]>(() => rootData.productTypes.map((item) => item), [rootData.productTypes]);
  const filter = createFilterOptions<ProductPricingCadenceDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as ProductPricingCadenceDetails).type}
      getOptionLabel={(option: string | ProductPricingCadenceDetails) => (option as ProductPricingCadenceDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as ProductPricingCadenceDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as ProductPricingCadenceDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceProductType);
