import { BodyIconTypography } from '@/components/commons';
import type { multipleChoicesBookingPaymentMethodTypes_query$key } from '@/queries/__generated__/multipleChoicesBookingPaymentMethodTypes_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesBookingPaymentMethodTypes_query$key;
  name: string;
  required?: boolean;
};

type BookingPaymentMethodTypeDetails = {
  type: string;
  name: string;
};

const MultipleChoicesBookingPaymentMethodTypes = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<multipleChoicesBookingPaymentMethodTypes_query$key>(
    graphql`
      fragment multipleChoicesBookingPaymentMethodTypes_query on Query {
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
      multiple={true}
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
      disableCloseOnSelect
      filterOptions={(options, params) => filter(options as BookingPaymentMethodTypeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesBookingPaymentMethodTypes);
