import Grid from '@mui/material/Grid2';
import Typography from '@mui/material/Typography';
import { memo } from 'react';
import type { DeskTypeDetails } from './desk-type';
import DeskType from './desk-type';

type Props = {
  deskTypes: readonly DeskTypeDetails[];
  maxWidth?: number;
};

const DeskTypes = ({ deskTypes, maxWidth }: Props) => {
  if (deskTypes.length === 0) {
    return <Typography variant="body1">N/A</Typography>;
  }

  return (
    <Grid container spacing={1}>
      {deskTypes.map((deskType) => (
        <Grid key={deskType.id}>
          <DeskType deskType={deskType} maxWidth={maxWidth} />
        </Grid>
      ))}
    </Grid>
  );
};

export default memo(DeskTypes);
