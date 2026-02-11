import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceBookingCategory_query$key } from '@/queries/__generated__/singleChoiceBookingCategory_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceBookingCategory_query$key;
  name: string;
  required?: boolean;
};

type CategoryDetails = {
  category: string;
  name: string;
};

const SingleChoiceBookingCategory = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceBookingCategory_query$key>(
    graphql`
      fragment singleChoiceBookingCategory_query on Query {
        bookingCategories {
          category
          name
        }
      }
    `,
    rootDataRelay,
  );

  const categories = useMemo<CategoryDetails[]>(() => rootData.bookingCategories.map((item) => item), [rootData.bookingCategories]);
  const filter = createFilterOptions<CategoryDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={categories}
      getOptionValue={(option) => (option as CategoryDetails).category}
      getOptionLabel={(option: string | CategoryDetails) => (option as CategoryDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as CategoryDetails;

        return (
          <li {...props} key={castedOption.category}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as CategoryDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceBookingCategory);
