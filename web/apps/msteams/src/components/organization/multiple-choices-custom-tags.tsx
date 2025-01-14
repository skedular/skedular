import { createFilterOptions } from '@mui/material/useAutocomplete';
import { BodyIconTypography } from '@repo/shared/components/commons';
import graphql from 'babel-plugin-relay/macro';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { useFragment } from 'react-relay';
import type { multipleChoicesCustomTags_query$key } from './__generated__/multipleChoicesCustomTags_query.graphql';

type Props = {
  rootDataRelay: multipleChoicesCustomTags_query$key;
  name: string;
  required?: boolean;
};

type CustomTagDetails = {
  id: string;
  name: string;
};

const MultipleChoicesCustomTags = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<multipleChoicesCustomTags_query$key>(
    graphql`
      fragment multipleChoicesCustomTags_query on Query {
        customTags(where: { organizationId: $organizationId }, orderBy: $multipleChoicesCustomTagsSortingValues) {
          __id
          totalCount
          edges {
            node {
              id
              name
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const customTags = useMemo<CustomTagDetails[]>(() => {
    if (!rootData.customTags) {
      return [];
    }

    return rootData.customTags.edges.map(({ node }) => node);
  }, [rootData.customTags]);

  if (!rootData.customTags) {
    return <></>;
  }

  const filter = createFilterOptions<CustomTagDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={true}
      required={required}
      options={customTags}
      getOptionValue={(option) => (option as CustomTagDetails).id}
      getOptionLabel={(option: string | CustomTagDetails) => (option as CustomTagDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as CustomTagDetails;

        return (
          <li {...props}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      disableCloseOnSelect={true}
      freeSolo={true}
      filterOptions={(options, params) => filter(options as CustomTagDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesCustomTags);
