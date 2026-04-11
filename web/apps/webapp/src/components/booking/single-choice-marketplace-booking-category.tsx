import { BodyIconTypography } from '@/components/commons';
import { Autocomplete } from '@/components/forms';
import type { singleChoiceMarketplaceBookingCategory_query$key } from '@/queries/__generated__/singleChoiceMarketplaceBookingCategory_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceMarketplaceBookingCategory_query$key;
  name: string;
  required?: boolean;
};

type CategoryDetails = {
  category: string;
  name: string;
};

const SingleChoiceMarketplaceBookingCategory = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceMarketplaceBookingCategory_query$key>(
    graphql`
      fragment singleChoiceMarketplaceBookingCategory_query on Query {
        marketplaceBookingCategories {
          category
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<CategoryDetails[]>(() => rootData.marketplaceBookingCategories.map((item) => item), [rootData.marketplaceBookingCategories]);
  const filter = createFilterOptions<CategoryDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
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

export default memo(SingleChoiceMarketplaceBookingCategory);
