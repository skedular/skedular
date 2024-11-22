import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { LocationAvatar } from '@repo/shared/components/avatars';
import { LocationIcon } from '@repo/shared/components/icons';
import graphql from 'babel-plugin-relay/macro';
import { memo, useMemo, useState } from 'react';
import { useFragment } from 'react-relay';
import type { locationSelector_allLocations_query$key } from './__generated__/locationSelector_allLocations_query.graphql';

type Props = {
  rootDataRelay: locationSelector_allLocations_query$key;
  onLocationChanged: (locationId?: string) => void;
};

const allLocationsId = 'kkigMVsUXwi2YMSSrXv7i';

const LocationSelector = ({ rootDataRelay, onLocationChanged }: Props) => {
  const rootData = useFragment<locationSelector_allLocations_query$key>(
    graphql`
      fragment locationSelector_allLocations_query on Query {
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

  const [selectedLocationId, setSelectedLocationId] = useState<string>(allLocationsId);
  const allLocations = useMemo(() => (rootData.locations?.edges ? rootData.locations.edges.map(({ node }) => node) : []), [rootData.locations]);

  const handleSelectedLocationChange = (event: SelectChangeEvent) => {
    const id = event.target.value as string;

    setSelectedLocationId(id);
    onLocationChanged(id === allLocationsId ? undefined : id);
  };

  return (
    <Select
      value={selectedLocationId}
      onChange={handleSelectedLocationChange}
      sx={{
        '& .MuiOutlinedInput-notchedOutline': {
          borderRadius: 4,
        },
        width: {
          xs: '100%',
          sm: 'min(100%, 300px)',
        },
      }}
      size="small"
      renderValue={(selectedId) => {
        const selectedLocation = allLocations.find((location) => location.id === selectedId);
        if (selectedLocation) {
          return (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <LocationIcon />
              <Typography variant="h6">Location</Typography>
              <Divider orientation="vertical" flexItem />
              <Typography variant="body1">{selectedLocation.name}</Typography>
            </Stack>
          );
        }

        return (
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <LocationIcon />
            <Typography variant="h6">Location</Typography>
            <Divider orientation="vertical" flexItem />
            <Typography variant="body1">All</Typography>
          </Stack>
        );
      }}
    >
      <MenuItem value={allLocationsId}>
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <Typography variant="h6">All</Typography>
        </Stack>
      </MenuItem>

      {allLocations.map((location) => (
        <MenuItem key={location.id} value={location.id}>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <LocationAvatar name={{ name: location.name }} size="small" />
            <Typography variant="h6">{location.name}</Typography>
          </Stack>
        </MenuItem>
      ))}
    </Select>
  );
};

export default memo(LocationSelector);
