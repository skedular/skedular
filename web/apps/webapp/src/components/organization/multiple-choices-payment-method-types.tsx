import { BodyIconTypography } from '@skedular/ui';
import type { multipleChoicesPaymentMethodTypes_query$key } from '@/queries/__generated__/multipleChoicesPaymentMethodTypes_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesPaymentMethodTypes_query$key;
  name: string;
  required?: boolean;
};

type BookingPaymentMethodTypeDetails = {
  type: string;
  name: string;
};

const MultipleChoicesPaymentMethodTypes = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<multipleChoicesPaymentMethodTypes_query$key>(
    graphql`
      fragment multipleChoicesPaymentMethodTypes_query on Query {
        paymentMethodTypes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<BookingPaymentMethodTypeDetails[]>(() => rootData.paymentMethodTypes.map((item) => item), [rootData.paymentMethodTypes]);
  const filter = createFilterOptions<BookingPaymentMethodTypeDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={true}
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
      disableCloseOnSelect
      filterOptions={(options, params) => filter(options as BookingPaymentMethodTypeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesPaymentMethodTypes);
