import { BodyIconTypography } from '@skedular/ui';
import type { singleChoiceLocationRestrictedInformationCategory_query$key } from '@/queries/__generated__/singleChoiceLocationRestrictedInformationCategory_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceLocationRestrictedInformationCategory_query$key;
  name: string;
  required?: boolean;
};

type LocationRestrictedInformationCategoryDetails = {
  category: string;
  name: string;
};

const SingleChoiceLocationRestrictedInformationCategory = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceLocationRestrictedInformationCategory_query$key>(
    graphql`
      fragment singleChoiceLocationRestrictedInformationCategory_query on Query {
        locationRestrictedInformationCategories {
          category
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<LocationRestrictedInformationCategoryDetails[]>(
    () => rootData.locationRestrictedInformationCategories.map((item) => item),
    [rootData.locationRestrictedInformationCategories],
  );
  const filter = createFilterOptions<LocationRestrictedInformationCategoryDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as LocationRestrictedInformationCategoryDetails).category}
      getOptionLabel={(option: string | LocationRestrictedInformationCategoryDetails) => (option as LocationRestrictedInformationCategoryDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as LocationRestrictedInformationCategoryDetails;

        return (
          <li {...props} key={castedOption.category}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as LocationRestrictedInformationCategoryDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceLocationRestrictedInformationCategory);
