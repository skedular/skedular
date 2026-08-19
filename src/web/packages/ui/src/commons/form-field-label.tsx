'use client';

import Grid from '@mui/material/Grid';
import type { CSSProperties } from '@mui/material/styles';
import { Theme } from '@mui/material/styles';
import type { SxProps } from '@mui/system';
import type { PropsWithChildren, ReactNode } from 'react';
import { memo } from 'react';
import FieldHelp from './field-help';
import BodyIconTypography from '../typography/body-icon-typography';
import StackRow from '../stack-row';

type Props = {
  sx?: SxProps<Theme>;
  label?: string;
  help?: ReactNode;
  helpLabel?: string;
  fontWeight?: CSSProperties['fontWeight'];
  required?: boolean;
  stackLabelOnTop?: boolean;
  useWiderSpace?: boolean;
};

const FormFieldLabel = ({ children, sx, label, help, helpLabel, fontWeight, required, stackLabelOnTop, useWiderSpace }: PropsWithChildren<Props>) =>
  stackLabelOnTop ? (
    <Grid
      container
      columnSpacing={2}
      rowSpacing={1}
      sx={{
        alignItems: { xs: 'flex-start', md: 'center' },
        ...sx,
      }}
    >
      <Grid
        size={{ xs: 12, md: useWiderSpace ? 3 : 1 }}
        sx={{
          display: 'flex',
          alignItems: { xs: 'flex-start', md: 'center' },
        }}
      >
        {label || help ? (
          <StackRow sx={{ alignItems: 'center', gap: 0.25 }}>
            {label ? <BodyIconTypography label={required ? `${label} *` : label} fontWeight={fontWeight} /> : null}
            {help ? <FieldHelp label={helpLabel ?? label ?? 'Field'}>{help}</FieldHelp> : null}
          </StackRow>
        ) : null}
      </Grid>
      <Grid size={{ xs: 12, md: useWiderSpace ? 9 : 11 }}>{children}</Grid>
    </Grid>
  ) : (
    <Grid
      container
      columnSpacing={2}
      rowSpacing={1}
      sx={{
        alignItems: 'flex-start',
        ...sx,
      }}
    >
      <Grid
        size={12}
        sx={{
          display: 'flex',
          alignItems: 'flex-start',
        }}
      >
        {label || help ? (
          <StackRow sx={{ alignItems: 'center', gap: 0.25 }}>
            {label ? <BodyIconTypography label={required ? `${label} *` : label} fontWeight={fontWeight} /> : null}
            {help ? <FieldHelp label={helpLabel ?? label ?? 'Field'}>{help}</FieldHelp> : null}
          </StackRow>
        ) : null}
      </Grid>
      <Grid size={12}>{children}</Grid>
    </Grid>
  );

export default memo(FormFieldLabel);
