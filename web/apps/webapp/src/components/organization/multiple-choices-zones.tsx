import type { multipleChoicesZones_query$key } from '@/queries/__generated__/multipleChoicesZones_query.graphql';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesZones_query$key;
  name: string;
  required?: boolean;
};

type ZoneDetails = {
  id: string;
  name: string;
};

const MultipleChoicesZones = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<multipleChoicesZones_query$key>(
    graphql`
      fragment multipleChoicesZones_query on Query {
        zones(where: { organizationId: $organizationId }, orderBy: $multipleChoicesZonesSortingValues) {
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

  const zones = useMemo<ZoneDetails[]>(() => {
    if (!rootData.zones) {
      return [];
    }

    return rootData.zones.edges.map(({ node }) => node);
  }, [rootData.zones]);

  if (!rootData.zones) {
    return <></>;
  }

  const filter = createFilterOptions<ZoneDetails>();

  return (
    <Autocomplete
      label="Desk types"
      name={name}
      multiple={true}
      required={required}
      options={zones}
      getOptionValue={(option) => (option as ZoneDetails).id}
      getOptionLabel={(option: string | ZoneDetails) => (option as ZoneDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as ZoneDetails;

        return (
          <li {...props}>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <Typography variant="body1">{castedOption.name}</Typography>
            </Stack>
          </li>
        );
      }}
      disableCloseOnSelect={true}
      freeSolo={true}
      filterOptions={(options, params) => filter(options as ZoneDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesZones);
