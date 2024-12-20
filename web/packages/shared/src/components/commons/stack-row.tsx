import Stack from '@mui/material/Stack';
import type { CSSProperties } from '@mui/material/styles/createTypography';
import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import { PropsWithChildren } from 'react';

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  color?: CSSProperties['color'];
};

const StackRow = ({ children, sx, spacing, color }: PropsWithChildren<Props>) => (
  <Stack direction="row" spacing={spacing ?? 1} sx={{ alignItems: 'center', flexWrap: 'wrap', ...sx }} color={color}>
    {children}
  </Stack>
);

export default StackRow;
