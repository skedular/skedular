import Stack from '@mui/material/Stack';
import type { CSSProperties } from '@mui/material/styles/createTypography';
import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import type { PropsWithChildren } from 'react';
import { forwardRef } from 'react';

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  color?: CSSProperties['color'];
};

const StackColumn = forwardRef<HTMLDivElement, PropsWithChildren<Props>>(({ children, sx, spacing, color }, ref) => (
  <Stack direction="column" spacing={spacing ?? 1} sx={sx} color={color} ref={ref}>
    {children}
  </Stack>
));

export default StackColumn;
