import Grid from '@mui/material/Grid2';
import { memo } from 'react';
import { GridContainer, SmallIconTypography } from '../commons';
import type { DeskTypeDetails } from './desk-type';
import DeskType from './desk-type';

type Props = {
  deskTypes: readonly DeskTypeDetails[];
  maxWidth?: number;
};

const DeskTypes = ({ deskTypes, maxWidth }: Props) => {
  if (deskTypes.length === 0) {
    return <SmallIconTypography label="N/A" />;
  }

  return (
    <GridContainer spacing={1}>
      {deskTypes.map((deskType) => (
        <Grid key={deskType.id}>
          <DeskType deskType={deskType} maxWidth={maxWidth} />
        </Grid>
      ))}
    </GridContainer>
  );
};

export default memo(DeskTypes);
