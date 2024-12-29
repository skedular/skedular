import type { GridSpacing } from '@mui/material/Grid2';
import Grid from '@mui/material/Grid2';
import { Theme } from '@mui/material/styles';
import type { ResponsiveStyleValue, SxProps } from '@mui/system';
import type { PropsWithChildren } from 'react';

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<GridSpacing>;
};

const GridContainer = ({ children, sx, spacing }: PropsWithChildren<Props>) => (
  <Grid container spacing={spacing || { xs: 1, sm: 7, md: 7, lg: 7 }} sx={{ alignItems: 'flex-start', ...sx }}>
    {children}
  </Grid>
);

export default GridContainer;
