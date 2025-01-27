import { Chip, Tooltip } from '@mui/material';
import Grid from '@mui/material/Grid2';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';
import { GridContainer, SmallIconTypography, StackRow } from '../commons';
import { DeskIcon } from '../icons';
import type { DeskDetails } from './desk';
import Desk from './desk';

type Props = {
  sx?: SxProps<Theme>;
  desks: readonly DeskDetails[];
  hideIcon?: boolean;
};

const maxItemToDisplay = 2;

const Desks = ({ sx, desks, hideIcon }: Props) => {
  if (desks.length === 0) {
    return <SmallIconTypography label="N/A" startElement={!hideIcon && <DeskIcon />} sx={sx} />;
  }

  const visibleItems = desks.slice(0, maxItemToDisplay);
  const extraItems = desks.slice(maxItemToDisplay);

  return (
    <StackRow sx={sx}>
      <GridContainer spacing={1}>
        {!hideIcon && (
          <Grid>
            <DeskIcon />
          </Grid>
        )}
        {visibleItems.map((desk) => (
          <Grid key={desk.id}>
            <Desk key={desk.id} desk={desk} />
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

export default memo(Desks);
