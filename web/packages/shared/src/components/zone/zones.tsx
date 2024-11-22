import Grid from '@mui/material/Grid2';
import { memo } from 'react';
import type { ZoneType } from './zone';
import Zone from './zone';

type Props = {
  zones: readonly ZoneType[];
  maxWidth?: number;
};

const Zones = ({ zones, maxWidth }: Props) => (
  <Grid container spacing={1}>
    {zones.map((zone) => (
      <Grid key={zone.id}>
        <Zone zone={zone} maxWidth={maxWidth} />
      </Grid>
    ))}
  </Grid>
);

export default memo(Zones);
