import { Chip, Tooltip } from '@mui/material';
import Grid from '@mui/material/Grid2';
import { memo } from 'react';
import { GridContainer, SmallIconTypography } from '../commons';
import type { ZoneDetails } from './zone';
import Zone from './zone';

type Props = {
  zones: readonly ZoneDetails[];
};

const maxItemToDisplay = 2;

const Zones = ({ zones }: Props) => {
  if (zones.length === 0) {
    return <SmallIconTypography label="N/A" />;
  }

  const visibleItems = zones.slice(0, maxItemToDisplay);
  const extraItems = zones.slice(maxItemToDisplay);

  return (
    <GridContainer spacing={1}>
      {visibleItems.map((zone) => (
        <Grid key={zone.id}>
          <Zone key={zone.id} zone={zone} maxWidth={100} />
        </Grid>
      ))}
      {extraItems.length > 0 && (
        <Grid>
          <Tooltip title={extraItems.map((item) => item.name).join(', ')}>
            <Chip label={`+${extraItems.length}`} />
          </Tooltip>
        </Grid>
      )}
    </GridContainer>
  );
};

export default memo(Zones);
