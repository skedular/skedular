import { BodyIconTypography } from '@/components/commons';
import { AddOrganizationCustomTagButton } from '@/components/organization/addOrganizationCustomTag';
import type { multipleChoicesCustomTags_query$key } from '@/queries/__generated__/multipleChoicesCustomTags_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesCustomTags_query$key;
  name: string;
  required?: boolean;
  organizationId: string;
};

type CustomTagDetails = {
  id: string;
  name: string;
  color: string | null | undefined;
};

const MultipleChoicesCustomTags = ({ rootDataRelay, name, required, organizationId }: Props) => {
  const rootData = useFragment<multipleChoicesCustomTags_query$key>(
    graphql`
      fragment multipleChoicesCustomTags_query on Query @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null }) {
        customTags(first: $count, after: $cursor, where: { organizationId: $organizationId }, orderBy: $multipleChoicesCustomTagsSortingValues)
          @connection(key: "multipleChoicesCustomTags_customTags") {
          __id
          totalCount
          edges {
            node {
              id
              name
              color
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const connectionIds = useMemo(() => (rootData.customTags ? [rootData.customTags.__id] : []), [rootData.customTags]);
  const customTags = useMemo<CustomTagDetails[]>(() => (rootData.customTags ? rootData.customTags.edges.map(({ node }) => node) : []), [rootData.customTags]);
  const filter = createFilterOptions<CustomTagDetails>();

  if (customTags.length === 0) {
    return <AddOrganizationCustomTagButton organizationId={organizationId} connectionIds={connectionIds} size="medium" />;
  }

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
      disableCloseOnSelect
      filterOptions={(options, params) => filter(options as CustomTagDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesCustomTags);
