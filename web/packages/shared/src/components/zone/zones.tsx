import Grid from '@mui/material/Grid2';
import Typography from '@mui/material/Typography';
import { memo } from 'react';
import type { ZoneDetails } from './zone';
import Zone from './zone';

type Props = {
  zones: readonly ZoneDetails[];
  maxWidth?: number;
};

const Zones = ({ zones, maxWidth }: Props) => {
  if (zones.length === 0) {
    return <Typography variant="body1">N/A</Typography>;
  }

  return (
    <Grid container spacing={1}>
      {zones.map((zone) => (
        <Grid key={zone.id}>
          <Zone zone={zone} maxWidth={maxWidth} />
        </Grid>
      ))}
    </Grid>
  );
};

export default memo(Zones);
