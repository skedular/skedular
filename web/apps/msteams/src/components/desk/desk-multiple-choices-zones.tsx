import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import graphql from 'babel-plugin-relay/macro';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { usePaginationFragment } from 'react-relay';
import type { deskMultipleChoicesZones_query$key } from './__generated__/deskMultipleChoicesZones_query.graphql';
import type { deskMultipleChoicesZones_refetchableFragment } from './__generated__/deskMultipleChoicesZones_refetchableFragment.graphql';

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
  const { data: rootData } = usePaginationFragment<deskMultipleChoicesZones_refetchableFragment, deskMultipleChoicesZones_query$key>(
    graphql`
      fragment deskMultipleChoicesZones_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "deskMultipleChoicesZones_refetchableFragment") {
        locationTags(
          first: $count
          after: $cursor
          where: { locationId: $locationId, tagType: $zoneTagType }
          orderBy: $deskMultipleChoicesZonesSortingValues
        ) @connection(key: "locationZonesTab_locationTags") {
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

  if (!rootData.locationTags) {
    return <></>;
  }

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
