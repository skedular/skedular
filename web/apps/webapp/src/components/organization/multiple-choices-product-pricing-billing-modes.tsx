import { BodyIconTypography } from '@/components/commons';
import type { multipleChoicesProductPricingBillingModes_query$key } from '@/queries/__generated__/multipleChoicesProductPricingBillingModes_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesProductPricingBillingModes_query$key;
  name: string;
  required?: boolean;
};

type BookingProductPricingBillingModeDetails = {
  type: string;
  name: string;
};

const MultipleChoicesProductPricingBillingModes = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<multipleChoicesProductPricingBillingModes_query$key>(
    graphql`
      fragment multipleChoicesProductPricingBillingModes_query on Query {
        productPricingBillingModes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<BookingProductPricingBillingModeDetails[]>(() => rootData.productPricingBillingModes.map((item) => item), [rootData.productPricingBillingModes]);
  const filter = createFilterOptions<BookingProductPricingBillingModeDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={true}
      required={required}
      options={items}
      getOptionValue={(option) => (option as BookingProductPricingBillingModeDetails).type}
      getOptionLabel={(option: string | BookingProductPricingBillingModeDetails) => (option as BookingProductPricingBillingModeDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as BookingProductPricingBillingModeDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      disableCloseOnSelect
      filterOptions={(options, params) => filter(options as BookingProductPricingBillingModeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesProductPricingBillingModes);
