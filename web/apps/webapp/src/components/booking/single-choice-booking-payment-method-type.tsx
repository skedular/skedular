import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceBookingPaymentMethodType_query$key } from '@/queries/__generated__/singleChoiceBookingPaymentMethodType_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceBookingPaymentMethodType_query$key;
  name: string;
  required?: boolean;
};

type BookingPaymentMethodTypeDetails = {
  readonly type: string;
  readonly name: string;
};

const SingleChoiceBookingPaymentMethodType = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceBookingPaymentMethodType_query$key>(
    graphql`
      fragment singleChoiceBookingPaymentMethodType_query on Query {
        bookingPaymentMethodTypes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const bookingPaymentMethodTypes = useMemo<BookingPaymentMethodTypeDetails[]>(() => rootData.bookingPaymentMethodTypes.map((item) => item), [rootData.bookingPaymentMethodTypes]);
  const filter = createFilterOptions<BookingPaymentMethodTypeDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={bookingPaymentMethodTypes}
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

export default memo(SingleChoiceBookingPaymentMethodType);
