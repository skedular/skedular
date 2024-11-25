import type { zoneSelector_allZones_query$key } from '@/queries/__generated__/zoneSelector_allZones_query.graphql';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { ZoneIcon } from '@repo/shared/components/icons';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: zoneSelector_allZones_query$key;
  onChange: (id?: string) => void;
};

const allId = 'kkigMVsUXwi2YMSSrXv7i';

const ZoneSelector = ({ rootDataRelay, onChange }: Props) => {
  const rootData = useFragment<zoneSelector_allZones_query$key>(
    graphql`
      fragment zoneSelector_allZones_query on Query {
        zonesLocations: locations(where: { organizationId: $organizationId }, orderBy: $locationsSortingValues) {
          __id
          totalCount
          edges {
            node {
              locationTags {
                id
                name
              }
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [id, setId] = useState<string>(allId);
  const allItems = useMemo(() => {
    const allLocations = rootData.zonesLocations?.edges ? rootData.zonesLocations.edges.map(({ node }) => node) : [];

    return allLocations.flatMap((location) => location.locationTags);
  }, [rootData.zonesLocations]);

  const handleSelectedChanged = (event: SelectChangeEvent) => {
    const id = event.target.value as string;

    setId(id);
    onChange(id === allId ? undefined : id);
  };

  return (
    <Select
      value={id}
      onChange={handleSelectedChanged}
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
        const selectedItem = allItems.find((item) => item.id === selectedId);
        if (selectedItem) {
          return (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <ZoneIcon />
              <Typography variant="h6">Zones</Typography>
              <Divider orientation="vertical" flexItem />
              <Typography variant="body1">{selectedItem.name}</Typography>
            </Stack>
          );
        }

        return (
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <ZoneIcon />
            <Typography variant="h6">Zones</Typography>
            <Divider orientation="vertical" flexItem />
            <Typography variant="body1">All</Typography>
          </Stack>
        );
      }}
    >
      <MenuItem value={allId}>
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <Typography variant="h6">All</Typography>
        </Stack>
      </MenuItem>

      {allItems.map((location) => (
        <MenuItem key={location.id} value={location.id}>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">{location.name}</Typography>
          </Stack>
        </MenuItem>
      ))}
    </Select>
  );
};

export default memo(ZoneSelector);
