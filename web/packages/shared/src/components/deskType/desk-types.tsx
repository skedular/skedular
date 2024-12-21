import { Chip, Tooltip } from '@mui/material';
import Grid from '@mui/material/Grid2';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';
import { GridContainer, SmallIconTypography, StackRow } from '../commons';
import { DeskTypeIcon } from '../icons';
import type { DeskTypeDetails } from './desk-type';
import DeskType from './desk-type';

type Props = {
  sx?: SxProps<Theme>;
  deskTypes: readonly DeskTypeDetails[];
  hideIcon?: boolean;
};

const maxItemToDisplay = 2;

const DeskTypes = ({ sx, deskTypes, hideIcon }: Props) => {
  if (deskTypes.length === 0) {
    return <SmallIconTypography label="N/A" startElement={!hideIcon && <DeskTypeIcon />} sx={sx} />;
  }

  const visibleItems = deskTypes.slice(0, maxItemToDisplay);
  const extraItems = deskTypes.slice(maxItemToDisplay);

  return (
    <StackRow sx={sx}>
      <GridContainer spacing={1}>
        {!hideIcon && (
          <Grid>
            <DeskTypeIcon />
          </Grid>
        )}
        {visibleItems.map((deskType) => (
          <Grid key={deskType.id}>
            <DeskType key={deskType.id} deskType={deskType} />
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

export default memo(DeskTypes);
