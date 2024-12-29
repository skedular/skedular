import Grid from '@mui/material/Grid2';
import { BodyIconTypography } from '@repo/shared/components/commons';
import type { PropsWithChildren } from 'react';
import { memo } from 'react';

type Props = {
  label?: string;
  useWiderSpace?: boolean;
};

const FormFieldLabel = ({ children, label, useWiderSpace }: PropsWithChildren<Props>) => (
  <Grid container sx={{ alignItems: 'center' }}>
    <Grid size={{ xs: useWiderSpace ? 3 : 1 }}>
      <BodyIconTypography label={label} />
    </Grid>
    <Grid size={{ xs: useWiderSpace ? 9 : 11 }}>{children}</Grid>
  </Grid>
);

export default memo(FormFieldLabel);
