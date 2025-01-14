import { createFilterOptions } from '@mui/material/useAutocomplete';
import graphql from 'babel-plugin-relay/macro';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { useFragment } from 'react-relay';
import type { organizationMultipleChoicesIndustries_query$key } from './__generated__/organizationMultipleChoicesIndustries_query.graphql';

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

  const industries = useMemo<SubCategoryDetails[]>(
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
      options={industries}
      getOptionValue={(option) => (option as SubCategoryDetails).id}
      getOptionLabel={(option: string | SubCategoryDetails) => (option as SubCategoryDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as SubCategoryDetails;

        return <li {...props}>{castedOption.name}</li>;
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
