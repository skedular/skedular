import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceProductPricingCancellationType_query$key } from '@/queries/__generated__/singleChoiceProductPricingCancellationType_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  fieldProps?: {
    onChange?: (event: { target: { value: string } }) => void;
  };
  rootDataRelay: singleChoiceProductPricingCancellationType_query$key;
  name: string;
  required?: boolean;
};

type ProductPricingCancellationType = {
  type: string;
  name: string;
};

const SingleChoiceProductPricingCancellationType = ({ fieldProps, rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceProductPricingCancellationType_query$key>(
    graphql`
      fragment singleChoiceProductPricingCancellationType_query on Query {
        productPricingCancellationTypes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<ProductPricingCancellationType[]>(() => rootData.productPricingCancellationTypes.map((item) => item), [rootData.productPricingCancellationTypes]);
  const filter = createFilterOptions<ProductPricingCancellationType>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      fieldProps={fieldProps}
      getOptionValue={(option) => (option as ProductPricingCancellationType).type}
      getOptionLabel={(option: string | ProductPricingCancellationType) => (option as ProductPricingCancellationType).name}
      renderOption={(props, option) => {
        const castedOption = option as ProductPricingCancellationType;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as ProductPricingCancellationType[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceProductPricingCancellationType);
