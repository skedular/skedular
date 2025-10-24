import { GridContainer, SmallIconTypography, StackRow } from '@/components/commons';
import { ZoneIcon } from '@/components/icons';
import { Chip, Tooltip } from '@mui/material';
import Grid from '@mui/material/Grid';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';
import type { ZoneDetails } from './zone';
import Zone from './zone';

type Props = {
  sx?: SxProps<Theme>;
  zones: readonly ZoneDetails[];
  hideIcon?: boolean;
  hideNAText?: boolean;
};

const maxItemToDisplay = 2;

const Zones = ({ sx, zones, hideIcon, hideNAText }: Props) => {
  if (zones.length === 0) {
    return hideNAText ? null : <SmallIconTypography label="N/A" startElement={!hideIcon && <ZoneIcon />} sx={sx} />;
  }

  const visibleItems = zones.slice(0, maxItemToDisplay);
  const extraItems = zones.slice(maxItemToDisplay);

  return (
    <StackRow sx={sx}>
      <GridContainer spacing={1}>
        {!hideIcon && (
          <Grid>
            <ZoneIcon />
          </Grid>
        )}
        {visibleItems.map((zone) => (
          <Grid key={zone.id}>
            <Zone zone={zone} />
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
    </StackRow>
  );
};

export default memo(Zones);
