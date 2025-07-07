import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceBookingType_query$key } from '@/queries/__generated__/singleChoiceBookingType_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceBookingType_query$key;
  name: string;
  required?: boolean;
};

type BookingTypeDetails = {
  type: string;
  name: string;
};

const SingleChoiceBookingType = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceBookingType_query$key>(
    graphql`
      fragment singleChoiceBookingType_query on Query {
        bookingTypes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const bookingTypes = useMemo<BookingTypeDetails[]>(() => rootData.bookingTypes.map((item) => item), [rootData.bookingTypes]);
  const filter = createFilterOptions<BookingTypeDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={bookingTypes}
      getOptionValue={(option) => (option as BookingTypeDetails).type}
      getOptionLabel={(option: string | BookingTypeDetails) => (option as BookingTypeDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as BookingTypeDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as BookingTypeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceBookingType);
