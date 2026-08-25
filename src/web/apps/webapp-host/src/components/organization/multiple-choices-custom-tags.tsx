import { BodyIconTypography, StackColumn } from '@skedular/ui';
import { AddOrganizationCustomTagButton } from '@/components/organization/addOrganizationCustomTag';
import Box from '@mui/material/Box';
import type { multipleChoicesCustomTags_query$key } from '@/queries/__generated__/multipleChoicesCustomTags_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesCustomTags_query$key;
  name: string;
  required?: boolean;
  organizationCustomDomain: string;
};

type CustomTagDetails = {
  id: string;
  name: string;
  color: string | null | undefined;
};

const MultipleChoicesCustomTags = ({ rootDataRelay, name, required, organizationCustomDomain }: Props) => {
  const rootData = useFragment<multipleChoicesCustomTags_query$key>(
    graphql`
      fragment multipleChoicesCustomTags_query on Query @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null }) {
        organization(customDomain: $organizationCustomDomain) {
          customTags(first: $count, after: $cursor, orderBy: $multipleChoicesCustomTagsSortingValues) @connection(key: "multipleChoicesCustomTags_customTags") {
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
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<CustomTagDetails[]>(() => (rootData.organization ? rootData.organization.customTags.edges.map(({ node }) => node) : []), [rootData.organization]);
  const filter = createFilterOptions<CustomTagDetails>();

  if (items.length === 0) {
    return (
      <Box sx={{ border: 1, borderColor: 'divider', borderRadius: 2, bgcolor: 'action.hover', px: 2, py: 1.75 }}>
        <StackColumn spacing={0.75} sx={{ alignItems: 'flex-start' }}>
          <BodyIconTypography label="No tags yet" />
          <BodyIconTypography label="Create your first tag to help group and filter resources." />
          <AddOrganizationCustomTagButton organizationCustomDomain={organizationCustomDomain} label="Create tag" size="small" variant="outlined" />
        </StackColumn>
      </Box>
    );
  }

  return (
    <Autocomplete
      name={name}
      multiple={true}
      required={required}
      options={items}
      getOptionValue={(option) => (option as CustomTagDetails).id}
      getOptionLabel={(option: string | CustomTagDetails) => (option as CustomTagDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as CustomTagDetails;

        return (
          <li {...props} key={castedOption.id}>
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
