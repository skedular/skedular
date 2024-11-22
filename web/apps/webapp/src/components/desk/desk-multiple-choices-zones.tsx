import type { deskMultipleChoicesZones_query$key } from '@/queries/__generated__/deskMultipleChoicesZones_query.graphql';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: deskMultipleChoicesZones_query$key;
  name: string;
  required?: boolean;
};

type ZoneDetails = {
  id: string;
  name: string;
};

const DeskMultipleChoicesZones = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment< deskMultipleChoicesZones_query$key>(
    graphql`
      fragment deskMultipleChoicesZones_query on Query{
        locationTags(
          where: { locationId: $locationId, tagType: $zoneTagType }
          orderBy: $deskMultipleChoicesZonesSortingValues
        )  {
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
    if (!rootData.locationTags) {
      return [];
    }

    return rootData.locationTags.edges.map(({ node }) => node);
  }, [rootData.locationTags]);

  const filter = createFilterOptions<ZoneDetails>();

  return (
    <Autocomplete
      label="Zones"
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

export default memo(DeskMultipleChoicesZones);
