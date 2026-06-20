import { BodyIconTypography } from '@skedular/ui';
import type { singleChoicePaymentMethodType_query$key } from '@/queries/__generated__/singleChoicePaymentMethodType_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoicePaymentMethodType_query$key;
  name: string;
  required?: boolean;
  acceptedBookingPaymentMethods?: string[];
};

type BookingPaymentMethodTypeDetails = {
  type: string;
  name: string;
};

const SingleChoicePaymentMethodType = ({ rootDataRelay, name, required, acceptedBookingPaymentMethods }: Props) => {
  const rootData = useFragment<singleChoicePaymentMethodType_query$key>(
    graphql`
      fragment singleChoicePaymentMethodType_query on Query {
        paymentMethodTypes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<BookingPaymentMethodTypeDetails[]>(
    () => rootData.paymentMethodTypes.filter((item) => !acceptedBookingPaymentMethods || acceptedBookingPaymentMethods.some((x) => item.type === x)).map((item) => item),
    [rootData.paymentMethodTypes, acceptedBookingPaymentMethods],
  );
  const filter = createFilterOptions<BookingPaymentMethodTypeDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as BookingPaymentMethodTypeDetails).type}
      getOptionLabel={(option: string | BookingPaymentMethodTypeDetails) => (option as BookingPaymentMethodTypeDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as BookingPaymentMethodTypeDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as BookingPaymentMethodTypeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoicePaymentMethodType);
