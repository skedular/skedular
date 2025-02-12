import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceLocation_locations_query$key } from '@/queries/__generated__/singleChoiceLocation_locations_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceLocation_locations_query$key;
  id: string;
  required?: boolean;
};

interface LocationDetails {
  id: string;
  name: string;
}

const SingleChoiceLocation = ({ rootDataRelay, id, required }: Props) => {
  const rootData = useFragment<singleChoiceLocation_locations_query$key>(
    graphql`
      fragment singleChoiceLocation_locations_query on Query {
        locations(where: { organizationId: $organizationId }) @include(if: $organizationExists) {
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

  const locations = useMemo(() => (rootData.locations?.edges ? rootData.locations.edges.map(({ node }) => node) : []), [rootData.locations]);
  const filter = createFilterOptions<LocationDetails>();

  return (
    <Autocomplete
      name={id}
      multiple={false}
      required={required}
      options={locations}
      getOptionValue={(option) => (option as LocationDetails).id}
      getOptionLabel={(option: string | LocationDetails) => (option as LocationDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as LocationDetails;

        return (
          <li {...props}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as LocationDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceLocation);
