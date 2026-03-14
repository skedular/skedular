import { BodyIconTypography } from '@/components/commons';
import { AddOrganizationProductTagButton } from '@/components/organization/addOrganizationProductTag';
import type { multipleChoicesProductTags_query$key } from '@/queries/__generated__/multipleChoicesProductTags_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesProductTags_query$key;
  name: string;
  required?: boolean;
  organizationUniqueAlphanumericName: string;
};

type ProductTagDetails = {
  id: string;
  name: string;
  color: string | null | undefined;
};

const MultipleChoicesProductTags = ({ rootDataRelay, name, required, organizationUniqueAlphanumericName }: Props) => {
  const rootData = useFragment<multipleChoicesProductTags_query$key>(
    graphql`
      fragment multipleChoicesProductTags_query on Query @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null }) {
        organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
          productTags(first: $count, after: $cursor, orderBy: $multipleChoicesProductTagsSortingValues) @connection(key: "multipleChoicesProductTags_productTags") {
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

  const items = useMemo<ProductTagDetails[]>(() => (rootData.organization ? rootData.organization.productTags.edges.map(({ node }) => node) : []), [rootData.organization]);
  const connectionIds = useMemo(() => (rootData.organization ? [rootData.organization.productTags.__id] : []), [rootData.organization]);
  const filter = createFilterOptions<ProductTagDetails>();

  if (items.length === 0) {
    return <AddOrganizationProductTagButton organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} connectionIds={connectionIds} size="medium" />;
  }

  return (
    <Autocomplete
      name={name}
      multiple={true}
      required={required}
      options={items}
      getOptionValue={(option) => (option as ProductTagDetails).id}
      getOptionLabel={(option: string | ProductTagDetails) => (option as ProductTagDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as ProductTagDetails;

        return (
          <li {...props} key={castedOption.id}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      disableCloseOnSelect
      filterOptions={(options, params) => filter(options as ProductTagDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesProductTags);
