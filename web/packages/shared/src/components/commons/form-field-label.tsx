import Grid from '@mui/material/Grid2';
import { Theme } from '@mui/material/styles';
import type { SxProps } from '@mui/system';
import type { PropsWithChildren } from 'react';
import { memo } from 'react';
import BodyIconTypography from './body-icon-typography';

type Props = {
  sx?: SxProps<Theme>;
  label?: string;
  useWiderSpace?: boolean;
};

const FormFieldLabel = ({ children, sx, label, useWiderSpace }: PropsWithChildren<Props>) => (
  <Grid container sx={{ alignItems: 'center', ...sx }}>
    <Grid size={{ xs: useWiderSpace ? 3 : 1 }}>
      <BodyIconTypography label={label} />
    </Grid>
    <Grid size={{ xs: useWiderSpace ? 9 : 11 }}>{children}</Grid>
  </Grid>
);

export default memo(FormFieldLabel);
