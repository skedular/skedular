import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceProductPricingBillingInterval_query$key } from '@/queries/__generated__/singleChoiceProductPricingBillingInterval_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceProductPricingBillingInterval_query$key;
  name: string;
  required?: boolean;
  acceptedBookingPaymentMethods?: string[];
};

type BookingProductPricingBillingIntervalDetails = {
  type: string;
  name: string;
};

const SingleChoiceProductPricingBillingInterval = ({ rootDataRelay, name, required, acceptedBookingPaymentMethods }: Props) => {
  const rootData = useFragment<singleChoiceProductPricingBillingInterval_query$key>(
    graphql`
      fragment singleChoiceProductPricingBillingInterval_query on Query {
        productPricingBillingIntervals {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<BookingProductPricingBillingIntervalDetails[]>(
    () =>
      rootData.productPricingBillingIntervals.filter((item) => !acceptedBookingPaymentMethods || acceptedBookingPaymentMethods.some((x) => item.type === x)).map((item) => item),
    [rootData.productPricingBillingIntervals, acceptedBookingPaymentMethods],
  );
  const filter = createFilterOptions<BookingProductPricingBillingIntervalDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as BookingProductPricingBillingIntervalDetails).type}
      getOptionLabel={(option: string | BookingProductPricingBillingIntervalDetails) => (option as BookingProductPricingBillingIntervalDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as BookingProductPricingBillingIntervalDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as BookingProductPricingBillingIntervalDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceProductPricingBillingInterval);
