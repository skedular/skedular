import { Chip, Tooltip } from '@mui/material';
import Grid from '@mui/material/Grid2';
import { memo } from 'react';
import { GridContainer, SmallIconTypography } from '../commons';
import type { DeskTypeDetails } from './desk-type';
import DeskType from './desk-type';

type Props = {
  deskTypes: readonly DeskTypeDetails[];
};

const maxItemToDisplay = 2;

const DeskTypes = ({ deskTypes }: Props) => {
  if (deskTypes.length === 0) {
    return <SmallIconTypography label="N/A" />;
  }

  const visibleItems = deskTypes.slice(0, maxItemToDisplay);
  const extraItems = deskTypes.slice(maxItemToDisplay);

  return (
    <GridContainer spacing={1}>
      {visibleItems.map((deskType) => (
        <Grid key={deskType.id}>
          <DeskType key={deskType.id} deskType={deskType} maxWidth={100} />
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

export default memo(DeskTypes);
