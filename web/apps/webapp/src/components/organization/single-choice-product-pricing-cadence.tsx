import { BodyIconTypography } from '@/components/commons';
import { Autocomplete } from '@/components/forms';
import type { singleChoiceProductPricingCadence_query$key } from '@/queries/__generated__/singleChoiceProductPricingCadence_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceProductPricingCadence_query$key;
  name: string;
  required?: boolean;
};

type ProductPricingCadenceDetails = {
  type: string;
  name: string;
};

const SingleChoiceProductPricingCadence = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceProductPricingCadence_query$key>(
    graphql`
      fragment singleChoiceProductPricingCadence_query on Query {
        productPricingCadences {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<ProductPricingCadenceDetails[]>(() => rootData.productPricingCadences.map((item) => item), [rootData.productPricingCadences]);
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

export default memo(SingleChoiceProductPricingCadence);
