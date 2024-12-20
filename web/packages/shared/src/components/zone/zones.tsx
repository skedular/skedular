import { Chip, Tooltip } from '@mui/material';
import Grid from '@mui/material/Grid2';
import { memo } from 'react';
import { GridContainer, SmallIconTypography, StackRow } from '../commons';
import { ZoneIcon } from '../icons';
import type { ZoneDetails } from './zone';
import Zone from './zone';

type Props = {
  zones: readonly ZoneDetails[];
  hideIcon?: boolean;
};

const maxItemToDisplay = 2;

const Zones = ({ zones, hideIcon }: Props) => {
  const visibleItems = zones.slice(0, maxItemToDisplay);
  const extraItems = zones.slice(maxItemToDisplay);

  return (
    <StackRow sx={{ paddingTop: 1, paddingBottom: 1 }}>
      {!hideIcon && <ZoneIcon />}
      {zones.length === 0 && <SmallIconTypography label="N/A" />}
      {zones.length !== 0 && (
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
      )}
    </StackRow>
  );
};

export default memo(Zones);
