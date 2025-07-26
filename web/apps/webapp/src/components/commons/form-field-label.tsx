import Grid from '@mui/material/Grid';
import type { CSSProperties } from '@mui/material/styles';
import { Theme } from '@mui/material/styles';
import type { SxProps } from '@mui/system';
import type { PropsWithChildren } from 'react';
import { memo } from 'react';
import BodyIconTypography from './body-icon-typography';

type Props = {
  sx?: SxProps<Theme>;
  label?: string;
  useWiderSpace?: boolean;
  fontWeight?: CSSProperties['fontWeight'];
  required?: boolean;
};

const FormFieldLabel = ({ children, sx, label, useWiderSpace, fontWeight, required }: PropsWithChildren<Props>) => (
  <Grid container sx={{ alignItems: 'center', ...sx }}>
    <Grid size={{ xs: useWiderSpace ? 3 : 1 }}>{label && <BodyIconTypography label={required ? `${label} *` : label} fontWeight={fontWeight} />}</Grid>
    <Grid size={{ xs: useWiderSpace ? 9 : 11 }}>{children}</Grid>
  </Grid>
);

export default memo(FormFieldLabel);
