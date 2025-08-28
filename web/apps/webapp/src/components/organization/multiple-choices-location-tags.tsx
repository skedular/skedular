import { BodyIconTypography } from '@/components/commons';
import { AddOrganizationLocationTagButton } from '@/components/organization/addOrganizationLocationTag';
import type { multipleChoicesLocationTags_query$key } from '@/queries/__generated__/multipleChoicesLocationTags_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesLocationTags_query$key;
  name: string;
  required?: boolean;
  organizationUniqueAlphanumericName: string;
};

type LocationTagDetails = {
  id: string;
  name: string;
  color: string | null | undefined;
};

const MultipleChoicesLocationTags = ({ rootDataRelay, name, required, organizationUniqueAlphanumericName }: Props) => {
  const rootData = useFragment<multipleChoicesLocationTags_query$key>(
    graphql`
      fragment multipleChoicesLocationTags_query on Query @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null }) {
        locationTags(
          first: $count
          after: $cursor
          where: { organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName }
          orderBy: $multipleChoicesLocationTagsSortingValues
        ) @connection(key: "multipleChoicesLocationTags_locationTags") {
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

  const locationTags = useMemo<LocationTagDetails[]>(() => rootData.locationTags.edges.map(({ node }) => node), [rootData.locationTags]);
  const connectionIds = useMemo(() => [rootData.locationTags.__id], [rootData.locationTags]);
  const filter = createFilterOptions<LocationTagDetails>();

  if (locationTags.length === 0) {
    return <AddOrganizationLocationTagButton organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} connectionIds={connectionIds} size="medium" />;
  }

  return (
    <Autocomplete
      name={name}
      multiple={true}
      required={required}
      options={locationTags}
      getOptionValue={(option) => (option as LocationTagDetails).id}
      getOptionLabel={(option: string | LocationTagDetails) => (option as LocationTagDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as LocationTagDetails;

        return (
          <li {...props} key={castedOption.id}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      disableCloseOnSelect
      filterOptions={(options, params) => filter(options as LocationTagDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesLocationTags);
