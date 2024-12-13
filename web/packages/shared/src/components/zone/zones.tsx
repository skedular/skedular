import Grid from '@mui/material/Grid2';
import { memo } from 'react';
import { BodyIconTypography, GridContainer } from '../commons';
import type { ZoneDetails } from './zone';
import Zone from './zone';

type Props = {
  zones: readonly ZoneDetails[];
  maxWidth?: number;
};

const Zones = ({ zones, maxWidth }: Props) => {
  if (zones.length === 0) {
    return <BodyIconTypography label="N/A" />;
  }

  return (
    <GridContainer spacing={1}>
      {zones.map((zone) => (
        <Grid key={zone.id}>
          <Zone zone={zone} maxWidth={maxWidth} />
        </Grid>
      ))}
    </GridContainer>
  );
};

export default memo(Zones);
