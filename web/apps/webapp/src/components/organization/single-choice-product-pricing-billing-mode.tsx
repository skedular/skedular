import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceProductPricingBillingMode_query$key } from '@/queries/__generated__/singleChoiceProductPricingBillingMode_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceProductPricingBillingMode_query$key;
  name: string;
  required?: boolean;
  acceptedBookingPaymentMethods?: string[];
};

type BookingProductPricingBillingModeDetails = {
  type: string;
  name: string;
};

const SingleChoiceProductPricingBillingMode = ({ rootDataRelay, name, required, acceptedBookingPaymentMethods }: Props) => {
  const rootData = useFragment<singleChoiceProductPricingBillingMode_query$key>(
    graphql`
      fragment singleChoiceProductPricingBillingMode_query on Query {
        productPricingBillingModes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<BookingProductPricingBillingModeDetails[]>(
    () => rootData.productPricingBillingModes.filter((item) => !acceptedBookingPaymentMethods || acceptedBookingPaymentMethods.some((x) => item.type === x)).map((item) => item),
    [rootData.productPricingBillingModes, acceptedBookingPaymentMethods],
  );
  const filter = createFilterOptions<BookingProductPricingBillingModeDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
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
      filterOptions={(options, params) => filter(options as BookingProductPricingBillingModeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceProductPricingBillingMode);
