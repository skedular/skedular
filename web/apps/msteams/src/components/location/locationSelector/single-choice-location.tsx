import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import graphql from 'babel-plugin-relay/macro';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { useFragment } from 'react-relay';
import type { singleChoiceLocation_locations_query$key } from './__generated__/singleChoiceLocation_locations_query.graphql';

type Props = {
  rootDataRelay: singleChoiceLocation_locations_query$key;
  id: string;
  required?: boolean;
  label?: string;
};

interface LocationDetails {
  id: string;
  name: string;
}

const SingleChoiceLocation = ({ rootDataRelay, id, required, label }: Props) => {
  const rootData = useFragment<singleChoiceLocation_locations_query$key>(
    graphql`
      fragment singleChoiceLocation_locations_query on Query {
        locations(where: { organizationId: $organizationId }) {
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
      label={label ?? 'Location'}
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
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <Typography variant="body1">{castedOption.name}</Typography>
            </Stack>
          </li>
        );
      }}
      disableCloseOnSelect={false}
      freeSolo={true}
      filterOptions={(options, params) => filter(options as LocationDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceLocation);
