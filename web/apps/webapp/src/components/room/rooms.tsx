import { GridContainer, SmallIconTypography, StackRow } from '@/components/commons';
import { RoomIcon } from '@/components/icons';
import { Chip, Tooltip } from '@mui/material';
import Grid from '@mui/material/Grid2';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';
import type { RoomDetails } from './room';
import Room from './room';

type Props = {
  sx?: SxProps<Theme>;
  rooms: readonly RoomDetails[];
  hideIcon?: boolean;
  hideNAText?: boolean;
};

const maxItemToDisplay = 2;

const Rooms = ({ sx, rooms, hideIcon, hideNAText }: Props) => {
  if (rooms.length === 0) {
    return hideNAText ? <></> : <SmallIconTypography label="N/A" startElement={!hideIcon && <RoomIcon />} sx={sx} />;
  }

  const visibleItems = rooms.slice(0, maxItemToDisplay);
  const extraItems = rooms.slice(maxItemToDisplay);

  return (
    <StackRow sx={sx}>
      <GridContainer spacing={1}>
        {!hideIcon && (
          <Grid>
            <RoomIcon />
          </Grid>
        )}
        {visibleItems.map((room) => (
          <Grid key={room.id}>
            <Room key={room.id} room={room} />
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

export default memo(Rooms);
