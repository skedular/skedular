import type { organizationMultipleChoicesIndustries_query$key } from '@/queries/__generated__/organizationMultipleChoicesIndustries_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationMultipleChoicesIndustries_query$key;
  name: string;
  required?: boolean;
};

type SubCategoryDetails = {
  mainCategoryName: string;
  id: string;
  name: string;
};

const OrganizationMultipleChoicesIndustries = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment organizationMultipleChoicesIndustries_query on Query {
        organizationIndustryMainCategoriesReferences {
          id
          name
          subCategories {
            id
            name
          }
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<SubCategoryDetails[]>(
    () =>
      rootData.organizationIndustryMainCategoriesReferences.flatMap((item) =>
        item.subCategories.map<SubCategoryDetails>(({ id, name }) => ({
          mainCategoryName: item.name,
          id,
          name,
        })),
      ),
    [rootData.organizationIndustryMainCategoriesReferences],
  );

  const filter = createFilterOptions<SubCategoryDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={true}
      groupBy={(option) => (option as SubCategoryDetails).mainCategoryName}
      required={required}
      options={items}
      getOptionValue={(option) => (option as SubCategoryDetails).id}
      getOptionLabel={(option: string | SubCategoryDetails) => (option as SubCategoryDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as SubCategoryDetails;

        return (
          <li {...props} key={castedOption.id}>
            {castedOption.name}
          </li>
        );
      }}
      disableCloseOnSelect
      filterOptions={(options, params) => filter(options as SubCategoryDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
      helperText="E.g. Professional services"
    />
  );
};

export default memo(OrganizationMultipleChoicesIndustries);
