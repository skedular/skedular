import type { GridSpacing } from '@mui/material/Grid';
import Grid, { GridWrap } from '@mui/material/Grid';
import { Theme } from '@mui/material/styles';
import type { ResponsiveStyleValue, SxProps } from '@mui/system';
import type { PropsWithChildren } from 'react';

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<GridSpacing>;
  wrap?: GridWrap;
};

const GridContainer = ({ children, sx, spacing, wrap }: PropsWithChildren<Props>) => (
  <Grid container spacing={spacing || { xs: 1, sm: 7, md: 7, lg: 7 }} sx={{ alignItems: 'flex-start', ...sx }} wrap={wrap}>
    {children}
  </Grid>
);

export default GridContainer;
